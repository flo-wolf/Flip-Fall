using Impulse;
using Impulse.Audio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Impulse.UI
{
    public class UIHomeManager : MonoBehaviour
    {
        public static UIHomeManager _instance;

        public Animation fadeAnimation;
        public Animation levelAnimation;
        public Animation settingsAnimation;
        public Animation tutorialAnimation;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
                return;
            }
            _instance = this;
        }

        private void Start()
        {
            FadeIn();
        }

        public void LevelSelectButton()
        {
            Main.SetScene(Main.Scene.levelselection);
            levelAnimation.Play("buttonClick");
            SoundManager.ButtonClicked();
            FadeOut();
        }

        public void TutorialButton()
        {
            Main.SetScene(Main.Scene.tutorial);
            tutorialAnimation.Play("buttonClick");
            SoundManager.ButtonClicked();
            FadeOut();
        }

        public void SettingsButton()
        {
            Main.SetScene(Main.Scene.settings);
            settingsAnimation.Play("buttonClick");
            SoundManager.ButtonClicked();
            FadeOut();
        }

        private void FadeIn()
        {
            fadeAnimation.Play("fadeFromBlack");
        }

        public void HomeButton()
        {
            Main.SetScene(Main.Scene.home);
        }

        private void FadeOut()
        {
            fadeAnimation.Play("fadeToBlack");
        }
    }
}