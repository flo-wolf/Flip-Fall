using Sliders.Models;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Sliders
{
    public class LevelLoader : MonoBehaviour
    {
        public LevelPlacer levelPlacer;
        private static List<Level> levelsLoading;
        private static Level currentLevel;

        public const string SavePath = "LevelsSave.dat";
        public static ProgressData progress;
        public static bool IsLoaded { get; private set; }
        public static bool AllowOverriteBeforeFirstRead { get; private set; }

        public static List<Level> Load()
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
                    Debug.Log("Failed to deserialize. Reason: " + e.Message);
                    throw;
                }
                finally
                {
                    fs.Close();
                }
                IsLoaded = true;
            }
            return levelsLoading;
        }

        public static Level Last()
        {
            return currentLevel;
        }
    }
}