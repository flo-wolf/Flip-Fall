using Sliders.Levels;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Sliders.UI
{
    public class UILevel : MonoBehaviour
    {
        public int id;
        public Text bestText;
        public Text ghostText;
        public Text levelNumberText;
        public Button levelButton;

        public void Start()
        {
            if (levelButton == null)
                levelButton = gameObject.GetComponentInChildren<Button>();

            UpdateButton();
        }

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

        public void PlayLevel()
        {
            //level can be set
            if (LevelManager.LevelExists(id))
            {
                LevelManager.SetLevel(id);
                Game.SetGameState(Game.GameState.playing);
            }
            // else - animate failure
        }
    }
}