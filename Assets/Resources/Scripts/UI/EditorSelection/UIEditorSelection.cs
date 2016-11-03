using Impulse;
using Impulse.Audio;
using Impulse.Levels;
using Impulse.Progress;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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

            int customLevelCount = ProgressManager.GetProgress().customIds.Count;

            Level[] customLevels = new Level[customLevelCount];

            for (int i = 0; i < customLevelCount; i++)
            {
                customLevels[i] = LevelLoader.LoadCustomLevel(ProgressManager.GetProgress().customIds[i]);
            }

            foreach (Level l in customLevels)
            {
                UIEditorLevel editorLevel = Instantiate(uiEditorLevelPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                editorLevel.transform.parent = placingParent;
                editorLevel.level = l;
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
            StartCoroutine(cCreateLevel());
        }

        private IEnumerator cCreateLevel()
        {
            int newId = ProgressManager.GetProgress().customIds[ProgressManager.GetProgress().customIds.Count - 1] + 1;
            ProgressManager.GetProgress().customIds.Add(newId);
            SoundManager.ButtonClicked();
            // animation

            // create new level prefab
            Level l = defaultLevelPrefab;
            l.id = newId;
            Object prefab = PrefabUtility.CreateEmptyPrefab("Assets/Resources/Prefabs/Levels/Custom/" + newId + ".prefab");
            PrefabUtility.ReplacePrefab(l.gameObject, prefab, ReplacePrefabOptions.ConnectToPrefab);

            // create new uiEditorLevel
            UIEditorLevel editorLevel = Instantiate(uiEditorLevelPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            editorLevel.transform.parent = placingParent;
            editorLevel.level = l;
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