using Impulse;
using System.Collections;
using UnityEngine;

/// <summary>
/// Camera Zoom Handler, triggered through CameraManager
/// </summary>

namespace Impulse.Cam
{
    public class CamZoom : MonoBehaviour
    {
        public static CamZoom _instance;
        public Camera cam;

        //Zoom camera sizes
        public float maxZoom = 120F;
        public float minZoom = 80F;
        public float deathZoom = 1F;
        public float winZoom = 1F;

        private float size;
        private float maxVelocity;

        private void Awake()
        {
            _instance = this;
        }

        private void Start()
        {
            size = cam.orthographicSize;
        }

        //zooms in from the current camera size to the minimum camera size
        public static void ZoomToMinimum(float duration)
        {
            _instance.StopAllCoroutines();
            _instance.StartCoroutine(_instance.cZoomToMinimum(duration));
        }

        //zooms out from the current camera size to the maximum camera size
        public static void ZoomToMaximum(float duration)
        {
            _instance.StopAllCoroutines();
            _instance.StartCoroutine(_instance.cZoomToMaximum(duration));
        }

        //translates camera size to a new value
        public static void Zoom(float size, float duration)
        {
            _instance.StopAllCoroutines();
            _instance.StartCoroutine(_instance.cZoom(size, duration));
        }

        public static void Zoom(float size, float duration, InterpolationType interpolationType)
        {
            _instance.StopAllCoroutines();
            _instance.StartCoroutine(_instance.cZoom(size, duration, interpolationType));
        }

        public static void DeathZoom(float duration)
        {
            Zoom(_instance.deathZoom, duration);
        }

        //Adjust the current camera size smoothly to its according velocity size, calculated through the behaviour's velocity
        public static void ZoomToVelocity(MonoBehaviour behaviour, float duration)
        {
            if (behaviour != null && behaviour.GetComponent<Rigidbody2D>() != null)
            {
                _instance.StopAllCoroutines();
                _instance.StartCoroutine(_instance.cZoomToVelocity(behaviour.GetComponent<Rigidbody2D>(), duration));
            }
            else
                Debug.LogError("[ZoomToVelocity] cant find rigidbody2D on gameobject");
        }

        public IEnumerator cZoomToMinimum(float duration)
        {
            float t = 0;
            while (t < 1F)
            {
                t += Time.deltaTime * (Time.timeScale / duration);
                cam.orthographicSize = Mathf.SmoothStep(cam.orthographicSize, minZoom, t);
                yield return new WaitForFixedUpdate();
            }
            yield break;
        }

        public IEnumerator cZoom(float size, float duration)
        {
            float t = 0;
            while (t < 1F)
            {
                t += Time.deltaTime * (Time.timeScale / duration);

                cam.orthographicSize = Mathf.SmoothStep(cam.orthographicSize, size, t);

                yield return new WaitForFixedUpdate();
            }
            yield break;
        }

        public IEnumerator cZoom(float size, float duration, InterpolationType interpolationType)
        {
            float t = 0;
            while (t < 1F)
            {
                t += Time.deltaTime * (Time.timeScale / duration);

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

        public IEnumerator cZoomToMaximum(float duration)
        {
            float t = 0;
            while (t < 1F)
            {
                t += Time.deltaTime * (Time.timeScale / duration);
                cam.orthographicSize = Mathf.SmoothStep(cam.orthographicSize, maxZoom, t);
                yield return new WaitForFixedUpdate();
            }
            yield break;
        }

        ////zooms from the current camera size to the death camera size
        //public IEnumerator cZoom(float duration)
        //{
        //    float t = 0;
        //    while (t < 1F)
        //    {
        //        t += Time.deltaTime * (Time.timeScale / duration);

        //        cam.orthographicSize = Mathf.SmoothStep(cam.orthographicSize, deathZoom, t);
        //        yield return new WaitForFixedUpdate();
        //    }
        //    yield break;
        //}

        //makes a smooth transition betwen the current size and the appropiate velocity size
        public IEnumerator cZoomToVelocity(Rigidbody2D rb, float duration)
        {
            maxVelocity = Player._instance.maxChargeVelocity;
            maxVelocity = 200;
            Vector2 velocity;
            float t = 0;

            while (t < 1F)
            {
                t += Time.deltaTime * (Time.timeScale / duration);
                velocity = rb.velocity;
                //velocity.x = System.Math.Abs(velocity.x);

                size = Mathf.Lerp(minZoom, maxZoom, Mathf.InverseLerp(minZoom, maxVelocity, velocity.magnitude));
                cam.orthographicSize = Mathf.SmoothStep(cam.orthographicSize, size, t);

                yield return new WaitForFixedUpdate();
            }
            StartCoroutine(cVelocityZoom(rb));
        }

        //transforms the camera size according to the players velocity
        public IEnumerator cVelocityZoom(Rigidbody2D rb)
        {
            maxVelocity = Player._instance.maxChargeVelocity;
            maxVelocity = 200;

            Vector2 velocity;

            while (true)
            {
                velocity = rb.velocity;
                velocity.x = System.Math.Abs(velocity.x);

                if (velocity.x > (maxVelocity - Constants.velocityThreshhold))
                {
                    velocity.x = maxVelocity;
                }

                size = Mathf.Lerp(minZoom, maxZoom, Mathf.InverseLerp(minZoom, maxVelocity, velocity.magnitude));
                cam.orthographicSize = size;
                yield return new WaitForFixedUpdate();
            }
        }
    }
}