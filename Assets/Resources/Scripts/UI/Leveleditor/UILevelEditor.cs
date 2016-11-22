using FlipFall;
using FlipFall.Editor;
using FlipFall.LevelObjects;
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
        public static UILevelEditor _instance;

        public Animator animator;

        private bool unsavedLeaveRequest = false;
        private bool saveRequest = false;

        public void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
                return;
            }
            _instance = this;
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
            // reward the used items back, because we do not save the changes
            foreach (LevelObject.ObjectType objectType in LevelPlacer.generatedLevel.changedObjects.Keys)
            {
                ProgressManager.GetProgress().unlocks.inventory.Add(objectType, (int)LevelPlacer.generatedLevel.changedObjects[objectType]);
            }

            DeleteShow(false);
            Main.SetScene(Main.Scene.editor);
        }

        public void LeaveSave()
        {
            animator.SetTrigger("leaveSave");
            LevelEditor.SaveLevel();
            DeleteShow(false);
            Main.SetScene(Main.Scene.editor);
        }

        public void LeaveAbort()
        {
            animator.SetTrigger("leaveAbort");
        }

        public void PreferencesButton()
        {
        }

        public void TestButton()
        {
            if (LevelEditor.TryTestLevel())
            {
                // animations
            }
        }

        public void HelpButton()
        {
        }

        public static void DeleteShow(bool active)
        {
            Debug.Log("DELETE SHOW " + active);
            if (_instance != null)
            {
                if (active)
                {
                    _instance.animator.ResetTrigger("deleteHide");
                    _instance.animator.ResetTrigger("deleteShow");
                    _instance.animator.SetTrigger("deleteShow");
                }
                else
                {
                    _instance.animator.ResetTrigger("deleteShow");
                    _instance.animator.ResetTrigger("deleteHide");
                    _instance.animator.SetTrigger("deleteHide");
                }
            }
        }

        public void DeleteButton()
        {
            // the movearea is selected, thus delete vertices
            if (LevelEditor.selectedObject.objectType == LevelObject.ObjectType.moveArea)
                VertHandler._instance.DeleteAllSelectedVerts();

            // another levelobject is selected, delete it and reward the item back into the inventory
            else if (LevelEditor.selectedObject.objectType != LevelObject.ObjectType.moveArea)
            {
                _instance.animator.SetTrigger("deleteHide");
                LevelPlacer.generatedLevel.DeleteObject(LevelEditor.selectedObject);
            }
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