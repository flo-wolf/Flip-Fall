using Sliders;
using Sliders.Progress;
using Sliders.UI;
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
        public static int currentPage = 0;

        private static List<Highscore> highscores;
        private static Highscore highscore;
        public Text levelCount;

        // Use this for initialization
        private void Awake()
        {
            _instance = this;
        }

        private void Start()
        {
            highscores = ProgressManager.GetProgress().highscores;
            UpdateTexts();
        }

        public static void Show()
        {
            //animations fade in / bounce in
            _instance.gameObject.SetActive(true);
            UpdateTexts();
        }

        public static void Hide()
        {
            _instance.gameObject.SetActive(false);
            ProgressManager.SaveProgressData();
        }

        public static void PlaceTime(int levelId, double time)
        {
            highscore = ProgressManager.GetProgress().GetHighscore(levelId);
            highscore.PlaceTime(time);
        }

        public static void UpdateTexts()
        {
            //edit to only update those visible in the cameraview, not neccessarily all UILevels
            highscores = ProgressManager.GetProgress().highscores;

            foreach (Highscore h in highscores)
            {
                UpdateText(h.levelId);
            }
        }

        public static void UpdateText(int levelId)
        {
            highscore = ProgressManager.GetProgress().GetHighscore(levelId);
            UILevel level = _instance.GetUILevel(levelId);

            //fire updated text event and send the changes that were made (1 to 2 stars, new bestTime etc)
            //in the animationManager, check for changes in star amount - update them with animations

            string timeString = "--.--";
            double t = highscore.bestTime;

            int secs = (int)t;
            int milSecs = (int)((t - (int)t) * 100);
            timeString = string.Format(Constants.timerFormat, secs, milSecs);

            level.bestText.text = timeString;
        }

        private UILevel GetUILevel(int id)
        {
            //edit
            List<UILevel> uiLevels = gameObject.GetComponentsInChildren<UILevel>().ToList();
            UILevel uiLevel = uiLevels.Find(x => x.id == id);

            return uiLevel;
        }
    }
}