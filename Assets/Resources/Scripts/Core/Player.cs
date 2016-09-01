using Sliders.Cam;
using Sliders.Levels;
using Sliders.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// The Player controller class - handles player actions and events
/// </summary>
namespace Sliders
{
    public class Player : MonoBehaviour
    {
        public static Player _instance;

        public enum PlayerState { alive, dead, fin };
        public enum PlayerAction { reflect, charge, decharge };
        private PlayerState playerState = PlayerState.dead;
        private PlayerAction playerAction;

        public static PlayerStateChangeEvent onPlayerStateChange = new PlayerStateChangeEvent();
        public static PlayerActionEvent onPlayerAction = new PlayerActionEvent();

        //public Player instance;
        [Header("References")]
        public ParticleSystem finishParticles;
        private ParticleSystem.EmissionModule finishParticlesEmit;

        public ParticleSystem trailParticles;
        private ParticleSystem.EmissionModule trailParticlesEmit;

        public LayerMask finishMask;
        public LayerMask killMask;
        public LayerMask moveMask;
        public TrailRenderer trail; //Full color
        public TrailRenderer trail2; //Transparency - use shading instead

        public Rigidbody2D rBody;
        public Material defaultMaterial;
        public Material winMaterial;

        [Header("Settings")]
        public float gravity = 15F;
        public float trialTime = 5F;
        public float maxChargeVelocity = 250F;
        public float chargeForcePerTick = 5F;
        public float respawnDuration = 1f;
        public float aliveTime = 0f;

        [Range(0.0f, 0.1f)]
        public float triggerExitCheckDelay = 0.001F;

        private Spawn spawn;
        private Vector3 spawnPosition;
        private Quaternion spawnRotaion;
        private int speed;
        private bool facingLeft = true;

        [HideInInspector]
        public bool charging = false;
        private Vector2 chargeVelocity;
        private bool firstChargeDone = false;
        private int collisionCount = 0;

        private static List<Collider2D> colliderList = new List<Collider2D>();

        private void Awake()
        {
            //instance = this;
            _instance = this;
            finishParticlesEmit = finishParticles.emission;
            trailParticlesEmit = trailParticles.emission;
        }

        private void Start()
        {
            ReloadSpawnPoint();
            MoveToSpawn();
            rBody.Sleep();
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
                    Spawn();
                    StartCoroutine(AliveTimerCorutine());
                    finishParticles.gameObject.SetActive(false);
                    break;

                case Game.GameState.deathscreen:

                    break;

                case Game.GameState.finishscreen:

                    break;

                case Game.GameState.levelselection:
                    MoveToSpawn();
                    break;

                default:
                    break;
            }
        }

        //Whenever the Level gets changed the LevelManager fires the LevelChangeEvent, calling this method.
        //Resets the Player to the Spawn
        private void LevelChanged(Level level)
        {
            ReloadSpawnPoint();
            MoveToSpawn();
        }

        private void ReloadSpawnPoint()
        {
            spawn = LevelManager.GetSpawn();
            spawnPosition = spawn.GetPosition();
            spawnPosition.z = Constants.playerZ;
            facingLeft = spawn.facingLeftOnSpawn;
        }

        //The Player has hit an object, either the finish or an enemy
        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (!colliderList.Find(x => x == collider))
                colliderList.Add(collider);
            collisionCount++;

