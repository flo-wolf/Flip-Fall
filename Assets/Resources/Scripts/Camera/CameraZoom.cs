using Sliders;
using System.Collections;
using UnityEngine;

//Zoom and CameraShaking on player Velocity Changes
namespace Sliders
{
    public class CameraZoom : MonoBehaviour
    {
        public Player player;
        public Camera cam;
        public float maxZoom;
        public float minZoom;
        public float velocityThreshold; //value around 5 works fine

        private float size;
        private float maxVelocity;

        private void Start()
        {
            cam.orthographicSize = minZoom;
            player.onPlayerAction.AddListener(PlayerAction);
            player.onPlayerStateChange.AddListener(PlayerStateChanged);
            size = cam.orthographicSize;
        }

        private void PlayerAction(Player.PlayerAction playerAction)
        {
            switch (playerAction)
            {
                case Player.PlayerAction.charge:
                    StopAllCoroutines();
                    StartCoroutine(TranslateToVelocityZoom());
                    break;

                case Player.PlayerAction.decharge:
                    break;

                case Player.PlayerAction.reflect:
                    break;

                default:
                    break;
            }
        }

        private void PlayerStateChanged(Player.PlayerState playerState)
        {
            switch (playerState)
            {
                case Player.PlayerState.dead:
                    StopAllCoroutines();
                    StartCoroutine(ZoomIn());
                    break;

                case Player.PlayerState.alive:
                    StopAllCoroutines();
                    StartCoroutine(VelocityZoom());
                    break;

                default:
                    break;
            }
        }

        //zooms from the current camera size to the minimum camera size
        private IEnumerator ZoomIn()
        {
            Debug.Log("ZoomIn()");
            float zoomLerpTime = 0;
            bool zooming = true;

            while (zooming)
            {
                zoomLerpTime += Time.fixedDeltaTime;
                cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, minZoom, zoomLerpTime);

                if (cam.orthographicSize == minZoom)
                {
                    zooming = false;
                }
                yield return new WaitForFixedUpdate();
            }

            StartCoroutine(VelocityZoom());
        }

        //zooms from the current camera size to the maximum camera size
        private IEnumerator ZoomOut()
        {
            Debug.Log("ZoomOut()");
            float zoomLerpTime = 0;
            while (true)
            {
                zoomLerpTime += Time.fixedDeltaTime;
                cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, maxZoom, zoomLerpTime);
                yield return new WaitForFixedUpdate();
            }
        }

        private IEnumerator TranslateToVelocityZoom()
        {
            Debug.Log("TranslateToVelocityZoom()");
            float zoomLerpTime = 0;
            bool zooming = true;
            Vector2 velocity;

            while (zooming)
            {
                velocity = player.rBody.velocity;
                velocity.x = System.Math.Abs(velocity.x);

                size = Mathf.Lerp(minZoom, maxZoom, Mathf.InverseLerp(cam.orthographicSize, maxVelocity, velocity.magnitude));

                zoomLerpTime += Time.fixedDeltaTime;

                cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, size, zoomLerpTime);

                if (cam.orthographicSize == size)
                {
                    zooming = false;
                }
                yield return new WaitForFixedUpdate();
            }

            StartCoroutine(VelocityZoom());
        }

        private IEnumerator VelocityZoom()
        {
            Debug.Log("VeloctiyZoom()");
            maxVelocity = player.maxChargeVelocity;
            Vector2 velocity;

            while (true)
            {
                velocity = player.rBody.velocity;
                velocity.x = System.Math.Abs(velocity.x);

                if (velocity.x > (maxVelocity - velocityThreshold))
                {
                    velocity.x = maxVelocity;
                }

                size = Mathf.Lerp(minZoom, maxZoom, Mathf.InverseLerp(cam.orthographicSize, maxVelocity, velocity.magnitude));
                cam.orthographicSize = size;
                yield return new WaitForFixedUpdate();
            }
        }

        /*
        private void FixedUpdate()
        {
                if (velocity.y + speedThreshold < System.Math.Abs(lastVelocity.y))
                {
                    StopAllCoroutines();
                    StartCoroutine(ZoomIn());
                }
        }
        */
    }
}