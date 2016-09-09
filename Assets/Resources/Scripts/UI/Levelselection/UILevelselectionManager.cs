using Impulse;
using Impulse.Audio;
using Impulse.Levels;
using Impulse.Progress;
using Impulse.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// Handles the UI Elememnts for the levelselection scene
/// Singletone
/// </summary>
namespace Impulse.UI
{
    public class UILevelselectionManager : MonoBehaviour
    {
        public static UILevelselectionManager _instance;
        public static UILevel currentUILevel;

        public static UILevelSwitchEvent onUILevelSwitch = new UILevelSwitchEvent();

        public class UILevelSwitchEvent : UnityEvent { }

        //References
        public Animation fadeAnimation;
        public Animation uiLevelAnimation;
        public Animation playButtonAnimation;
        public float fadeOutTime = 1F;
        public float fadeInTime = 1F;

        private static List<Highscore> highscores;
        private static Highscore highscore;

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
            Main.onSceneChange.AddListener(SceneChanging);
            currentUILevel = UILevelPlacer.LoadUILevel(ProgressManager.GetProgress().lastUnlockedLevel);
            UpdateLevelView();
            UILevelDrag.UpdateDragObject();
            fadeAnimation.Play("fadeFromBlack");
            uiLevelAnimation.Play("uiLevelselectionFadeIn");
        }

        private void SceneChanging(Main.Scene scene)
        {
            currentUILevel = UILevelPlacer.LoadUILevel(ProgressManager.GetProgress().lastUnlockedLevel);
            UpdateLevelView();
            UILevelDrag.UpdateDragObject();
            //fadeAnimation.Play("fadeToBlack");
            uiLevelAnimation.Play("uiLevelselectionFadeOut");
            //FadeOut();
        }

        //display the next UILevel - if it exists and if it is unlocked, then place it
        public static bool NextLevel()
        {
            Debug.Log("NextLevel() currentUILevel.id " + currentUILevel.id);
            if (currentUILevel.id + 1 <= ProgressManager.GetProgress().lastUnlockedLevel)
            {
                currentUILevel = UILevelPlacer.LoadUILevel(currentUILevel.id + 1);
                _instance.UpdateLevelView();
                UILevelDrag.UpdateDragObject();
                return true;
            }
            return false;
        }

        //display the last UILevel - if it exists and if it is unlocked, then place it
        public static bool LastLevel()
        {
            Debug.Log("LastLevel() currentUILevel.id " + currentUILevel.id);
            if (currentUILevel.id - 1 > 0)
            {
                currentUILevel = UILevelPlacer.LoadUILevel(currentUILevel.id - 1);
                _instance.UpdateLevelView();
                UILevelDrag.UpdateDragObject();
                return true;
            }
            return false;
        }

        public void HomeBtnClicked()
        {
            Main.SetScene(Main.Scene.home);
        }

        //called on GameState.levelselection/finishscreen
        private void UpdateLevelView()
        {
            currentUILevel.UpdateTexts();
            currentUILevel.UpdateStars();
        }

        public void PlayLevel()
        {
            //level can be set
            if (LevelManager.LevelExists(currentUILevel.id))
            {
                LevelManager.SetLevel(currentUILevel.id);
                playButtonAnimation.Play("playButtonDisappear");
                SoundManager.PlayCamTransitionSound();
                Main.SetScene(Main.Scene.game);
            }
            // else - animate failure
        }

        public static UILevel GetUILevel(int id)
        {
            UILevel uiLevel = null;
            UILevel[] uiLevels = _instance.gameObject.GetComponentsInChildren<UILevel>();
            foreach (UILevel lvl in uiLevels)
            {
                if (lvl.id == id)
                {
                    uiLevel = lvl;
                    break;
                }
            }
            return uiLevel;
        }
    }
}