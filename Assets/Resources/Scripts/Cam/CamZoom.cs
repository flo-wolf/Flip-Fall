using Sliders;
using System.Collections;
using UnityEngine;

/// <summary>
/// Camera Zoom Handler, triggered through CameraManager
/// </summary>

namespace Sliders.Cam
{
    public class CamZoom : MonoBehaviour
    {
        public static CamZoom _instance;

        public Player player;
        public Camera cam;

        //Zoom camera sizes
        public float maxZoom = 120F;
        public float minZoom = 80F;
        public float deathZoom = 50F;

        //Zooming times
        public float deathZoomDuration = 0.5F;
        public float defaultZoomDuration = 0.5F;
        public float translationZoomDuration = 0.5F;

        //Used for checking
        public float velocityThreshold = 5F;

        private float size;
        private float maxVelocity;

        private void Awake()
        {
            _instance = this;
        }

        private void Start()
        {
            cam.orthographicSize = minZoom;
            size = cam.orthographicSize;
        }

        //Listener
        private void PlayerAction(Player.PlayerAction playerAction)
        {
            switch (playerAction)
            {
                case Player.PlayerAction.charge:
                    break;

                case Player.PlayerAction.decharge:
                    break;

                case Player.PlayerAction.reflect:
                    break;

                default:
                    break;
            }
        }

        //Listener
        private void PlayerStateChanged(Player.PlayerState playerState)
        {
            switch (playerState)
            {
                case Player.PlayerState.alive:
                    StopAllCoroutines();
                    StartCoroutine(cVelocityZoom());
                    break;

                case Player.PlayerState.dead:
                    StopAllCoroutines();
                    StartCoroutine(cDeathZoom());
                    break;

                default:
                    break;
            }
        }

        //zooms in from the current camera size to the minimum camera size
        public static void ZoomToMinimum()
        {
            _instance.StopAllCoroutines();
            _instance.cZoomToMinimum();
        }

        //zooms out from the current camera size to the maximum camera size
        public static void ZoomToMaximum()
        {
            _instance.StopAllCoroutines();
            _instance.cZoomToMaximum();
        }

        //translates camera size to a new value
        public static void Zoom(float size)
        {
            _instance.StopAllCoroutines();
            _instance.cZoom(size);
        }

        public static void Zoom(float size, InterpolationType interpolationType)
        {
            _instance.StopAllCoroutines();
            _instance.cZoom(size, interpolationType);
        }

        public static void DeathZoom()
        {
            _instance.StopAllCoroutines();
            _instance.cDeathZoom();
        }

        public static void TranslateToVelocityZoom()
        {
            _instance.StopAllCoroutines();
            _instance.cTranslateToVelocityZoom();
        }

        private IEnumerator cZoomToMinimum()
        {
            Debug.Log("ZoomIn()");
            float t = 0;
            while (t < 1F)
            {
                t += Time.deltaTime * (Time.timeScale / defaultZoomDuration);
                cam.orthographicSize = Mathf.SmoothStep(cam.orthographicSize, minZoom, t);
                yield return new WaitForFixedUpdate();
            }
            yield break;
        }

        private IEnumerator cZoom(float size)
        {
            float t = 0;
            while (t < 1F)
            {
                t += Time.deltaTime * (Time.timeScale / defaultZoomDuration);

                cam.orthographicSize = Mathf.SmoothStep(cam.orthographicSize, size, t);

                yield return new WaitForFixedUpdate();
            }
            yield break;
        }

        private IEnumerator cZoom(float size, InterpolationType interpolationType)
        {
            float t = 0;
            while (t < 1F)
            {
                t += Time.deltaTime * (Time.timeScale / defaultZoomDuration);

                switch (interpolationType)
                {
                    case InterpolationType.smoothstep:
                        cam.orthographicSize = Mathf.SmoothStep(cam.orthographicSize, size, t);
                        break;

                    case InterpolationType.linear:
                        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, size, t);
                        break;

                    default:
                        cam.orthographicSize = Mathf.SmoothStep(cam.orthographicSize, size, t);
                        break;
                }

                yield return new WaitForFixedUpdate();
            }
            yield break;
        }

        private IEnumerator cZoomToMaximum()
        {
            float t = 0;
            while (t < 1F)
            {
                t += Time.deltaTime * (Time.timeScale / defaultZoomDuration);
                cam.orthographicSize = Mathf.SmoothStep(cam.orthographicSize, maxZoom, t);
                yield return new WaitForFixedUpdate();
            }
            yield break;
        }

        //zooms from the current camera size to the death camera size
        private IEnumerator cDeathZoom()
        {
            float t = 0;
            while (t < 1F)
            {
                t += Time.deltaTime * (Time.timeScale / deathZoomDuration);

                cam.orthographicSize = Mathf.SmoothStep(cam.orthographicSize, deathZoom, t);
                yield return new WaitForFixedUpdate();
            }
            StartCoroutine(cZoomToMinimum());
            yield break;
        }

        //makes a smooth transition betwen the current size and the appropiate velocity size
        private IEnumerator cTranslateToVelocityZoom()
        {
            Vector2 velocity;
            float t = 0;

            while (t < 1F)
            {
                t += Time.deltaTime * (Time.timeScale / translationZoomDuration);
                velocity = player.rBody.velocity;
                velocity.x = System.Math.Abs(velocity.x);

                size = Mathf.SmoothStep(minZoom, maxZoom, Mathf.InverseLerp(cam.orthographicSize, maxVelocity, velocity.magnitude));
                cam.orthographicSize = Mathf.SmoothStep(cam.orthographicSize, size, t);

                yield return new WaitForFixedUpdate();
            }
            StartCoroutine(cVelocityZoom());
        }

        //transforms the camera size according to the players velocity
        private IEnumerator cVelocityZoom()
        {
            maxVelocity = player.maxChargeVelocity;
            Vector2 velocity;

            while (true)
            {
                velocity = player.rBody.velocity;
                velocity.x = System.Math.Abs(velocity.x);

                if (velocity.x > (maxVelocity - velocityThreshold))
                {
                    velocity.x = maxVelocity;
                }

                size = Mathf.SmoothStep(minZoom, maxZoom, Mathf.InverseLerp(cam.orthographicSize, maxVelocity, velocity.magnitude));
                cam.orthographicSize = size;
                yield return new WaitForFixedUpdate();
            }
        }
    }
}