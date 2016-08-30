#define AllowDoubles //Allow double entries in data?

using Sliders.Levels;
using Sliders.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Events;

namespace Sliders.Progress
{
    public static class ProgressManager
    {
        public static string SavePath = "ProgressSave.dat";
        public static string SavePathAndroid = Application.persistentDataPath + "/ProgressSave.dat";

        [SerializeField]
        private static ProgressData progress = new ProgressData();

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
            SetProgress(new ProgressData());
        }

        public static void LoadProgressData()
        {
            string savePath;
#if UNITY_ANDROID && !UNITY_EDITOR
            savePath = SavePathAndroid;
#else
            savePath = SavePath;
#endif

            if (File.Exists(savePath))
            {
                var fs = new FileStream(savePath, FileMode.Open);
                try
                {
                    var bf = new BinaryFormatter();
                    SetProgress(bf.Deserialize(fs) as ProgressData);
                    if (progress.highscores.Count > 0)
                    {
                        Debug.Log("[ProgressManager]: LoadProgressData() id of first highscore: " + progress.GetHighscore(LevelManager.GetID()));
                    }
                }
                catch (SerializationException e)
                {
                    Debug.LogError("[ProgressManager]: Failed to deserialize. Reason: " + e.Message);
                    throw;
                }
                finally
                {
                    fs.Close();
                }
                IsLoaded = true;
            }
            else
            {
                SaveProgressData();
            }

            Debug.Log("[ProgressManager]: LoadProgressData() loaded. LastPlayedLevel: " + progress.GetLastPlayedID());
        }

        public static void SaveProgressData()
        {
            string savePath;
#if UNITY_ANDROID && !UNITY_EDITOR
            savePath = SavePathAndroid;
#else
            savePath = SavePath;
#endif

            LevelManager.onLevelChange.AddListener(LevelStateChanged);

            FileStream file;
            if (!File.Exists(savePath))
            {
                file = File.Create(savePath);
            }
            else
            {
                file = new FileStream(savePath, FileMode.Open);
            }

            var bf = new BinaryFormatter();
            Debug.Log("[ProgressManager]: SaveProgressData() id of current level " + progress.GetLastPlayedID());
            bf.Serialize(file, progress);
            file.Close();
        }

        public static void LevelStateChanged(Level level)
        {
            progress.SetLastPlayedID(level.id);
        }

        public static void ClearHighscore(int _id)
        {
            if (progress.highscores.Any(x => x.levelId == _id))
            {
                //var model = (Scoreboard)progress.scoreboards.FirstOrDefault(x => x.levelId == _id);
                progress.highscores.Remove(progress.highscores.Find(x => x.levelId == _id));
            }
        }
    }
}