using Impulse.Levels;
using System.Collections;
using UnityEngine;

namespace Impulse
{
    public class Leveleditor : MonoBehaviour
    {
        //look at tileeditor sctipt to get an idea how a level editor could be build up like

        public enum EDITORMODE { selectObject, selectVertex, moveVertex, moveObject, addVertex }

        public static EDITORMODE EditorMode = EDITORMODE.moveVertex;

        public static Level editLevel;

        // Use this for initialization
        private void Start()
        {
        }

        // Update is called once per frame
        private void Update()
        {
        }
    }
}