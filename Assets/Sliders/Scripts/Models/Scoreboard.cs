using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Sliders.Models
{
    [SerializeField]
    public class Scoreboard : MonoBehaviour
    {
        public int id;
        public static List<ScoreboardElement> _elements;

        private void Start()
        {
            //scoreboards = LevelManager.level.getScores();
        }

        private void UpdateLevelTime(double t)
        {
            //scoreboards = LevelManager.level.getScores();
        }

        public void Display()
        {
        }

        public void UpdateScoreboards()
        {
            //foreach (Scoreboard ls in _elements)
            //{
            //}
        }
    }
}