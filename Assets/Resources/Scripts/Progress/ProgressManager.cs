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
        public const string SavePath = "ProgressSave.dat";

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
            if (File.Exists(SavePath))
            {
                var fs = new FileStream(SavePath, FileMode.Open);
                try
                {
                    var bf = new BinaryFormatter();
                    SetProgress(bf.Deserialize(fs) as ProgressData);
                    if (progress.scoreboards.Count > 0)
                    {
                        Debug.Log("[ProgressManager]: LoadProgressData() element count in first scoreboard: " + progress.scoreboards[0].elements.Count);
                        Debug.Log("[ProgressManager]: LoadProgressData() id of first scoreboard: " + progress.GetScoreboard(UIScoreboardsManager.currentPage));
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
            LevelManager.onLevelChange.AddListener(LevelStateChanged);

            FileStream file;
            if (!File.Exists(SavePath))
            {
                file = File.Create(SavePath);
            }
            else
            {
                file = new FileStream(SavePath, FileMode.Open);
            }

            var bf = new BinaryFormatter();
            if (progress.scoreboards.Count > 0)
                Debug.Log("[ProgressManager]: SaveProgressData() id of current level " + progress.GetLastPlayedID() + " scoreboard: " + progress.scoreboards.Find(x => x.levelId == progress.GetLastPlayedID()).levelId);
            bf.Serialize(file, progress);
            file.Close();
        }

        public static void LevelStateChanged(Level level)
        {
            progress.SetLastPlayedID(level.id);
        }

        public static void SaveTime()
        {
            double time = Timer.pauseTime;
            ProgressData p = progress;
            Scoreboard scoreboard = new Scoreboard();

            //Scoreboard doesnt exist
            if (!progress.scoreboards.Any(x => x.levelId == LevelManager.GetID()))
            {
                //create new

                scoreboard.levelId = LevelManager.GetID();
                scoreboard.created = DateTime.UtcNow;
                scoreboard.updated = DateTime.UtcNow;
                scoreboard.TryPlacingTime(time);
            }
            //Scoreboard exists already
            else
            {
                scoreboard = progress.scoreboards.Find(x => x.levelId == LevelManager.GetID());
                scoreboard.TryPlacingTime(time);
                scoreboard.updated = DateTime.UtcNow;
            }
            p.scoreboards.Add(scoreboard);
            SetProgress(p);
        }

        public static void ClearScores()
        {
            if (progress.scoreboards.Any(x => x.levelId == progress.GetLastPlayedID()))
            {
                var model = (Scoreboard)progress.scoreboards.FirstOrDefault(x => x.levelId == progress.GetLastPlayedID());
                progress.scoreboards.Remove(model);
            }
        }

        public static void ClearLevelScores(int _id)
        {
            if (progress.scoreboards.Any(x => x.levelId == _id))
            {
                //var model = (Scoreboard)progress.scoreboards.FirstOrDefault(x => x.levelId == _id);
                progress.scoreboards.Remove(progress.scoreboards.Find(x => x.levelId == _id));
            }
        }

        public static bool IsLevelFinished(int _id)
        {
            return progress.scoreboards.Any(x => x.finished);
        }

        public static void FinishLevel()
        {
            Debug.Log("[ProgressManager]: FinishLevel()");
            SaveTime();
        }

        public static double GetBestTime(int _id)
        {
            if (progress.scoreboards.Any(x => x.levelId == _id))
            {
                var model = progress.scoreboards.FirstOrDefault(x => x.levelId == _id);
                return model.elements[0].time;
            }
            return -1D;
        }
    }
}