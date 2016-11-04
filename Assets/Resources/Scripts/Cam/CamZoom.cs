using FlipFall;
using System.Collections;
using UnityEngine;

/// <summary>
/// Controls the zoom of an array of cameras simultaniously
/// </summary>

namespace FlipFall.Cam
{
    public class CamZoom : MonoBehaviour
    {
        public static CamZoom _instance;
        public Camera[] cams;

        // velocity zoom camera sizes
        public float maxZoom = 120F;
        public float minZoom = 80F;

        // death zoom, translates to first and then to second
        public float deathZoomFirst = 50F;
        public float deathZoomSecond = 300F;

        // win zoom, translates to first and then to second
        public float winZoomFirst = 300F;
        public float winZoomSecond = 50F;

        private float size;
        private float maxVelocity;

        private void Awake()
        {
            _instance = this;
        }

        private void Start()
        {
            size = cams[0].orthographicSize;
        }

        //zooms in from the current camera size to the minimum camera size
        public static void ZoomToMinimum(float duration)
        {
            _instance.StopAllCoroutines();
            _instance.StartCoroutine(_instance.cZoom(_instance.minZoom, duration));
        }

        //zooms out from the current camera size to the maximum camera size
        public static void ZoomToMaximum(float duration)
        {
            _instance.StopAllCoroutines();
            _instance.StartCoroutine(_instance.cZoom(_instance.maxZoom, duration));
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
            ZoomDouble(_instance.deathZoomFirst, _instance.deathZoomSecond, duration);
        }

        public static void WinZoom(float duration)
        {
            ZoomDouble(_instance.winZoomFirst, _instance.winZoomSecond, duration);
        }

        public static void ZoomDouble(float size1, float size2, float duration)
        {
            _instance.StopAllCoroutines();
            _instance.StartCoroutine(_instance.cZoomDouble(size1, size2, duration));
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

        public IEnumerator cZoom(float size, float duration)
        {
            float t = 0;
            while (t < 1F)
            {
                t += Time.deltaTime * (Time.timeScale / duration);

                foreach (Camera cam in cams)
                {
                    cam.orthographicSize = Mathf.SmoothStep(cam.orthographicSize, size, t);
                }

                yield return new WaitForFixedUpdate();
            }
            yield break;
        }

        public IEnumerator cZoomDouble(float size1, float size2, float duration)
        {
            float distance1 = Mathf.Abs(cams[0].orthographicSize - size1);
            float distance2 = Mathf.Abs(size1 - size2);

            float duration1 = (distance1 / (distance1 + distance2)) / 2;
            float duration2 = (distance2 / (distance1 + distance2)) / 2;

            float t = 0;
            while (t < 1F)
            {
                t += Time.deltaTime * (Time.timeScale / duration1);

                foreach (Camera cam in cams)
                {
                    cam.orthographicSize = Mathf.SmoothStep(cam.orthographicSize, size1, t);
                }

                yield return new WaitForFixedUpdate();
            }

            t = 0;
            while (t < 1F)
            {
                t += Time.deltaTime * (Time.timeScale / duration2);

                foreach (Camera cam in cams)
                {
                    cam.orthographicSize = Mathf.SmoothStep(cam.orthographicSize, size2, t);
                }

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
                        foreach (Camera cam in cams)
                        {
                            cam.orthographicSize = Mathf.SmoothStep(cam.orthographicSize, size, t);
                        }

                        break;

                    case InterpolationType.linear:
                        foreach (Camera cam in cams)
                        {
                            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, size, t);
                        }

                        break;

                    default:
                        foreach (Camera cam in cams)
                        {
                            cam.orthographicSize = Mathf.SmoothStep(cam.orthographicSize, size, t);
                        }

                        break;
                }

                yield return new WaitForFixedUpdate();
            }
            yield break;
        }

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

                foreach (Camera cam in cams)
                {
                    cam.orthographicSize = Mathf.SmoothStep(cam.orthographicSize, size, t);
                }

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

                foreach (Camera cam in cams)
                {
                    cam.orthographicSize = size;
                }

                yield return new WaitForFixedUpdate();
            }
        }
    }
}