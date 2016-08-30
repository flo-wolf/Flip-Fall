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
        public static LevelManager _instance;
        public static LevelChangeEvent onLevelChange = new LevelChangeEvent();

        public class LevelChangeEvent : UnityEvent<Level> { }

        public AudioClip changeLevelSound;
        public Level defaultlevel;
        private Level activeLevel;

        private void Awake()
        {
            _instance = this;
        }

        private void OnEnable()
        {
            _instance = this;
            Reload();
        }

        private void Start()
        {
        }

        public void LoadLevel(int levelID)
        {
        }

        public static Level GetLevel()
        {
            return _instance.activeLevel;
        }

        public static Level GetLevel(int id)
        {
            return LevelLoader.LoadLevel(id);
        }

        public static void Reload()
        {
            int lastID = ProgressManager.GetProgress().GetLastPlayedID();
            Debug.Log("[LevelManager]: Reload() lastID: " + lastID);
            SetLevel(lastID);
        }

        public static int GetID()
        {
            if (_instance.activeLevel != null)
                return _instance.activeLevel.id;
            else
                return -1;
        }

        public static int GetDefaultID()
        {
            return _instance.defaultlevel.id;
        }

        public static Vector3 GetSpawnPosition()
        {
            return GetSpawn().GetPosition();
        }

        public static Spawn GetSpawn()
        {
            return _instance.activeLevel.spawn;
        }

        //Try to Place Level with ID newID, destroying all other levels in the scene
        //if the level can be placed OR if the level is alrady placed it returns true
        public static void SetLevel(int newID)
        {
            //add fadein animation

            if (Resources.Load("Prefabs/Levels/" + newID))
            {
                if (GetID() != newID)
                {
                    _instance.activeLevel = LevelLoader.LoadLevel(newID);
                    _instance.activeLevel = LevelPlacer.Place(GetLevel());
                    ProgressManager.GetProgress().SetLastPlayedID(GetID());
                    onLevelChange.Invoke(GetLevel());
                }
            }
            else
            {
                Debug.LogError("ERROR setLevel() levelprefab not found");
                Debug.Log("[LevelManager]: SetLevel(int) Level trying to be set does not exist!");
            }
        }

        public static bool LevelExists(int newID)
        {
            if (Resources.Load("Prefabs/Levels/" + newID))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static Vector2 GetFinish()
        {
            Vector2 finishlocation = new Vector2();
            return finishlocation;
        }

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