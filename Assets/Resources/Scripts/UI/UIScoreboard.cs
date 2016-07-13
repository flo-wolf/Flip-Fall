using Sliders.Levels;
using Sliders.Progress;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Sliders.UI
{
    public class UIScoreboard : MonoBehaviour
    {
        public static UIScoreboard uiScoreboard;
        public static Scoreboard scoreboard;
        public Text title;
        public Text text1;
        public Text text2;
        public Text text3;
        public Text text4;
        public Text text5;

        private void Awake()
        {
            uiScoreboard = this;
        }

        private void Start()
        {
            LevelManager.onLevelChange.AddListener(LevelChanged);
            scoreboard = ProgressManager.progress.GetScoreboard(LevelManager.levelManager.GetID());
            UpdateTexts();
        }

        //Use it for updating
        private void LevelChanged(Level level)
        {
            UpdateTexts();
        }

        public void Hide()
        {
            Debug.Log("[UIScoreboard]: Hide()");
            gameObject.SetActive(false);
            ProgressManager.SaveProgressData();
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        //works, but its unclean.
        public void ShowAndUpdate(double time)
        {
            Debug.Log("[UIScoreboard]: ShowAndUpdate() time: " + time);
            scoreboard = ProgressManager.progress.GetScoreboard(LevelManager.levelManager.GetID());
            scoreboard.TryPlacingTime(time);
            UpdateTexts();
            Show();
        }

        public void UpdateTexts()
        {
            //Debug.Log("[UIScoreboard]: UpdateTexts()");
            scoreboard = ProgressManager.GetProgress().GetScoreboard(LevelManager.levelManager.GetID());
            int count = scoreboard.elements.Count;
            string empty = "-.-";
            title.text = LevelManager.levelManager.GetLevel().id.ToString();
            text1.text = empty;
            text2.text = empty;
            text3.text = empty;
            text4.text = empty;
            text5.text = empty;

            if (count > 0)
            {
                foreach (Highscore se in scoreboard.elements)
                {
                    double t = se.time;
                    int secs = (int)t;
                    int milSecs = (int)((t - (int)t) * 100);
                    string format = string.Format(Constants.timerFormat, secs, milSecs);

                    if (text1.text == empty)
                        text1.text = format;
                    else if (text2.text == empty)
                        text2.text = format;
                    else if (text3.text == empty)
                        text3.text = format;
                    else if (text4.text == empty)
                        text4.text = format;
                    else if (text5.text == empty)
                        text5.text = format;
                    else return;
                }
            }
        }
    }
}