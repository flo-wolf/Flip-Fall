using Sliders;
using System.Collections;
using UnityEngine;

/// <summary>
/// Camera Rotation Handler, triggered through CameraManager
/// </summary>

namespace Sliders.Cam
{
    public class CamRotation : MonoBehaviour
    {
        public enum RotationType { smoothstepDecreasing, linearDecreaing, constant };

        public static CamRotation _instance;
        public Camera cam;
        public Player player;

        public float maxRotationAngle = 10F;
        public float defaultRotationAngle = 0F;
        public float rotationSpeed = 0.3F;
        public int rotationCount = 2;
        public RotationType rotationType;

        private Vector3 rotation;
        private Vector3 playerVelocity;

        private void Awake()
        {
            _instance = this;
        }

        private void Start()
        {
            player.onPlayerStateChange.AddListener(PlayerStateChanged);
            player.onPlayerAction.AddListener(PlayerAction);
        }

        private void PlayerStateChanged(Player.PlayerState playerState)
        {
            switch (playerState)
            {
                case Player.PlayerState.alive:
                    //StopAllCoroutines();
                    //StartCoroutine(SpawnRotation());
                    break;

                case Player.PlayerState.dead:
                    StopAllCoroutines();
                    StartCoroutine(DeathRotation());
                    break;

                default:
                    break;
            }
        }

        private void PlayerAction(Player.PlayerAction playerAction)
        {
            switch (playerAction)
            {
                case Player.PlayerAction.charge:
                    // StopAllCoroutines();
                    // StartCoroutine(TranslateToVelocityZoom());
                    break;

                case Player.PlayerAction.decharge:
                    break;

                case Player.PlayerAction.reflect:
                    break;

                default:
                    break;
            }
        }

        public static void DeathRotation()
        {
            _instance.StopAllCoroutines();
            _instance.StartCoroutine(cDeathRotation());
        }

        private IEnumerator cDeathRotation()
        {
            float lerpTime = 0;
            bool right = true;
            float endTime = Time.time + rotationSpeed;

            while (Time.time < endTime)
            {
                if (right)
                {
                    lerpTime += Time.fixedDeltaTime;
                    if (lerpTime == rotationSpeed)
                    {
                        maxRotationAngle = maxRotationAngle * (-1);
                        right = false;
                        lerpTime = 0;
                    }
                }
                else
                {
                    lerpTime -= Time.fixedDeltaTime;
                }

                cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, maxRotationAngle, lerpTime);

                yield return new WaitForFixedUpdate();
            }

            StartCoroutine(RotateToDefault());
        }

        private IEnumerator RotateToAngle(float angle)
        {
            float zoomLerpTime = 0;
            bool zooming = true;

            while (zooming)
            {
                zoomLerpTime += Time.fixedDeltaTime;
                rotation = new Vector3(0, 0, Mathf.SmoothStep(cam.transform.rotation.y, angle, zoomLerpTime));
                cam.transform.Rotate(rotation);

                if (rotation.z == defaultRotationAngle)
                {
                    yield break;
                }

                yield return new WaitForFixedUpdate();
            }
        }

        private IEnumerator RotateToDefault()
        {
            Debug.Log("RotateToDefault()");
            float zoomLerpTime = 0;
            bool zooming = true;

            while (zooming)
            {
                zoomLerpTime += Time.fixedDeltaTime;
                rotation = new Vector3(0, 0, Mathf.SmoothStep(cam.transform.rotation.y, defaultRotationAngle, zoomLerpTime));
                cam.transform.Rotate(rotation);

                if (rotation.z == defaultRotationAngle)
                {
                    StopCoroutine(RotateToDefault());
                }

                yield return new WaitForFixedUpdate();
            }
        }

        /*
        private IEnumerator SpawnRotation()
        {
            Debug.Log("DeathRotation()");
            float zoomLerpTime = 0;
            bool zooming = true;

            while (true)
            {
                playerVelocity = player.rBody.velocity;
                playerVelocity.x = System.Math.Abs(velocity.x);

                if (playerVelocity.x > (maxVelocity - velocityThreshold))
                {
                    playerVelocity.x = maxVelocity;
                }

                size = Mathf.Lerp(minZoom, maxZoom, Mathf.InverseLerp(cam.orthographicSize, maxVelocity, velocity.magnitude));
                cam.orthographicSize = size;
                yield return new WaitForFixedUpdate();
            }

            StartCoroutine(RotateToDefault());
        }
        */
    }
}