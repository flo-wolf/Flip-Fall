using Sliders.Audio;
using Sliders.Levels;
using Sliders.UI;
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
        public double bestTime = 9999;
        public int levelId = 0;

        public Highscore(int id, double time)
        {
            levelId = id;
            scoreType = ScoreType.unvalid;
            PlaceTime(time);
        }

        public void SetScoreType(ScoreType st)
        {
            scoreType = st;
        }

        public void PlaceTime(double t)
        {
            Debug.Log("------------------------------ t: " + t);
            //as soon as ghosts are there do it like this
            //double ghostBest = LevelManager.GetLevel(levelId).GetGhost().time;
            double ghostBest = UILevelManager._instance.GetUILevel(levelId).ghostTime;
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
            Debug.Log("id: " + levelId + " scoretype: " + scoreType);
        }
    }
}