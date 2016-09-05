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
/// Shows and hides all levels, gets moved by the
/// </summary>
namespace Impulse.UI
{
    public class UILevelSelection : MonoBehaviour
    {
        public static UILevelSelection _instance;
        public static int currentPage = 1;
        public static int pageCount = 10;
        public float fadeOutTime = 1F;
        public float fadeInTime = 1F;

        private static List<Highscore> highscores;
        private static Highscore highscore;
        public Text pageText;

        public Animation fadeAnimation;
        public Animation pageAnimation;
        public Animation pageInfoAnim;

        private bool levelChosen;

        // Use this for initialization
        private void Awake()
        {
            _instance = this;
        }

        private void Start()
        {
            UIStarCount.Show();

            UpdatePageCount();
            FadeIn();
            Main.onSceneChange.AddListener(SceneChanging);
        }

        private void SceneChanging(Main.Scene scene)
        {
            fadeAnimation.Play("fadeToBlack");
        }

        public static void Show()
        {
            //animations fade in / bounce in
            _instance.gameObject.SetActive(true);
            _instance.UpdatePage();
            FadeIn();
        }

        public static void Hide()
        {
            ProgressManager.SaveProgressData();
            FadeOut();
        }

        public void NextPage(Button b)
        {
            if (currentPage + 1 <= pageCount)
            {
                currentPage++;
                UpdatePageCount();
            }
        }

        public void LastPage(Button b)
        {
            if (currentPage - 1 >= 1)
            {
                currentPage--;
                UpdatePageCount();
            }
        }

        public static void FadeIn()
        {
            Debug.Log("[UILevelSelection] FadeIn()");
            _instance.fadeAnimation.Play("fadeFromBlack");
            _instance.pageAnimation.Play("pageShow");
            _instance.pageInfoAnim.Play();
            _instance.StartCoroutine(_instance.cEnablePages(_instance.fadeInTime));
        }

        public static void FadeOut()
        {
            Debug.Log("[UILevelSelection] FadeOut()");
            _instance.pageAnimation.Play("pageHide");
            _instance.pageInfoAnim.Play();

            _instance.StartCoroutine(_instance.cDisablePages(_instance.fadeOutTime));
        }

        private IEnumerator cEnablePages(float delay)
        {
            yield return new WaitForSeconds(delay);
            ScrollRect pagesScrollRect = gameObject.GetComponentInChildren<ScrollRect>();
            pagesScrollRect.enabled = true;
        }

        private IEnumerator cDisablePages(float delay)
        {
            yield return new WaitForSeconds(delay);
            ScrollRect pagesScrollRect = gameObject.GetComponentInChildren<ScrollRect>();
            pagesScrollRect.enabled = false;
        }

        private static void UpdatePageCount()
        {
            //edit to only update those visible in the cameraview, not neccessarily all UILevels
            _instance.pageText.text = currentPage.ToString();
            _instance.UpdatePage();
        }

        //Updates the scores in the text fields inside the levelselection for all levels
        //called on GameState.levelselection/finishscreen
        private void UpdatePage()
        {
            //edit to only update those visible in the cameraview, not neccessarily all UILevels
            highscores = ProgressManager.GetProgress().highscores;

            List<UILevel> pageUiLevels = GetCurrentPageUILevels();

            foreach (UILevel l in pageUiLevels)
            {
                if (l != null && l.UILevelMatchesLevel())
                {
                    l.UpdateTexts();
                    l.UpdateStars();
                    l.UpdateButton();
                }
            }
        }

        private List<UILevel> GetCurrentPageUILevels()
        {
            List<UILevel> uiLevels = new List<UILevel>();
            for (int i = 0; i < Constants.itemsPerPage; i++)
            {
                uiLevels.Add(GetUILevel(i * currentPage));
            }
            return uiLevels;
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