using FlipFall;
using FlipFall.Theme;
using System.Collections;
using UnityEngine;

namespace FlipFall.LevelObjects
{
    public class Finish : LevelObject
    {
        public Vector2 finishLocation;

        private ParticleSystem ps;
        private ParticleSystem.EmissionModule psEmit;

        private void Awake()
        {
            objectType = ObjectType.finish;
        }

        // Use this for initialization
        private void Start()
        {
            ps = GetComponentInChildren<ParticleSystem>();
            psEmit = ps.emission;
            ps.Stop();
            ps.gameObject.SetActive(false);

            finishLocation = (Vector2)this.gameObject.transform.position;
            //finishParticlesEmit = GetComponent<ParticleSystem>().emission;
            //finishParticlesEmit.enabled = false;

            ps.gameObject.GetComponent<ParticleSystemRenderer>().material.SetColor("_Color", ThemeManager.theme.finishColor);

            MeshRenderer mr = GetComponent<MeshRenderer>();
            if (mr != null)
                mr.material.SetColor("_Color", ThemeManager.theme.finishColor);
            else
                Debug.LogError("No MeshRenderer attached to the Spawn, can't set the color.");

            Game.onGameStateChange.AddListener(GameStateChanged);
        }

        private void GameStateChanged(Game.GameState gs)
        {
            switch (gs)
            {
                case Game.GameState.finishscreen:
                    //GetComponent<ParticleSystem>().Clear();
                    GetComponent<Animation>().Play("finishFadeOut");

                    Debug.Log("PLAYFINISHPARTICLES");
                    ps.gameObject.SetActive(true);
                    ps.Play();
                    //GetComponent<ParticleSystem>().Play();
                    break;
            }
        }
    }
}