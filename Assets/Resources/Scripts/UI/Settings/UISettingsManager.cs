﻿using FlipFall;
using FlipFall.Audio;
using FlipFall.Background;
using FlipFall.Levels;
using FlipFall.Progress;
using FlipFall.Theme;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace FlipFall.UI
{
    public class UISettingsManager : MonoBehaviour
    {
        public static UISettingsManager _instance;
        public static MusicVolumeChangeEvent onMusicVolumeChange = new MusicVolumeChangeEvent();

        public class MusicVolumeChangeEvent : UnityEvent<float> { }

        public static FXVolumeChangeEvent onFXVolumeChange = new FXVolumeChangeEvent();

        public class FXVolumeChangeEvent : UnityEvent<float> { }

        public static HorizonSpeedChangeEvent onHorizonSpeedChange = new HorizonSpeedChangeEvent();

        public class HorizonSpeedChangeEvent : UnityEvent<float> { }

        public Animator animator;
        public Animation homeAnimation;
        public Animation resetAnimation;
        public Animation testSaveAnimation;

        public Slider fxSlider;
        public Slider musicSlider;
        public Slider bgAmplitudeSlider;
        public Slider cameraZoomSlider;

        // theme toggles
        private Toggle[] toggles;

        private bool requestActive = false;

        // Use this for initialization
        private void Start()
        {
            SetSliders();
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
                return;
            }
            _instance = this;

            // if there are other different toggles this wont work, define horizontoggle root object -  search for horizontoggles
            toggles = GetComponentsInChildren<Toggle>();
            Main.onSceneChange.AddListener(SceneChanging);
            ProgressManager.onProgressChange.AddListener(ProgressChanged);
            UpdateToggles();
        }

        private void UpdateToggles()
        {
            foreach (Toggle t in toggles)
            {
                if (t.GetComponent<HorizonToggle>().skin == ProgressManager.GetProgress().unlocks.currentSkin)
                    t.isOn = true;
                else
                    t.isOn = false;
            }
        }

        public void ToogleChanged(Toggle t)
        {
            if (t.isOn)
            {
                // set all other toggles to off
                foreach (Toggle tog in toggles)
                {
                    if (tog != t)
                        tog.isOn = false;
                }

                ThemeManager.Skin skin = t.GetComponent<HorizonToggle>().skin;
                ThemeManager.SetSkin(skin);
            }
            if (!t.isOn)
            {
                foreach (Toggle tog in toggles)
                {
                    if (tog != t)
                        tog.isOn = false;
                }
                int offCounter = 0;
                for (int i = 0; i < toggles.Length; i++)
                {
                    if (!toggles[i].isOn)
                    {
                        offCounter++;
                    }
                    if (offCounter == toggles.Length)
                    {
                        t.isOn = true;
                    }
                }
            }
        }

        private void SceneChanging(Main.ActiveScene scene)
        {
            animator.SetTrigger("fadeout");
        }

        private void ProgressChanged(ProgressData p)
        {
            SetSliders();
        }

        private void SetSliders()
        {
            fxSlider.value = ProgressManager.GetProgress().settings.fxVolume;
            musicSlider.value = ProgressManager.GetProgress().settings.musicVolume;
            bgAmplitudeSlider.value = ProgressManager.GetProgress().settings.backgroundSpeed;
            cameraZoomSlider.value = ProgressManager.GetProgress().settings.cameraZoomStep;
            onMusicVolumeChange.Invoke(musicSlider.value);
        }

        public void DeactivateImageFX()
        {
        }

        //THIS COULD LEAD TO PROBLEMS, if so change lastlevel to last level able to find under prefabs/levels
        //public void UnlockAllButtonClicked()
        //{
        //    ProgressManager.GetProgress().storyProgress.lastUnlockedLevel = LevelManager._instance;
        //    SoundManager.ButtonClicked();
        //    testSaveAnimation.Play("buttonClick");
        //}

        public void ResetProgress()
        {
            requestActive = true;
            animator.SetTrigger("resetRequest");
            SoundManager.ButtonClicked();
        }

        public void ResetYes()
        {
            if (requestActive)
            {
                animator.SetTrigger("resetYes");
                ProgressManager.ClearProgress();
                SoundManager.ButtonClicked();
                requestActive = false;

                // Reset Button
                Social.ReportProgress("CgkIqIqqjZYFEAIQFg", 100.0f, (bool success) =>
                {
                    if (success)
                        Main.onAchievementUnlock.Invoke();
                });
            }
        }

        public void ResetNo()
        {
            if (requestActive)
            {
                animator.SetTrigger("resetNo");
                SoundManager.ButtonClicked();
                requestActive = false;
            }
        }

        public void HomeButtonClicked()
        {
            SoundManager.ButtonClicked();
            homeAnimation.Play("buttonClick");
            Main.SetScene(Main.ActiveScene.home);
        }

        public void CreditsButtonClicked()
        {
            Main.SetScene(Main.ActiveScene.credits);
            SoundManager.ButtonClicked();
            homeAnimation.Play("buttonClick");
        }

        public void FXSliderChanged(Slider s)
        {
            onFXVolumeChange.Invoke(s.value);
            ProgressManager.GetProgress().settings.fxVolume = s.value;
        }

        public void MusicSliderChanged(Slider s)
        {
            onMusicVolumeChange.Invoke(s.value);
            ProgressManager.GetProgress().settings.musicVolume = s.value;
        }

        // zoom steps 1-5
        public void CameraZoomSliderChanged(Slider s)
        {
            Debug.Log("CHANGED CAMERA ZOOM " + s.value);
            int i = (int)s.value;
            ProgressManager.GetProgress().settings.cameraZoomStep = i;
        }

        public void SpeedSliderChanged(Slider s)
        {
            ProgressManager.GetProgress().settings.backgroundSpeed = s.value;
            Debug.Log("Speedslider");
            onHorizonSpeedChange.Invoke(s.value);
        }

        public void EditorUnlockToogle(Toggle t)
        {
            if (t.isOn)
            {
                ProgressManager.GetProgress().proVersion = true;
                print("unlocked pro, editor activated");
            }
            else
            {
                ProgressManager.GetProgress().proVersion = false;
                print("locked pro, editor disabled");
            }
        }
    }
}