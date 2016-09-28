using Impulse.Levels;
using Impulse.Progress;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages all UI Elements through externally fired events and turns them on and off
/// </summary>

namespace Impulse.UI
{
    public class UIGameManager : MonoBehaviour
    {
        //public enum UIState { levelSelection, home, settings, game, title, shop, editor, credits, buyPro }
        //public static UIState uiState;
        public static UIGameManager _instance;
        public Animation fadeAnimation;

        private void Start()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
                return;
            }
            _instance = this;

            //FadeIn();

            Game.onGameStateChange.AddListener(GameStateChanged);
            Player.onPlayerAction.AddListener(PlayerAction);
            Player.onPlayerStateChange.AddListener(PlayerStateChanged);
            Main.onSceneChange.AddListener(SceneChanging);

            UIGameTimer.Show();
        }

        private void SceneChanging(Main.Scene scene)
        {
            //FadeOut();
        }

        private void GameStateChanged(Game.GameState gameState)
        {
            switch (gameState)
            {
                case Game.GameState.playing:
                    //
                    break;

                case Game.GameState.deathscreen:
                    //show deathscreen
                    break;

                case Game.GameState.finishscreen:
                    //show finishscreen
                    break;

                default:
                    Debug.Log("Incorrect PlayerState");
                    break;
            }
        }

        //Player Listener
        private void PlayerAction(Player.PlayerAction playerAction)
        {
            switch (playerAction)
            {
                case Player.PlayerAction.reflect:
                    break;

                case Player.PlayerAction.charge:
                    break;

                case Player.PlayerAction.decharge:
                    break;

                default:
                    break;
            }
        }

        //Player Listener
        private void PlayerStateChanged(Player.PlayerState playerState)
        {
            switch (playerState)
            {
                case Player.PlayerState.alive:
                    break;

                case Player.PlayerState.dead:
                    break;

                default:
                    break;
            }
        }

        private void FadeIn()
        {
            fadeAnimation.Play("fadeFromBlack");
        }

        private void FadeOut()
        {
            fadeAnimation.Play("fadeToBlack");
        }
    }
}