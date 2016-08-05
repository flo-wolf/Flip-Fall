using Sliders;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Sliders
{
    public class CameraMovement : MonoBehaviour
    {
        public enum CameraState { following, transitioning, resting }

        public enum InterpolationType { smoothstep, linear, smootherstep, sin }

        //Spielfigur für Berechnung der relativen Kameraposition
        public Player player;

        public InterpolationType interpolationType;
        public AudioClip transitionSound;

        public static CameraState cameraState;
        public static CameraStateEvent onCameraStateChange = new CameraStateEvent();
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
            SoundManager.instance.RandomizeSfx(transitionSound);

            switch (interpolationType)
            {
                case InterpolationType.linear:
                    while (t < 1.0f)
                    {
                        t += Time.deltaTime * (Time.timeScale / transitionDuration);
                        Camera.main.transform.position = Vector3.Lerp(startingPos, target, t);
                        yield return 0;
                    }
                    break;

                case InterpolationType.sin:
                    while (t < 1.0f)
                    {
                        t += Time.deltaTime * (Time.timeScale / transitionDuration);
                        float x = Mathf.Sin(t) + 1;
                        Camera.main.transform.position = Vector3.Lerp(startingPos, target, x);
                        yield return 0;
                    }
                    break;

                case InterpolationType.smoothstep:
                    while (t < 1.0f)
                    {
                        t += Time.deltaTime * (Time.timeScale / transitionDuration);

                        float x = t / 1;
                        x = x * x * (3f - 2f * t); //Smoothstep formula: t = t*t * (3f - 2f*t)

                        Camera.main.transform.position = Vector3.Lerp(startingPos, target, x);
                        yield return 0;
                    }
                    break;

                case InterpolationType.smootherstep:
                    while (t < 1.0f)
                    {
                        t += Time.deltaTime * (Time.timeScale / transitionDuration);
                        Camera.main.transform.position = Vector3.Lerp(startingPos, target, Mathf.SmoothStep(0, 1, t));
                        yield return 0;
                    }
                    break;

                default:
                    break;
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