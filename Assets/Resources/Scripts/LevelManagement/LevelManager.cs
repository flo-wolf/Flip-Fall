using FlipFall.Levels;

using FlipFall.Levels;

using FlipFall.Progress;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// This class manages all incoming calls of other game components regarding leveldata modification and saving.
/// Levels are serializable and saved to a file.
/// </summary>

namespace FlipFall.Levels
{
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager _instance;

        public static LevelChangeEvent onLevelChange = new LevelChangeEvent();

        public class LevelChangeEvent : UnityEvent<int> { }

        public static List<Level> prefabLevels = new List<Level>();
        public static List<LevelData> customLevels = new List<LevelData>();

        private static int activeLevel = 1;
        public static int lastPlayedID;
        public static int lastID;
        public static int firstID;

        public float DissolveDelay = 0.2f;
        public float DissolveLevelDuration = 0.3f;

        // is this already loaded
        public static bool started = false;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(this.gameObject);

            if (!started)
            {
                prefabLevels = LevelLoader.LoadPrefabLevels();
                customLevels = LevelLoader.LoadCustomLevels();
                firstID = prefabLevels.First<Level>().id;
                lastID = prefabLevels.Last<Level>().id;
            }
        }

        public static LevelData NewCustomLevel(int id)
        {
            LevelData l = new LevelData(id);
            customLevels.Add(l);
            LevelLoader.SaveCustomLevel(l);
            return l;
        }

        //This has to be OnEnable for loading order purposes.
        private void OnEnable()
        {
            if (!started)
            {
                lastPlayedID = ProgressManager.GetProgress().lastPlayedLevelID;
                if (lastPlayedID <= ProgressManager.GetProgress().lastUnlockedLevel)
                    activeLevel = lastPlayedID;
                else
                    activeLevel = 1;
                started = true;
            }
        }

        public static int GetActiveID()
        {
            return activeLevel;
        }

        public static Level GetActiveLevel()
        {
            return prefabLevels.Find(x => x.id == activeLevel);
        }

        public static Level GetLevel()
        {
            return LevelPlacer.placedLevel;
        }

        // load prefab level
        public static Level GetLevel(int id)
        {
            return LevelLoader.LoadPrefabLevel(id);
        }

        public static void SetLevel(int newID)
        {
            Debug.Log("Setlevel " + newID);
            activeLevel = newID;
            ProgressManager.GetProgress().lastPlayedLevelID = activeLevel;
            onLevelChange.Invoke(newID);
        }

        //Does this level exitst?
        public static bool LevelExists(int newID, bool custom)
        {
            if (!custom)
            {
                if (prefabLevels.Any<Level>(x => x.id == newID))
                    return true;
            }
            else
            {
                if (customLevels.Any<LevelData>(x => x.id == newID))
                    return true;
            }

            return false;
        }
    }
}