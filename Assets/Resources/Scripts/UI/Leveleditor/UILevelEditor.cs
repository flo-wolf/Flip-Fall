using FlipFall;
using FlipFall.Audio;
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
        public ScrollRect inventoryScrollRect;

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

        private void SceneChanged(Main.ActiveScene s)
        {
            if (s == Main.ActiveScene.game)
            {
                animator.SetTrigger("fadeout");
            }
        }

        public void NewLevel()
        {
        }

        public void UndoButton()
        {
            if (!UIObjectPreferences.menuOpen)
            {
                if (UndoManager.Undo())
                {
                    SoundManager.ButtonClicked();
                    //animation
                }
                else
                {
                    SoundManager.PlayUnvalidSound();
                }
            }
        }

        public void RedoButton()
        {
            if (!UIObjectPreferences.menuOpen)
            {
                if (UndoManager.Redo())
                {
                    SoundManager.ButtonClicked();
                    //animation
                }
                else
                {
                    SoundManager.PlayUnvalidSound();
                }
            }
        }

        public void SaveButton()
        {
            SoundManager.ButtonClicked();
            if (!UIObjectPreferences.menuOpen)
            {
                LevelEditor.SaveLevel();
                animator.SetTrigger("saveRequest");
            }
        }

        public void BackButton()
        {
            SoundManager.ButtonClicked();
            if (!UIObjectPreferences.menuOpen)
            {
                UIObjectPreferences.menuOpen = true;
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
            SoundManager.ButtonClicked();
            animator.SetTrigger("leaveUnsaved");

            // retore the last savepoint
            UndoManager.RestoreSavePoint();

            DeleteShow(false);
            Main.SetScene(Main.ActiveScene.editor);
        }

        public void LeaveSave()
        {
            SoundManager.ButtonClicked();
            animator.SetTrigger("leaveSave");
            LevelEditor.SaveLevel();
            DeleteShow(false);
            Main.SetScene(Main.ActiveScene.editor);
        }

        public void LeaveAbort()
        {
            animator.SetTrigger("leaveAbort");
        }

        public void TestButton()
        {
            if (!UIObjectPreferences.menuOpen)
            {
                if (LevelEditor.TryTestLevel())
                {
                    SoundManager.ButtonClicked();
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
            if (!UIObjectPreferences.menuOpen)
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
                SoundManager.ButtonClicked();
            }
        }

        // the preferences button got clicked
        public void PreferencesButton()
        {
            if (!UIObjectPreferences.menuOpen)
            {
                // some levelobject is selected, which is not a movearea - open the preferences window to edit it
                if (LevelEditor.selectedObject.objectType != LevelObject.ObjectType.moveArea)
                {
                    // _instance.animator.SetTrigger("prefHide");
                    // _instance.animator.SetTrigger("deleteHide");

                    UIPreferenceMenu menu = UIObjectPreferences.ShowMenu();
                    SoundManager.ButtonClicked();
                    Debug.Log(menu);
                }
            }
            else
            {
                SoundManager.PlayUnvalidSound();
            }
        }

        // toggles the grid on and of, doesn't effect the snapping
        public void GridToggle(Toggle t)
        {
            SoundManager.ButtonClicked();
            if (!UIObjectPreferences.menuOpen)
            {
                if (t.isOn)
                    GridOverlay.Active(true);
                else
                    GridOverlay.Active(false);
            }
        }
    }
}