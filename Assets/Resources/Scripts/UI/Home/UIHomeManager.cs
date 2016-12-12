﻿using FlipFall;
using FlipFall.Audio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///  Manages all UI Elements of the Home Scene.
/// </summary>

namespace FlipFall.UI
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

        private void SceneChanged(Main.ActiveScene s)
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

        public void AchievementButton()
        {
            Main.SetScene(Main.ActiveScene.achievements);
            animator.SetTrigger("achievements");
            SoundManager.ButtonClicked();
        }

        public void LevelSelectButton()
        {
            Main.SetScene(Main.ActiveScene.levelselection);
            animator.SetTrigger("play");
            SoundManager.ButtonClicked();
        }

        public void TutorialButton()
        {
            Main.SetScene(Main.ActiveScene.tutorial);
            animator.SetTrigger("howto");
            SoundManager.ButtonClicked();
        }

        public void SettingsButton()
        {
            Main.SetScene(Main.ActiveScene.settings);
            animator.SetTrigger("settings");
            SoundManager.ButtonClicked();
        }

        public void EditorButton()
        {
            Main.SetScene(Main.ActiveScene.editor);
            animator.SetTrigger("editor");
            SoundManager.ButtonClicked();
        }

        public void CreditsButton()
        {
            Main.SetScene(Main.ActiveScene.credits);
            animator.SetTrigger("fadeout");
            SoundManager.ButtonClicked();
        }

        public void ShopButton()
        {
            Main.SetScene(Main.ActiveScene.shop);
            animator.SetTrigger("shop");
            SoundManager.ButtonClicked();
        }

        public void GoProButton()
        {
            Main.SetScene(Main.ActiveScene.gopro);
            animator.SetTrigger("fadeout");
            SoundManager.ButtonClicked();
        }

        private void FadeIn()
        {
            fadeAnimation.Play("fadeFromBlack");
        }

        private void FadeOut()
        {
            fadeAnimation.Play("fadeToBlack");
        }
    }
}