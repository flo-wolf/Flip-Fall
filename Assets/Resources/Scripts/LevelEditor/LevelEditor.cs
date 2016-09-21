using System.Collections;
using UnityEngine;

namespace Impulse
{
    public class Leveleditor : MonoBehaviour
    {
        //look at tileeditor sctipt to get an idea how a level editor could be build up like

        public enum EDITORMODE { setup, draw, select, move }

        public static EDITORMODE EditorMode = EDITORMODE.setup;

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