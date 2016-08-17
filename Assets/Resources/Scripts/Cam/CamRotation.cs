﻿using Sliders;
using System.Collections;
using UnityEngine;

/// <summary>
/// Camera Rotation Handler, triggered through CameraManager
/// </summary>

namespace Sliders.Cam
{
    public class CamRotation : MonoBehaviour
    {
        public static CamRotation _instance;
        public Camera cam;
        public Player player;

        public float maxRotationAngle = 10F;
        public float defaultRotationAngle = 0F;
        public float rotationSpeed = 0.3F;
        public int rotationCount = 2;

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
                _instance.StartCoroutine(_instance.cVelocityRotation(behaviour.GetComponent<Rigidbody2D>()));
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
                Quaternion memoryRotation = _instance.minRotation;
                _instance.minRotation = _instance.maxRotation;
                _instance.maxRotation = memoryRotation;
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

                cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, maxRotationAngle, lerpTime);

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
                rotation = new Vector3(0, 0, Mathf.SmoothStep(cam.transform.rotation.y, angle, zoomLerpTime));
                cam.transform.Rotate(rotation);

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

                transform.rotation = Quaternion.Slerp(transform.rotation, defaultRotation, t);

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

            while (t < 1F)
            {
                t += Time.deltaTime * (Time.timeScale / duration);
                velocity = rb.velocity;

                currentVelocityRotation = Quaternion.Lerp(minRotation, maxRotation, Mathf.InverseLerp(0, maxVelocity, velocity.magnitude));
                newRotation = Quaternion.Lerp(transform.rotation, currentVelocityRotation, t);
                transform.rotation = newRotation;

                yield return new WaitForFixedUpdate();
            }
            StartCoroutine(cVelocityRotation(rb));
            yield break;
        }

        private IEnumerator cVelocityRotation(Rigidbody2D rb)
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

                transform.rotation = newRotation;

                //Debug.Log("newRotation" + newRotation);

                yield return new WaitForFixedUpdate();
            }
        }
    }
}