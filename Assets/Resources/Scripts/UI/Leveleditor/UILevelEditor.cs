using FlipFall;
using FlipFall.Editor;
using FlipFall.Levels;
using FlipFall.Progress;
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

        private bool unsavedLeaveRequest = false;
        private bool saveRequest = false;

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
            animator.SetTrigger("saveRequest");
        }

        public void BackButton()

        {
            LevelEditor.changesAreSaved = false;
            if (!LevelEditor.changesAreSaved)
            {
                animator.SetTrigger("leaveUnsavedRequest");
            }
            else
            {
                animator.SetTrigger("fadeout");
            }
        }

        public void LeaveUnsaved()
        {
            animator.SetTrigger("leaveUnsaved");
            Main.SetScene(Main.Scene.editor);
        }

        public void LeaveSave()
        {
            animator.SetTrigger("leaveSave");
            LevelEditor.SaveLevel();
            Main.SetScene(Main.Scene.editor);
        }

        public void LeaveAbort()
        {
            animator.SetTrigger("leaveAbort");
        }

        public void SelectButton()
        {
            //LevelEditor.editorMode = LevelEditor.EditorMode.selectVertex;
            print("----- SELECTING ");
        }

        public void ModeSwitcher()
        {
            //LevelEditor.editorMode = LevelEditor.EditorMode.moveVertex;
            print("----- Moving " + VertHandler.selectedHandles.Count + " elements.");
        }

        public void GridToggle(Toggle t)
        {
            if (t.isOn)
                GridOverlay.Active(true);
            else
                GridOverlay.Active(false);
        }
    }
}