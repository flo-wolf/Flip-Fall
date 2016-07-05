using Sliders.Models;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Sliders.UI
{
    public class UIScoreboard : MonoBehaviour
    {
        public static Scoreboard scoreboard;
        public Text text1;
        public Text text2;
        public Text text3;
        public Text text4;
        public Text text5;

        private void Start()
        {
            UpdateTexts();
            scoreboard = new Scoreboard();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        //works, but its unclean.
        public void Show(double time)
        {
            scoreboard = ProgressManager.progress.GetScoreboard(LevelManager.level.id);
            scoreboard.TryPlacingTime(time);
            Debug.Log(scoreboard.elements[0].time);
            UpdateTexts();
            gameObject.SetActive(true);
        }

        private void UpdateTexts()
        {
            int count = scoreboard.elements.Count;
            if (count > 0)
            {
                string empty = "-.-";
                text1.text = empty;
                text2.text = empty;
                text3.text = empty;
                text4.text = empty;
                text5.text = empty;
                foreach (ScoreboardElement se in scoreboard.elements)
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
                }
            }
        }
    }
}