using Impulse;
using Impulse.Audio;
using Impulse.Background;
using Impulse.Levels;
using Impulse.Progress;
using Impulse.Theme;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Impulse.UI
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

        // theme toggles
        private Toggle[] toggles;

        private bool requestActive = false;

        // Use this for initialization
        private void Start()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
                return;
            }
            _instance = this;

            // if there are other different toggles this wont work, define horizontoggle root object -  search for horizontoggles
            toggles = GetComponentsInChildren<Toggle>();

            SetSliders();
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

        private void SceneChanging(Main.Scene scene)
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

            onMusicVolumeChange.Invoke(musicSlider.value);
        }

        public void DeactivateImageFX()
        {
        }

        //THIS COULD LEAD TO PROBLEMS, if so change lastlevel to last level able to find under prefabs/levels
        public void UnlockAllButtonClicked()
        {
            ProgressManager.GetProgress().lastUnlockedLevel = LevelManager.lastID;
            SoundManager.ButtonClicked();
            testSaveAnimation.Play("buttonClick");
        }

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
            Main.SetScene(Main.Scene.home);
        }

        public void CreditsButtonClicked()
        {
            Main.SetScene(Main.Scene.credits);
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

        public void SpeedSliderChanged(Slider s)
        {
            ProgressManager.GetProgress().settings.backgroundSpeed = s.value;
            Debug.Log("Speedslider");
            onHorizonSpeedChange.Invoke(s.value);
        }
    }
}