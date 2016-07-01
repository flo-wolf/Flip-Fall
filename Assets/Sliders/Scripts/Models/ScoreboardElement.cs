using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Sliders.Models
{
    [SerializeField]
    public class ScoreboardElement
    {
        public enum ScoreType { gold, silver, bronze, normal, unranked }

        public ScoreType scoreType { get; set; }
        public double time { get; set; }

        public ScoreboardElement()
        {
            scoreType = ScoreType.normal;
            time = -1f;
        }
    }
}