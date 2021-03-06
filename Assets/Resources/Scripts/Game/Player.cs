﻿using FlipFall.Cam;
using FlipFall.Editor;
using FlipFall.LevelObjects;
using FlipFall.Levels;
using FlipFall.Progress;
using FlipFall.UI;
using GooglePlayGames;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// The Player controller class - handles player actions and events
/// </summary>
namespace FlipFall
{
    public class Player : MonoBehaviour
    {
        public static Player _instance;

        public enum PlayerState { alive, dead, win, teleporting };
        public enum PlayerAction { reflect, charge, decharge, teleport };
        public static PlayerState playerState;
        private static PlayerAction playerAction;

        public static PlayerStateChangeEvent onPlayerStateChange = new PlayerStateChangeEvent();
        public static PlayerActionEvent onPlayerAction = new PlayerActionEvent();

        public static Portal startPortal;
        public static Portal destinationPortal;

        [Header("References")]
        public ParticleSystem deathParticles;
        private ParticleSystem.EmissionModule deathParticlesEmit;

        public ParticleSystem trailParticles;
        private ParticleSystem.EmissionModule trailParticlesEmit;

        public LayerMask finishMask;
        public LayerMask killMask;
        public LayerMask moveMask;

        public Rigidbody2D rBody;
        public Material defaultMaterial;
        public Material winMaterial;

        [Header("Settings")]
        public float gravity = 15F;
        public float trialTime = 5F;
        public float maxChargeVelocity = 250F;
        public float chargeForcePerTick = 5F;
        public float respawnDuration = 1f;
        public float teleportDuration = 0.2F;
        public float directionSwitchForce = 25F;

        [Range(0.0f, 0.1f)]
        public float triggerExitCheckDelay = 0.001F;

        private Spawn spawn;
        private Vector3 spawnPosition;
        private Quaternion spawnRotaion;
        private int speed;
        private bool facingLeft = true;

        private CircleCollider2D circleCollider;
        private List<Bounds> bounds;

        private Vector3 deathPos;
        private ParticleCollisionEvent[] collisionEvents = new ParticleCollisionEvent[16];

        [HideInInspector]
        public bool charging = false;
        private Vector2 chargeVelocity;
        private bool firstChargeDone = false;
        public static bool teleporting = false;

        // the collider of the movearea mesh
        private static PolygonCollider2D levelCollider;

        private static Mesh levelMesh;

        public float chargeDelayAfterClik = 0.3F;

        private void Awake()
        {
            _instance = this;

            deathParticlesEmit = deathParticles.emission;
            trailParticlesEmit = trailParticles.emission;
        }

        private void Start()
        {
            // load the referenced position check data

            levelMesh = LevelPlacer.generatedLevel.moveArea.meshFilter.mesh;

            circleCollider = GetComponent<CircleCollider2D>();
            ReloadSpawnPoint();
            MoveToSpawn();
            Spawn();
            deathParticles.Stop();
            deathParticles.gameObject.SetActive(false);
            Game.onGameStateChange.AddListener(GameStateChanged);
            LevelManager.onLevelChange.AddListener(LevelChanged);

            //trail.sortingOrder = 2;
        }

        //Everything in here happends after the delay on death. To execute before, enter calls in the Trigger function.
        private void GameStateChanged(Game.GameState gameState)
        {
            Debug.Log("[Player]: GameState changed to: " + gameState);
            switch (gameState)
            {
                case Game.GameState.playing:

                    break;

                case Game.GameState.deathscreen:

                    break;

                case Game.GameState.finishscreen:

                    break;

                default:
                    break;
            }
        }

        //Whenever the Level gets changed the LevelManager fires the LevelChangeEvent, calling this method.
        //Resets the Player to the Spawn
        private void LevelChanged(int levelID)
        {
            bounds = LevelPlacer.placedLevel.moveBounds;
            ReloadSpawnPoint();
            MoveToSpawn();
        }

        private void ReloadSpawnPoint()
        {
            spawn = LevelPlacer.generatedLevel.spawn;
            spawnPosition = spawn.transform.position;

            spawnPosition.z = Constants.playerZ;
            facingLeft = spawn.facingLeftOnSpawn;
        }

        // player has hit a trigger collider
        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.tag == Constants.moveAreaTag && IsAlive())
            {
            }
            else if (collider.tag == Constants.finishTag && IsAlive())
            {
                // Debug.Log("TriggerEnter - Fin - Collider: " + collider.gameObject);
                Win();
                Game.SetGameState(Game.GameState.finishscreen);
            }

