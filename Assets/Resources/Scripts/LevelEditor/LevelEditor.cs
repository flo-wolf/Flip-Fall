using FlipFall.Levels;

using FlipFall.Levels;

using System.Collections;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace FlipFall.Editor
{
    public class LevelEditor : MonoBehaviour
    {
        //look at tileeditor sctipt to get an idea how a level editor could be build up like

        public enum EditorMode { selectObject, selectVertex, moveVertex, moveObject, addVertex }

        public static EditorMode editorMode = EditorMode.selectVertex;

        // current level getting edited
        public static LevelData editLevel;

        public static Mesh selectedMesh;

        private void Start()
        {
            if (editLevel != null)
                LevelPlacer.PlaceCustom(editLevel);
        }

        // saves the current editLevel
        public static void SaveLevel()
        {
            if (editLevel != null)
            {
                //Object prefabObj = PrefabUtility.CreateEmptyPrefab("Assets/Resources/Prefabs/Levels/Custom/" + LevelPlacer.placedLevel.id + 1 + ".prefab");
                //PrefabUtility.ReplacePrefab(LevelPlacer.placedLevel.gameObject, prefabObj, ReplacePrefabOptions.ReplaceNameBased);
                print("Editor: Level saved.");

                GameObject prefab = null;
                string outputPath = "Assets/Resources/Prefabs/Levels/Custom/" + LevelPlacer.placedLevel.id;

                prefab = (GameObject)AssetDatabase.LoadAssetAtPath(outputPath + ".prefab", typeof(GameObject));
                if (prefab == null)
                {
                    print("Editor: safe null");
                    prefab = new GameObject();
                    PrefabUtility.CreatePrefab(outputPath + ".prefab", LevelPlacer.placedLevel.gameObject);
                }

                //// From here on we can modify the prefab while each modification will update the prefab asset

                prefab = LevelPlacer.placedLevel.gameObject;

                // delete all children
                //for (int i = prefab.transform.childCount - 1; i >= 0; i--)
                //{
                //    Destroy(prefab.transform.GetChild(i));
                //}

                //// add level children
                //foreach (Transform child in LevelPlacer.placedLevel.transform)
                //{
                //    child.SetParent(prefab.transform);
                //}

                //PrefabUtility.ReplacePrefab(LevelPlacer.placedLevel.gameObject, prefab, ReplacePrefabOptions.ConnectToPrefab);
                //prefab = LevelPlacer.placedLevel.gameObject;
                // Now any changes made to the prefab object will be reflected in the prefab asset and in any instances in the scene.
            }
        }

        // loads a level by its id
        public void LoadLevel(string id)
        {
        }
    }
}