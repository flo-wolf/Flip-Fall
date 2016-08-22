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

        //delay time between the players death and the appearance of the next screen
        public static float scoreScreenAppearDelay = 1F;
        public static float finishScreenAppearDelay = 1F;

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
            SetGameState(GameState.ready);
        }

        private void OnApplicationQuit()
        {
            Debug.Log("Game Closed down");
            ProgressManager.SaveProgressData();
        }

        // Only set the GameState through this. All other classes will be able to use GameState listeners.
        public static void SetGameState(GameState gs)
        {
            switch (gs)
            {
                case GameState.scorescreen:
                    Debug.Log("Game: SCORESREEN");
                    //Executed before event is fired!
                    Timer.Pause();
                    _instance.StartCoroutine(DelayedGameStateSwitch(gs, scoreScreenAppearDelay));
                    break;

                case GameState.finishscreen:
                    //Executed before event is fired!
                    Debug.Log("Game: FINISHSCREEN");
                    Timer.Pause();
                    ProgressManager.GetProgress().EnterHighscore(LevelManager.GetID(), UITimer.GetTime());
                    _instance.StartCoroutine(DelayedGameStateSwitch(gs, finishScreenAppearDelay));
                    break;

                case GameState.playing:
                    onGameStateChange.Invoke(gs);
                    break;

                case GameState.ready:
                    onGameStateChange.Invoke(gs);
                    break;

                default:
                    onGameStateChange.Invoke(gs);
                    break;
            }
            gameState = gs;
        }

        public static IEnumerator DelayedGameStateSwitch(GameState gs, float delay)
        {
            yield return new WaitForSeconds(delay);
            onGameStateChange.Invoke(gs);
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