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

        public Text levelID;

        private void Start()
        {
            _instance = this;
            Game.onGameStateChange.AddListener(GameStateChanged);
            Player.onPlayerAction.AddListener(PlayerAction);
            Player.onPlayerStateChange.AddListener(PlayerStateChanged);
        }

        private void GameStateChanged(Game.GameState gameState)
        {
            switch (gameState)
            {
                case Game.GameState.playing:
                    //add fancy fadeouts, save
                    UILevelSelection.Hide();
                    UIStarCount.Hide();
                    UITimer.Show();
                    UIButtonManager.Hide(UIButtonManager._instance.playBtn);
                    break;

                case Game.GameState.deathscreen:
                    UILevelSelection.Show();
                    break;

                case Game.GameState.finishscreen:
                    UILevelSelection.Show();
                    break;

                case Game.GameState.levelselection:
                    UITimer.Hide();
                    UIStarCount.Show();
                    UILevelSelection.Show();
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
    }
}