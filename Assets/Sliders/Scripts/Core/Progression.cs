#define AllowDoubles //Allow double entries in data?

using Sliders.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Sliders
{
    public static class Progression
    {
        public const string SavePath = "save.dat";
        private static List<LevelProgressionModel> _ProgressData = new List<LevelProgressionModel>();
        public static bool AllowOverriteBeforeFirstRead { get; set; }
        public static bool IsLoaded { get; private set; }

        public static List<LevelProgressionModel> LevelData
        {
            get
            {
                return _ProgressData;
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
                    _ProgressData = bf.Deserialize(fs) as List<LevelProgressionModel>;
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
                bf.Serialize(file, _ProgressData);
                file.Close();
            }
        }

        public static void AddLevelProgress(int _id, double _time, bool _completed)
        {
            if (_ProgressData.Any(x => x.id == _id))
            {
                var model = new LevelProgressionModel
                {
                    created = DateTime.UtcNow,
                    //id = _ProgressData.Count,
                    id = _id,
                    time = _time,
                    completed = _completed,
                    updated = DateTime.UtcNow
                };
                _ProgressData.Add(model);
            }
            else
            {
                Debug.LogError("PlayerProgression: Could not add level progress, it already exists");
            }
        }

        public static void SetLevelProgress(int _id, double _time, bool _completed)
        {
            //Add a check if the id can be allocated to a level, if not, dont create a progress entry for a level that doesnt exist
            if (_ProgressData.Any(x => x.id == _id))
            {
                var model = _ProgressData.FirstOrDefault(x => x.id == _id);
                model.time = _time;
                model.completed = _completed;
                model.updated = DateTime.UtcNow;
            }
            else
            {
                AddLevelProgress(_id, _time, _completed);
            }
        }

        public static void ClearProgress()
        {
            _ProgressData = new List<LevelProgressionModel>();
        }

        public static void ClearLevelProgress(int _id)
        {
            if (_ProgressData.Any(x => x.id == _id))
            {
                var model = _ProgressData.FirstOrDefault(x => x.id == _id);
                _ProgressData.Remove(model);
            }
        }

        public static bool IsLevelFinished(int _id)
        {
            return _ProgressData.Any(x => x.id == _id);
        }

        public static double GetLevelFinishTime(int _id)
        {
            double finishTime;
            if (_ProgressData.Any(x => x.id == _id))
            {
                var model = _ProgressData.FirstOrDefault(x => x.id == _id);
                finishTime = model.time;
                _ProgressData.Remove(model);
                return finishTime;
            }
            return -1D;
        }

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
    }
}