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
        public double ghostTime = 10F;

        //Stars
        public Image Star1;
        public Image Star2;
        public Image Star3;

        //Times
        public Text bestText;
        public Text ghostText;

        public Text levelNumberText;
        public Button levelButton;

        private Highscore.ScoreType oldScoreType;

        public void Start()
        {
            if (levelButton == null)
                levelButton = gameObject.GetComponentInChildren<Button>();

            UpdateButton();
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

        public void UpdateStars(Highscore h)
        {
            Debug.Log("[UILevel] UpdateStars(" + h.scoreType + ")");
            if (h.scoreType != oldScoreType && LevelManager.LevelExists(id))
            {
                oldScoreType = h.scoreType;
                switch (h.scoreType)
                {
                    case Highscore.ScoreType.oneStar:
                        Star1.enabled = true;
                        Star2.enabled = false;
                        Star3.enabled = false;
                        break;

                    case Highscore.ScoreType.twoStar:
                        Star1.enabled = true;
                        Star2.enabled = true;
                        Star3.enabled = false;
                        break;

                    case Highscore.ScoreType.threeStar:
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
    }
}