using Sliders.UI;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Sliders
{
    public class Player : MonoBehaviour
    {
        public enum PlayerState { alive, ready, dead };

        public PlayerState playerState;
        public CameraMovement cm;
        public LayerMask collisionMask;
        public GameObject trail;
        public UITimer timer;
        public Text text;

        public float gravity = 15F;
        public float maxChargeVelocity;
        public float chargeForcePerTick;
        public float respawnTime = 1f;

        public Vector2 spawnPosition = new Vector2(0f, 478.5f);
        public static Quaternion spawnRotaion;

        public static float lockedPlayerZ = -1f;
        public static bool leftMovement = true;
        public static bool alive = false;

        private bool charging = false;
        private Vector2 chargeVelocity;
        private bool firstChargeDone = false;

        private void Awake()
        {
            transform.position = new Vector3(spawnPosition.x, spawnPosition.y, lockedPlayerZ);
            spawnRotaion = transform.rotation;
            GetComponent<Rigidbody2D>().velocity = Vector3.zero;
            GetComponent<Rigidbody2D>().gravityScale = 0f;
            GetComponent<Rigidbody2D>().Sleep();
        }

        private void Start()
        {
        }

        public bool IsAlive()
        {
            if (playerState == PlayerState.alive)
                return true;
            else
                return false;
        }

        private void Spawn()
        {
            Debug.Log("spawn");
            CameraMovement.cameraFollow = true;

            trail.GetComponent<TrailRenderer>().time = 0.5f;
            trail.GetComponent<TrailRenderer>().enabled = true;

            GetComponent<Rigidbody2D>().gravityScale = gravity;
            GetComponent<Rigidbody2D>().WakeUp();
            GetComponent<Rigidbody2D>().velocity = new Vector3(0f, -0.00001f, 0f);
            alive = true;
            timer.Run();
            timer.Reset();
            timer.Continue();
        }

        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (1 << collider.gameObject.layer == collisionMask.value && alive)
            {
                Death();
            }
        }

        private void Death()
        {
            Debug.Log("death");
            alive = false;
            charging = false;
            leftMovement = true;
            firstChargeDone = false;
            timer.Pause();

            GetComponent<Rigidbody2D>().velocity = Vector3.zero;
            GetComponent<Rigidbody2D>().gravityScale = 0f;
            GetComponent<Rigidbody2D>().Sleep();

            trail.GetComponent<TrailRenderer>().time = 0.0f;
            trail.GetComponent<TrailRenderer>().enabled = false;

            transform.rotation = spawnRotaion;
            transform.position = new Vector3(spawnPosition.x, spawnPosition.y, lockedPlayerZ);

            CameraMovement.cameraFollow = false;
            cm.moveCamTo(new Vector3(spawnPosition.x, spawnPosition.y + Constants.cameraY, transform.position.z), respawnTime);
        }

        //X-Achsen-Spiegelung der Figurenflugbahn
        private void Reflect()
        {
            GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x * (-1), GetComponent<Rigidbody2D>().velocity.y);

            if (leftMovement)
                leftMovement = false;
            else
                leftMovement = true;
        }

        //Gravitationsabbruch, FIgur wird auf der Ebene gehalten und beschleunigt
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
            if (alive)
            {
                Vector2 velocity = transform.GetComponent<Rigidbody2D>().velocity;
                float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

                if (charging && Mathf.Abs(GetComponent<Rigidbody2D>().velocity.x) < maxChargeVelocity)
                {
                    if (leftMovement)
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
            if (alive && CameraMovement.cameraFollow && !CameraMovement.cameraMove)
            {
                //Keyboard
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    Death();
                }
                if (Input.GetKeyDown(KeyCode.M))
                {
                    Reflect();
                }
                if (Input.GetKeyDown(KeyCode.Y) && !charging)
                {
                    Charge();
                }
                else if (Input.GetKeyUp(KeyCode.Y) && charging)
                {
                    Decharge();
                }

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
            }
            else if (!alive && !CameraMovement.cameraFollow && !CameraMovement.cameraMove)
            {
                if (Input.anyKeyDown && !Input.GetKeyDown(KeyCode.Escape))
                {
                    Spawn();
                }
            }
        }
    }
}

/*
 *
 *
*/