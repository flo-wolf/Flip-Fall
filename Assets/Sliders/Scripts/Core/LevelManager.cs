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
        public Level emptyLevelPrefab;
        public static Level activeLevel;
        public static List<Level> loadedLevels = new List<Level>();

        public static LevelChangeEvent onLevelChange = new LevelChangeEvent();

        public class LevelChangeEvent : UnityEvent<Level> { }

        private void Awake()
        {
            levelManager = this;
        }

        private void Start()
        {
            ReloadAll();
            LevelPlacer.Place(activeLevel);
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
            if (activeLevel.id == 5)
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

        public static Level GetLevelAt(int _id)
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