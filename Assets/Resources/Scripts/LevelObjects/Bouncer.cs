using FlipFall;
using FlipFall.Theme;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlipFall.LevelObjects
{
    public class Bouncer : LevelObject
    {
        // added force to the
        public int forceAdd = 0;
        private float colorFadeInDuration = 0.1F; //add sound
        private float colorFadeBackDuration = 0.5F;

        // when the player doesn't touch the strip anymore, we blend back to the first color.
        private bool autoFadeBack = true;

        private Rigidbody2D playerRb;
        private bool colliding;
        private float blend;

        private Material mat;

        private void Start()
        {
            objectType = ObjectType.bouncer;
            mat = GetComponent<MeshRenderer>().material;

            if (mat != null)
            {
                mat.SetColor("_Color", ThemeManager.theme.speedstripUnactiveColor);
                mat.SetColor("_ColorTouch", ThemeManager.theme.speedstripColor);
            }

            if (Player._instance != null)
            {
                playerRb = Player._instance.GetComponent<Rigidbody2D>();
            }
            colliding = false;
        }

        private void OnCollisionEnter2D(Collision2D coll)
        {
            if (coll.collider.tag == Constants.playerTag && playerRb != null)
            {
                colliding = true;
                StartCoroutine(cFadeIn());
                //Quaternion speedStripAngle = transform.rotation;
                //Vector2 forceDirection = speedStripAngle * Vector2.up;
                //playerRb.AddForce(forceDirection * Time.fixedDeltaTime * accelSpeed * accelMulti);
            }
        }

        private void OnCollisionExit2D(Collision2D coll)
        {
            if (coll.collider.tag == Constants.playerTag && playerRb != null)
            {
                Debug.Log("this");
                StartCoroutine(cFadeOut());
                colliding = false;
            }
        }

        private IEnumerator cFadeIn()
        {
            while ((blend < 1) && mat != null)
            {
                blend += Time.deltaTime * (Time.timeScale / colorFadeInDuration);
                mat.SetFloat("_Blend", blend);
                yield return new WaitForFixedUpdate();
            }
            yield break;
        }

        private IEnumerator cFadeOut()
        {
            while (autoFadeBack && blend > 0 && mat != null)
            {
                blend -= Time.deltaTime * (Time.timeScale / colorFadeBackDuration);
                if (blend < 0)
                {
                    blend = 0;
                    mat.SetFloat("_Blend", blend);
                    yield break;
                }
                mat.SetFloat("_Blend", blend);
                yield return 0;
            }
            yield break;
        }
    }
}