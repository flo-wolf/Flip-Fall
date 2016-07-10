using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Sliders.Models
{
    [Serializable]
    public class Highscore
    {
        public enum ScoreType { gold, silver, bronze, normal, unranked }

        public ScoreType scoreType { get; set; }
        public double time { get; set; }
        public int ranking { get; set; }

        public Highscore()
        {
            scoreType = ScoreType.normal;
            time = -2f;
            ranking = 3;
        }
    }
}