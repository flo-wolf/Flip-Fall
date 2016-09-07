using Impulse;
using Impulse.Audio;
using Impulse.Progress;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Impulse.UI
{
    public class UISettingsManager : MonoBehaviour
    {
        public static UISettingsManager _instance;
        public Animation fadeAnimation;

        public Slider fxSlider;
        public Slider musicSlider;

        // Use this for initialization
        private void Start()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
                return;
            }
            _instance = this;

            FadeIn();
            SetSliders();
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

        private void SetSliders()
        {
            fxSlider.value = ProgressManager.GetProgress().settings.fxVolume;
            musicSlider.value = ProgressManager.GetProgress().settings.musicVolume;
        }

        public void DeactivateImageFX()
        {
        }

        public void HomeButtonClicked()
        {
            Main.SetScene(Main.Scene.home);
        }

        public void FXSliderChanged(Slider s)
        {
            ProgressManager.GetProgress().settings.fxVolume = s.value;
        }

        public void MusicSliderChanged(Slider s)
        {
            ProgressManager.GetProgress().settings.musicVolume = s.value;
        }

        public void ResetProgress()
        {
            ProgressManager.ClearProgress();
        }
    }
}