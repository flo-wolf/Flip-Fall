using Impulse;
using Impulse.Audio;
using Impulse.Levels;
using Impulse.Progress;
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

        public Animation fadeAnimation;
        public Animation homeAnimation;
        public Animation resetAnimation;
        public Animation testSaveAnimation;

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
            ProgressManager.ClearProgress();
            SoundManager.ButtonClicked();
            resetAnimation.Play("buttonClick");
        }

        public void HomeButtonClicked()
        {
            Main.SetScene(Main.Scene.home);
            ProgressManager.ClearProgress();
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
            Debug.Log("Speedslider");
            onHorizonSpeedChange.Invoke(s.value);
            ProgressManager.GetProgress().settings.backgroundSpeed = s.value;
        }
    }
}