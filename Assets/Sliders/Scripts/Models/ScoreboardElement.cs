using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Sliders.Models
{
    [SerializeField]
    public class ScoreboardElement : MonoBehaviour
    {
        public Scoreboard scoreboard;
        public Text text;
        public string formattedTime { get; set; }
        public double time { get; set; }
        public int rankingID { get; set; }

        public ScoreboardElement()
        {
        }

        public ScoreboardElement(Scoreboard ls)
        {
            scoreboard = ls;
        }

        private void Update()
        {
        }
    }
}