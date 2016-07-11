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

        public Level activeLevel = new Level();
        public Level defaultlevel = new Level();

        private void Awake()
        {
            levelManager = this;
        }

        private void Start()
        {
            Reload();
            PlaceActiveLevel();
        }

        public void Reload()
        {
            int lastID = ProgressManager.progress.lastPlayedLevelID;
            Debug.Log("[LevelManager]: Reload() lastID: " + lastID);
            activeLevel = LevelLoader.LoadLevel(lastID);
        }

        public int GetID()
        {
            return levelManager.activeLevel.id;
        }

        public void PlaceActiveLevel()
        {
            Debug.Log("[LevelManager]: PlaceActiveLevel() activelevel: " + levelManager.activeLevel.id);
            if (levelManager.activeLevel.id >= 0)
            {
                ProgressManager.SetLastPlayedLevel(levelManager.GetID());
                activeLevel = LevelPlacer.Place(levelManager.activeLevel);
            }
        }

        public static void SetLevel(int newId)
        {
            levelManager.activeLevel = LevelLoader.LoadLevel(newId);
            ProgressManager.progress.lastPlayedLevelID = levelManager.activeLevel.id;
            onLevelChange.Invoke(levelManager.activeLevel);
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