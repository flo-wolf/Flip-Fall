using Sliders.Cam;
using Sliders.Levels;
using Sliders.UI;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Sliders
{
    public class Player : MonoBehaviour
    {
        public static Player _instance;

        public enum PlayerState { alive, dead, ready, waiting };
        public enum PlayerAction { reflect, charge, decharge };
        private PlayerState playerState;
        private PlayerAction playerAction;

        public PlayerStateChangeEvent onPlayerStateChange = new PlayerStateChangeEvent();
        public PlayerActionEvent onPlayerAction = new PlayerActionEvent();

        //public Player instance;
        [Header("References")]
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
        public float maxChargeVelocity = 250F;
        public float chargeForcePerTick = 5F;
        public float respawnDuration = 1f;
        public float aliveTime = 0f;

        private Spawn spawn;
        private Vector3 spawnPosition;
        private Quaternion spawnRotaion;
        private int speed;
        private bool facingLeft = true;
        private bool charging = false;
        private Vector2 chargeVelocity;
        private bool firstChargeDone = false;

        private void Awake()
        {
            //instance = this;
            _instance = this;
        }

        private void Start()
        {
            rBody.Sleep();
            ReloadSpawnPoint();
            MoveToSpawn();
            Game.onGameStateChange.AddListener(GameStateChanged);
            LevelManager.onLevelChange.AddListener(LevelChanged);
            trail.sortingOrder = 2;
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

        //The Player has left the moveable area
        private void OnTriggerExit2D(Collider2D collider)
        {
            Debug.Log("Exit");
            if (1 << collider.gameObject.layer == moveMask.value && IsAlive() || collider == null)
            {
                Debug.Log("Die");
                Die();
                Game.SetGameState(Game.GameState.scorescreen);
            }
        }

        private void OnCollisionExit2D(Collision collision)
        {
            Debug.Log("Exit2");
            if (1 << collision.collider.gameObject.layer == moveMask.value && IsAlive() || collision.collider == null)
            {
                Debug.Log("Die2");
                Die();
                Game.SetGameState(Game.GameState.scorescreen);
            }
        }

        private void OnTriggerEnter2D(Collider2D collider)
        {
            //the collided object is on one of the layers marked as killMask => death
            if (1 << collider.gameObject.layer == killMask.value && IsAlive())
            {
                Die();
                Game.SetGameState(Game.GameState.scorescreen);
            }
            //the collided object is the finish => fin
            else if (1 << collider.gameObject.layer == finishMask.value && IsAlive())
            {
                Fin();
                Game.SetGameState(Game.GameState.finishscreen);
            }
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

        private void Spawn()
        {
            SetPlayerState(PlayerState.alive);

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
            SetPlayerState(PlayerState.dead);
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
            SetPlayerState(PlayerState.dead);
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
            rBody.velocity = Vector3.zero;
            rBody.gravityScale = 0f;
            rBody.Sleep();
            SetPlayerState(PlayerState.ready);
        }

        private void SwitchFacingDirection()
        {
            facingLeft = !facingLeft;
        }

        //Spieleraktion - X-Achsen-Spiegelung der Figurenflugbahn
        private void Reflect()
        {
            rBody.velocity = new Vector2(rBody.velocity.x * (-1), rBody.velocity.y);
            SwitchFacingDirection();
            onPlayerAction.Invoke(PlayerAction.reflect);
        }

        //Spieleraktion - Gravitationsabbruch, Figur wird auf der Ebene gehalten und beschleunigt
        private void Charge()
        {
            rBody.gravityScale = 0f;
            rBody.velocity = new Vector2(rBody.velocity.x, 0f);
            charging = true;
            onPlayerAction.Invoke(PlayerAction.charge);
        }

        //Spieleraktion - Erneutes Hinzufügen der Gravitation
        private void Decharge()
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

        //Inputs, alles in den Input Manager!
        private void Update()
        {
            if (IsAlive())
            {
                //Keyboard
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    Die();
                }
                else if (Input.GetKeyDown(KeyCode.M))
                {
                    Reflect();
                }
                else if (Input.GetKeyDown(KeyCode.Y) && !charging)
                {
                    Charge();
                }
                else if (Input.GetKeyUp(KeyCode.Y) && charging)
                {
                    Decharge();
                }
                /*
                //Touch
                foreach (Touch touch in Input.touches)
                {
                    Vector3 position = touch.position;

                    //Right Touch
                    if (position.x >= Camera.main.pixelWidth / 2 && touch.phase == TouchPhase.Began && firstChargeDone)
                        Reflect();

                    //Left Touch
                    else if (position.x < Camera.main.pixelWidth / 2)
                    {
                        if (touch.phase == TouchPhase.Began && !charging)
                        {
                            Charge();
                            firstChargeDone = true;
                        }
                        else if (touch.phase == TouchPhase.Ended && touch.phase != TouchPhase.Canceled && charging)
                            Decharge();
                    }
                }
                */
            }
        }

        public void SetPlayerState(PlayerState ps)
        {
            playerState = ps;
            onPlayerStateChange.Invoke(playerState);
        }

        public bool IsAlive()
        {
            if (playerState == PlayerState.alive)
                return true;
            else
                return false;
        }

        public bool IsReady()
        {
            if (playerState == PlayerState.ready)
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