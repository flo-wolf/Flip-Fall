using FlipFall.Levels;

using FlipFall.Progress;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// This class manages the story level selection, loading and placing.
/// Levels are serializable and saved to a file.
/// </summary>

namespace FlipFall.Levels
{
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager _instance;

        public static LevelChangeEvent onLevelChange = new LevelChangeEvent();

        public class LevelChangeEvent : UnityEvent<int> { }

        public static List<LevelData> customLevels = new List<LevelData>();

        public static int activeId;
        public float DissolveDelay = 0.2f;
        public float DissolveLevelDuration = 0.3f;

        // is this already loaded
        public static bool started = false;

        public List<string> storyHashes = new List<string>();

        public static List<LevelData> storyLevels;

        private void Awake()
        {
            if (Main.currentScene == Main.ActiveScene.editor)
                customLevels = LevelLoader.LoadCustomLevels();
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(this.gameObject);

            if (!started)
            {
                storyLevels = new List<LevelData>();
                List<LevelData> foundStoryLevels = LevelLoader.LoadStoryLevels();
                foreach (LevelData l in foundStoryLevels)
                {
                    l.levelChecksum = l.GenerateLevelChecksum();
                    if (storyHashes.Contains(l.levelChecksum))
                        storyLevels.Add(l);
                }
                Debug.Log("storylevels: " + storyLevels.Count);
            }
        }

        // get the next id
        public static int GetNextId()
        {
            int newId = 1;
            // if there are custom levels and the default id is occupied find an un-occupied id and use it
            if (customLevels.Count > 0 && customLevels.Any(x => x.id == newId))
            {
                for (int i = newId + 1; i < 600; i++)
                {
                    if (!customLevels.Any(x => x.id == i))
                    {
                        newId = i;
                        break;
                    }
                }
            }
            return newId;
        }

        public static LevelData NewCustomLevel(int id)
        {
            LevelData l = new LevelData(id);
            l.title = id.ToString("D3");

            customLevels.Add(l);
            LevelLoader.SaveCustomLevel(l);
            return l;
        }

        public static LevelData GetActiveStoryLevel()
        {
            return GetStoryLevel(activeId);
        }

        public static LevelData GetStoryLevel(int id)
        {
            LevelData l = null;
            if (_instance.storyHashes.Count > id && id >= 0)
            {
                Debug.Log("GetStoryLevel " + id);
                string hash = _instance.storyHashes[id];
                l = storyLevels.Find(x => x.levelChecksum == hash);
            }
            if (l != null)

                Debug.Log("GetStoryLevel " + l.levelChecksum);
            return l;
        }

        public static int GetFirstStoryLevel()
        {
            if (storyLevels.Count > 0)
                return 0;
            else
                return -1;
        }

        public static int GetLastStoryLevel()
        {
            if (storyLevels.Count > 0)
                return storyLevels.Count - 1;
            else
                return -1;
        }

        public static LevelData GetStoryLevel(string hash)
        {
            return storyLevels.Find(x => x.levelChecksum == hash);
        }

        public static LevelData NewCustomLevel(int id, LevelData l)
        {
            l.id = id;
            customLevels.Add(l);
            LevelLoader.SaveCustomLevel(l);
            return l;
        }

        //This has to be OnEnable for loading order purposes.
        private void OnEnable()
        {
            if (!started)
            {
                int lastPlayedID = ProgressManager.GetProgress().storyProgress.lastPlayedLevelID;
                if (lastPlayedID <= ProgressManager.GetProgress().storyProgress.lastUnlockedLevel)
                    activeId = lastPlayedID;
                else
                    activeId = 1;
                started = true;
            }
        }

        public static void SetLevel(int newID)
        {
            Debug.Log("Setlevel " + newID);
            activeId = newID;
            ProgressManager.GetProgress().storyProgress.lastPlayedLevelID = activeId;
            onLevelChange.Invoke(newID);
        }

        //Does this level exitst?
        public static bool LevelExists(string hash, bool story)
        {
            if (!story)
            {
                if (customLevels.Any<LevelData>(x => x.levelChecksum == hash))
                    return true;
            }
            else
            {
                if (storyLevels.Any(x => x.levelChecksum == hash))
                    return true;
            }
            return false;
        }
    }
}