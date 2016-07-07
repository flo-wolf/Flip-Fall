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
        public enum GameState { editor, playing, deathscreen, settings, ready, finishscreen, levelswitch }

        public static GameState gameState;
        public static GameStateChangeEvent onGameStateChange = new GameStateChangeEvent();

        public class GameStateChangeEvent : UnityEvent<GameState> { }

        public static CameraMovement cm;
        public ScoreboardManager scoreboardManager;
        public Player player;

        private bool _firsttime;

        private void Awake()
        {
            ProgressManager.ClearProgress();
            ProgressManager.LoadProgressData();
            SetGameState(GameState.ready);
            Player.onPlayerStateChange.AddListener(PlayerStateChanged);
            LevelManager.PlaceActiveLevel();

            //load progress -> set firsttime
            //firsttime? -> load tutorial
            //load last played level -> get value from "progress"
        }

        public static void SetGameState(GameState gs)
        {
            gameState = gs;
            onGameStateChange.Invoke(gameState);
            if (gameState == GameState.deathscreen)
            {
                //start coroutine (wait 3 secs, blink times, them switch to ready -> playbtn will apear)
            }
            //Debug.Log(gameState);
        }

        public void Edit()
        {
            SetGameState(GameState.editor);
        }

        public void CloseGame()
        {
            ProgressManager.SaveProgressData();
            Application.Quit();
        }

        public void LoadLevel(int levelID)
        {
            //player.
            //currentlevel = levelloader.load(levelID);
        }

        public static void FinishLevel()
        {
            SetGameState(GameState.finishscreen);
            ProgressManager.SaveTime();
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
            /*
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

                    CameraMovement.SetCameraState(CameraMovement.CameraState.resting);
                    //display scoreboard
                    break;

                case Player.PlayerState.ready:
                    break;

                default:
                    Debug.LogError("Incorrect PlayerState");
                    break;
            }
            */
        }
    }
}