            // collided object is on one of the layers marked as killMask => death
            if ((collider.tag == Constants.killTag || collider.tag == Constants.turretTag) && IsAlive() && teleporting == false)
            {
                Debug.Log("TriggerEnter - Die - Collider: " + collider.gameObject);

                //cannot get the contact point, otherise the trigger needs a rigidbody for collision data
                Die(transform.position);
                Game.SetGameState(Game.GameState.deathscreen);
            }
            else if (collider.tag == Constants.portalTag && teleporting == false && IsAlive())
            {
                Debug.Log("TriggerEnter - Portal - Collider: " + collider.gameObject);
                Portal portal = collider.gameObject.GetComponent<Portal>();
                if (portal != null)
                {
                    portal.Enter();
                }
            }
        }

        // Player has left the area allowed for moving(moveMask)
        private void OnTriggerExit2D(Collider2D collider)
        {
            if (collider.gameObject.tag == Constants.portalTag && teleporting)
            {
                Portal portal = collider.gameObject.GetComponent<Portal>();
                portal.Exit();
                //PortalExit(collider.gameObject.GetComponent<Portal>());
            }
        }

        public void OnParticleCollision(GameObject go)
        {
            if ((go.tag == Constants.killTag || go.tag == Constants.turretTag) && IsAlive())
            {
                //ParticleSystem particleSystem = go.GetComponent<ParticleSystem>();

                Die(GetParticleCollisionLoc(go));
                Debug.Log("[Player] Death by particle");
            }
        }

        private Vector3 GetParticleCollisionLoc(GameObject go)
        {
            int safeLength = deathParticles.GetSafeCollisionEventSize();
            if (collisionEvents.Length < safeLength)
                collisionEvents = new ParticleCollisionEvent[safeLength];

            int numCollisionEvents = deathParticles.GetCollisionEvents(go, collisionEvents);
            int i = 0;
            while (i < numCollisionEvents)
            {
                return collisionEvents[i].intersection;
                i++;
            }
            return transform.position;
        }

        private IEnumerator cPlayerTeleport(Vector3 pos, float duration)
        {
            yield return new WaitForSeconds(duration);
            transform.position = pos;
            //rBody.velocity = teleportVelocityMemory;
            yield break;
        }

        public void Spawn()
        {
            deathPos = Vector3.zero;
            trailParticles.Clear();
            trailParticles.Simulate(0.0f, true, true);
            trailParticlesEmit.enabled = true;
            trailParticles.Play();
            teleporting = false;

            SetPlayerState(PlayerState.alive);

            rBody.gravityScale = gravity;
            if (spawn.facingLeftOnSpawn)
                rBody.velocity = new Vector3(-0.00001f, 0f, 0f);
            else
                rBody.velocity = new Vector3(0.00001f, 0f, 0f);
            rBody.WakeUp();
        }

        public void Die(Vector3 pos)
        {
            Debug.Log("DEATHPOS: " + pos + " playerpos " + transform.position + " deathpos " + deathPos);

            // Death is my Hobby
            PlayGamesPlatform.Instance.IncrementAchievement("CgkIqIqqjZYFEAIQCg", 1, (bool success) =>
            {
            });

            SetPlayerState(PlayerState.dead);
            charging = false;
            facingLeft = spawn.facingLeftOnSpawn;
            firstChargeDone = false;

            trailParticlesEmit.enabled = false;
            trailParticles.Stop();
            trailParticles.gameObject.SetActive(false);

            //disables player mesh, only leaving particle effectss
            GetComponent<MeshRenderer>().enabled = false;

            if (deathPos == Vector3.zero)
                deathPos = transform.position;

            // start level dissolve effect

            LevelPlacer.generatedLevel.moveArea.DissolveLevel();

            Vector3 deathParticlePos = new Vector3(deathPos.x, deathPos.y, Constants.playerZ);
            deathParticles.gameObject.transform.position = deathParticlePos;
            deathParticles.gameObject.SetActive(true);
            //finishParticles.Clear();
            //finishParticles.Simulate(0.0f, true, true);
            deathParticlesEmit.enabled = true;
            deathParticles.Play();

            rBody.velocity = Vector3.zero;
            rBody.gravityScale = 0f;
            rBody.Sleep();

            Game.SetGameState(Game.GameState.deathscreen);
        }

        private void Win()
        {
            SetPlayerState(PlayerState.win);

            PlayGamesPlatform.Instance.IncrementAchievement("CgkIqIqqjZYFEAIQFQ", 1, (bool success) =>
            {
            });

            // start level dissolve effect
            LevelPlacer.generatedLevel.moveArea.DissolveLevel();

            trailParticlesEmit.enabled = false;
            trailParticles.Stop();
            trailParticles.gameObject.SetActive(false);

            deathParticles.gameObject.SetActive(true);
            //finishParticles.Clear();
            //finishParticles.Simulate(0.0f, true, true);
            //deathParticlesEmit.enabled = true;
            //deathParticles.Play();

            gameObject.GetComponent<MeshRenderer>().material = winMaterial;
            charging = false;
            facingLeft = spawn.facingLeftOnSpawn;
            firstChargeDone = false;

            //disables player mesh, only leaving particle effectss
            GetComponent<MeshRenderer>().enabled = false;

            rBody.velocity = Vector3.zero;
            rBody.gravityScale = 0f;
            rBody.Sleep();
        }

        private void MoveToSpawn()
        {
            ReloadSpawnPoint();
            transform.position = spawnPosition;
            Debug.Log("Spawnposition:" + spawnPosition);

            deathParticlesEmit.enabled = false;
            deathParticles.Stop();
            gameObject.GetComponent<MeshRenderer>().material = defaultMaterial;
            rBody.velocity = Vector3.zero;
            rBody.gravityScale = 0f;
            rBody.Sleep();
        }

        private void SwitchFacingDirection()
        {
            facingLeft = !facingLeft;
        }

        // Switch the players x-velocity
        public void Reflect()
        {
            bool newMovement = true;

            // old movement - always replace the force by a default force on each reflect
            if (!newMovement)
            {
                // currently moving to the right => switch to the left
                if (rBody.velocity.x > 0)
                    rBody.velocity = new Vector2(-directionSwitchForce, rBody.velocity.y);
                // currently moving to the left => switch to the right
                else
                    rBody.velocity = new Vector2(directionSwitchForce, rBody.velocity.y);
            }
            // keep the force and just flip it when its bigger than the default value
            else
            {
                // currently moving to the right => switch to the left
                if (rBody.velocity.x > 0)
                    if (rBody.velocity.x > directionSwitchForce)
                        rBody.velocity = new Vector2(-rBody.velocity.x, rBody.velocity.y);
                    else
                        rBody.velocity = new Vector2(-directionSwitchForce, rBody.velocity.y);
                // currently moving to the left => switch to the right
                else {
                    if (rBody.velocity.x < -directionSwitchForce)
                        rBody.velocity = new Vector2(-rBody.velocity.x, rBody.velocity.y);
                    else
                        rBody.velocity = new Vector2(directionSwitchForce, rBody.velocity.y);
                }
            }

            SwitchFacingDirection();
            onPlayerAction.Invoke(PlayerAction.reflect);

            PlayGamesPlatform.Instance.IncrementAchievement("CgkIqIqqjZYFEAIQFA", 1, (bool success) =>
            {
            });
        }

        // reflect the player to the right when he flies to the left
        public void ReflectToLeft()
        {
            if (rBody.velocity.x > 0)
            {
                Reflect();
            }
            else
            {
                onPlayerAction.Invoke(PlayerAction.reflect);
            }
            StartCoroutine(cChargeDelay());
        }

        // reflect the player to the left when he flies to the right
        public void ReflectToRight()
        {
            if (rBody.velocity.x < 0)
            {
                Reflect();
            }
            else
            {
                onPlayerAction.Invoke(PlayerAction.reflect);
            }
            StartCoroutine(cChargeDelay());
        }

        // delay between charge call and actual charge beginn
        private IEnumerator cChargeDelay()
        {
            charging = true;
            rBody.gravityScale = 0F;
            rBody.velocity = new Vector2(rBody.velocity.x, 0);
            onPlayerAction.Invoke(PlayerAction.charge);
            yield return new WaitForSeconds(chargeDelayAfterClik);
            yield break;
        }

        //Spieleraktion - Gravitationsabbruch, Figur wird auf der Ebene gehalten und beschleunigt
        //public void Charge()
        //{
        //    StartCoroutine(charge());
        //}

        //private IEnumerator charge()
        //{
        //    float duration = 0.5F;
        //    float t = 0;
        //    while (t < 1.0f)
        //    {
        //        t += Time.deltaTime * (Time.timeScale / chargeDelayAfterClik);
        //        rBody.gravityScale = Mathf.Lerp(rBody.gravityScale, 0, t);
        //        //.velocity = new Vector2(rBody.velocity.x, Mathf.SmoothStep(rBody.velocity.y, 0, t));
        //        yield return 0;
        //    }

        //    charging = true;
        //    yield break;
        //}

        //Spieleraktion - Erneutes Hinzufügen der Gravitation
        public void Decharge()
        {
            charging = false;
            rBody.gravityScale = gravity;
            onPlayerAction.Invoke(PlayerAction.decharge);
        }

        //Geschwindigkeitszuwachs während die Figur gehalten wird
        private void FixedUpdate()
        {
            if (IsAlive())
            {
                Vector2 velocity = rBody.velocity;
                float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
                Quaternion quad = Quaternion.AngleAxis(angle, Vector3.forward);
                transform.rotation = quad;

                if (charging && Mathf.Abs(rBody.velocity.x) < maxChargeVelocity)
                {
                    if (velocity.x < 0)
                    {
                        velocity.x = velocity.x - chargeForcePerTick;
                    }
                    else
                    {
                        velocity.x = velocity.x + chargeForcePerTick;
                    }
                    rBody.velocity = velocity;
                }

                if (!IsOnMoveArea())
                {
                    if (deathPos != null && deathPos != Vector3.zero)
                        Die(deathPos);
                    else
                        Die(transform.position);
                }
            }
        }

        public void Update()
        {
        }

        public bool IsOnMoveArea()
        {
            int counter = 0;

            for (int i = 1; i <= 6; i++)
            {
                Vector2 checkPos = transform.position;
                switch (i)
                {
                    //right
                    case 1:
                        checkPos.x = checkPos.x + circleCollider.bounds.extents.x;
                        break;
                    //left
                    case 2:
                        checkPos.x = checkPos.x - circleCollider.bounds.extents.x;
                        break;
                    //up
                    case 3:
                        checkPos.y = checkPos.y + circleCollider.bounds.extents.y;
                        break;
                    //down
                    case 4:
                        checkPos.y = checkPos.y - circleCollider.bounds.extents.y;
                        break;
                    //lower left
                    case 5:
                        checkPos.x = checkPos.x - (circleCollider.bounds.extents.x * (2 / 3));
                        checkPos.y = checkPos.y - (circleCollider.bounds.extents.y * (2 / 3));
                        break;
                    //lower right
                    case 6:
                        checkPos.x = checkPos.x + (circleCollider.bounds.extents.x * (2 / 3));
                        checkPos.y = checkPos.y - (circleCollider.bounds.extents.y * (2 / 3));
                        break;
                }

                //Debug.Log("checkPos " + checkPos);
                Vector3 isInsideCheckPos = Vector3.zero;
                isInsideCheckPos = LevelPlacer.generatedLevel.moveArea.transform.InverseTransformPoint(checkPos);
                if (VertHelper.IsInsideMesh(levelMesh, Vector3.zero, isInsideCheckPos))
                {
                    counter++;
                }
                else
                {
                    deathPos = checkPos;
                }
            }

            if (counter >= 6)
            {
                return true;
            }

            // not on movearea
            return false;
        }

        public void SetPlayerState(PlayerState ps)
        {
            playerState = ps;
            onPlayerStateChange.Invoke(playerState);
            Debug.Log("[Player] PlayerState changed to: " + ps);
        }

        public bool IsAlive()
        {
            if (playerState == PlayerState.alive)
                return true;
            else
                return false;
        }

        public bool IsFacingLeft()
        {
            return facingLeft;
        }

        static public bool IsInside(PolygonCollider2D coll, Vector2 p)
        {
            bool inside = false;

            for (int c = 0; c < coll.pathCount; c++)
            {
                Vector2[] pPoints = coll.GetPath(c);

                inside = false;
                for (int i = 0, j = coll.points.Length - 1; i < coll.points.Length; j = i++)
                {
                    if ((pPoints[i].y > p.y) != (pPoints[j].y > p.y) &&
                         p.x < (pPoints[j].x - pPoints[i].x) * (p.y - pPoints[i].y) / (pPoints[j].y - pPoints[i].y) + pPoints[i].x)
                    {
                        inside = !inside;
                    }
                }
                if (inside)
                    return inside;
            }
            return inside;
        }

        //Fired whenever the playerstate gets changed through SetPlayerState()
        public class PlayerStateChangeEvent : UnityEvent<PlayerState> { }

        public class PlayerActionEvent : UnityEvent<PlayerAction> { }
    }

    public static class Boundsxtension
    {
        public static bool ContainBounds(this Bounds bounds, Bounds target)
        {
            return bounds.Contains(target.min) && bounds.Contains(target.max);
        }
    }
}