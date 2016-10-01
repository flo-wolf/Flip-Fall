using Impulse;
using Impulse.Cam;
using Impulse.Levels;
using Impulse.Progress;
using Impulse.UI;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// Controls the gameplay durimg MainState.playing
/// </summary>
namespace Impulse
{
    public class Game : MonoBehaviour
    {
        public enum GameState { playing, pause, finishscreen, deathscreen }

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
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
                return;
            }
            _instance = this;
        }

        private void Start()
        {
            LevelPlacer.Place(LevelManager.GetActiveLevel());
            SetGameState(GameState.playing);
            Main.onSceneChange.AddListener(SceneChanged);
        }

        private void SceneChanged(Main.Scene s)
        {
            ProgressManager.GetProgress().lastPlayedLevelID = LevelManager.GetActiveID();
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
                    Timer.Pause();
                    if (ProgressManager.GetProgress().highscores.Any(x => x.levelId == LevelManager.GetActiveID()))
                    {
                        ProgressManager.GetProgress().highscores.Find(x => x.levelId == LevelManager.GetActiveID()).fails++;
                    }

                    //_instance.StartCoroutine(DelayedGameStateInvoke(gs, deathDelay));
                    //_instance.StartCoroutine(DelayedGameStateSet(Game.GameState.levelselection, deathTolevelselectionDelay + deathDelay));

                    onGameStateChange.Invoke(gs);
                    Main.SetScene(Main.Scene.levelselection);
                    break;

                case GameState.finishscreen:
                    Debug.Log("[Game] finishscreen");
                    Timer.Pause();
                    if (LevelManager.GetActiveID() == ProgressManager.GetProgress().lastPlayedLevelID)
                        ProgressManager.GetProgress().EnterHighscore(LevelManager.GetActiveID(), UIGameTimer.GetTime());

                    _instance.StartCoroutine(DelayedGameStateInvoke(gs, deathDelay));
                    //_instance.StartCoroutine(DelayedGameStateSet(Game.GameState.levelselection, deathTolevelselectionDelay + deathDelay));
                    Main.SetScene(Main.Scene.levelselection);
                    onGameStateChange.Invoke(gs);
                    break;

                case GameState.playing:
                    Debug.Log("[Game] playing");
                    onGameStateChange.Invoke(gs);
                    break;

                //case GameState.ready:
                //    Debug.Log("[Game] levelselection");
                //    onGameStateChange.Invoke(gs);
                //    break;

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