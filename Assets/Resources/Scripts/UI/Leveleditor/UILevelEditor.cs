using FlipFall.Editor;
using Impulse;
using Impulse.Levels;
using Impulse.Progress;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Controls all the ui button intput of the Editor scene
/// </summary>

namespace FlipFall.UI
{
    public class UILevelEditor : MonoBehaviour
    {
        public Animator animator;

        #region Public Methods

        public void Start()
        {
        }

        public void NewLevel()
        {
        }

        public void RemoveLevel()
        {
        }

        public void SaveButton()
        {
            LevelEditor.SaveLevel();
            print("Editor: Level saved.");
        }

        public void HomeButton()
        {
            Main.SetScene(Main.Scene.home);
            animator.SetTrigger("fadeout");
        }

        public void SelectButton()
        {
            LevelEditor.editorMode = LevelEditor.EditorMode.selectVertex;
            print("----- SELECTING ");
        }

        public void MoveButton()
        {
            LevelEditor.editorMode = LevelEditor.EditorMode.moveVertex;
            print("----- Moving " + VertHandler.selectedHandles.Count + " elements.");
        }

        public void GridToggle(Toggle t)
        {
            if (t.isOn)
                GridOverlay.Active(true);
            else
                GridOverlay.Active(false);
        }

        #endregion Public Methods
    }
}