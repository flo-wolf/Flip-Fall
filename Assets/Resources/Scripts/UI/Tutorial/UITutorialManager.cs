using FlipFall.Audio;
using FlipFall.Levels;
using FlipFall.Progress;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages all UI Elements through externally fired events and turns them on and off
/// </summary>

namespace FlipFall.UI
{
    public class UITutorialManager : MonoBehaviour
    {
        //public enum UIState { levelSelection, home, settings, game, title, shop, editor, credits, buyPro }
        //public static UIState uiState;
        public static UITutorialManager _instance;

        public Animator animator;

        public Animation switchAnimation;

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

            //FadeIn();
            Main.onSceneChange.AddListener(SceneChanging);

            reflectSprite = reflectImage.sprite;
            chargeSprite = chargeImage.sprite;
            SetSprites();
        }

        private void SceneChanging(Main.Scene scene)
        {
            animator.SetTrigger("fadeout");
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

        public void HomeButtonClicked()
        {
            SoundManager.ButtonClicked();
            Main.SetScene(Main.Scene.home);
        }

        public void SwitchControls()
        {
            Debug.Log("Switched Controls");
            SoundManager.ButtonClicked();
            switchAnimation.Play("buttonClick");
            ProgressManager.GetProgress().settings.chargeOnLeftSide = !ProgressManager.GetProgress().settings.chargeOnLeftSide;
            reflectImage.sprite = chargeSprite;
            chargeImage.sprite = reflectSprite;

            reflectSprite = reflectImage.sprite;
            chargeSprite = chargeImage.sprite;
        }
    }
}