#define AllowDoubles //Allow double entries in data?

using Sliders.Models;
using Sliders.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Sliders
{
    public static class ProgressManager
    {
        public const string SavePath = "ProgressSave.dat";
        public static ProgressData progress;
        public static bool IsLoaded { get; private set; }
        public static bool AllowOverriteBeforeFirstRead { get; private set; }

        public static ProgressData ProgressdData
        {
            get
            {
                return progress;
            }
        }

        public static void LoadProgressData()
        {
            if (File.Exists(SavePath))
            {
                var fs = new FileStream(SavePath, FileMode.Open);
                try
                {
                    var bf = new BinaryFormatter();
                    progress = bf.Deserialize(fs) as ProgressData;
                }
                catch (SerializationException e)
                {
                    Debug.LogError("Failed to deserialize. Reason: " + e.Message);
                    throw;
                }
                finally
                {
                    fs.Close();
                }
                IsLoaded = true;
            }
        }

        public static void SaveProgressData()
        {
            if (IsLoaded || AllowOverriteBeforeFirstRead || !File.Exists(SavePath))
            {
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
                bf.Serialize(file, progress);
                file.Close();
            }
        }

        public static void SaveTime(double time)
        {
            //Scoreboard doesnt exist
            if (!progress.scoreboards.Any(x => x.levelId == LevelManager.level.id))
            {
                //create new
                Scoreboard scoreboard = new Scoreboard();
                scoreboard.levelId = LevelManager.level.id;
                scoreboard.created = DateTime.UtcNow;
                scoreboard.updated = DateTime.UtcNow;
                scoreboard.TryPlacingTime(time);

                progress.scoreboards.Add(scoreboard);

                //sort time into elements, while only allowing 10 items.
                //scoreboard.elements
            }
            //Scoreboard exists already
            else
            {
                Scoreboard scoreboard = progress.GetScoreboard(LevelManager.level.id);
                scoreboard.TryPlacingTime(time);
                scoreboard.updated = DateTime.UtcNow;
                Debug.LogError("PlayerProgression: Could not add level progress, it already exists");
            }
        }

        public static void ClearProgress()
        {
            progress = new ProgressData();
        }

        public static void ClearLevelScores(int _id)
        {
            if (progress.scoreboards.Any(x => x.levelId == _id))
            {
                var model = (Scoreboard)progress.scoreboards.FirstOrDefault(x => x.levelId == _id);
                progress.scoreboards.Remove(model);
            }
        }

        public static bool IsLevelFinished(int _id)
        {
            return progress.scoreboards.Any(x => x.finished);
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

        /*
        public static void FinishLevel(int _id, double _time)
        {
#if AllowDoubles
            AddLevelProgress(_id, _time, true);
#else
            //Update
            if (_ProgressData.Any(x => x.id == _id))
            {
                var model = _ProgressData.FirstOrDefault(x => x.id == _id);
                if (_time > model.time && _time > 0)
                {
                    model.time = _time;
                }
                model.updated = DateTime.UtcNow;
            } else
            {
                AddProgressData(_id, _time, true);
            }
#endif
        }
        */
    }
}