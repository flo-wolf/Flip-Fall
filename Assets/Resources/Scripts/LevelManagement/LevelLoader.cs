using FlipFall.Levels;
using FlipFall.Progress;
using FlipFall.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;

/// <summary>
/// Level Loading Component. Loads story level prefabs and/or loads and safes custom serialized levels.
/// </summary>

namespace FlipFall.Levels
{
    public static class LevelLoader
    {
        public static bool IsLoaded { get; private set; }

        public static string SavePath = "CustomLevels/";
        public static string SavePathAndroid = Application.persistentDataPath + "/";
        public static string saveExtention = ".json";

        public static List<LevelData> LoadCustomLevels()
        {
            string path;
#if UNITY_ANDROID && !UNITY_EDITOR
            path = SavePathAndroid;
#else
            path = SavePath;
#endif
            // get the names of all levels in the directory
            DirectoryInfo info = new DirectoryInfo(path);
            FileInfo[] fileInfos = info.GetFiles();

            Debug.Log("fileInfo length: " + fileInfos.Length);

            List<LevelData> dataLoading = new List<LevelData>();
            foreach (FileInfo fi in fileInfos)
            {
                // load each leveldata in the directory, but only if its not the progress
                if (!fi.Name.Contains("ProgressSave"))
                {
                    string filename = fi.Name.Replace(saveExtention, "");
                    dataLoading.Add(LoadCustomLevel(filename));
                }
            }
            return dataLoading;
        }

        // load and return all LevelData contained in the resources folder named "StoryLevels"
        public static List<LevelData> LoadStoryLevels()
        {
            string path;
#if UNITY_ANDROID && !UNITY_EDITOR
            path = SavePathAndroid;
#else
            path = SavePath;
#endif
            List<LevelData> dataLoading = new List<LevelData>();

            TextAsset[] storyFiles = Resources.LoadAll<TextAsset>("StoryLevels");
            foreach (TextAsset t in storyFiles)
            {
                dataLoading.Add(JsonUtility.FromJson<LevelData>(t.text));
            }
            return dataLoading;
        }

        public static Level LoadPrefabLevel(int id)
        {
            Debug.Log("Loaded Level " + id);
            Level level = null;
            try
            {
                GameObject go = (GameObject)Resources.Load("Prefabs/Levels/" + id);
                if (go != null)
                {
                    level = go.GetComponent<Level>();
                }
            }
            catch (UnityException e)
            {
                Debug.Log(e);
            }

            if (level == null)
            {
                Debug.Log("LevelLoader: Levelprefab could not be found.");
                return null;
            }

            IsLoaded = true;
            return level;
        }

        // saves the given levelData into the file directory, overwrites old versions of the leveldata if existent.
        public static bool SaveCustomLevel(LevelData levelData)
        {
            if (levelData != null)
            {
                string savePath;
#if UNITY_ANDROID && !UNITY_EDITOR
            savePath = SavePathAndroid;
#else
                savePath = SavePath;
#endif
                savePath = savePath + levelData.id + ".json";

                //LevelManager.onLevelChange.AddListener(LevelStateChanged);

                // the file doesn't exist yet, create it and populate it
                if (!File.Exists(savePath))
                {
                    FileStream file;
                    file = File.Create(savePath);
                    Debug.Log("[LevelLoader] Created LevelData " + levelData.id);

                    using (StreamWriter sw = new StreamWriter(file))
                    {
                        levelData.objectChecksum = levelData.GenerateObjectChecksum();
                        levelData.levelChecksum = levelData.GenerateLevelChecksum();

                        string jsonLevelData = JsonUtility.ToJson(levelData);
                        // dont overwrite, just add - there is nothing to overwrite anyways
                        sw.Write(jsonLevelData);
                    }
                    file.Close();
                }
                // the file does exist, overwrite its contents
                else
                {
                    //file = new FileStream(savePath, FileMode.Open);
                    Debug.Log("[LevelLoader] Overwritten LevelData " + levelData.id);

                    using (StreamWriter sw = new StreamWriter(savePath, false))
                    {
                        levelData.objectChecksum = levelData.GenerateObjectChecksum();
                        levelData.levelChecksum = levelData.GenerateLevelChecksum();

                        string jsonLevelData = JsonUtility.ToJson(levelData);
                        sw.Write(jsonLevelData);
                    }
                }

                return true;
            }
            return false;
        }

        // finds the levelData by id and deletes it
        public static bool DeleteCustomLevel(LevelData levelData)
        {
            Debug.Log("DDEEEEELEEEEEETEEEE");
            if (levelData != null)
            {
                string savePath;
#if UNITY_ANDROID && !UNITY_EDITOR
            savePath = SavePathAndroid;
#else
                savePath = SavePath;
#endif
                savePath = savePath + levelData.id + ".json";

                if (File.Exists(savePath))
                {
                    Debug.Log("Dadffffffffffffffffff");
                    File.Delete(savePath);
                    return true;
                }
            }
            return false;
        }

        public static LevelData LoadCustomLevel(string filename)
        {
            LevelData loadedLevelData = null;

            string savePath;
#if UNITY_ANDROID && !UNITY_EDITOR
            savePath = SavePathAndroid;
#else
            savePath = SavePath;
#endif
            savePath = savePath + filename + ".json";

            if (File.Exists(savePath))
            {
                try
                {
                    loadedLevelData = JsonUtility.FromJson<LevelData>(File.ReadAllText(savePath, Encoding.UTF8));

                    // check object integrity - aka were objects added or removed - not effected by object attribute changes
                    string loadedObjChecksum = loadedLevelData.objectChecksum;
                    if (loadedObjChecksum == loadedLevelData.GenerateObjectChecksum())
                        Debug.Log("LevelData object checksums are the same, have fun!");
                    else
                        Debug.LogError("LevelData object checksums mismatching. Someone tried to mess with the levelData!");

                    // check full level integrity - aka did ANYTHING get changed
                    string loadedLvlChecksum = loadedLevelData.levelChecksum;
                    if (loadedLvlChecksum == loadedLevelData.GenerateLevelChecksum())
                        Debug.Log("LevelData level checksums are the same, have fun!");
                    else
                        Debug.LogError("LevelData level checksums mismatching. Someone tried to mess with the levelData!");

                    Debug.Log("[LevelLoader]: Successfully loaded LevelData " + loadedLevelData.id);
                }
                catch (SerializationException e)
                {
                    Debug.LogError("[LevelLoader]: Failed to deserialize level. Reason: " + e.Message);
                    throw;
                }
                IsLoaded = true;
            }
            else
            {
                Debug.LogError("[LevelLoader]: Failed to load LevelData " + filename + ", it does not exist.");
            }

            return loadedLevelData;
        }

        //public static int LoadLevels()
        //{
        //    //= new Level[levelsLoadingLength

        //    Level l = null;
        //    int currentHighest = 1;
        //    for (int i = 0; i < Constants.lastLevel; i++)
        //    {
        //        GameObject go = (GameObject)Resources.Load("Prefabs/Levels/" + i);

        //        if (go != null)
        //        {
        //            l = go.GetComponent<Level>();

        //            if (l.id > currentHighest)
        //                currentHighest = l.id;
        //        }
        //    }
        //    return currentHighest;
        //}
    }
}