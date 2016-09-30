using Impulse.Levels;
using Impulse.Progress;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

/*
* This class manages all incoming calls of other game components regarding leveldata modification and saving.
* Levels are serializable and saved to a file.
*/

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

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(this.gameObject);

            levels = LevelLoader.LoadLevels();
            firstID = levels.First<Level>().id;
            lastID = levels.Last<Level>().id;
        }

        //This has to be OnEnable for loading order purposes.
        private void OnEnable()
        {
            lastPlayedID = ProgressManager.GetProgress().lastPlayedLevelID;
            if (lastPlayedID <= ProgressManager.GetProgress().lastUnlockedLevel)
                activeLevel = lastPlayedID;
            else
                activeLevel = 1;
        }

        private void Start()
        {
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

        //
        //public void NextLevel()
        //{
        //    Debug.Log("[LevelManager]: NextLevel()");
        //    int nextID = activeLevel.id + 1;
        //    if (Resources.Load("Prefabs/Levels/" + nextID))
        //    {
        //        SetLevel(nextID);
        //    }
        //    else
        //        Debug.Log("[LevelManager]: NextLevel() could not be found.");
        //}

        //public void LastLevel()
        //{
        //    Debug.Log("[LevelManager]: LastLevel()");
        //    int nextID = activeLevel.id - 1;
        //    if (Resources.Load("Prefabs/Levels/" + nextID))
        //    {
        //        SetLevel(nextID);
        //    }
        //    else
        //        Debug.Log("[LevelManager]: LastLevel() could not be found.");
        //}
    }
}