using Sliders;
using System.Collections;
using UnityEngine;

namespace Sliders
{
    public class CameraMovement : MonoBehaviour
    {
        //Spielfigur für Berechnung der relativen Kameraposition
        public GameObject player;

        //variables transmitted by an outside function
        private static Vector3 target;

        private static float transitionDuration;

        //Die Kamera wird flüssig von A nach B bewegt
        public static bool cameraMove = false;

        //Die Kamera folgt der Spielfigur
        public static bool cameraFollow = false;

        //camera info
        public float getCamHeight()
        {
            return 2f * Camera.main.orthographicSize;
        }

        public float getCamWidth()
        {
            return getCamHeight() * Camera.main.aspect;
        }

        public Vector3 getCamPos()
        {
            return Camera.main.transform.position;
        }

        private void Start()
        {
            Vector3 PlayerPOS = player.transform.transform.position;
            Camera.main.transform.position = new Vector3(PlayerPOS.x, PlayerPOS.y + Constants.cameraY, Camera.main.transform.position.z);
        }

        //Smooth camera Transitions, calls Transition()
        public void moveCamTo(Vector3 t, float d)
        {
            cameraMove = true;
            target = t;
            target.z = Constants.cameraY;
            transitionDuration = d;

            StartCoroutine(Transition());
        }

        private IEnumerator Transition()
        {
            float t = 0.0f;
            Vector3 startingPos = Camera.main.transform.position;

            while (t < 1.0f)
            {
                t += Time.deltaTime * (Time.timeScale / transitionDuration);

                float accelTime = t / 1;
                accelTime = accelTime * accelTime * (3f - 2f * t); //Smoothstep formula: t = t*t * (3f - 2f*t)

                Camera.main.transform.position = Vector3.Lerp(startingPos, target, accelTime);
                yield return 0;
            }

            cameraMove = false;
            Debug.Log("Camera Transition completed.");
        }

        //instead of using transforms rather use joints and only transform one
        private void Update()
        {
            if (cameraFollow && !cameraMove)
            {
                Vector3 PlayerPOS = player.transform.transform.position;

                //35 wegen der Ballhöhe (455) minus der Camerahöhe (420), die im BallMovement und BTN_PLAY gesetzt werden.
                Camera.main.transform.position = new Vector3(PlayerPOS.x, PlayerPOS.y + Constants.cameraY, Camera.main.transform.position.z);
            }
        }
    }
}