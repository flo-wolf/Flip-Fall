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

        //zooms in from the current camera size to the minimum camera size
        public static void ZoomToMinimum()
        {
            _instance.StopAllCoroutines();
            _instance.StartCoroutine(_instance.cZoomToMinimum());
        }

        //zooms out from the current camera size to the maximum camera size
        public static void ZoomToMaximum()
        {
            _instance.StopAllCoroutines();
            _instance.StartCoroutine(_instance.cZoomToMaximum());
        }

        //translates camera size to a new value
        public static void Zoom(float size)
        {
            _instance.StopAllCoroutines();
            _instance.StartCoroutine(_instance.cZoom(size));
        }

        public static void Zoom(float size, InterpolationType interpolationType)
        {
            _instance.StopAllCoroutines();
            _instance.StartCoroutine(_instance.cZoom(size, interpolationType));
        }

        public static void DeathZoom()
        {
            Debug.Log("dzoom and instance: " + _instance);
            _instance.StopAllCoroutines();
            _instance.StartCoroutine(_instance.cDeathZoom());
        }

        //Adjust the current camera size smoothly to its according velocity size, calculated through the behaviour's velocity
        public static void ZoomToVelocity(MonoBehaviour behaviour)
        {
            if (behaviour != null && behaviour.GetComponent<Rigidbody2D>() != null)
            {
                _instance.StopAllCoroutines();
                _instance.StartCoroutine(_instance.cZoomToVelocity(behaviour.GetComponent<Rigidbody2D>()));
            }
            else
                Debug.LogError("[ZoomToVelocity] cant find rigidbody2D on gameobject");
        }

        public IEnumerator cZoomToMinimum()
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

        public IEnumerator cZoom(float size)
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

        public IEnumerator cZoom(float size, InterpolationType interpolationType)
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

        public IEnumerator cZoomToMaximum()
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
        public IEnumerator cDeathZoom()
        {
            Debug.Log("cDeathZoom");
            float t = 0;
            while (t < 1F)
            {
                t += Time.deltaTime * (Time.timeScale / deathZoomDuration);

                cam.orthographicSize = Mathf.SmoothStep(cam.orthographicSize, deathZoom, t);
                yield return new WaitForFixedUpdate();
            }
            yield break;
        }

        //makes a smooth transition betwen the current size and the appropiate velocity size
        public IEnumerator cZoomToVelocity(Rigidbody2D rb)
        {
            Vector2 velocity;
            float t = 0;

            while (t < 1F)
            {
                t += Time.deltaTime * (Time.timeScale / translationZoomDuration);
                velocity = rb.velocity;
                velocity.x = System.Math.Abs(velocity.x);

                size = Mathf.SmoothStep(minZoom, maxZoom, Mathf.InverseLerp(cam.orthographicSize, maxVelocity, velocity.magnitude));
                cam.orthographicSize = Mathf.SmoothStep(cam.orthographicSize, size, t);

                yield return new WaitForFixedUpdate();
            }
            StartCoroutine(cVelocityZoom(rb));
        }

        //transforms the camera size according to the players velocity
        public IEnumerator cVelocityZoom(Rigidbody2D rb)
        {
            maxVelocity = Player._instance.maxChargeVelocity;
            Vector2 velocity;

            while (true)
            {
                velocity = rb.velocity;
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