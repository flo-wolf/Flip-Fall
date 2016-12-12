using FlipFall;
using FlipFall.Cam;
using FlipFall.Editor;
using FlipFall.Levels;
using FlipFall.Progress;
using FlipFall.UI;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// Controls the gameplay durimg MainState.playing
/// </summary>
namespace FlipFall
{
    public class Game : MonoBehaviour
    {
        public enum GameType { story, testing, shared }
        public enum GameState { playing, pause, finishscreen, deathscreen }

        public static GameType gameType = GameType.story;
        public static GameState gameState;
        public static GameStateChangeEvent onGameStateChange = new GameStateChangeEvent();

        public static CamMove cm;
        public static Game _instance;

        //delay time between the players death and the deathscreen
        public static float deathDelay = 1.5F;

        public float timestep = 0.8F;

        //delay time between the switch of the gameState form deathscreen to levelselection
        public static float deathTolevelselectionDelay = 0.0F;

        public static float levelselectionDelay = 1F;

        private void Awake()
        {
            Time.timeScale = timestep;
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
                return;
            }
            _instance = this;
        }

        private void Start()
        {
            if (gameType == GameType.story)
            {
                LevelPlacer.Place(LevelManager.GetActiveLevel());
            }
            else if (gameType == GameType.testing && LevelPlacer._instance != null)
            {
                LevelPlacer._instance.PlaceCustom(LevelEditor.editLevel);
            }
            SetGameState(GameState.playing);
            Main.onSceneChange.AddListener(SceneChanged);
        }

        private void SceneChanged(Main.ActiveScene s)
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
                    UILevelselectionManager.enterType = UILevelselectionManager.EnterType.failed;

                    Timer.Pause();

                    if (gameType == GameType.story)
                    {
                        if (ProgressManager.GetProgress().highscores.Any(x => x.levelId == LevelManager.GetActiveID()))
                        {
                            ProgressManager.GetProgress().highscores.Find(x => x.levelId == LevelManager.GetActiveID()).fails++;
                        }
                        else
                        {
                            ProgressManager.GetProgress().EnterHighscore(LevelManager.GetActiveID(), -1);
                            ProgressManager.GetProgress().highscores.Find(x => x.levelId == LevelManager.GetActiveID()).fails++;
                        }
                        onGameStateChange.Invoke(gs);
                        Main.SetScene(Main.ActiveScene.levelselection);
                    }
                    else if (gameType == GameType.testing)
                    {
                        Main.SetScene(Main.ActiveScene.editor);
                    }
                    break;

                case GameState.finishscreen:
                    Debug.Log("[Game] finishscreen");

                    Timer.Pause();

                    if (gameType == GameType.story)
                    {
                        // Highscore Management
                        int oldStars = 0;
                        if (ProgressManager.GetProgress().highscores.Any(x => x.levelId == LevelManager.GetActiveID()))
                        {
                            oldStars = ProgressManager.GetProgress().highscores.Find(x => x.levelId == LevelManager.GetActiveID()).starCount;
                        }

                        Highscore newHighscore = null;
                        if (LevelManager.GetActiveID() == ProgressManager.GetProgress().lastPlayedLevelID)
                        {
                            newHighscore = ProgressManager.GetProgress().EnterHighscore(LevelManager.GetActiveID(), UIGameTimer.GetTime());
                        }

                        UILevelPlacer.CalcStarsToUnlock(oldStars, newHighscore);

                        _instance.StartCoroutine(DelayedGameStateInvoke(gs, deathDelay));

                        UILevelselectionManager.enterType = UILevelselectionManager.EnterType.finished;

                        Main.SetScene(Main.ActiveScene.levelselection);
                        onGameStateChange.Invoke(gs);

                        //Unlock the next level, if possible
                        if (ProgressManager.GetProgress().TryUnlockNextLevel(LevelManager.GetActiveID()))
                        {
                        }
                    }
                    else if (gameType == GameType.testing)
                    {
                        Main.SetScene(Main.ActiveScene.editor);
                    }

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