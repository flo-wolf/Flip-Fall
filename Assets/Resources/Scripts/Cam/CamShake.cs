using Sliders;
using System.Collections;
using UnityEngine;

namespace Sliders.Cam
{
    public class CamShake : MonoBehaviour
    {
        public static CamShake _instance;
        public Camera cam;

        [Space(10)]
        public float shakeDuration;
        public float shakeAmount;
        public float maxChargingShake = 5F;

        [Space(10)]
        public float deathShakeAmount = 5f;
        public float deathShakeSpeed = 5f;
        public float deathShakeDuration = 1f;

        private Vector3 originalPos;

        private void Awake()
        {
            _instance = this;
            Debug.Log("awake");
        }

        public static void Shake()
        {
            _instance.StopAllCoroutines();
            _instance.StartCoroutine(_instance.cShakeCoroutine(_instance.shakeDuration, _instance.shakeAmount));
        }

        public static void Shake(float duration, float amount)
        {
            _instance.StopAllCoroutines();
            _instance.StartCoroutine(_instance.cShakeCoroutine(duration, amount));
        }

        public static void DeathShake()
        {
            _instance.StopAllCoroutines();
            _instance.StartCoroutine(_instance.cShakeCoroutine(_instance.deathShakeDuration, _instance.deathShakeAmount));
        }

        public static void PlayerChargingShake(Player player)
        {
            _instance.StopAllCoroutines();
            _instance.StartCoroutine(_instance.cPlayerChargingShake(player));
        }

        public IEnumerator cShakeCoroutine(float duration, float amount)
        {
            float endTime = Time.time + duration;

            while (Time.time < endTime)
            {
                cam.transform.position = originalPos + Random.insideUnitSphere * amount;

                duration -= Time.deltaTime;

                yield return null;
            }

            cam.transform.position = originalPos;
        }

        //creates increasing shake depending on the players velocity
        public IEnumerator cPlayerChargingShake(Player player)
        {
            float maxVelocity = player.maxChargeVelocity;
            Vector2 velocity;
            float amount;

            while (true)
            {
                velocity = player.rBody.velocity;
                velocity.x = System.Math.Abs(velocity.x);

                //change 1 to a higher value for shake strength changes
                amount = Mathf.SmoothStep(0, maxChargingShake, Mathf.InverseLerp(0, maxVelocity, velocity.magnitude));
                cam.transform.position = originalPos + Random.insideUnitSphere * amount;
                yield return new WaitForFixedUpdate();
            }
        }
    }
}