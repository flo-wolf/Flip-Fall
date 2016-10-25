using Impulse.Audio;
using Impulse.Levels;
using Impulse.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// A Levels Highscore, containing time, reached stars and id.
/// </summary>
namespace Impulse.Progress
{
    [Serializable]
    public class Highscore
    {
        /// <summary>
        /// 1. stars
        /// 2. levelId
        /// </summary>
        public static HighscoreStarChangeEvent onStarChange = new HighscoreStarChangeEvent();

        /// <summary>
        /// 1. Number of stars to add
        /// 2. Highscore
        /// </summary>
        public class HighscoreStarChangeEvent : UnityEvent<int, Highscore> { }

        public bool unlocked = false;
        public bool finished = false;
        public double bestTime = -1;
        public int levelId = 0;
        public int starCount = -1;
        public int fails = 0;

        public Highscore(int id, double time)
        {
            levelId = id;
            bestTime = -1;
            fails = 0;
            starCount = -1;

            if (time > 0)
                PlaceTime(time);
            UpdateStarCount();
        }

        public void UpdateStarCount()
        {
            if (bestTime > 0)
            {
                double presetTime = LevelManager.GetActiveLevel().presetTime;
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
            double presetTime = LevelManager.GetActiveLevel().presetTime;
            int oldStarCount = 0;

            if (t < bestTime || bestTime < 0 && presetTime > 0)
            {
                bestTime = t;
                if (t < presetTime)
                {
                    oldStarCount = starCount;
                    SetStarCount(3);
                }
                else if (t < presetTime + (presetTime * Constants.twoStarPercantage))
                {
                    oldStarCount = starCount;
                    SetStarCount(2);
                }
                else
                {
                    oldStarCount = starCount;
                    SetStarCount(1);
                }
            }
            else
            {
                Debug.Log("[Highscore] Time trying to be placed is smaller than the existing best and/or there was no presetTime set");
                //display too small time, maybe let the timer blink up red then make it disappear and replace with timer
            }

            //onStarChange.Invoke(oldStarCount, this);
            SetStarUnlocks(oldStarCount);

            Debug.Log("[Highscore] New starCount: " + starCount + " of level: " + levelId);
        }

        private void SetStarUnlocks(int olderStarCount)
        {
        }
    }
}