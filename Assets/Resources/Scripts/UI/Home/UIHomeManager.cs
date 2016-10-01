﻿using Impulse;
using Impulse.Audio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///  Manages all UI Elements of the Home Scene.
/// </summary>

namespace Impulse.UI
{
    public class UIHomeManager : MonoBehaviour
    {
        public static UIHomeManager _instance;

        // Animations

        public Animator animator;
        public Animation startupAnimation;
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
            Main.onStartup.AddListener(AppStartup);

            Main.onSceneChange.AddListener(SceneChanged);
        }

        private void SceneChanged(Main.Scene s)
        {
            //animator.SetTrigger("fadeout");
        }

        private void AppStartup()
        {
            startupAnimation["fadeFromBlack"].speed = 0.5F;
            startupAnimation.Play("fadeFromBlack");
        }

        private void Start()
        {
            //FadeIn();
        }

        public void LevelSelectButton()
        {
            Main.SetScene(Main.Scene.levelselection);
            animator.SetTrigger("play");
            SoundManager.ButtonClicked();
            //FadeOut();
        }

        public void TutorialButton()
        {
            Main.SetScene(Main.Scene.tutorial);
            animator.SetTrigger("howto");
            SoundManager.ButtonClicked();
            //FadeOut();
        }

        public void SettingsButton()
        {
            Main.SetScene(Main.Scene.settings);
            animator.SetTrigger("settings");
            SoundManager.ButtonClicked();
            //FadeOut();
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