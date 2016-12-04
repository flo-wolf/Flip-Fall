using FlipFall;
using System.Collections;
using UnityEngine;

/// <summary>
/// Camera Rotation Handler, triggered through CameraManager
/// </summary>

namespace FlipFall.Cam
{
    public class CamRotation : MonoBehaviour
    {
        public enum RotationType { defaultToMax, minToMax }
        public static CamRotation _instance;
        public Camera[] cams;
        private Player player;

        [Header("Settings")]
        public RotationType rotationType;
        public float maxRotationAngle = 10F;
        public float defaultRotationAngle = 0F;
        public float rotationSpeed = 0.3F;

        private Quaternion minRotation;
        private Quaternion maxRotation;

        private Vector3 rotation;
        private Vector3 playerVelocity;

        private void Awake()
        {
            _instance = this;
            minRotation = Quaternion.AngleAxis(-maxRotationAngle, Vector3.forward);
            maxRotation = Quaternion.AngleAxis(maxRotationAngle, Vector3.forward);
        }

        private void Start()
        {
            player = Player._instance;
        }

        public static void DeathRotation()
        {
            //_instance.StopAllCoroutines();
            //_instance.StartCoroutine(_instance.cDeathRotation());
        }

        public static void RotateToDefault(float duration)
        {
            _instance.StopAllCoroutines();
            _instance.StartCoroutine(_instance.cRotateToDefault(duration));
        }

        public static void RotateToVelocity(MonoBehaviour behaviour, float duration)
        {
            if (behaviour != null && behaviour.GetComponent<Rigidbody2D>() != null)
            {
                _instance.StopAllCoroutines();
                _instance.StartCoroutine(_instance.cRotateToVelocity(behaviour.GetComponent<Rigidbody2D>(), duration));
            }
            else
                Debug.LogError("[VelocityShake] cant find rigidbody2D on gameobject");
        }

        public static void VelocityRotation(MonoBehaviour behaviour)
        {
            if (behaviour != null && behaviour.GetComponent<Rigidbody2D>() != null)
            {
                _instance.StopAllCoroutines();

                if (_instance.rotationType == RotationType.minToMax)
                    _instance.StartCoroutine(_instance.cVelocityRotationMinMax(behaviour.GetComponent<Rigidbody2D>()));
                else
                    _instance.StartCoroutine(_instance.cVelocityRotationDefMax(behaviour.GetComponent<Rigidbody2D>()));
            }
            else
                Debug.LogError("[VelocityShake] cant find rigidbody2D on gameobject");
        }

        public static void ReflectRotationSwitch(MonoBehaviour behaviour, float duration)
        {
            Debug.Log("Reflect");
            if (behaviour != null && behaviour.GetComponent<Rigidbody2D>() != null)
            {
                _instance.StopAllCoroutines();

                Quaternion memoryRotation;

                // movement to the right
                if (Player._instance.rBody.velocity.x > 0)
                {
                    if (_instance.maxRotation != Quaternion.AngleAxis(_instance.maxRotationAngle, Vector3.forward))
                    {
                        memoryRotation = _instance.minRotation;
                        _instance.minRotation = _instance.maxRotation;
                        _instance.maxRotation = memoryRotation;
                    }
                }
                // movement to the left
                else
                {
                    if (_instance.maxRotation != Quaternion.AngleAxis(-_instance.maxRotationAngle, Vector3.forward))
                    {
                        memoryRotation = _instance.maxRotation;
                        _instance.maxRotation = _instance.minRotation;
                        _instance.minRotation = memoryRotation;
                    }
                }
                _instance.StartCoroutine(_instance.cRotateToVelocity(behaviour.GetComponent<Rigidbody2D>(), duration));
            }
        }

        private IEnumerator cDeathRotation(float duration)
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

                foreach (Camera cam in cams)
                {
                    cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, maxRotationAngle, lerpTime);
                }

