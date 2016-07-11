using Sliders.Progress;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
using UnityEngine;

namespace Sliders.Levels
{
    public static class LevelLoader
    {
        public static bool IsLoaded { get; private set; }

        public static Level LoadLevel(int id)
        {
            Level level = new Level();
            try
            {
                GameObject go = (GameObject)Resources.Load("Prefabs/Levels/" + id);
                level = go.GetComponent<Level>();
            }
            catch (UnityException e)
            {
                Debug.Log(e);
            }

            if (level == null)
            {
                Debug.Log("LevelLoader: Levelprefab could not be found.");
            }
            return level;
        }

        public static void SaveLevel(Level level)
        {
            Object prefab = EditorUtility.CreateEmptyPrefab("Assets/Resources/Prefabs/" + level.id + ".prefab");
            EditorUtility.ReplacePrefab(LevelManager.levelManager.activeLevel.gameObject, prefab, ReplacePrefabOptions.ReplaceNameBased);
            //activelevel
        }
    }
}