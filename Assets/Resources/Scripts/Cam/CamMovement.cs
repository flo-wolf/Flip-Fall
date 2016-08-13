using Sliders;
using Sliders.Audio;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Sliders.Cam
{
    public class CamMovement : MonoBehaviour
    {
        public enum CamMoveState { following, transitioning, resting }

        //public enum InterpolationType { smoothstep, linear, smootherstep, sin }

        //Spielfigur für Berechnung der relativen Kameraposition
        public Player player;

        public InterpolationType interpolationType;
        public AudioClip transitionSound;

        public static CamMovement _instance;

        public CamMoveState camMoveState;
        public static CameraStateEvent onCameraStateChange = new CameraStateEvent();
        private Vector3 target;
        private float transitionDuration;

        private void Awake()
        {
            _instance = this;
        }

        private void Start()
        {
            camMoveState = CamMoveState.resting;
            Vector3 PlayerPOS = player.transform.transform.position;
            Camera.main.transform.position = new Vector3(PlayerPOS.x, PlayerPOS.y + Constants.cameraY, Camera.main.transform.position.z);
        }

        public static void SetCameraState(CamMoveState cs)
        {
            _instance.camMoveState = cs;
            onCameraStateChange.Invoke(_instance.camMoveState);
        }

        //Smooth camera Transitions, calls Transition()
        public static void moveCamTo(Vector2 t, float d)
        {
            target = t;
            target.z = Constants.cameraY;
            transitionDuration = d;

            SetCameraState(CamMoveState.transitioning);
            _instance.StartCoroutine(Transition());
        }

        private IEnumerator Transition()
        {
            float t = 0F;
            Vector3 startingPos = Camera.main.transform.position;

            ///AUSLAGERN! cameraManager class erstellen, dort die listeners reintun
            SoundPlayer.instance.RandomizeSfx(transitionSound);

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

            SetCameraState(CamMoveState.resting);
            Game.SetGameState(Game.GameState.ready);
        }

        //instead of using transforms rather use joints and only transform one
        private void Update()
        {
            if (camMoveState == CamMoveState.following)
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
            if (camMoveState == CamMoveState.following)
                return true;
            else
                return false;
        }

        public static bool IsResting()
        {
            if (camMoveState == CamMoveState.resting)
                return true;
            else
                return false;
        }

        public static bool IsTransitioning()
        {
            if (camMoveState == CamMoveState.transitioning)
                return true;
            else
                return false;
        }

        public class CameraStateEvent : UnityEvent<CamMoveState> { }
    }
}