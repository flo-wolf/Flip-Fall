using Sliders;
using System.Collections;
using UnityEngine;

public class CamShake : MonoBehaviour
{
    private Vector3 _originalPos;
    public static CamShake _instance;
    public float shakeDuration;
    public float shakeAmount;

    public float deathShakeAmount = 15;

    private void Awake()
    {
        _originalPos = transform.localPosition;
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
        _instance.StartCoroutine(_instance.cShakeCoroutine(1F, 15F));
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
            transform.localPosition = _originalPos + Random.insideUnitSphere * amount;

            duration -= Time.deltaTime;

            yield return null;
        }

        transform.localPosition = _originalPos;
    }

    //creates increasing shake depending on the players velocity
    public IEnumerator cPlayerChargingShake(Player player)
    {
        float maxVelocity = player.maxChargeVelocity;
        Vector2 velocity;
        float amount;
        float maxAmount;

        while (true)
        {
            velocity = player.rBody.velocity;
            velocity.x = System.Math.Abs(velocity.x);

            amount = Mathf.SmoothStep(0, maxAmount, Mathf.InverseLerp(cam.orthographicSize, maxVelocity, velocity.magnitude));

            transform.localPosition = _originalPos + Random.insideUnitSphere * amount;

            cam.orthographicSize = size;
            yield return new WaitForFixedUpdate();
        }
    }
}