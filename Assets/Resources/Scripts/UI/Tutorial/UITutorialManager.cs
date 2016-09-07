using Impulse.Levels;
using Impulse.Progress;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages all UI Elements through externally fired events and turns them on and off
/// </summary>

namespace Impulse.UI
{
    public class UITutorialManager : MonoBehaviour
    {
        //public enum UIState { levelSelection, home, settings, game, title, shop, editor, credits, buyPro }
        //public static UIState uiState;
        public static UITutorialManager _instance;
        public Animation fadeAnimation;

        public Image reflectImage;
        public Image chargeImage;

        private Sprite reflectSprite;
        private Sprite chargeSprite;

        private Sprite memorySprite;

        private void Start()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
                return;
            }
            _instance = this;

            FadeIn();

            reflectSprite = reflectImage.sprite;
            chargeSprite = chargeImage.sprite;
            SetSprites();
            Main.onSceneChange.AddListener(SceneChanging);
        }

        private void SceneChanging(Main.Scene scene)
        {
            FadeOut();
        }

        private void FadeIn()
        {
            fadeAnimation.Play("fadeFromBlack");
        }

        private void FadeOut()
        {
            fadeAnimation.Play("fadeToBlack");
        }

        public void HomeButtonClicked()
        {
            Main.SetScene(Main.Scene.home);
        }

        private void SetSprites()
        {
            bool chargeLeft = ProgressManager.GetProgress().settings.chargeOnLeftSide;
            if (!chargeLeft)
            {
                reflectImage.sprite = chargeSprite;
                chargeImage.sprite = reflectSprite;

                reflectSprite = reflectImage.sprite;
                chargeSprite = chargeImage.sprite;
            }
        }

        public void SwitchControls()
        {
            Debug.Log("Switched Controls");
            ProgressManager.GetProgress().settings.chargeOnLeftSide = !ProgressManager.GetProgress().settings.chargeOnLeftSide;
            reflectImage.sprite = chargeSprite;
            chargeImage.sprite = reflectSprite;

            reflectSprite = reflectImage.sprite;
            chargeSprite = chargeImage.sprite;
        }
    }
}