using Sliders.Levels;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages all UI Elements through externally fired events and turns them on and off
/// </summary>

namespace Sliders.UI
{
    public class UIManager : MonoBehaviour
    {
        public enum UIState { levelSelection, home, settings, game, title, shop, editor, credits, buyPro }
        public static UIManager _instance;
        public static UIState uiState;

        public Text levelID;

        private void Start()
        {
            _instance = this;
            Game.onGameStateChange.AddListener(GameStateChanged);
            Player.onPlayerAction.AddListener(PlayerAction);
            Player.onPlayerStateChange.AddListener(PlayerStateChanged);
            LevelManager.onLevelChange.AddListener(LevelChanged);

            UILevelManager.UpdateTexts();
        }

        private void GameStateChanged(Game.GameState gameState)
        {
            switch (gameState)
            {
                case Game.GameState.playing:
                    //add fancy fadeouts, save
                    UILevelManager.Hide();
                    UITimer.Run();
                    UIButtonManager.Hide(UIButtonManager._instance.playBtn);
                    break;

                case Game.GameState.deathscreen:
                    UILevelManager.UpdateTexts();
                    UILevelManager.Show();
                    break;

                case Game.GameState.finishscreen:
                    UILevelManager.UpdateTexts();
                    UILevelManager.Show();
                    break;

                case Game.GameState.scorescreen:
                    UILevelManager.UpdateStars();
                    break;

                case Game.GameState.ready:
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

                case Player.PlayerState.ready:
                    break;

                default:
                    break;
            }
        }

        private void LevelChanged(Level level)
        {
            UILevelManager.UpdateTexts();
        }
    }
}