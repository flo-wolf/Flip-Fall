using Sliders;
using Sliders.Levels;
using Sliders.Progress;
using Sliders.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Shows and hides all levels, gets moved by the
/// </summary>
namespace Sliders.UI
{
    public class UILevelManager : MonoBehaviour
    {
        public static UILevelManager _instance;
        public static int currentPage = 1;
        public static int pageCount = 10;

        private static List<Highscore> highscores;
        private static Highscore highscore;
        public Text pageText;

        public Animation pageAnimation;
        public Animation pageInfoAnimation;

        // Use this for initialization
        private void Awake()
        {
            _instance = this;
        }

        private void Start()
        {
            UpdatePageCount();
            FadeIn();
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
            _instance.gameObject.SetActive(false);
            ProgressManager.SaveProgressData();
        }

        public void NextPage(Button b)
        {
            if (currentPage + 1 <= pageCount)
            {
                currentPage++;
                UpdatePageCount();
                UIButtonManager.onButtonClick.Invoke(b);
            }
        }

        public void LastPage(Button b)
        {
            if (currentPage - 1 >= 1)
            {
                currentPage--;
                UpdatePageCount();
                UIButtonManager.onButtonClick.Invoke(b);
            }
        }

        public static void FadeIn()
        {
            Debug.Log("FADEIIIIN!");
            _instance.pageAnimation.Play();
            _instance.pageInfoAnimation.Play();
        }

        private static void UpdatePageCount()
        {
            //edit to only update those visible in the cameraview, not neccessarily all UILevels
            _instance.pageText.text = currentPage.ToString();
            _instance.UpdatePage();
        }

        //Updates the scores in the text fields inside the levelselection for all levels
        //called on GameState.scorescreen/finishscreen
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
                }
            }
        }

        //called on Game.GameState.scorescreen, has a timeframe of Game.scoreScreenDelay (default 1sec)
        //private void UpdatePageStars()
        //{
        //    highscores = ProgressManager.GetProgress().highscores;

        //    foreach (Highscore h in highscores)
        //    {
        //        if (GetCurrentPageUILevels().Any(x => x.id == h.levelId))
        //            GetUILevel(h.levelId).UpdateStars();
        //    }
        //}

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