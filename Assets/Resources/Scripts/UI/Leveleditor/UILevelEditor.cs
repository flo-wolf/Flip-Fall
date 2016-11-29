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

            Main.onSceneChange.AddListener(SceneChanged);
        }

        private void SceneChanged(Main.Scene s)
        {
            if (s == Main.Scene.game)
            {
                animator.SetTrigger("fadeout");
            }
        }

        public void NewLevel()
        {
        }

        public void UndoButton()
        {
            if (!UIObjectlPreferences.menuOpen)
            {
                if (UndoManager.Undo())
                {
                    //animation
                }
            }
        }

        public void RedoButton()
        {
            if (!UIObjectlPreferences.menuOpen)
            {
                if (UndoManager.Redo())
                {
                    //animation
                }
            }
        }

        public void SaveButton()
        {
            if (!UIObjectlPreferences.menuOpen)
            {
                LevelEditor.SaveLevel();
                animator.SetTrigger("saveRequest");
            }
        }

        public void BackButton()
        {
            if (!UIObjectlPreferences.menuOpen)
            {
                UIObjectlPreferences.menuOpen = true;
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
        }

        public void LeaveUnsaved()
        {
            animator.SetTrigger("leaveUnsaved");

            // retore the last savepoint
            UndoManager.RestoreSavePoint();

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

        public void TestButton()
        {
            if (!UIObjectlPreferences.menuOpen)
            {
                if (LevelEditor.TryTestLevel())
                {
                    // animations
                }
            }
        }

        public void HelpButton()
        {
        }

        // show or hide the delete button
        public static void DeleteShow(bool active)
        {
            if (_instance != null)
            {
                Debug.Log("DELETE SHOW " + active);
                PreferencesShow(active);
                if (active && LevelEditor.selectedObject != null)
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

        // show or hide the preferences button
        public static void PreferencesShow(bool active)
        {
            Debug.Log("Preferences SHOW " + active);
            if (_instance != null)
            {
                if (active && LevelEditor.selectedObject != null)
                {
                    _instance.animator.ResetTrigger("prefHide");
                    _instance.animator.ResetTrigger("prefShow");
                    _instance.animator.SetTrigger("prefShow");
                }
                else
                {
                    _instance.animator.ResetTrigger("prefShow");
                    _instance.animator.ResetTrigger("prefHide");
                    _instance.animator.SetTrigger("prefHide");
                }
            }
        }

        // the delete button got clicked
        public void DeleteButton()
        {
            if (!UIObjectlPreferences.menuOpen)
            {
                // the movearea is selected, thus delete vertices
                if (LevelEditor.selectedObject.objectType == LevelObject.ObjectType.moveArea)
                    VertHandler._instance.DeleteAllSelectedVerts();

                // another levelobject is selected, delete it and reward the item back into the inventory
                else if (LevelEditor.selectedObject.objectType != LevelObject.ObjectType.moveArea)
                {
                    _instance.animator.SetTrigger("deleteHide");
                    _instance.animator.SetTrigger("prefHide");
                    LevelPlacer.generatedLevel.DeleteObject(LevelEditor.selectedObject);
                }
            }
        }

        // the preferences button got clicked
        public void PreferencesButton()
        {
            if (!UIObjectlPreferences.menuOpen)
            {
                // some levelobject is selected, which is not a movearea - open the preferences window to edit it
                if (LevelEditor.selectedObject.objectType != LevelObject.ObjectType.moveArea)
                {
                    // _instance.animator.SetTrigger("prefHide");
                    // _instance.animator.SetTrigger("deleteHide");

                    UIPreferenceMenu menu = UIObjectlPreferences.ShowMenu();
                    Debug.Log(menu);
                }
            }
        }

        // toggles the grid on and of, doesn't effect the snapping
        public void GridToggle(Toggle t)
        {
            if (!UIObjectlPreferences.menuOpen)
            {
                if (t.isOn)
                    GridOverlay.Active(true);
                else
                    GridOverlay.Active(false);
            }
        }
    }
}