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

        public void PlayLevel()
        {
            Debug.Log("MÖÖÖÖÖÖÖÖÖÖÖÖÖÖÖÖÖÖÖÖÖÖÖÖÖÖp");
            LevelManager.SetLevel(id);
            Game.SetGameState(Game.GameState.playing);
        }
    }
}