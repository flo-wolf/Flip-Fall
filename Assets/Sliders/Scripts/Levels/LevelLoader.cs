using Sliders.Models;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Sliders
{
    public static class LevelLoader
    {
        public const string SavePath = "Levels.dat";
        public static bool IsLoaded { get; private set; }

        private static List<LevelData> levelsLoading;

        public static List<LevelData> LoadLevels()
        {
            Debug.Log("levelLoader - ReloadALl");
            if (File.Exists(SavePath))
            {
                var fs = new FileStream(SavePath, FileMode.Open);
                try
                {
                    var bf = new BinaryFormatter();
                    levelsLoading = bf.Deserialize(fs) as List<LevelData>;
                    return levelsLoading;
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
            else
            {
                SaveLevels();
                return LoadLevels();
            }
            return levelsLoading;
        }

        public static void SaveLevels()
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
            bf.Serialize(file, LevelManager.loadedLevels);
            file.Close();
        }
    }
}