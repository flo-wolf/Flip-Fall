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
        public UITimer uiTimer;
        public Text levelID;
        public GameObject deathscreen;
        public Button next;
        public Button last;
        public Button play;

        private void Start()
        {
            levelID.text = LevelManager.currentLevel.ToString();
            Game.onGameStateChange.AddListener(GameStateChanged);
        }

        private void GameStateChanged(Game.GameState gameState)
        {
            switch (gameState)
            {
                case Game.GameState.playing:
                    scoreboard.Hide();
                    deathscreen.SetActive(false);
                    play.gameObject.SetActive(false);
                    uiTimer.Run();
                    break;

                case Game.GameState.deathscreen:
                    uiTimer.Pause();
                    deathscreen.SetActive(true);
                    scoreboard.Show(uiTimer.GetTime());
                    CameraMovement.SetCameraState(CameraMovement.CameraState.resting);
                    //display scoreboard
                    break;

                case Game.GameState.ready:
                    play.gameObject.SetActive(true);
                    break;

                case Game.GameState.finishscreen:
                    break;

                default:
                    Debug.Log("Incorrect PlayerState");
                    break;
            }
        }

        public void PlayBtn()
        {
            Game.SetGameState(Game.GameState.playing);
            play.gameObject.SetActive(false);
        }
    }
}