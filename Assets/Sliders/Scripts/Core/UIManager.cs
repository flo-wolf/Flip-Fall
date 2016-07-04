using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Sliders.UI
{
    public class UIManager : MonoBehaviour
    {
        /*
        This class controls all UI Elements like
        Scoreboards, Levelselection, Playbuttons, Instructions, Timers,..
        depending on the current gamestate, called by listeners
        */

        public UIScoreboard scoreboard;
        public UITimer timer;
        public Text levelInfo;
        public Button next;
        public Button last;
        public Button play;

        private void Start()
        {
            Game.onGameStateChange.AddListener(GameStateChanged);
        }

        private void GameStateChanged(Game.GameState gameState)
        {
            switch (gameState)
            {
                case Game.GameState.playing:
                    scoreboard.Hide();
                    timer.Run();
                    timer.Reset();
                    timer.Continue();
                    break;

                case Game.GameState.deathscreen:
                    timer.Pause();
                    scoreboard.Show(timer.GetTime());
                    CameraMovement.SetCameraState(CameraMovement.CameraState.resting);
                    //display scoreboard
                    break;

                case Game.GameState.ready:
                    break;

                case Game.GameState.finishscreen:
                    break;

                default:
                    Debug.LogError("Incorrect PlayerState");
                    break;
            }
        }

        public void HideTimer()
        {
        }

        public void ShowTimer()
        {
        }

        public void HideScoreboard()
        {
        }

        public void ShowScoreboard()
        {
        }

        public void HideLevelInfo()
        {
        }

        public void ShowLevelInfo()
        {
        }

        public void HideNext()
        {
        }

        public void ShowNext()
        {
        }

        public void HideLast()
        {
        }

        public void ShowLast()
        {
        }

        // Update is called once per frame
        private void Update()
        {
        }
    }
}