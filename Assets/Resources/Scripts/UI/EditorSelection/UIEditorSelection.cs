using FlipFall;
using FlipFall.Audio;
using FlipFall.Levels;

using FlipFall.Progress;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the editor level selection
/// </summary>

namespace FlipFall.UI
{
    public class UIEditorSelection : MonoBehaviour
    {
        //public enum UIState { levelSelection, home, settings, game, title, shop, editor, credits, buyPro }
        //public static UIState uiState;
        public static UIEditorSelection _instance;

        public Animator animator;

        public UIEditorLevel uiEditorLevelPrefab;
        public Transform placingParent;
        public Level defaultLevelPrefab;

        public UIScrollFade uiScrollFade;

        public static List<UIEditorLevel> uiEditorLevels;

        private void Start()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
                return;
            }
            _instance = this;

            uiEditorLevels = new List<UIEditorLevel>();
            LoadUIEditorLevels();

            Main.onSceneChange.AddListener(SceneChanging);
        }

        // loads the list of custom levels
        public void LoadUIEditorLevels()
        {
            DestroyChildren(placingParent);

            foreach (LevelData l in LevelManager.customLevels)
            {
                UIEditorLevel editorLevel = Instantiate(uiEditorLevelPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                editorLevel.transform.parent = placingParent;
                editorLevel.levelData = l;
                editorLevel.UpdateTexts();
                uiEditorLevels.Add(editorLevel);
            }
        }

        private void SceneChanging(Main.Scene scene)
        {
            animator.SetTrigger("fadeout");
        }

        public void HomeButtonClicked()
        {
            SoundManager.ButtonClicked();
            Main.SetScene(Main.Scene.home);
        }

        public void AddButtonClicked()
        {
            print("addbtn");
            SoundManager.ButtonClicked();
            StartCoroutine(cCreateNewLevel());
            uiScrollFade.UpdateScrollElements();
            // animation
        }

        // creates a new levelData and displays it as an UIEditorLevel in the EditorSelection
        private IEnumerator cCreateNewLevel()
        {
            int newId = 505;
            // if there are custom levels and the default id is occupied find an un-occupied id and use it
            if (LevelManager.customLevels.Count > 0 && LevelManager.customLevels.Any(x => x.id == newId))
            {
                for (int i = newId + 1; i < 600; i++)
                {
                    if (!LevelManager.customLevels.Any(x => x.id == i))
                    {
                        newId = i;
                        break;
                    }
                }
            }

            // create new levelData
            LevelData l = LevelManager.NewCustomLevel(newId);

            // create new uiEditorLevel
            UIEditorLevel editorLevel = Instantiate(uiEditorLevelPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            editorLevel.transform.parent = placingParent;
            editorLevel.levelData = l;
            editorLevel.UpdateTexts();
            uiEditorLevels.Add(editorLevel);

            yield break;
        }

        public static void DestroyChildren(Transform root)
        {
            int childCount = root.childCount;
            for (int i = 0; i < childCount; i++)
            {
                GameObject.Destroy(root.GetChild(0).gameObject);
            }
        }
    }
}