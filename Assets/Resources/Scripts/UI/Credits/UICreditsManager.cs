using Impulse.Audio;
using Impulse.Levels;
using Impulse.Progress;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages all UI Elements of the respective scene
/// </summary>

namespace Impulse.UI
{
    public class UICreditsManager : MonoBehaviour
    {
        //public enum UIState { levelSelection, home, settings, game, title, shop, editor, credits, buyPro }
        //public static UIState uiState;
        public static UICreditsManager _instance;

        public Animator animator;
        public GameObject credits;

        private void Start()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
                return;
            }
            _instance = this;

            Main.onSceneChange.AddListener(SceneChanging);
        }

        private void SceneChanging(Main.Scene scene)
        {
            animator.SetTrigger("fadeout");
        }

        public void HomeButtonClicked()
        {
            SoundManager.ButtonClicked();
            Main.SetScene(Main.Scene.home);
        }
    }
}