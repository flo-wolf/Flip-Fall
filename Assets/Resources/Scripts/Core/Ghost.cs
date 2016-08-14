using Sliders.Cam;
using Sliders.Levels;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Sliders
{
    public class Ghost : MonoBehaviour
    {
        public enum GhostState { alive, dead, ready, finished };

        public GhostStateChangeEvent onGhostStateChange = new GhostStateChangeEvent();

        //public Player instance;
        public CamMove cm;

        public GhostState ghostState;
        public LayerMask finishMask;
        public LayerMask killMask;
        public TrailRenderer trail; //Full color
        public TrailRenderer trail2; //Transparency - use shading instead
        public Rigidbody2D rBody;
        public Material defaultMaterial;
        public Material winMaterial;

        public float gravity = 15F;
        public float maxChargeVelocity;
        public float chargeForcePerTick;
        public float respawnDuration = 1f; //change to: Deathdelay !?
        public float aliveTime = 0f;

        private Spawn spawn;
        private Quaternion spawnRotaion;
        private Vector3 spawnPosition;
        private int speed;

        public float _playerZ;
        public bool facingLeft = true;

        private bool charging = false;
        private Vector2 chargeVelocity;
        private bool firstChargeDone = false;

        private void Awake()
        {
            //instance = this;
            _playerZ = Constants.playerZ;
        }

        private void Start()
        {
            ReloadSpawnPoint();
            MoveToSpawn();
            Game.onGameStateChange.AddListener(GameStateChanged);
            LevelManager.onLevelChange.AddListener(LevelChanged);
            trail.sortingOrder = 2;
        }

        public void SetGhostState(GhostState gs)
        {
            ghostState = gs;
            onGhostStateChange.Invoke(ghostState);
        }

        private void ReloadSpawnPoint()
        {
            spawn = LevelManager.levelManager.GetLevel().spawn;
            spawnPosition = spawn.transform.position;
            spawnPosition.z = _playerZ;
            facingLeft = spawn.facingLeftOnSpawn;
        }

        //Whenever the Level gets changed the LevelManager fires the LevelChangeEvent, calling this method.
        //Resets the Player to the Spawn
        private void LevelChanged(Level level)
        {
            ReloadSpawnPoint();
            MoveToSpawn();
        }

        //Everything in here happends after the delay on death. To execute before, enter calls in the Trigger function.
        private void GameStateChanged(Game.GameState gameState)
        {
            Debug.Log("[Player]: GameState changed to: " + gameState);
            switch (gameState)
            {
                case Game.GameState.playing:
                    Spawn();
                    break;

                case Game.GameState.scorescreen:
                    MoveToSpawn();
                    break;

                case Game.GameState.finishscreen:
                    MoveToSpawn();
                    break;

                default:
                    break;
            }
        }

        private void SwitchFacingDirection()
        {
            facingLeft = !facingLeft;
        }

        //The Player has hit something!
        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (1 << collider.gameObject.layer == killMask.value && IsAlive())
            {
                Die();
            }
            else if (1 << collider.gameObject.layer == finishMask.value && IsAlive())
            {
                Fin();
            }
        }

        private void Spawn()
        {
            SetGhostState(GhostState.alive);
            aliveTime = 0;
            trail.time = 0.5f;
            trail.enabled = true;
            trail2.time = 1f;
            trail2.enabled = true;

            rBody.gravityScale = gravity;
            rBody.velocity = new Vector3(0f, -0.00001f, 0f);
            rBody.WakeUp();
        }

        private void Die()
        {
            SetGhostState(GhostState.dead);
            charging = false;
            facingLeft = spawn.facingLeftOnSpawn;
            firstChargeDone = false;

            //Add here animations, fadeaway etc on death
            trail.time = 0.0f;
            trail.enabled = false;
            trail2.time = 0.0f;
            trail2.enabled = false;
            rBody.velocity = Vector3.zero;
            rBody.gravityScale = 0f;
            rBody.Sleep();
        }

        private void Fin()
        {
            gameObject.GetComponent<MeshRenderer>().material = winMaterial;
            charging = false;
            facingLeft = spawn.facingLeftOnSpawn;
            firstChargeDone = false;

            //Add here animations, fadeaway etc on death
            trail.time = 0.0f;
            trail.enabled = false;
            trail2.time = 0.0f;
            trail2.enabled = false;
            rBody.velocity = Vector3.zero;
            rBody.gravityScale = 0f;
            rBody.Sleep();
        }

        private void MoveToSpawn()
        {
            transform.position = spawnPosition;
            aliveTime = 0;
            gameObject.GetComponent<MeshRenderer>().material = defaultMaterial;
            CamMove.moveCamTo(new Vector3(spawnPosition.x, spawnPosition.y + Constants.cameraY, transform.position.z), respawnDuration);
            rBody.velocity = Vector3.zero;
            rBody.gravityScale = 0f;
            rBody.Sleep();
        }

        //X-Achsen-Spiegelung der Figurenflugbahn
        private void Reflect()
        {
            rBody.velocity = new Vector2(rBody.velocity.x * (-1), rBody.velocity.y);
            SwitchFacingDirection();
        }

        //Gravitationsabbruch, Figur wird auf der Ebene gehalten und beschleunigt
        private void Charge()
        {
            rBody.gravityScale = 0f;
            rBody.velocity = new Vector2(rBody.velocity.x, 0f);
            charging = true;
        }

        //Erneutes Hinzufügen der Gravitation
        private void Decharge()
        {
            charging = false;
            rBody.gravityScale = gravity;
        }

        //Geschwindigkeitszuwachs während die Figur gehalten wird
        private void FixedUpdate()
        {
            if (IsAlive())
            {
                Vector2 velocity = rBody.velocity;
                //float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
                //Quaternion quad = Quaternion.AngleAxis(angle, Vector3.forward);
                //transform.rotation = quad;

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

        public bool IsAlive()
        {
            if (ghostState == GhostState.alive)
                return true;
            else
                return false;
        }

        public bool IsReady()
        {
            if (ghostState == GhostState.ready)
                return true;
            else
                return false;
        }

        public class GhostStateChangeEvent : UnityEvent<GhostState> { }
    }
}