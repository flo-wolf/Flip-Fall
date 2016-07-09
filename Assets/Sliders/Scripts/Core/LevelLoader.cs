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
    public class LevelLoader : MonoBehaviour
    {
        public static Level defaultLevel;
        public LevelPlacer levelPlacer;

        public const string SavePath = "Levels.dat";
        public static bool IsLoaded { get; private set; }
        public static bool AllowOverriteBeforeFirstRead { get; private set; }

        private static List<Level> levelsLoading;

        private void Start()
        {
            ReloadAll();
        }

        public static List<Level> ReloadAll()
        {
            Debug.Log("levelLoader - ReloadALl");
            if (File.Exists(SavePath))
            {
                var fs = new FileStream(SavePath, FileMode.Open);
                try
                {
                    var bf = new BinaryFormatter();
                    levelsLoading = bf.Deserialize(fs) as List<Level>;
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
            return levelsLoading;
        }

        public static void SaveLevels()
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
                bf.Serialize(file, LevelManager.loadedLevels);
                file.Close();
            }
        }

        public static Level Reload(int id)
        {
            //Level already known
            if (LevelManager.loadedLevels.Any(x => x.id == id))
            {
                Level l = LevelManager.loadedLevels.Find(y => y.id == id);
                Debug.Log("LevelLoader.Load - 1");
                return levelsLoading.Find(x => x.id == id);
            }
            else
            {
                Debug.Log("LevelLoader.Load - 2");
                return LevelPlacer.Place(defaultLevel);
            }
        }
    }
}