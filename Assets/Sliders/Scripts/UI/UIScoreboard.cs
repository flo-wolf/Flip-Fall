using Sliders.Models;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Sliders.UI
{
    public class UIScoreboard : MonoBehaviour
    {
        public static Scoreboard scoreboard;
        public Text text1;
        public Text text2;
        public Text text3;
        public Text text4;
        public Text text5;

        // Use this for initialization
        private void Start()
        {
            scoreboard = new Scoreboard();
        }

        // Update is called once per frame
        private void Update()
        {
        }

        public void Hide()
        {
            gameObject.active = false;
        }

        //works!
        public void Show(double time)
        {
            scoreboard = ProgressManager.progress.GetScoreboard(LevelManager.level.id);
            scoreboard.TryPlacingTime(time);
            Debug.Log(scoreboard.elements[0].time);
            gameObject.active = true;
        }
    }
}