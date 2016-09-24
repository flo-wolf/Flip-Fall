using System.Collections;
using UnityEngine;

namespace Impulse.Levels
{
    public class Finish : MonoBehaviour
    {
        public Vector2 finishLocation;

        private ParticleSystem ps;
        private ParticleSystem.EmissionModule psEmit;

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