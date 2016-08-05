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

                /*if (velocity.y + speedThreshold < lastVelocity.y)
                {
                    velocity = lastVelocity;
                }
                else
                {
                    lastVelocity = velocity;
                } */

                Debug.Log("Maxvelocity: " + maxVelocity + " velocityThreshold: " + velocityThreshold + " velocity.x: " + velocity.x);
                if (velocity.x > (maxVelocity - velocityThreshold))
                {
                    Debug.Log("sgehtabfdsddasda " + maxVelocity);
                    velocity.x = maxVelocity;
                }
                Camera.main.orthographicSize = Mathf.Lerp(minZoom, maxZoom, Mathf.InverseLerp(0F, maxVelocity, velocity.x));
            }
        }
    }
}