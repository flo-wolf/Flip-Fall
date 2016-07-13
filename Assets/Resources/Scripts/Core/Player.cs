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
        public enum PlayerState { alive, dead, ready, waiting };

        public static PlayerState playerState;

        public static PlayerStateChangeEvent onPlayerStateChange = new PlayerStateChangeEvent();

        public CameraMovement cm;
        public LayerMask finishMask;
        public LayerMask killMask;
        public GameObject trail;
        public GameObject trail2;
        public Text text;

        public AudioClip deathSound;
        public AudioClip finishSound;

        public float gravity = 15F;
        public float maxChargeVelocity;
        public float chargeForcePerTick;
        public float respawnTime = 1f;

        private Spawn spawn;
        private Quaternion spawnRotaion;
        private Vector3 spawnPosition;
        private int speed;

        public static float _playerZ;
        public static bool facingLeft = true;

        private bool charging = false;
        private Vector2 chargeVelocity;
        private bool firstChargeDone = false;

        private void Awake()
        {
            _playerZ = Constants.playerZ;
        }

        private void ReloadSpawnPoint()
        {
            spawn = LevelManager.levelManager.GetLevel().spawn;
            spawnPosition = spawn.transform.position;
            spawnPosition.z = _playerZ;
            facingLeft = spawn.facingLeftOnSpawn;
            transform.position = spawnPosition;
        }

        private void Start()
        {
            ReloadSpawnPoint();
            MoveToSpawn();
            Game.onGameStateChange.AddListener(GameStateChanged);
            LevelManager.onLevelChange.AddListener(LevelChanged);
        }

        private void LevelChanged(Level level)
        {
            ReloadSpawnPoint();
        }

        private void GameStateChanged(Game.GameState gameState)
        {
            Debug.Log("[Player]: GameState changed to: " + gameState);
            switch (gameState)
            {
                case Game.GameState.playing:
                    Spawn();
                    break;

                case Game.GameState.deathscreen:
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

        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (1 << collider.gameObject.layer == killMask.value && IsAlive())
            {
                Die();
                Game.SetGameState(Game.GameState.deathscreen);
                SoundManager.instance.RandomizeSfx(deathSound);
            }
            else if (1 << collider.gameObject.layer == finishMask.value && IsAlive())
            {
                Die();
                Game.SetGameState(Game.GameState.finishscreen);
                SoundManager.instance.RandomizeSfx(finishSound);
            }
        }

        private void Spawn()
        {
            SetPlayerState(PlayerState.alive);
            CameraMovement.SetCameraState(CameraMovement.CameraState.following);

            trail.GetComponent<TrailRenderer>().time = 0.5f;
            trail.GetComponent<TrailRenderer>().enabled = true;
            trail2.GetComponent<TrailRenderer>().time = 1f;
            trail2.GetComponent<TrailRenderer>().enabled = true;

            GetComponent<Rigidbody2D>().gravityScale = gravity;
            GetComponent<Rigidbody2D>().velocity = new Vector3(0f, -0.00001f, 0f);
            GetComponent<Rigidbody2D>().WakeUp();
        }

        private void Die()
        {
            SetPlayerState(PlayerState.dead);
            charging = false;
            facingLeft = spawn.facingLeftOnSpawn;
            firstChargeDone = false;

            trail.GetComponent<TrailRenderer>().time = 0.0f;
            trail.GetComponent<TrailRenderer>().enabled = false;
            trail2.GetComponent<TrailRenderer>().time = 0.0f;
            trail2.GetComponent<TrailRenderer>().enabled = false;
            GetComponent<Rigidbody2D>().velocity = Vector3.zero;
            GetComponent<Rigidbody2D>().gravityScale = 0f;
            GetComponent<Rigidbody2D>().Sleep();
        }

        private void MoveToSpawn()
        {
            transform.position = spawnPosition;
            cm.moveCamTo(new Vector3(spawnPosition.x, spawnPosition.y + Constants.cameraY, transform.position.z), respawnTime);
            GetComponent<Rigidbody2D>().velocity = Vector3.zero;
            GetComponent<Rigidbody2D>().gravityScale = 0f;
            GetComponent<Rigidbody2D>().Sleep();
            SetPlayerState(PlayerState.ready);
        }

        //X-Achsen-Spiegelung der Figurenflugbahn
        private void Reflect()
        {
            GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x * (-1), GetComponent<Rigidbody2D>().velocity.y);
            SwitchFacingDirection();
        }

        //Gravitationsabbruch, Figur wird auf der Ebene gehalten und beschleunigt
        private void Charge()
        {
            GetComponent<Rigidbody2D>().gravityScale = 0f;
            GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, 0f);
            charging = true;
        }

        //Erneutes Hinzufügen der Gravitation
        private void Decharge()
        {
            charging = false;
            GetComponent<Rigidbody2D>().gravityScale = gravity;
        }

        //Geschwindigkeitszuwachs während die Figur gehalten wird
        private void FixedUpdate()
        {
            if (IsAlive())
            {
                Vector2 velocity = transform.GetComponent<Rigidbody2D>().velocity;
                //float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
                //Quaternion quad = Quaternion.AngleAxis(angle, Vector3.forward);
                //transform.rotation = quad;

                if (charging && Mathf.Abs(GetComponent<Rigidbody2D>().velocity.x) < maxChargeVelocity)
                {
                    if (facingLeft)
                    {
                        velocity.x = velocity.x - chargeForcePerTick;
                    }
                    else
                    {
                        velocity.x = velocity.x + chargeForcePerTick;
                    }
                    GetComponent<Rigidbody2D>().velocity = velocity;
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

        public static bool IsAlive()
        {
            if (playerState == PlayerState.alive)
                return true;
            else
                return false;
        }

        public static bool IsReady()
        {
            if (playerState == PlayerState.ready)
                return true;
            else
                return false;
        }

        public class PlayerStateChangeEvent : UnityEvent<PlayerState> { }
    }
}