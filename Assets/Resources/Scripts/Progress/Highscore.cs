using Sliders.Audio;
using Sliders.Levels;
using Sliders.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// A Levels Highscore, containing time, reached stars and id.
/// </summary>
namespace Sliders.Progress
{
    [Serializable]
    public class Highscore
    {
        public int starCount = -1;

        /// <summary>
        /// 1. stars
        /// 2. levelId
        /// </summary>
        public static LevelStarChangeEvent onLevelStarChange = new LevelStarChangeEvent();

        /// <summary>
        /// 1. stars
        /// 2. levelId
        /// </summary>
        public class LevelStarChangeEvent : UnityEvent<int, int> { }

        public bool unlocked = false;
        public bool finished = false;
        public double bestTime = -1;
        public int levelId = 0;

        public Highscore(int id, double time)
        {
            levelId = id;
            PlaceTime(time);
            starCount = -1;
            UpdateStarCount();
        }

        public void UpdateStarCount()
        {
            double presetTime = LevelManager.GetLevel().presetTime;

            if (bestTime < presetTime)
            {
                SetStarCount(3);
            }
            else if (bestTime < presetTime + Constants.twoStarPercantage)
            {
                SetStarCount(2);
            }
            else
                SetStarCount(1);
        }

        private void SetStarCount(int stars)
        {
            if (stars > 0)
            {
                starCount = stars;
                //starcount set event
            }
        }

        public void PlaceTime(double t)
        {
            double presetTime = LevelManager.GetLevel().presetTime;

            if (t < bestTime || bestTime < 0 && presetTime > 0)
            {
                bestTime = t;
                if (t < presetTime)
                {
                    SetStarCount(3);
                }
                else if (t < presetTime + (presetTime * Constants.twoStarPercantage))
                {
                    SetStarCount(2);
                }
                else
                    SetStarCount(1);
            }
            else
            {
                Debug.Log("[Highscore] Time trying to be placed is smaller than the existing best and/or there was no presetTime set");
                //display too small time, maybe let the timer blink up red then make it disappear and replace with timer
            }

            onLevelStarChange.Invoke(starCount, levelId);
            Debug.Log("[Highscore] New starCount: " + starCount + " of level: " + levelId);
        }
    }
}