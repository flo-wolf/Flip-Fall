using Impulse.Levels;
using Impulse.Progress;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Impulse.UI
{
    public class UILevel : MonoBehaviour
    {
        public int id;

        //Stars
        public Image Star1;
        public Image Star2;
        public Image Star3;

        //Times
        public Text timeSecText;
        public Text timeMilText;
        public Text topTimeSecText;
        public Text topTimeMilText;

        public Text levelNumberText;

        private int starScore = 0;

        public void Start()
        {
            Highscore h = ProgressManager.GetProgress().highscores.Find(x => x.levelId == id);
            if (h != null && h.starCount > 0)
            {
                starScore = h.starCount;
            }
            Highscore.onLevelStarChange.AddListener(HighscoreStarChanged);

            UpdateTexts();
            UpdateStars();
        }

        private void HighscoreStarChanged(int stars, int levelId)
        {
            if (levelId == id)
                SetStars(stars);
        }

        public void SetStars(int newStars)
        {
            if (newStars != starScore && UILevelMatchesLevel() && newStars > 0)
            {
                starScore = newStars;
                UpdateStars();
            }
        }

        public void UpdateStars()
        {
            switch (starScore)
            {
                case 1:
                    Star1.enabled = true;
                    Star2.enabled = false;
                    Star3.enabled = false;
                    break;

                case 2:
                    Star1.enabled = true;
                    Star2.enabled = true;
                    Star3.enabled = false;
                    break;

                case 3:
                    Star1.enabled = true;
                    Star2.enabled = true;
                    Star3.enabled = true;
                    break;

                default:
                    Star1.enabled = false;
                    Star2.enabled = false;
                    Star3.enabled = false;
                    break;
            }
        }

        public void UpdateTexts()
        {
            Debug.Log("UILevel Updatetexts of id: " + id);

            if (UILevelMatchesLevel())
            {
                double topTime = LevelManager.levels.Find(x => x.id == id).presetTime;

                levelNumberText.text = id.ToString();

                //Preset (top) time seconds
                string topSec = ((int)topTime).ToString();
                topTimeSecText.text = topSec;

                //Preset (top) time milseconds
                string topMilSec = ((topTime - (int)topTime) * 100).ToString();
                if (topMilSec.Length == 1)
                    topMilSec = topMilSec + "0";
                else if (topMilSec.Length == 0)
                {
                    topMilSec = topMilSec + "00";
                }
                topTimeMilText.text = topMilSec;

                Highscore h = ProgressManager.GetProgress().highscores.Find(x => x.levelId == id);
                if (h != null)
                {
                    Debug.Log("id: " + id);

                    // this doesn't belong in here, even through it does its job right
                    if (ProgressManager.GetProgress().lastUnlockedLevel <= id)
                    {
                        ProgressManager.GetProgress().lastUnlockedLevel++;
                        UILevelselectionManager.NextWasUnlocked();
                    }

                    double bestTime = h.bestTime;

                    //Personal best time seconds
                    string bestTimeString = string.Format("{0:0}", bestTime);
                    timeSecText.text = bestTimeString;

                    //Personal best time milseconds
                    string milSecs = string.Format("{0:0.00}", bestTime);
                    milSecs = milSecs.Substring(milSecs.IndexOf(".") + 1);
                    timeMilText.text = milSecs;
                }
                else
                {
                    timeSecText.text = "--";
                    timeMilText.text = "--";
                }
            }
        }

        public bool UILevelMatchesLevel()
        {
            if (LevelManager.LevelExists(id))
            {
                return true;
            }
            return false;
        }
    }
}