using Impulse;
using Impulse.Models;
using Impulse.UI;
using System.Collections;
using UnityEngine;

namespace Impulse
{
    public class Game : MonoBehaviour
    {
        public enum GameState { menu, editor, playing }

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