            if (finishMask == (finishMask | (1 << collider.gameObject.layer)) && IsAlive())
            {
                //Debug.Log("TriggerEnter - Fin - Collider: " + collider.gameObject);
                Fin();
                Game.SetGameState(Game.GameState.finishscreen);
            }
            //the collided object is on one of the layers marked as killMask => death
            else if (killMask == (killMask | (1 << collider.gameObject.layer)) && IsAlive())
            {
                //Debug.Log("TriggerEnter - Die - Collider: " + collider.gameObject);
                Die();
                Game.SetGameState(Game.GameState.deathscreen);
            }
            //the collided object is the finish => fin
        }

        public void OnParticleCollision(GameObject go)
        {
            if (killMask == (killMask | (1 << go.layer)) && IsAlive())
            {
                Die();
                Game.SetGameState(Game.GameState.deathscreen);
                Debug.Log("[Player] Death by particle");
            }
        }

        //The Player has left the area allowed for moving(moveMask)
        private void OnTriggerExit2D(Collider2D collider)
        {
            colliderList.Remove(colliderList.Find(x => x == collider));
            collisionCount--;

            if ((moveMask == (moveMask | (1 << collider.gameObject.layer))) && (collisionCount <= 0) && colliderList.Count == 0 && IsAlive())
            {
                //Debug.Log("TriggerExit - Die - Collider: " + collider.gameObject);
                Die();
                Game.SetGameState(Game.GameState.deathscreen);
            }

            //StartCoroutine(DelayedTriggerExit(collider));
        }

        private void OnTriggerStay2D(Collider2D collider)
        {
            if (!colliderList.Find(x => x == collider))
                colliderList.Add(collider);
        }

        private IEnumerator DelayedTriggerExit(Collider2D collider)
        {
            //yield return new WaitForSeconds(triggerExitCheckDelay);

            //Debug.Log("coll count exit " + collisionCount + " IsAlive() " + IsAlive() + " mask " + (moveMask == (moveMask | (1 << collider.gameObject.layer))));
            //Debug.Log("collisionList: " + colliderList.Count);

            yield break;
        }

        //Counts the time the Player is alive in this try
        private IEnumerator AliveTimerCorutine()
        {
            while (playerState == PlayerState.alive)
            {
                aliveTime += Time.fixedDeltaTime;
                var time = TimeSpan.FromSeconds(aliveTime);

                yield return new WaitForFixedUpdate();
            }
        }

        public void Spawn()
        {
            colliderList = new List<Collider2D>();
            collisionCount = 0;

            trailParticles.Clear();
            trailParticles.Simulate(0.0f, true, true);
            trailParticlesEmit.enabled = true;
            trailParticles.Play();

            SetPlayerState(PlayerState.alive);

            aliveTime = 0;
            //trail.time = trialTime;
            //trail.enabled = true;
            //trail2.time = 1f;
            //trail2.enabled = true;

            rBody.gravityScale = gravity;
            rBody.velocity = new Vector3(0f, -0.00001f, 0f);
            rBody.WakeUp();
        }

        public void Die()
        {
            SetPlayerState(PlayerState.dead);
            charging = false;
            facingLeft = spawn.facingLeftOnSpawn;
            firstChargeDone = false;

            trailParticlesEmit.enabled = false;
            trailParticles.Stop();
            trailParticles.gameObject.SetActive(true);

            finishParticles.gameObject.SetActive(true);
            //finishParticles.Clear();
            //finishParticles.Simulate(0.0f, true, true);
            finishParticlesEmit.enabled = true;
            finishParticles.Play();

            //Add here animations, fadeaway etc on death
            //trail.time = 0.0f;
            //trail.enabled = false;
            //trail2.time = 0.0f;
            //trail2.enabled = false;
            rBody.velocity = Vector3.zero;
            rBody.gravityScale = 0f;
            rBody.Sleep();
        }

        private void Fin()
        {
            SetPlayerState(PlayerState.fin);

            trailParticlesEmit.enabled = false;
            trailParticles.Stop();

            finishParticles.gameObject.SetActive(true);
            //finishParticles.Clear();
            //finishParticles.Simulate(0.0f, true, true);
            finishParticlesEmit.enabled = true;
            finishParticles.Play();

            gameObject.GetComponent<MeshRenderer>().material = winMaterial;
            charging = false;
            facingLeft = spawn.facingLeftOnSpawn;
            firstChargeDone = false;

            //Add here animations, fadeaway etc on death
            //trail.time = 0.0f;
            //trail.enabled = false;
            //trail2.time = 0.0f;
            //trail2.enabled = false;
            rBody.velocity = Vector3.zero;
            rBody.gravityScale = 0f;
            rBody.Sleep();
        }

        private void MoveToSpawn()
        {
            transform.position = spawnPosition;

            finishParticlesEmit.enabled = false;
            finishParticles.Stop();

            aliveTime = 0;
            gameObject.GetComponent<MeshRenderer>().material = defaultMaterial;
            rBody.velocity = Vector3.zero;
            rBody.gravityScale = 0f;
            rBody.Sleep();
        }

        private void SwitchFacingDirection()
        {
            facingLeft = !facingLeft;
        }

        //Spieleraktion - X-Achsen-Spiegelung der Figurenflugbahn
        public void Reflect()
        {
            rBody.velocity = new Vector2(rBody.velocity.x * (-1), rBody.velocity.y);
            SwitchFacingDirection();
            onPlayerAction.Invoke(PlayerAction.reflect);
        }

        //Spieleraktion - Gravitationsabbruch, Figur wird auf der Ebene gehalten und beschleunigt
        public void Charge()
        {
            rBody.gravityScale = 0f;
            rBody.velocity = new Vector2(rBody.velocity.x, 0f);
            charging = true;
            onPlayerAction.Invoke(PlayerAction.charge);
        }

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
                    if (facingLeft)
                    {
                        velocity.x = velocity.x - chargeForcePerTick;
                    }
                    else
                    {
                        velocity.x = velocity.x + chargeForcePerTick;
                    }
                    rBody.velocity = velocity;
                }
            }
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

        //Fired whenever the playerstate gets changed through SetPlayerState()
        public class PlayerStateChangeEvent : UnityEvent<PlayerState> { }

        public class PlayerActionEvent : UnityEvent<PlayerAction> { }
    }
}