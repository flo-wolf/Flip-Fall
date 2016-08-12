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

        //Zoom camera sizes
        public float maxZoom = 120F;
        public float minZoom = 80F;
        public float deathZoom = 50F;

        //Zooming times
        public float deathZoomDuration = 0.5F;
        public float defaultZoomDuration = 0.5F;

        //Used for checking
        public float velocityThreshold = 5F;

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
                case Player.PlayerState.alive:
                    StopAllCoroutines();
                    StartCoroutine(VelocityZoom());
                    break;

                case Player.PlayerState.dead:
                    StopAllCoroutines();
                    StartCoroutine(DeathZoom());
                    break;

                default:
                    break;
            }
        }

        //zooms from the current camera size to the minimum camera size
        private IEnumerator ZoomToMinimum()
        {
            Debug.Log("ZoomIn()");
            float t = 0;
            while (t < 1F)
            {
                t += Time.deltaTime * (Time.timeScale / defaultZoomDuration);
                cam.orthographicSize = Mathf.SmoothStep(cam.orthographicSize, minZoom, t);
                yield return new WaitForFixedUpdate();
            }
            yield break;
        }

        //zooms from the current camera size to the maximum camera size
        private IEnumerator ZoomToMaximum()
        {
            Debug.Log("ZoomOut()");
            float t = 0;
            while (t < 1F)
            {
                t += Time.deltaTime * (Time.timeScale / defaultZoomDuration);
                cam.orthographicSize = Mathf.SmoothStep(cam.orthographicSize, maxZoom, t);
                yield return new WaitForFixedUpdate();
            }
            yield break;
        }

        //zooms from the current camera size to the minimum camera size
        private IEnumerator DeathZoom()
        {
            Debug.Log("DeathZoom() : IEnumerator");
            float t = 0;
            while (t < 1F)
            {
                t += Time.deltaTime * (Time.timeScale / deathZoomDuration);

                cam.orthographicSize = Mathf.SmoothStep(cam.orthographicSize, deathZoom, t);
                yield return new WaitForFixedUpdate();
            }
            StartCoroutine(ZoomToMinimum());
            yield break;
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

                size = Mathf.SmoothStep(minZoom, maxZoom, Mathf.InverseLerp(cam.orthographicSize, maxVelocity, velocity.magnitude));

                zoomLerpTime += Time.fixedDeltaTime;

                cam.orthographicSize = Mathf.SmoothStep(cam.orthographicSize, size, zoomLerpTime);

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

                size = Mathf.SmoothStep(minZoom, maxZoom, Mathf.InverseLerp(cam.orthographicSize, maxVelocity, velocity.magnitude));
                cam.orthographicSize = size;
                yield return new WaitForFixedUpdate();
            }
        }
    }
}