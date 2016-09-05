﻿using Impulse.Progress;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Impulse.UI
{
    public class UIStarCount : MonoBehaviour
    {
        public static UIStarCount _instance;
        public Text starText;

        private void Start()
        {
            _instance = this;
            UpdateStarCount();
        }

        public static void Show()
        {
            _instance.gameObject.SetActive(true);
            _instance.UpdateStarCount();
            //animation
        }

        public static void Hide()
        {
            _instance.gameObject.SetActive(false);
            //animation
        }

        public void UpdateStarCount()
        {
            int starCount = 0;
            List<Highscore> highscores = ProgressManager.GetProgress().highscores;
            foreach (Highscore h in highscores)
            {
                starCount = starCount + h.starCount;
            }
            ProgressManager.GetProgress().totalStars = starCount;
            starText.text = starCount.ToString();
        }
    }
}