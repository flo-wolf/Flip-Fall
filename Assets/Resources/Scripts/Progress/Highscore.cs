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
        public bool unlocked = true;
        public bool finished = false;
        public double bestTime { get; set; }
        public int levelId = 0;

        public Highscore(int id, double time)
        {
            levelId = id;
            scoreType = ScoreType.unvalid;
            bestTime = time;
        }

        public void SetScoreType(ScoreType st)
        {
            scoreType = st;
        }

        public void PlaceTime(double t)
        {
            //as soon as ghosts are there do it like this
            //double ghostBest = LevelManager.GetLevel(levelId).GetGhost().time;
            double ghostBest = 10;
            if (bestTime > t)
            {
                bestTime = t;
                if (t <= ghostBest)
                    SetScoreType(ScoreType.threeStar);
                else if (t <= ghostBest + (ghostBest * Constants.twoStarPercantage))
                    SetScoreType(ScoreType.twoStar);
                else
                    SetScoreType(ScoreType.oneStar);
            }
            else
            {
                //display too small time, maybe let the timer blink up red then make it disappear and replace with timer
            }
        }
    }
}