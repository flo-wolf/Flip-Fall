using System.Collections;
using UnityEngine;

namespace Impulse
{
    public class SmoothMovement : MonoBehaviour
    {
        public static bool movingSmooth = false;

        private static GameObject gameObject;
        private static Vector3 target;
        private static float transitionDuration;

        public void moveGameObject(GameObject go, Vector3 t, float d)
        {
            movingSmooth = true;

            gameObject = go;
            target = t;
            transitionDuration = d;

            StartCoroutine(Transition());
        }

        private IEnumerator Transition()
        {
            float t = 0.0f;
            Vector3 startingPos = gameObject.transform.position;

            while (t < 1.0f)
            {
                t += Time.deltaTime * (Time.timeScale / transitionDuration);

                float accelTime = t / 1;
                accelTime = accelTime * accelTime * (3f - 2f * t); //Smoothstep formula: t = t*t * (3f - 2f*t)

                gameObject.transform.position = Vector3.Lerp(startingPos, target, accelTime);
                yield return 0;
            }

            movingSmooth = false;
            Debug.Log("Smooth Transition completed.");
        }
    }
}