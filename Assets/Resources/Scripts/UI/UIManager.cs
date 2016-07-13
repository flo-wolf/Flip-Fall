using Sliders.Levels;
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

        public static UIManager uiManager;

        public AudioClip playSound;
        public UIDeathscreen uiDeathscreen;
        public UIScoreboard uiScoreboard;
        public UITimer uiTimer;
        public Text levelID;
        public GameObject deathscreen;
        public Button play;

        private void Start()
        {
            uiManager = this;
            Game.onGameStateChange.AddListener(GameStateChanged);
        }

        private void GameStateChanged(Game.GameState gameState)
        {
            switch (gameState)
            {
                case Game.GameState.playing:
                    UIScoreboard.uiScoreboard.Hide(); //add fancy fadeouts, save
                    SoundManager.instance.PlaySingle(playSound);
                    deathscreen.SetActive(false);
                    play.gameObject.SetActive(false);
                    uiTimer.Run();
                    break;

                case Game.GameState.deathscreen:
                    uiTimer.Pause();
                    deathscreen.SetActive(true);
                    UIScoreboard.uiScoreboard.Show();
                    CameraMovement.SetCameraState(CameraMovement.CameraState.resting);
                    //display scoreboard
                    break;

                case Game.GameState.ready:
                    play.gameObject.SetActive(true);
                    break;

                case Game.GameState.finishscreen:
                    uiTimer.Pause();
                    deathscreen.SetActive(true);
                    UIScoreboard.uiScoreboard.ShowAndUpdate(uiTimer.GetTime()); //add fancy fadeouts
                    CameraMovement.SetCameraState(CameraMovement.CameraState.resting);
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