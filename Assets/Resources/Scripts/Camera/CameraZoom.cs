using Sliders;
using System.Collections;
using UnityEngine;

//Zoom and CameraShaking on player Velocity Changes
namespace Sliders
{
    public class CameraZoom : MonoBehaviour
    {
        public Player player;
        public float maxZoom;
        public float minZoom;
        public float speedThreshold; //used to check for imediate velocity stops
        public float velocityThreshold; //value around 5 works fine

        private float size;
        private float stopLerpTime;
        private Vector2 lastVelocity = Vector2.zero;
        private float maxVelocity;

        private void Start()
        {
            size = Camera.main.orthographicSize;
        }

        private void FixedUpdate()
        {
            if (player.playerState == Player.PlayerState.alive)
            {
                maxVelocity = player.maxChargeVelocity;
                Vector2 velocity = player.rBody.velocity;
                velocity.x = System.Math.Abs(velocity.x);

                //y-axis
                if (velocity.y + speedThreshold < System.Math.Abs(lastVelocity.y))
                {
                    StopAllCoroutines();
                    StartCoroutine(VelocityStopBufferCoroutine());
                }

                //x-axis
                if (velocity.x > (maxVelocity - velocityThreshold))
                {
                    velocity.x = maxVelocity;
                }
                lastVelocity = velocity;
                Camera.main.orthographicSize = Mathf.Lerp(minZoom, maxZoom, Mathf.InverseLerp(0F, maxVelocity, velocity.magnitude));
            }
        }

        private IEnumerator VelocityStopBufferCoroutine()
        {
            stopLerpTime = 0;
            while (true)
            {
                stopLerpTime += Time.fixedDeltaTime;
                Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, minZoom, stopLerpTime);
                yield return new WaitForFixedUpdate();
            }
        }
    }
}