using Sliders;
using Sliders.Levels;
using Sliders.Progress;
using Sliders.UI;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Sliders
{
    public class Game : MonoBehaviour
    {
        public enum GameState { titlescreen, settingsscreen, ready, playing, deathscreen, finishscreen, editor }

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
        }

        public static void SetGameState(GameState gs)
        {
            gameState = gs;
            onGameStateChange.Invoke(gameState);
            switch (gs)
            {
                case GameState.deathscreen: //start coroutine (wait 3 secs, blink times, them switch to ready -> playbtn will apear)
                    break;

                case GameState.finishscreen:
                    ProgressManager.FinishLevel();
                    break;

                case GameState.playing:
                    break;
            }
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

        public static void RestartLevel()
        {
            //cam to beginning
            //player to beginning
            //restart timer
        }
    }
}