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
        public static UIManager _instance;

        public Text levelID;
        public Button play;

        private void Start()
        {
            _instance = this;
            Game.onGameStateChange.AddListener(GameStateChanged);
            Player._instance.onPlayerAction.AddListener(PlayerAction);
            Player._instance.onPlayerStateChange.AddListener(PlayerStateChanged);
            LevelManager.onLevelChange.AddListener(LevelChanged);
        }

        private void GameStateChanged(Game.GameState gameState)
        {
            switch (gameState)
            {
                case Game.GameState.playing:
                    UIScoreboard.Hide(); //add fancy fadeouts, save
                    UILevelManager.Hide();
                    UITimer.Run();
                    play.gameObject.SetActive(false);
                    break;

                case Game.GameState.scorescreen:
                    UIScoreboard.Show();
                    UILevelManager.Show();
                    break;

                case Game.GameState.ready:
                    play.gameObject.SetActive(true);
                    break;

                case Game.GameState.finishscreen:
                    UITimer.Pause();
                    UIScoreboard.PlaceTime();
                    UIScoreboard.Show();
                    UILevelManager.Show();
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
            UIScoreboard.UpdateTexts();
        }

        public void PlayBtn()
        {
            Game.SetGameState(Game.GameState.playing);
            play.gameObject.SetActive(false);
        }
    }
}