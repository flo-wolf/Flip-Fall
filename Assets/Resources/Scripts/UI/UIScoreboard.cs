using Sliders.Levels;
using Sliders.Progress;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles the UI Scoreboard and Updates it if neccessary
/// </summary>

namespace Sliders.UI
{
    public class UIScoreboardsManager : MonoBehaviour
    {
        private static UIScoreboardsManager _instance;
        public static List<Scoreboard> scoreboards;
        public static int currentPage = 0;
        public Text title;
        public Text text1;
        public Text text2;
        public Text text3;
        public Text text4;
        public Text text5;
        public Text text6;

        private void Awake()
        {
            _instance = this;
        }

        private void Start()
        {
            scoreboards = ProgressManager.GetProgress().GetCurrentScoreboard();
            UpdateTexts();
        }

        public static void Hide()
        {
            Debug.Log("[UIScoreboard]: Hide()");
            _instance.gameObject.SetActive(false);
            ProgressManager.SaveProgressData();
        }

        public static void Show()
        {
            _instance.gameObject.SetActive(true);
            UpdateTexts();
        }

        public static void PlaceTime()
        {
            scoreboard = ProgressManager.GetProgress().GetCurrentScoreboard();
            scoreboard.TryPlacingTime(UITimer.GetTime());
        }

        public static void UpdateTexts()
        {
            //Debug.Log("[UIScoreboard]: UpdateTexts()");
            scoreboard = ProgressManager.GetProgress().GetCurrentScoreboard();
            int count = scoreboard.elements.Count;
            string empty = "-.-";
            _instance.title.text = LevelManager.GetID().ToString();
            _instance.text1.text = empty;
            _instance.text2.text = empty;
            _instance.text3.text = empty;
            _instance.text4.text = empty;
            _instance.text5.text = empty;

            if (count > 0)
            {
                foreach (Highscore se in scoreboard.elements)
                {
                    double t = se.time;
                    int secs = (int)t;
                    int milSecs = (int)((t - (int)t) * 100);
                    string format = string.Format(Constants.timerFormat, secs, milSecs);

                    if (_instance.text1.text == empty)
                        _instance.text1.text = format;
                    else if (_instance.text2.text == empty)
                        _instance.text2.text = format;
                    else if (_instance.text3.text == empty)
                        _instance.text3.text = format;
                    else if (_instance.text4.text == empty)
                        _instance.text4.text = format;
                    else if (_instance.text5.text == empty)
                        _instance.text5.text = format;
                    else return;
                }
            }
        }
    }
}