                yield return new WaitForFixedUpdate();
            }

            StartCoroutine(cRotateToDefault(duration));
        }

        private IEnumerator cRotateToAngle(float angle)
        {
            float zoomLerpTime = 0;
            bool zooming = true;

            while (zooming)
            {
                zoomLerpTime += Time.fixedDeltaTime;
                rotation = new Vector3(0, 0, Mathf.SmoothStep(cams[0].transform.rotation.y, angle, zoomLerpTime));

                foreach (Camera cam in cams)
                {
                    cam.transform.Rotate(rotation);
                }

                if (rotation.z == defaultRotationAngle)
                {
                    yield break;
                }

                yield return new WaitForFixedUpdate();
            }
        }

        //rotates to the default angle
        private IEnumerator cRotateToDefault(float duration)
        {
            float t = 0;

            Quaternion defaultRotation = Quaternion.AngleAxis(defaultRotationAngle, Vector3.forward);

            while (t < 1F)
            {
                t += Time.deltaTime * (Time.timeScale / duration);

                foreach (Camera cam in cams)
                {
                    cam.transform.rotation = Quaternion.Slerp(cams[0].transform.rotation, defaultRotation, t);
                }

                yield return new WaitForFixedUpdate();
            }
            yield break;
        }

        public IEnumerator cRotateToVelocity(Rigidbody2D rb, float duration)
        {
            float maxVelocity = Player._instance.maxChargeVelocity;
            Vector2 velocity;
            float t = 0;

            Quaternion currentVelocityRotation;
            Quaternion newRotation;
            Quaternion defaultRotation = Quaternion.AngleAxis(defaultRotationAngle, Vector3.forward);

            while (t < 1F)
            {
                t += Time.deltaTime * (Time.timeScale / duration);
                velocity = rb.velocity;

                if (rotationType == RotationType.defaultToMax)
                    currentVelocityRotation = Quaternion.Lerp(defaultRotation, maxRotation, Mathf.SmoothStep(0, 1, Mathf.InverseLerp(0, maxVelocity, Mathf.Abs(velocity.x))));
                else
                    currentVelocityRotation = Quaternion.Lerp(minRotation, maxRotation, Mathf.SmoothStep(0, 1, Mathf.InverseLerp(0, maxVelocity, Mathf.Abs(velocity.x))));

                newRotation = Quaternion.Lerp(cams[0].transform.rotation, currentVelocityRotation, t);

                foreach (Camera cam in cams)
                {
                    cam.transform.rotation = newRotation;
                }

                yield return new WaitForFixedUpdate();
            }

            if (rotationType == RotationType.minToMax)
                StartCoroutine(cVelocityRotationMinMax(rb));
            else
                StartCoroutine(cVelocityRotationDefMax(rb));

            yield break;
        }

        private IEnumerator cVelocityRotationDefMax(Rigidbody2D rb)
        {
            float maxVelocity = Player._instance.maxChargeVelocity;
            Vector2 velocity;

            Quaternion defaultRotation = Quaternion.AngleAxis(defaultRotationAngle, Vector3.forward);
            Quaternion minRotation = Quaternion.AngleAxis(-maxRotationAngle, Vector3.forward);
            Quaternion maxRotation = Quaternion.AngleAxis(maxRotationAngle, Vector3.forward);
            Quaternion newRotation;

            while (true)
            {
                velocity = rb.velocity;
                //velocity.x = System.Math.Abs(velocity.x);

                if (velocity.x > (maxVelocity - Constants.velocityThreshhold))
                {
                    //velocity.x = maxVelocity;
                }

                if (velocity.x != 0)
                {
                    newRotation = Quaternion.Lerp(defaultRotation, maxRotation, Mathf.SmoothStep(0, 1, Mathf.InverseLerp(0, maxVelocity, Mathf.Abs(velocity.x))));
                }
                else
                {
                    newRotation = Quaternion.Lerp(defaultRotation, minRotation, Mathf.SmoothStep(0, 1, Mathf.InverseLerp(0, maxVelocity, Mathf.Abs(velocity.x))));
                }

                //change Lerp to Smoothstep, since min and max are fixed values
                //transform.rotation = Quaternion.Lerp(minRotation, maxRotation, Mathf.InverseLerp(0, maxVelocity, velocity.magnitude));
                foreach (Camera cam in cams)
                {
                    cam.transform.rotation = newRotation;
                }

                //Debug.Log("newRotation" + newRotation);

                yield return new WaitForFixedUpdate();
            }
        }

        private IEnumerator cVelocityRotationMinMax(Rigidbody2D rb)
        {
            float maxVelocity = Player._instance.maxChargeVelocity;
            Vector2 velocity;
            Quaternion minRotation = Quaternion.AngleAxis(-maxRotationAngle, Vector3.forward);
            Quaternion maxRotation = Quaternion.AngleAxis(maxRotationAngle, Vector3.forward);
            Quaternion newRotation;

            while (true)
            {
                velocity = rb.velocity;
                //velocity.x = System.Math.Abs(velocity.x);

                if (velocity.x > (maxVelocity - Constants.velocityThreshhold))
                {
                    //velocity.x = maxVelocity;
                }

                if (velocity.x < 0)
                {
                    newRotation = Quaternion.Lerp(minRotation, maxRotation, Mathf.SmoothStep(0, 1, Mathf.InverseLerp(0, maxVelocity, velocity.magnitude)));
                }
                else
                {
                    newRotation = Quaternion.Lerp(maxRotation, minRotation, Mathf.SmoothStep(0, 1, Mathf.InverseLerp(0, maxVelocity, velocity.magnitude)));
                }

                //change Lerp to Smoothstep, since min and max are fixed values
                //transform.rotation = Quaternion.Lerp(minRotation, maxRotation, Mathf.InverseLerp(0, maxVelocity, velocity.magnitude));

                foreach (Camera cam in cams)
                {
                    cam.transform.rotation = newRotation;
                }

                yield return new WaitForFixedUpdate();
            }
        }
    }
}