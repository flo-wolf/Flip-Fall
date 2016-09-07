using Impulse;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Impulse.UI
{
    public class UIHomeManager : MonoBehaviour
    {
        public static UIHomeManager _instance;

        public Animation fadeAnimation;

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
            FadeOut();
        }

        public void TutorialButton()
        {
            Main.SetScene(Main.Scene.tutorial);
            FadeOut();
        }

        public void SettingsButton()
        {
            Main.SetScene(Main.Scene.settings);
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