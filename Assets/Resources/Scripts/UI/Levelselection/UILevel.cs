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
        public Text failsCount;

        private Animator animator;

        private int starScore = 0;
        private Highscore highscore;

        [HideInInspector]
        public bool createdByLevelswitch = false;

        public void Start()
        {
            animator = GetComponent<Animator>();

            highscore = ProgressManager.GetProgress().highscores.Find(x => x.levelId == id);
            if (highscore != null && highscore.starCount > 0)
            {
                starScore = highscore.starCount;
            }

            Main.onSceneChange.AddListener(SceneChanged);

            Highscore.onStarChange.AddListener(HighscoreStarChanged);

            UpdateUILevel();

            if (createdByLevelswitch)
                animator.SetTrigger("levelswitch");
            else
                animator.SetTrigger("fadein");
        }

        private void SceneChanged(Main.Scene s)
        {
            FadeOut();
            createdByLevelswitch = false;
        }

        public void FadeOut()
        {
            animator.SetTrigger("fadeout");
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

        public void UpdateUILevel()
        {
            highscore = ProgressManager.GetProgress().highscores.Find(x => x.levelId == id);

            //levelNumberText.text = id.ToString();

            UpdateFails(highscore);
            UpdateStars();
            UpdateTimes();
        }

        public void UpdateFails(Highscore h)
        {
            if (h != null)
            {
                failsCount.text = string.Format("{0:0000}", highscore.fails);
            }
            else
                failsCount.text = "1111";
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

        public void UpdateTimes()
        {
            // Debug.Log("UILevel Updatetexts of id: " + id);

            if (UILevelMatchesLevel())
            {
                double topTime = LevelManager.levels.Find(x => x.id == id).presetTime;

                // Preset top time seconds
                string topSec = ((int)topTime).ToString();
                topTimeSecText.text = topSec;

                // top time milseconds
                string topMilSec = string.Format("{0:0.00}", topTime);
                topMilSec = topMilSec.Substring(topMilSec.IndexOf(".") + 1);
                topTimeMilText.text = topMilSec;

                Highscore h = ProgressManager.GetProgress().highscores.Find(x => x.levelId == id);
                if (h != null)
                {
                    //Debug.Log("id: " + id);

                    // this doesn't belong in here, even through it does its job right
                    if (ProgressManager.GetProgress().lastUnlockedLevel <= id)
                    {
                        ProgressManager.GetProgress().lastUnlockedLevel++;
                        UILevelselectionManager.NextWasUnlocked();
                    }

                    double bestTime = h.bestTime;
                    //Debug.Log("UpdateTexts() h.bestTime " + bestTime);

                    // Personal best seconds
                    string bestTimeString = ((int)bestTime).ToString();
                    timeSecText.text = bestTimeString;

                    //Debug.Log("UpdateTexts() bestTimeString " + bestTimeString);

                    // Personal best milseconds
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