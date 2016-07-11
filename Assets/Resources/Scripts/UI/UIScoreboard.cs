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
            scoreboard = ProgressManager.progress.GetScoreboard(LevelManager.levelManager.activeLevel.id);
            UpdateTexts();
        }

        public void Hide()
        {
            Debug.Log("Hide Scoreboards");
            gameObject.SetActive(false);
            ProgressManager.SaveProgressData();
        }

        //works, but its unclean.
        public void ShowAndUpdate(double time)
        {
            Debug.Log("Display Scoreboards");
            scoreboard = ProgressManager.progress.GetScoreboard(LevelManager.levelManager.activeLevel.id);
            scoreboard.TryPlacingTime(time);
            UpdateTexts();
            gameObject.SetActive(true);
        }

        public void UpdateTexts()
        {
            Debug.Log("Updating texts!");
            scoreboard = ProgressManager.GetProgress().GetScoreboard(LevelManager.levelManager.activeLevel.id);
            int count = scoreboard.elements.Count;
            string empty = "-.-";
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