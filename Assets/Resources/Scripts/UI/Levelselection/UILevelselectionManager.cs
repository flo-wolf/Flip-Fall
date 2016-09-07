using Impulse;
using Impulse.Levels;
using Impulse.Progress;
using Impulse.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles the UI Elememnts for the levelselection scene
/// Singletone
/// </summary>
namespace Impulse.UI
{
    public class UILevelselectionManager : MonoBehaviour
    {
        public static UILevelselectionManager _instance;
        public static UILevel currentUILevel;

        private static List<Highscore> highscores;
        private static Highscore highscore;

        public Animation fadeAnimation;
        public Animation pageAnimation;

        public float fadeOutTime = 1F;
        public float fadeInTime = 1F;

        private bool levelChosen;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
                return;
            }
            _instance = this;
        }

        private void Start()
        {
            Main.onSceneChange.AddListener(SceneChanging);
            UpdateLevelView();
            //FadeIn();
            _instance.fadeAnimation.Play("fadeFromBlack");
        }

        private void SceneChanging(Main.Scene scene)
        {
            fadeAnimation.Play("fadeToBlack");
            //FadeOut();
        }

        public void NextLevel(Button b)
        {
            if (currentUILevel.id + 1 <= Constants.lastLevel)
            {
                UpdateLevelView();
            }
        }

        public void LastLevel(Button b)
        {
            if (currentUILevel.id - 1 >= 1)
            {
                UpdateLevelView();
            }
        }

        public static void FadeIn()
        {
            Debug.Log("[UILevelSelection] FadeIn()");
            _instance.pageAnimation.Play("pageShow");
        }

        public static void FadeOut()
        {
            Debug.Log("[UILevelSelection] FadeOut()");
            _instance.pageAnimation.Play("pageHide");
        }

        public void HomeBtnClicked()
        {
            Main.SetScene(Main.Scene.home);
        }

        //Updates the scores in the text fields inside the levelselection for all levels
        //called on GameState.levelselection/finishscreen
        private void UpdateLevelView()
        {
            UIStarCount.Show();
            UILevel l = GetUILevel(LevelManager.GetID());
            if (l != null && l.UILevelMatchesLevel())
            {
                l.UpdateTexts();
                l.UpdateStars();
                l.UpdateButton();
            }
        }

        public static UILevel GetUILevel(int id)
        {
            UILevel uiLevel = null;
            UILevel[] uiLevels = _instance.gameObject.GetComponentsInChildren<UILevel>();
            foreach (UILevel lvl in uiLevels)
            {
                if (lvl.id == id)
                {
                    uiLevel = lvl;
                    break;
                }
            }
            return uiLevel;
        }
    }
}