using Sliders.Models;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Sliders
{
    [SerializeField]
    public static class LevelManager
    {
        public static List<Level> levels = new List<Level>();
        public static Level level = new Level();

        // Use this for initialization
        public static void LoadLevels()
        {
            level.id = 99;
            //levels = LevelLoader.Load();
            //level = LevelLoader.Last();

            //ProgressManager.pr
            //load levels from file, one of them is marked as lastPlayed
        }

        public static void SaveLevels()
        {
        }

        public static Level GetLevelAt(int _id)
        {
            var model = levels[_id];
            return model;
        }

        public static void SetLevel(int _id, Level _level)
        {
            var model = levels.FirstOrDefault(x => x.id == _id);
            //not done!
        }

        public static void NewLevel()
        {
            int newID = levels.Count;
            if (levels.Any(x => x.id != newID))
            {
                level.id = levels[levels.Count - 1].id + 1;
            }
            else
            {
                level.id = levels.Count + 1;
            }
            levels.Add(level);
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
    }
}