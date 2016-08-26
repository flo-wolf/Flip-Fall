using Sliders.Levels;
using Sliders.Progress;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Sliders.UI
{
    public class UILevel : MonoBehaviour
    {
        public int id;

        //Stars
        public Image Star1;
        public Image Star2;
        public Image Star3;

        //Times
        public Text bestText;
        public Text ghostText;

        public Text levelNumberText;
        public Button levelButton;

        private int starScore = 0;

        public void Start()
        {
            if (levelButton == null)
                levelButton = gameObject.GetComponentInChildren<Button>();
            UpdateButton();

            Highscore h = ProgressManager.GetProgress().highscores.Find(x => x.levelId == id);
            if (h != null && h.starCount > 0)
            {
                starScore = h.starCount;
            }
            Highscore.onLevelStarChange.AddListener(HighscoreStarChanged);
        }

        private void HighscoreStarChanged(int stars, int levelId)
        {
            if (levelId == id)
                SetStars(stars);
        }

        //if there is no level corresponding to the UILevel Element then deactivate the buttons functions
        public void UpdateButton()
        {
            if (!LevelManager.LevelExists(id))
            {
                levelButton.interactable = false;
                levelNumberText.text = "";
            }
            else
            {
                levelButton.interactable = true;
                levelNumberText.text = id.ToString();
            }
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
            Debug.Log("[UILevel] UpdateStars()");
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
            if (UILevelMatchesLevel())
            {
                ghostText.text = Constants.FormatTime(LevelManager.GetLevel(id).presetTime);
                levelNumberText.text = id.ToString();

                Highscore h = ProgressManager.GetProgress().highscores.Find(x => x.levelId == id);
                if (h != null)
                    bestText.text = Constants.FormatTime(h.bestTime + 0.01F);
            }
        }

        //depreciated, updated are done through UpdateTexts()
        public void SetBestText(Highscore h)
        {
            if (UILevelMatchesLevel())
            {
                bestText.text = Constants.FormatTime(h.bestTime + 0.01F);
                ghostText.text = Constants.FormatTime(LevelManager.GetLevel(h.levelId).presetTime);
                levelNumberText.text = id.ToString();
            }
        }

        public void PlayLevel()
        {
            //level can be set
            if (LevelManager.LevelExists(id) && Game.gameState == Game.GameState.ready)
            {
                LevelManager.SetLevel(id);
                Game.SetGameState(Game.GameState.playing);
            }
            // else - animate failure
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