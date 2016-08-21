using Sliders.Levels;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A Levels Highscore, containing time, reached stars and id.
/// </summary>
namespace Sliders.Progress
{
    [Serializable]
    public class Highscore
    {
        public enum ScoreType { oneStar, twoStar, threeStar, unvalid }

        public ScoreType scoreType { get; set; }
        public double bestTime { get; set; }
        public int levelId = 0;

        public Highscore()
        {
            levelId = 0;
            scoreType = ScoreType.unvalid;
            bestTime = -0.66D;
        }

        public void PlaceTime(double t)
        {
            double ghostBest = LevelManager.GetLevel(levelId).GetGhost().time;
            if (bestTime < t)
            {
                bestTime = t;
                if (t <= ghostBest)
                {
                    bestTime = t;
                    scoreType = ScoreType.threeStar;
                }
                else if (t <= ghostBest + Constants.twoStarPercantage)
                {
                }
            }
        }
    }
}