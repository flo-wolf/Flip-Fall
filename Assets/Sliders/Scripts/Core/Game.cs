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
        private bool _firsttime;

        private void Awake()
        {
            ProgressManager.ClearProgress();
            ProgressManager.LoadProgressData();
            SetGameState(GameState.ready);

            //firsttime? -> load tutorial
            //load last played level -> get value from "progress"
        }

        private void Start()
        {
            ProgressManager.LoadProgressData();
            LevelManager.PlaceActiveLevel();
            UIScoreboard.uiScoreboard.UpdateTexts();
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
    }
}