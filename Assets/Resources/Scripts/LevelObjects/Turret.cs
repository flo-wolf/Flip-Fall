using FlipFall;
using FlipFall.Audio;
using FlipFall.Theme;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlipFall.LevelObjects
{
    public class Turret : LevelObject
    {
        private ParticleSystem shotPS;
        public Animation shotAnimation;
        public MeshRenderer barrelMeshRenderer;
        public GameObject barrelOutline;

        // implement thoe by changing the particle system accordingly.
        public float shotDelay = 1F;
        public float startupDelay = 0F;

        // not yet implemented
        public float shotSpeed = 1F;

        public bool constantFire = true;

        // Use this for initialization
        private void Start()
        {
            objectType = ObjectType.turret;
            shotPS = GetComponent<ParticleSystem>();

            MeshRenderer mr = GetComponent<MeshRenderer>();
            if (mr != null)
            {
                shotPS.gameObject.GetComponent<ParticleSystemRenderer>().material.SetColor("_Color", ThemeManager.theme.turretColor);
                mr.material.SetColor("_Color", ThemeManager.theme.turretColor);
                barrelMeshRenderer.material.SetColor("_Color", ThemeManager.theme.turretColor);
            }
            else
                Debug.LogError("No MeshRenderer attached to the Turret, can't set the color.");

            Player.onPlayerStateChange.AddListener(PlayerStateChanged);
            StartCoroutine(cFire());
        }

        private void PlayerStateChanged(Player.PlayerState playerState)
        {
            switch (playerState)
            {
                case Player.PlayerState.alive:
                    break;

                case Player.PlayerState.dead:
                    break;

                case Player.PlayerState.win:
                    break;

                default:
                    break;
            }
        }

        private IEnumerator cFire()
        {
            yield return new WaitForSeconds(startupDelay);
            while (constantFire)
            {
                yield return new WaitForSeconds(shotDelay);
                Fire();
            }
            yield break;
        }

        private void Fire()
        {
            //shotPS.Stop();
            SoundManager.TurretShot(new Vector3(transform.position.x, transform.position.y, Constants.playerZ));
            shotPS.Play();
            shotAnimation["turretShooting"].time = 0F;
            shotAnimation["turretShooting"].speed = shotDelay * 2;
            shotAnimation.Play("turretShooting");
        }
    }
}