using Sliders;
using Sliders.Audio;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Sliders.Cam
{
    public class CamMove : MonoBehaviour
    {
        public static CamMove _instance;

        public static CameraStateEvent onCamMoveStateChange = new CameraStateEvent();
        public enum CamMoveState { following, transitioning, resting }

        public CamMoveState camMoveState;
        public InterpolationType interpolationType;
        public Player player;

        private Vector3 target;
        private float transitionDuration;

        private void Awake()
        {
            _instance = this;
        }

        private void Start()
        {
            _instance.camMoveState = CamMoveState.resting;
            Vector3 PlayerPOS = player.transform.transform.position;
            Camera.main.transform.position = new Vector3(PlayerPOS.x, PlayerPOS.y + Constants.cameraY, Camera.main.transform.position.z);
        }

        public static void StartFollowing()
        {
            SetCameraState(CamMoveState.following);
        }

        public static void StopFollowing()
        {
            SetCameraState(CamMoveState.resting);
        }

        public static void SetCameraState(CamMoveState cs)
        {
            _instance.camMoveState = cs;
            onCamMoveStateChange.Invoke(_instance.camMoveState);
        }

        //Smooth camera Transitions, calls Transition()
        public static void moveCamTo(Vector2 t, float d)
        {
            _instance.target = t;
            _instance.target.z = Constants.cameraY;
            _instance.transitionDuration = d;

            SetCameraState(CamMoveState.transitioning);
            _instance.StartCoroutine(_instance.Transition());
        }

        private IEnumerator Transition()
        {
            float t = 0F;
            Vector3 startingPos = Camera.main.transform.position;

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
            if (_instance.camMoveState == CamMoveState.following)
                return true;
            else
                return false;
        }

        public static bool IsResting()
        {
            if (_instance.camMoveState == CamMoveState.resting)
                return true;
            else
                return false;
        }

        public static bool IsTransitioning()
        {
            if (_instance.camMoveState == CamMoveState.transitioning)
                return true;
            else
                return false;
        }

        public class CameraStateEvent : UnityEvent<CamMoveState> { }
    }
}