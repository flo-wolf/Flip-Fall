using Impulse.Levels;
using Impulse.Progress;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// This class manages all incoming calls of other game components regarding leveldata modification and saving.
/// Levels are serializable and saved to a file.
/// </summary>

namespace Impulse.Levels
{
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager _instance;
        public static List<Level> levels = new List<Level>();
        public static LevelChangeEvent onLevelChange = new LevelChangeEvent();

        public class LevelChangeEvent : UnityEvent<int> { }

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
                levels = LevelLoader.LoadLevels();
                firstID = levels.First<Level>().id;
                lastID = levels.Last<Level>().id;
            }
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
            return levels.Find(x => x.id == activeLevel);
        }

        public static Level GetLevel()
        {
            return LevelPlacer.placedLevel;
        }

        public static Level GetLevel(int id)
        {
            return LevelLoader.LoadLevel(id);
        }

        public static Vector3 GetSpawnPosition()
        {
            return GetSpawn().GetPosition();
        }

        public static void SetLevel(int newID)
        {
            Debug.Log("Setlevel " + newID);
            activeLevel = newID;
            ProgressManager.GetProgress().lastPlayedLevelID = activeLevel;
            onLevelChange.Invoke(newID);
        }

        public static Spawn GetSpawn()
        {
            //Debug.Log("activeLevel: " + activeLevel);
            return GetLevel().spawn;
        }

        //Does this level exitst?
        public static bool LevelExists(int newID)
        {
            if (levels.Any<Level>(x => x.id == newID))
                return true;
            return false;
        }
    }
}