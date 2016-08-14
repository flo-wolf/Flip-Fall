using Sliders;
using System.Collections;
using UnityEngine;

namespace Sliders.Cam
{
    public class CamShake : MonoBehaviour
    {
        private Vector3 _originalPos;
        public static CamShake _instance;
        public float shakeDuration;
        public float shakeAmount;
        public float maxChargingShake = 5F;

        public float deathShakeAmount = 5f;
        public float deathShakeDuration = 1f;

        private void Awake()
        {
            _originalPos = transform.transform.position;
            _instance = this;
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
                transform.transform.position = _originalPos + Random.insideUnitSphere * amount;

                duration -= Time.deltaTime;

                yield return null;
            }

            transform.transform.position = _originalPos;
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
                transform.localPosition = _originalPos + Random.insideUnitSphere * amount;
                yield return new WaitForFixedUpdate();
            }
        }
    }
}