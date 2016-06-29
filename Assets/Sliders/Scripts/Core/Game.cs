using Sliders;
using Sliders.Models;
using Sliders.UI;
using System.Collections;
using UnityEngine;

namespace Sliders
{
    public class Game : MonoBehaviour
    {
        public enum GameState { editor, playing, ready }

        public LevelDataModel currentLevel;
        public UITimer timer;
        public Player player;
        public GameState gamestate;

        private bool firsttime;

        private void Start()
        {
            //load progress -> set firsttime
            //firsttime? -> load tutorial
            //load last played level -> get value from "progress"
        }

        public void loadLevel(int levelID)
        {
            //currentlevel = levelloader.load(levelID);
        }

        public void finishLevel()
        {
            //save progress
        }

        public void restartLevel()
        {
            //cam to beginning
            //player to beginning
            //restart timer
        }
    }
}