using Sliders;
using Sliders.Cam;
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
        public enum GameState { titlescreen, tutorial, ready, playing, pause, scorescreen, finishscreen, editor, settingsscreen }

        public static GameState gameState;
        public static GameStateChangeEvent onGameStateChange = new GameStateChangeEvent();

        public static CamMove cm;
        public static Game _instance;

        //The delay between when SetGameState() gets called and when the GameSTateChangeEvent gets fired.
        public int switchDelay = 1;

        private void Awake()
        {
            _instance = this;
            ProgressManager.ClearProgress();
            ProgressManager.LoadProgressData();
            SetGameState(GameState.ready);
        }

        private void Start()
        {
        }

        private void OnApplicationQuit()
        {
            Debug.Log("Game Closed down");
            ProgressManager.SaveProgressData();
        }

        // Only set the GameState through this. All other classes will be able to use GameState listeners.
        public static void SetGameState(GameState gs)
        {
            gameState = gs;

            switch (gs)
            {
                case GameState.scorescreen:
                    //Executed before event is fired!
                    UITimer.instance.Pause();
                    _instance.StartCoroutine(DelayedGameStateSwitch());
                    break;

                case GameState.finishscreen:
                    //Executed before event is fired!
                    UITimer.instance.Pause();
                    _instance.StartCoroutine(DelayedGameStateSwitch());
                    break;

                case GameState.playing:
                    onGameStateChange.Invoke(gameState);
                    break;

                case GameState.ready:
                    onGameStateChange.Invoke(gameState);
                    break;

                default:
                    onGameStateChange.Invoke(gameState);
                    break;
            }
        }

        public static IEnumerator DelayedGameStateSwitch()
        {
            yield return new WaitForSeconds(_instance.switchDelay);
            onGameStateChange.Invoke(gameState);
        }

        public void Edit()
        {
            SetGameState(GameState.editor);
        }

        public void CloseGame()
        {
        }

        public static void RestartLevel()
        {
            //cam to beginning
            //player to beginning
            //restart timer
        }

        public class GameStateChangeEvent : UnityEvent<GameState> { }
    }
}