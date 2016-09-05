using Impulse.Cam;
using Impulse.Levels;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Impulse
{
    public class Ghost : MonoBehaviour
    {
        public enum GhostState { alive, dead, ready, finished };

        public GhostStateChangeEvent onGhostStateChange = new GhostStateChangeEvent();

        //public Player instance;
        public GhostState ghostState;
        public TrailRenderer trail; //Full color
        public TrailRenderer trail2; //Transparency - use shading instead
        public Rigidbody2D rBody;
        public Material defaultGhostMaterial;
        public Material winGhostMaterial;
        public double time;

        private Spawn spawn;
        private Quaternion spawnRotaion;
        private Vector3 spawnPosition;
        private int speed;
        private bool facingLeft = true;
        private bool charging = false;
        private Vector2 chargeVelocity;
        private bool firstChargeDone = false;

        private void Start()
        {
            rBody = Player._instance.rBody;
            ReloadSpawnPoint();
            MoveToSpawn();
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
            spawn = LevelManager.GetSpawn();
            spawnPosition = spawn.GetPosition();
            spawnPosition.z = Constants.ghostZ;
            facingLeft = spawn.facingLeftOnSpawn;
        }

        //Whenever the Level gets changed the LevelManager fires the LevelChangeEvent, calling this method.
        //Resets the Player to the Spawn
        private void LevelChanged(Level level)
        {
            ReloadSpawnPoint();
            MoveToSpawn();
        }

        private void SwitchFacingDirection()
        {
            facingLeft = !facingLeft;
        }

        private void Spawn()
        {
            SetGhostState(GhostState.alive);
            trail.time = 0.5f;
            trail.enabled = true;
            trail2.time = 1f;
            trail2.enabled = true;

            rBody.gravityScale = Player._instance.gravity;
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
            gameObject.GetComponent<MeshRenderer>().material = winGhostMaterial;
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
            gameObject.GetComponent<MeshRenderer>().material = defaultGhostMaterial;
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
            rBody.gravityScale = Player._instance.gravity;
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

                if (charging && Mathf.Abs(rBody.velocity.x) < Player._instance.maxChargeVelocity)
                {
                    if (facingLeft)
                    {
                        velocity.x = velocity.x - Player._instance.chargeForcePerTick;
                    }
                    else
                    {
                        velocity.x = velocity.x + Player._instance.chargeForcePerTick;
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