using Impulse;
using Impulse.Audio;
using Impulse.Levels;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Controls the movement of an array of cameras simultaniously
/// </summary>

namespace Impulse.Cam
{
    public class CamMove : MonoBehaviour
    {
        public static CamMove _instance;
        public Camera[] cams;

        public static CameraStateEvent onCamMoveStateChange = new CameraStateEvent();
        public enum CamMoveState { following, transitioning, resting }

        public CamMoveState camMoveState = CamMoveState.resting;
        public InterpolationType interpolationType;

        private void Awake()
        {
            _instance = this;
        }

        private void Start()
        {
            Vector3 spawnPos = LevelManager.GetActiveLevel().spawn.GetPosition();
            spawnPos.z = Constants.cameraZ;
            foreach (Camera cam in cams)
            {
                cam.transform.position = spawnPos;
            }
        }

        public static void StartFollowing()
        {
            SetCameraState(CamMoveState.following);
            //_instance.joint.enabled = true;
        }

        public static void StopFollowing()
        {
            SetCameraState(CamMoveState.resting);
            //_instance.joint.enabled = false;
        }

        public static void SetCameraState(CamMoveState cs)
        {
            _instance.camMoveState = cs;
            onCamMoveStateChange.Invoke(_instance.camMoveState);
        }

        public static void MoveCamTo(Vector2 target)
        {
            Vector3 t = new Vector3(target.x, target.y, Constants.cameraZ);
            Camera.main.transform.position = t;
        }

        //Smooth camera Transitions, calls Transition()
        public static void MoveCamTo(Vector2 target, float duration)
        {
            Vector3 t = new Vector3(target.x, target.y, Constants.cameraZ);
            _instance.StartCoroutine(_instance.Transition(target, duration));
        }

        private IEnumerator Transition(Vector3 target, float duration)
        {
            SetCameraState(CamMoveState.transitioning);
            float t = 0F;
            Vector3 startingPos = Camera.main.transform.position;

            switch (interpolationType)
            {
                case InterpolationType.linear:
                    while (t < 1.0f)
                    {
                        t += Time.deltaTime * (Time.timeScale / duration);
                        Camera.main.transform.position = Vector3.Lerp(startingPos, target, t);
                        yield return 0;
                    }
                    break;

                case InterpolationType.smoothstep:
                    while (t < 1.0f)
                    {
                        t += Time.deltaTime * (Time.timeScale / duration);

                        float x = t / 1;
                        x = x * x * (3f - 2f * t); //Smoothstep formula: t = t*t * (3f - 2f*t)

                        Camera.main.transform.position = Vector3.Lerp(startingPos, target, x);
                        yield return 0;
                    }
                    break;

                default:
                    break;
            }

            SetCameraState(CamMoveState.following);
        }

        //instead of using transforms rather use joints and only transform one
        private void Update()
        {
            if (camMoveState == CamMoveState.following)
            {
                Vector3 PlayerPOS = Player._instance.transform.transform.position;
                foreach (Camera cam in cams)
                {
                    cam.transform.localPosition = new Vector3(PlayerPOS.x, PlayerPOS.y, Constants.cameraZ);
                }
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