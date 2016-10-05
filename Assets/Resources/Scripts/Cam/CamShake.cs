using Impulse;
using System.Collections;
using UnityEngine;

namespace Impulse.Cam
{
    public class CamShake : MonoBehaviour
    {
        public static CamShake _instance;
        public Camera[] cams;

        [Header("Shake Settings")]
        // shake while charging
        public float maxChargingShake = 5F;

        // shake on death
        public float deathShakeAmount = 10f;
        public float deathShakeDuration = 0.2f;

        // maximum shake while inside attractor
        public float attractorShake = 10F;

        // player distance defining strength of shake (0=no shake, 1=attractorShake)
        public static float attractorDistance = 0F;

        private void Awake()
        {
            _instance = this;
            Debug.Log("awake");
        }

        public static void Shake(float amount, float duration)
        {
            _instance.StopAllCoroutines();
            _instance.StartCoroutine(_instance.cShakeCoroutine(amount, duration));
        }

        public static void DeathShake()
        {
            Shake(_instance.deathShakeAmount, _instance.deathShakeDuration);
            _instance.StartCoroutine(_instance.cShakeCoroutine(_instance.deathShakeDuration, _instance.deathShakeAmount));
        }

        public static void AttractorShake()
        {
            _instance.StopAllCoroutines();
            _instance.StartCoroutine(_instance.cAttractorShake());
        }

        public static void AttractorShakeBreak()
        {
            _instance.StopCoroutine(_instance.cAttractorShake());
            _instance.transform.position = Vector3.zero;
        }

        public static void VelocityShake(MonoBehaviour behaviour)
        {
            if (behaviour != null && behaviour.GetComponent<Rigidbody2D>() != null)
            {
                _instance.StopAllCoroutines();
                _instance.StartCoroutine(_instance.cVelocityShake(behaviour.GetComponent<Rigidbody2D>()));
            }
            else
                Debug.LogError("[VelocityShake] cant find rigidbody2D on gameobject");
        }

        public IEnumerator cShakeCoroutine(float amount, float duration)
        {
            float endTime = Time.time + duration;
            Vector3 originalPos = Vector3.zero;
            Vector3 newPos = Vector3.zero;

            while (Time.time < endTime)
            {
                originalPos = cams[0].transform.position;
                newPos = originalPos + Random.insideUnitSphere * amount;
                newPos.z = originalPos.z;

                foreach (Camera cam in cams)
                {
                    cam.transform.position = newPos;
                }

                duration -= Time.deltaTime;

                yield return null;
            }

            foreach (Camera cam in cams)
            {
                cam.transform.position = originalPos;
            }
        }

        //creates increasing shake depending on the rigidbody's velocity
        public IEnumerator cVelocityShake(Rigidbody2D rb)
        {
            float maxVelocity = Player._instance.maxChargeVelocity;
            Vector2 velocity;
            Vector3 originalPos = Vector3.zero;
            Vector3 newPos = Vector3.zero;

            float amount;

            while (true)
            {
                originalPos = cams[0].transform.position;
                velocity = rb.velocity;
                velocity.x = System.Math.Abs(velocity.x);

                if (velocity.x > (maxVelocity - Constants.velocityThreshhold))
                {
                    velocity.x = maxVelocity;
                }

                amount = Mathf.SmoothStep(0, maxChargingShake, Mathf.InverseLerp(0, maxVelocity, velocity.magnitude));
                newPos = originalPos + Random.insideUnitSphere * amount;
                newPos.z = 0;

                foreach (Camera cam in cams)
                {
                    cam.transform.position = newPos;
                }

                yield return new WaitForFixedUpdate();
            }
        }

        // creates increasing shake depending on the players distance to the center of the attractor
        public IEnumerator cAttractorShake()
        {
            Debug.Log("shake shake shake");
            Vector3 originalPos = Vector3.zero;
            Vector3 newPos = Vector3.zero;

            while (true)
            {
                originalPos = Vector3.zero;

                newPos = originalPos + Random.insideUnitSphere * (attractorShake * attractorDistance);

                newPos.z = 0;

                transform.localPosition = newPos;
                Debug.Log(transform.localPosition);

                yield return new WaitForFixedUpdate();
            }
        }
    }
}