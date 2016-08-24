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
        public enum GameState { titlescreen, tutorial, ready, playing, pause, finishscreen, deathscreen, scorescreen, editor, settingsscreen }

        public static GameState gameState = GameState.ready;
        public static GameStateChangeEvent onGameStateChange = new GameStateChangeEvent();

        public static CamMove cm;
        public static Game _instance;

        //delay time between the players death and the deathscreen
        public static float deathDelay = 1F;

        //delay time between the switch of the gameState form deathscreen to scorescreen
        public static float deathToScorescreenDelay = 1F;

        public static float scoreScreenDelay = 1F;

        private void Awake()
        {
            _instance = this;
            ProgressManager.ClearProgress();
            ProgressManager.LoadProgressData();
            //SetGameState(GameState.ready);
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
            switch (gs)
            {
                case GameState.deathscreen:
                    Debug.Log("Game: DEATHSCREEN");
                    Timer.Pause();
                    _instance.StartCoroutine(DelayedGameStateInvoke(gs, deathDelay));
                    _instance.StartCoroutine(DelayedGameStateSet(Game.GameState.scorescreen, deathToScorescreenDelay + deathDelay));

                    break;

                case GameState.finishscreen:
                    Debug.Log("Game: FINISHSCREEN");
                    Timer.Pause();
                    ProgressManager.GetProgress().EnterHighscore(LevelManager.GetID(), UITimer.GetTime());
                    _instance.StartCoroutine(DelayedGameStateInvoke(gs, deathDelay));
                    _instance.StartCoroutine(DelayedGameStateSet(Game.GameState.scorescreen, deathToScorescreenDelay + deathDelay));
                    break;

                case GameState.scorescreen:
                    Debug.Log("Game: SCORESCREEN");
                    onGameStateChange.Invoke(gs);
                    _instance.StartCoroutine(DelayedGameStateSet(Game.GameState.ready, scoreScreenDelay));
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

        public static IEnumerator DelayedGameStateInvoke(GameState gs, float delay)
        {
            yield return new WaitForSeconds(delay);
            onGameStateChange.Invoke(gs);
        }

        public static IEnumerator DelayedGameStateSet(GameState gs, float delay)
        {
            yield return new WaitForSeconds(delay);
            SetGameState(gs);
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