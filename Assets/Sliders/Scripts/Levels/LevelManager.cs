using Sliders.Models;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

/*
* This class manages all incoming calls of other game components regarding leveldata modification and saving.
* Levels are serializable and saved to a file.
*/

namespace Sliders
{
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager levelManager;
        public LevelData emptyLevelPrefab;
        public static LevelData activeLevel;
        public static List<LevelData> loadedLevels = new List<LevelData>();

        public static LevelChangeEvent onLevelChange = new LevelChangeEvent();

        public class LevelChangeEvent : UnityEvent<LevelData> { }

        private void Awake()
        {
            levelManager = this;
        }

        private void Start()
        {
            ReloadAll();
            PlaceActiveLevel();
        }

        public void ReloadAll()
        {
            int lastID = ProgressManager.progress.lastPlayedLevelID;
            loadedLevels = LevelLoader.LoadLevels();

            if (loadedLevels.Any(x => x.id == lastID))
                activeLevel = loadedLevels.Find(x => x.id == lastID);
            else
            {
                activeLevel = emptyLevelPrefab;
                ProgressManager.SetLastPlayedLevel(activeLevel.id);
            }

            loadedLevels.Add(activeLevel);
        }

        public static int GetLevelID()
        {
            return activeLevel.id;
        }

        public static void PlaceActiveLevel()
        {
            if (activeLevel.id > 0)
            {
                ProgressManager.SetLastPlayedLevel(activeLevel.id);
                LevelPlacer.Place(activeLevel);
            }
        }

        // Use this for initialization
        public static void LoadAndPlace(int id)
        {
            //level = LevelLoader.Last();
            //ProgressManager.pr
            //load levels from file, one of them is marked as lastPlayed
        }

        public static LevelData GetLevelAt(int _id)
        {
            var model = loadedLevels[_id];
            return model;
        }

        public static void SetLevel(int nextlevelId)
        {
            activeLevel = LevelManager.loadedLevels.Find(x => x.id == nextlevelId);
            ProgressManager.progress.lastPlayedLevelID = activeLevel.id;
            onLevelChange.Invoke(activeLevel);
        }

        public static Vector2 GetSpawn()
        {
            //Placeholder, get spawnlocation from the current level.
            Vector2 spawnlocation = new Vector2();
            return spawnlocation;
        }

        public static Vector2 GetFinish()
        {
            Vector2 finishlocation = new Vector2();
            return finishlocation;
        }

        /*
        public static void NewLevel()
        {
            int newID = loadedLevels.Count;
            if (loadedLevels.Any(x => x.id != newID))
            {
                loadedLevels.id = loadedLevels[loadedLevels.Count - 1].id + 1;
            }
            else
            {
                activeLevel.id = loadedLevels.Count + 1;
            }
            levels.Add(activeLevel);
        }

        public static void RemoveLevel()
        {
            if (levels.Contains(level))
            {
                int newID = levels.Count;
                if (newID > 0)
                {
                    Debug.Log(levels[level.id - 1].id);
                    levels.Remove(level);
                    level = levels[levels.Count - 1];
                }
                else if (levels.Count - 1 <= 0)
                {
                    levels.Remove(level);
                    NewLevel();
                }
            }
        }

        public static void RemoveLevelAt(int _id)
        {
            if (levels.Any(x => x.id == _id))
            {
                var model = levels.FirstOrDefault(x => x.id == _id);
                levels.Remove(model);
            }
        }
        */
    }
}