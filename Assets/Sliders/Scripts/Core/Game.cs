using Sliders;
using Sliders.Models;
using Sliders.UI;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Sliders
{
    public class Game : MonoBehaviour
    {
        public enum GameState { editor, playing, ready }

        public LevelDataModel currentLevel;
        public UITimer timer;
        public Player player;
        public GameState gamestate;

        private bool _firsttime;

        private void Start()
        {
            player.onPlayerStateChange.AddListener(PlayerStateChanged);

            //load progress -> set firsttime
            //firsttime? -> load tutorial
            //load last played level -> get value from "progress"
        }

        public void LoadLevel(int levelID)
        {
            //currentlevel = levelloader.load(levelID);
        }

        public void FinishLevel()
        {
            //save progress
        }

        public void RestartLevel()
        {
            //cam to beginning
            //player to beginning
            //restart timer
        }

        public void PlayerStateChanged(Player.PlayerState newPlayerState)
        {
            switch (newPlayerState)
            {
                case Player.PlayerState.alive:
                    break;

                case Player.PlayerState.dead:
                    break;

                case Player.PlayerState.ready:
                    break;

                default:
                    Debug.LogError("Incorrect PlayerState");
                    break;
            }
        }
    }
}
}