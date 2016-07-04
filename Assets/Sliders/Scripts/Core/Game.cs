using Sliders;
using Sliders.Models;
using Sliders.UI;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Sliders
{
    public class Game : MonoBehaviour
    {
        public enum GameState { editor, playing, menu, ready, respawning, finish }

        public static GameState gameState;
        public static GameStateChangeEvent onGameStateChange = new GameStateChangeEvent();

        public class GameStateChangeEvent : UnityEvent<Player.PlayerState> { }

        public static CameraMovement cm;
        public UITimer timer;
        public UIScoreboard scoreboard;
        public Player player;

        private bool _firsttime;

        private void Start()
        {
            ProgressManager.ClearProgress();
            SetGameState(GameState.ready);
            Player.onPlayerStateChange.AddListener(PlayerStateChanged);
            CameraMovement.onCameraStateChange.AddListener(CameraStateChanged);
            LevelManager.LoadLevels();
            //load progress -> set firsttime
            //firsttime? -> load tutorial
            //load last played level -> get value from "progress"
        }

        public static void SetGameState(GameState gs)
        {
            gameState = gs;
            //Invoke Gamsteate event
        }

        public void CloseGame()
        {
            //save
            Application.Quit();
        }

        public void LoadLevel(int levelID)
        {
            //player.
            //currentlevel = levelloader.load(levelID);
        }

        public static void FinishLevel()
        {
            //save progress
        }

        public static void RestartLevel()
        {
            //cam to beginning
            //player to beginning
            //restart timer
        }

        public void PlayerStateChanged(Player.PlayerState newPlayerState)
        {
            switch (newPlayerState)
            {
                case Player.PlayerState.alive:
                    SetGameState(GameState.playing);
                    scoreboard.Hide();
                    timer.Run();
                    timer.Reset();
                    timer.Continue();
                    break;

                case Player.PlayerState.dead:
                    timer.Pause();
                    scoreboard.Show(timer.GetTime());
                    CameraMovement.SetCameraState(CameraMovement.CameraState.resting);
                    //display scoreboard
                    break;

                case Player.PlayerState.ready:
                    break;

                case Player.PlayerState.finish:

                    break;

                default:
                    Debug.LogError("Incorrect PlayerState");
                    break;
            }
        }

        public void CameraStateChanged(CameraMovement.CameraState newCameraState)
        {
            switch (newCameraState)
            {
                case CameraMovement.CameraState.following:
                    break;

                case CameraMovement.CameraState.transitioning:
                    break;

                case CameraMovement.CameraState.resting:
                    break;

                default:
                    Debug.LogError("Incorrect CameraState");
                    break;
            }
        }
    }
}