using System.Collections;

using System.Collections;

using UnityEngine;

using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private Vector3 _originalPos;
    public static CameraShake _instance;
    public float shakeDuration;
    public float shakeAmount;

    private void Awake()
    {
        _originalPos = transform.localPosition;

        _instance = this;
    }

    public static void Shake()
    {
        _instance.StopAllCoroutines();
        _instance.StartCoroutine(_instance.cShake(_instance.shakeDuration, _instance.shakeAmount));
    }

    public static void Shake(float duration, float amount)
    {
        _instance.StopAllCoroutines();
        _instance.StartCoroutine(_instance.cShake(duration, amount));
    }

    public IEnumerator cShake(float duration, float amount)
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
}