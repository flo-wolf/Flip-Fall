using Sliders;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Sliders
{
    public class CameraMovement : MonoBehaviour
    {
        public enum CameraState { following, transitioning, resting }

        public enum TransitionType { smoothstep, linear, resting }

        //Spielfigur für Berechnung der relativen Kameraposition
        public Player player;

        public TransitionType transitionType;
        public static CameraState cameraState;

        public static CameraStateEvent onCameraStateChange = new CameraStateEvent();

        //variables transmitted by an outside function
        private static Vector3 target;

        private static float transitionDuration;

        private void Start()
        {
            cameraState = CameraState.resting;
            Vector3 PlayerPOS = player.transform.transform.position;
            Camera.main.transform.position = new Vector3(PlayerPOS.x, PlayerPOS.y + Constants.cameraY, Camera.main.transform.position.z);
        }

        public static void SetCameraState(CameraState cs)
        {
            cameraState = cs;
            onCameraStateChange.Invoke(cameraState);
        }

        //Smooth camera Transitions, calls Transition()
        public void moveCamTo(Vector2 t, float d)
        {
            target = t;
            target.z = Constants.cameraY;
            transitionDuration = d;

            SetCameraState(CameraState.transitioning);
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
            SetCameraState(CameraState.resting);
            Game.SetGameState(Game.GameState.ready);
        }

        //instead of using transforms rather use joints and only transform one
        private void Update()
        {
            if (cameraState == CameraState.following)
            {
                Vector3 PlayerPOS = player.transform.transform.position;

                //35 wegen der Ballhöhe (455) minus der Camerahöhe (420), die im BallMovement und BTN_PLAY gesetzt werden.
                Camera.main.transform.position = new Vector3(PlayerPOS.x, PlayerPOS.y + Constants.cameraY, Camera.main.transform.position.z);
            }
        }

        //camera info
        public float GetCamHeight()
        {
            return 2f * Camera.main.orthographicSize;
        }

        public float GetCamWidth()
        {
            return GetCamHeight() * Camera.main.aspect;
        }

        public Vector3 GetCamPos()
        {
            return Camera.main.transform.position;
        }

        public static bool IsFollowing()
        {
            if (cameraState == CameraState.following)
                return true;
            else
                return false;
        }

        public static bool IsResting()
        {
            if (cameraState == CameraState.resting)
                return true;
            else
                return false;
        }

        public static bool IsTransitioning()
        {
            if (cameraState == CameraState.transitioning)
                return true;
            else
                return false;
        }

        public class CameraStateEvent : UnityEvent<CameraState> { }
    }
}