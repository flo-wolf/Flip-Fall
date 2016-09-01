using Sliders;
using Sliders.Cam;
using Sliders.Levels;
using Sliders.Progress;
using Sliders.UI;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// Controls the gameplay durimg MainState.playing
/// </summary>
namespace Sliders
{
    public class Game : MonoBehaviour
    {
        public enum GameState { startup, title, tutorial, homescreen, ready, playing, pause, finishscreen, deathscreen, levelselection, editor, settingsscreen }

        //create class main, make it completly static, surviving scene changes
        // main class logs the current screenstate and allows easy switching, accessable by the controller script of each scene
        //public enum MainState { startup, title, firstvisit, home, playing, pause, levelselection, editor, settings }
        //

        public static GameState gameState;
        public static GameStateChangeEvent onGameStateChange = new GameStateChangeEvent();

        public static CamMove cm;
        public static Game _instance;

        //delay time between the players death and the deathscreen
        public static float deathDelay = 1.5F;

        //delay time between the switch of the gameState form deathscreen to levelselection
        public static float deathTolevelselectionDelay = 0.0F;

        public static float levelselectionDelay = 1F;

        private void Awake()
        {
            _instance = this;
            ProgressManager.ClearProgress();
            ProgressManager.LoadProgressData();
            //SetGameState(GameState.ready);
        }

        private void Start()
        {
            SetGameState(GameState.levelselection);
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
                    Debug.Log("[Game] deathscreen");
                    Timer.Pause();
                    _instance.StartCoroutine(DelayedGameStateInvoke(gs, deathDelay));
                    _instance.StartCoroutine(DelayedGameStateSet(Game.GameState.levelselection, deathTolevelselectionDelay + deathDelay));
                    break;

                case GameState.finishscreen:
                    Debug.Log("[Game] finishscreen");
                    Timer.Pause();
                    if (LevelManager.GetID() == ProgressManager.GetProgress().lastUnlockedLevel)
                        ProgressManager.GetProgress().lastUnlockedLevel++;
                    ProgressManager.GetProgress().EnterHighscore(LevelManager.GetID(), UITimer.GetTime());
                    _instance.StartCoroutine(DelayedGameStateInvoke(gs, deathDelay));
                    _instance.StartCoroutine(DelayedGameStateSet(Game.GameState.levelselection, deathTolevelselectionDelay + deathDelay));
                    break;

                case GameState.levelselection:
                    Debug.Log("[Game] levelselection");
                    onGameStateChange.Invoke(gs);
                    //_instance.StartCoroutine(DelayedGameStateSet(Game.GameState.ready, levelselectionDelay));
                    break;

                case GameState.playing:
                    Debug.Log("[Game] playing");
                    onGameStateChange.Invoke(gs);
                    break;

                case GameState.ready:
                    Debug.Log("[Game] levelselection");
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