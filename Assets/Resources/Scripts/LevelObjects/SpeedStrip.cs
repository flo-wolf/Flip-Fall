using Impulse;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Impulse.Levelobjects
{
    public class SpeedStrip : MonoBehaviour
    {
        public float accelSpeed = 1000F;
        public float accelMulti = 2F;

        private Rigidbody2D playerRb;
        private float accelAngle;

        private void Start()
        {
            playerRb = Player._instance.GetComponent<Rigidbody2D>();
        }

        private void OnTriggerStay2D(Collider2D collider)
        {
            if (collider.tag == Constants.playerTag)
            {
                accelAngle = transform.rotation.eulerAngles.z;
                Vector2 accelDirection = new Vector2(Mathf.Sin(Mathf.Deg2Rad * accelAngle), Mathf.Cos(Mathf.Deg2Rad * accelAngle));
                playerRb.AddForce(accelDirection.normalized * Time.fixedDeltaTime * accelSpeed * accelMulti);

                Debug.Log("----2 " + accelAngle + " direction " + accelDirection);
            }
        }

        //private void FixedUpdate()
        //{
        //    foreach (Collider2D collider in Physics2D.OverlapCircleAll(center, pullRadius, 1 << LayerMask.NameToLayer("Player")))
        //    {
        //        // calculate direction from target to this
        //        Vector2 forceDirection = center - new Vector2(collider.transform.position.x, collider.transform.position.y);

        //        // apply force on player towards this
        //        playerRb.AddForce(forceDirection.normalized * maxPullForce * Time.fixedDeltaTime * pullAmplifier);

        //        float dist = Mathf.Abs(Vector3.Distance(collider.transform.position, transform.position));

        //        // update shader input
        //        moveZoneMaterial.SetVector("_AttractionCenter", transform.InverseTransformPoint(transform.position));
        //        moveZoneMaterial.SetFloat("_PlayerDistance", dist);
        //    }
    }
}