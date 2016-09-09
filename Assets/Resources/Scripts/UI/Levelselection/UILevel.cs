using Impulse.Levels;
using Impulse.Progress;
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
            Debug.Log("UILevel updatetexts: " + id);

            if (UILevelMatchesLevel())
            {
                Debug.Log("UILevel 1");
                double topTime = LevelManager.GetLevel(id).presetTime;

                topTimeSecText.text = ((int)topTime).ToString();
                topTimeMilText.text = ((topTime - (int)topTime) * 100).ToString();
                levelNumberText.text = id.ToString();

                Highscore h = ProgressManager.GetProgress().highscores.Find(x => x.levelId == id);
                if (h != null)
                {
                    Debug.Log("UILevel 2");
                    double bestTime = h.bestTime;
                    bestTime = Mathf.Round((float)bestTime * 100f) / 100f;

                    timeSecText.text = ((int)bestTime).ToString();
                    timeMilText.text = ((bestTime - (int)bestTime) * 100).ToString();
                }
                else
                {
                    timeSecText.text = "--";
                    timeMilText.text = "--";
                }
            }
        }

        ////depreciated, updated are done through UpdateTexts()
        //public void SetBestText(Highscore h)
        //{
        //    if (UILevelMatchesLevel())
        //    {
        //        bestText.text = Constants.FormatTime(h.bestTime + 0.01F);
        //        ghostText.text = Constants.FormatTime(LevelManager.GetLevel(h.levelId).presetTime);
        //        levelNumberText.text = id.ToString();
        //    }
        //}

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