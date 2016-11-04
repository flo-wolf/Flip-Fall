using Impulse;
using Impulse.Theme;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlipFall.LevelObjects
{
    public class SpeedStrip : MonoBehaviour
    {
        public float accelSpeed = 1000F;
        public float accelMulti = 2F;
        private float colorSwitchDuration = 50F; //add sound
        private float colorFadeBack = 1F;

        // when the player doesn't touch the strip anymore, we blend back to the first color.
        private bool autoFadeBack = true;

        private Rigidbody2D playerRb;
        private float accelAngle;
        private bool colliding;
        private float blend;

        private Material mat;

        private void Start()
        {
            mat = GetComponent<SpriteRenderer>().material;

            mat.color = ThemeManager.theme.moveZoneColor;
            mat.SetColor("_Color2", ThemeManager.theme.speedstripColor);
            mat.SetColor("_Color", ThemeManager.theme.speedstripUnactiveColor);

            playerRb = Player._instance.GetComponent<Rigidbody2D>();
            colliding = false;
        }

        private void OnTriggerStay2D(Collider2D collider)
        {
            if (collider.tag == Constants.playerTag)
            {
                colliding = true;
                accelAngle = transform.rotation.eulerAngles.z;
                Vector2 accelDirection = new Vector2(Mathf.Sin(Mathf.Deg2Rad * accelAngle), Mathf.Cos(Mathf.Deg2Rad * accelAngle));
                playerRb.AddForce(accelDirection.normalized * Time.fixedDeltaTime * accelSpeed * accelMulti);

                //Debug.Log("----2 " + accelAngle + " direction " + accelDirection);
            }
        }

        private void OnTriggerExit2D(Collider2D collider)
        {
            if (collider.tag == Constants.playerTag)
            {
                Debug.Log("this");
                colliding = false;
            }
        }

        public void FixedUpdate()
        {
            if (colliding && (blend < 1))
            {
                blend += Time.fixedDeltaTime * colorSwitchDuration;
                mat.SetFloat("_Blend", blend);
            }
            else if (autoFadeBack && blend > 0)
            {
                blend -= Time.fixedDeltaTime * colorFadeBack;
                mat.SetFloat("_Blend", blend);
            }
        }
    }
}