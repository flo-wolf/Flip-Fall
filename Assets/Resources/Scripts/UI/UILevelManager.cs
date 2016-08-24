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
            UpdateTexts();
            UpdatePageCount();
            FadeIn();
        }

        public static void Show()
        {
            //animations fade in / bounce in
            _instance.gameObject.SetActive(true);
            UpdateTexts();
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

        public static void UpdatePageCount()
        {
            //edit to only update those visible in the cameraview, not neccessarily all UILevels
            _instance.pageText.text = currentPage.ToString();
        }

        public static void FadeIn()
        {
            Debug.Log("FADEIIIIN!");
            _instance.pageAnimation.Play();
            _instance.pageInfoAnimation.Play();
        }

        //Updates the scores in the text fields inside the levelselection for all levels
        //called on GameState.scorescreen/finishscreen
        public static void UpdateTexts()
        {
            //edit to only update those visible in the cameraview, not neccessarily all UILevels
            highscores = ProgressManager.GetProgress().highscores;

            foreach (Highscore h in highscores)
            {
                UpdateText(h.levelId, h);
            }
        }

        //called on Game.GameState.scorescreen, has a timeframe of Game.scoreScreenDelay (default 1sec)
        public static void UpdateStars()
        {
            Debug.Log("UpdateStars");
            highscores = ProgressManager.GetProgress().highscores;

            foreach (Highscore h in highscores)
            {
                _instance.GetUILevel(h.levelId).UpdateStars(h);
            }
        }

        //fire updated text event and send the changes that were made (1 to 2 stars, new bestTime etc)
        //in the animationManager, check for changes in star amount - update them with animations
        public static void UpdateText(int levelId, Highscore highscore)
        {
            UILevel uiLevel = _instance.GetUILevel(levelId);

            if (uiLevel == null)
            {
                Debug.LogError("There is no UI Element to display this highscore: " + highscore.levelId);
            }
            else
            {
                uiLevel.bestText.text = FormatTime(highscore.bestTime + 0.01F);
                uiLevel.ghostText.text = FormatTime(uiLevel.ghostTime + 0.01F);
                uiLevel.levelNumberText.text = uiLevel.id.ToString();
            }
        }

        private static String FormatTime(double t)
        {
            string timeString = "--.--";
            int secs = (int)t;
            int milSecs = (int)((t - (int)t) * 100);
            timeString = string.Format(Constants.timerFormat, secs, milSecs);
            return timeString;
        }

        public UILevel GetUILevel(int id)
        {
            UILevel uiLevel = null;
            UILevel[] uiLevels = gameObject.GetComponentsInChildren<UILevel>();
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