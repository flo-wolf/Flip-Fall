using Impulse;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A point that pulls the player towards it with a set force,
/// that increases when the player comes closer and decreases when the player moves away.
/// Can even be used to elevate the player upwards if the attraction (pullForce) is greater than the downwards gravity.
/// </summary>
namespace Impulse.LevelObjects
{
    public class ObjectPuller : MonoBehaviour
    {
        public float pullRadius = 1000f;
        public float maxPullForce = 1000f;

        // amplifies the pull fprce by this value when the player comes closer
        public float pullAmplifier = 2F;

        // current force that gets applied to the object
        private float pullForce;
        private Rigidbody2D playerRb;
        private Vector2 center;

        public float EffectTime = 0F;
        private MeshRenderer mr;

        [ExecuteInEditMode]
        public void SetScale()
        {
            transform.localScale = new Vector3(pullRadius * 2, pullRadius * 2, transform.localScale.z);
        }

        public void Start()
        {
            playerRb = Player._instance.GetComponent<Rigidbody2D>();
            mr = this.GetComponent<MeshRenderer>();
            center = transform.position;
            SetScale();

            // reset shader input
            mr.sharedMaterial.SetFloat("_EffectDistance", pullRadius * 2);
            mr.sharedMaterial.SetVector("_ShieldColor", new Vector4(0.7f, 1f, 1f, 0f));
            mr.sharedMaterial.SetFloat("_EffectRadius", pullRadius);
        }

        public void FixedUpdate()
        {
            foreach (Collider2D collider in Physics2D.OverlapCircleAll(center, pullRadius, 1 << LayerMask.NameToLayer("Player")))
            {
                // calculate direction from target to this
                Vector2 forceDirection = center - new Vector2(collider.transform.position.x, collider.transform.position.y);

                // apply force on player towards this
                playerRb.AddForce(forceDirection.normalized * maxPullForce * Time.fixedDeltaTime * pullAmplifier);

                // update shader input
                mr.sharedMaterial.SetVector("_ShieldColor", new Vector4(0.7f, 1, 1, 0.00f));
                mr.sharedMaterial.SetVector("_Position", transform.InverseTransformPoint(transform.position));
                float dist = Mathf.Abs(Vector3.Distance(collider.transform.position, transform.position));
                mr.sharedMaterial.SetFloat("_EffectDistance", dist);
            }
        }
    }
}