using Sliders.Levels;
using Sliders.Progress;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

/*
* This class manages all incoming calls of other game components regarding leveldata modification and saving.
* Levels are serializable and saved to a file.
*/

namespace Sliders.Levels
{
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager levelManager;
        public static LevelChangeEvent onLevelChange = new LevelChangeEvent();

        public class LevelChangeEvent : UnityEvent<Level> { }

        private Level activeLevel = new Level();
        public Level defaultlevel = new Level();

        private void Awake()
        {
            levelManager = this;
        }

        private void Start()
        {
            Reload();
        }

        public void SetLevel(Level newLevel)
        {
            activeLevel = newLevel;
        }

        public Level GetLevel()
        {
            return activeLevel;
        }

        public void Reload()
        {
            int lastID = ProgressManager.progress.lastPlayedLevelID;
            Debug.Log("[LevelManager]: Reload() lastID: " + lastID);
            SetLevel(lastID);
        }

        public int GetID()
        {
            return levelManager.activeLevel.id;
        }

        public void NextLevel()
        {
            Debug.Log("[LevelManager]: NextLevel()");
            if (levelManager.activeLevel.id >= 0)
            {
                SetLevel(activeLevel.id + 1);
            }
        }

        public void PreviousLevel()
        {
            Debug.Log("[LevelManager]: NextLevel()");
            if (levelManager.activeLevel.id >= 0)
            {
                SetLevel(activeLevel.id - 1);
            }
        }

        //destroy previous levels + scoreboard updates
        public static void SetLevel(int newID)
        {
            if (LevelLoader.LoadLevel(newID) != null)
            {
                levelManager.activeLevel = LevelLoader.LoadLevel(newID);
                ProgressManager.SetLastPlayedLevel(levelManager.GetID());
                levelManager.activeLevel = LevelPlacer.Place(levelManager.activeLevel);
                onLevelChange.Invoke(levelManager.activeLevel);
            }
            else
            {
                Debug.Log("[LevelManager]: SetLevel(int) Level trying to be set does not exist!");
            }
        }

        public static Spawn GetSpawn()
        {
            //Placeholder, get spawnlocation from the current level.
            Spawn spawn = levelManager.activeLevel.spawn;
            return spawn;
        }

        public static Vector2 GetFinish()
        {
            Vector2 finishlocation = new Vector2();
            return finishlocation;
        }
    }
}