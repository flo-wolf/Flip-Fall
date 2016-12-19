#define AllowDoubles //Allow double entries in data?

using FlipFall.Levels;
using FlipFall.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

namespace FlipFall.Progress
{
    public class ProgressManager : MonoBehaviour
    {
        public static string SavePath = "ProgressSave.dat";
        public static string SavePathAndroid = Application.persistentDataPath + "/ProgressSave.dat";

        [SerializeField]
        private static ProgressData progress = new ProgressData();
        public static bool progressChanged = false;

        public static ProgressChangeEvent onProgressChange = new ProgressChangeEvent();

        public class ProgressChangeEvent : UnityEvent<ProgressData> { }

        public static bool IsLoaded { get; private set; }
        public static bool AllowOverriteBeforeFirstRead { get; private set; }

        public static void SetProgress(ProgressData pd)
        {
            progress = pd;
            onProgressChange.Invoke(progress);
        }

        public static ProgressData GetProgress()
        {
            return progress;
        }

        public static void ClearProgress()
        {
            Debug.Log("[ProgressManager]: Clear Progress");

            Settings s = new Settings();
            s = progress.settings;
            ProgressData resetStats = new ProgressData();
            resetStats.settings = s;

            SetProgress(resetStats);
        }

        public static void LoadProgressData()
        {
            ProgressData progressLoading;
            string savePath;
#if UNITY_ANDROID && !UNITY_EDITOR
            savePath = SavePathAndroid;
#else
            savePath = SavePath;
#endif

            if (File.Exists(savePath))
            {
                try
                {
                    progressLoading = JsonUtility.FromJson<ProgressData>(File.ReadAllText(savePath, Encoding.UTF8));

                    // check object integrity - aka were objects added or removed - not effected by object attribute changes
                    string loadedChecksum = progressLoading.checksum;
                    if (loadedChecksum == progressLoading.GenerateChecksum())
                    {
                        Debug.Log("Progress checksums are the same, have fun!");
                        progress = progressLoading;
                        progress.proVersion = InAppBilling.ProIsOwned();
                    }
                    else
                    {
                        Debug.LogError("Progress checksums mismatching. Someone tried to mess with the it!");
                        SaveProgressData();
                    }
                }
                catch (SerializationException e)
                {
                    Debug.LogError("[LevelLoader]: Failed to deserialize progress. Reason: " + e.Message);
                    throw;
                }
                IsLoaded = true;
            }
            else
            {
                SaveProgressData();
            }
        }

        public static void SaveProgressData()
        {
            string savePath;
#if UNITY_ANDROID && !UNITY_EDITOR
            savePath = SavePathAndroid;
#else
            savePath = SavePath;
#endif

            //LevelManager.onLevelChange.AddListener(LevelStateChanged);

            if (!File.Exists(savePath))
            {
                FileStream file;
                file = File.Create(savePath);

                using (StreamWriter sw = new StreamWriter(file))
                {
                    progress.checksum = progress.GenerateChecksum();

                    string jsonLevelData = JsonUtility.ToJson(progress);
                    // dont overwrite, just add - there is nothing to overwrite anyways
                    sw.Write(jsonLevelData);
                }
                file.Close();
            }
            // the file does exist, overwrite its contents
            else
            {
                using (StreamWriter sw = new StreamWriter(savePath, false))
                {
                    progress.checksum = progress.GenerateChecksum();

                    string jsonProgress = JsonUtility.ToJson(progress);
                    sw.Write(jsonProgress);
                }
            }
        }

        public static void LevelStateChanged(int levelID)
        {
            progress.storyProgress.lastPlayedLevelID = levelID;
        }

        public static void ClearHighscore(int _id)
        {
            if (progress.highscores.highscores.Any(x => x.levelId == _id))
            {
                //var model = (Scoreboard)progress.scoreboards.FirstOrDefault(x => x.levelId == _id);
                progress.highscores.highscores.Remove(progress.highscores.highscores.Find(x => x.levelId == _id));
            }
        }
    }
}