using Impulse.Levels;
using System.Collections;
using UnityEngine;

namespace FlipFall.Editor
{
    public class LevelEditor : MonoBehaviour
    {
        //look at tileeditor sctipt to get an idea how a level editor could be build up like

        public enum EditorMode { selectObject, selectVertex, moveVertex, moveObject, addVertex }

        public static EditorMode editorMode = EditorMode.selectVertex;

        // current level getting edited
        public static Level editLevel;

        // standard level getting placed when a new level gets created
        public GameObject levelPrefab;

        private void Start()
        {
        }

        // saves the current editLevel
        public static void SaveLevel()
        {
        }

        // loads a level by its id
        public void LoadLevel(string id)
        {
        }

        // creates a new level
        public Level NewLevel()
        {
            Level l = levelPrefab.GetComponent<Level>();
            string id = "";
            // create level by prefab, assign id
            return l;
        }

        // swtich grid active/deactive
        public static void Grid(bool b)
        {
            if (b)
            {
                // activate grid
            }
            else
            {
                // deactivate gríd
            }
        }
    }
}