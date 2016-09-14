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
        public static int activeUILevel;

        public static UILevelSwitchEvent onUILevelSwitch = new UILevelSwitchEvent();

        public class UILevelSwitchEvent : UnityEvent { }

        //References
        public Animation fadeAnimation;
        public Animation uiLevelAnimation;
        public Animation homeAnimation;
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

        private void OnEnable()
        {
            activeUILevel = LevelManager.lastPlayedID;
        }

        private void Start()
        {
            Main.onSceneChange.AddListener(SceneChanging);

            UpdateLevelView();
            UILevelDrag.UpdateDragObject();
            fadeAnimation.Play("fadeFromBlack");
            //uiLevelAnimation.Play("uiLevelselectionFadeIn");
        }

        private void SceneChanging(Main.Scene scene)
        {
            if (scene == Main.Scene.game)
            {
                //currentUILevel = UILevelPlacer.LoadUILevel(ProgressManager.GetProgress().lastUnlockedLevel);
                //UpdateLevelView();
                //UILevelDrag.UpdateDragObject();
                //fadeAnimation.Play("fadeToBlack");
                uiLevelAnimation.Play("uiLevelselectionFadeOut");
                //FadeOut();
            }
            else
            {
                fadeAnimation.Play("fadeToBlack");
            }
        }

        //display the next UILevel - if it exists and if it is unlocked, then place it
        public static bool NextLevel()
        {
            if (activeUILevel + 1 <= ProgressManager.GetProgress().lastUnlockedLevel)
            {
                activeUILevel++;
                UILevelPlacer.PlaceUILevel(activeUILevel);

                _instance.UpdateLevelView();
                UILevelDrag.UpdateDragObject();
                Debug.Log("NextLevel() currentUILevel.id " + activeUILevel);
                return true;
            }
            return false;
        }

        //display the last UILevel - if it exists and if it is unlocked, then place it
        public static bool LastLevel()
        {
            if (activeUILevel - 1 >= Constants.firstLevel)
            {
                activeUILevel--;
                UILevelPlacer.PlaceUILevel(activeUILevel);
                _instance.UpdateLevelView();
                UILevelDrag.UpdateDragObject();
                Debug.Log("LastLevel() currentUILevel.id " + activeUILevel);
                return true;
            }
            return false;
        }

        public void HomeBtnClicked()
        {
            SoundManager.ButtonClicked();
            homeAnimation.Play("buttonClick");
            Main.SetScene(Main.Scene.home);
        }

        //called on GameState.levelselection/finishscreen
        private void UpdateLevelView()
        {
            UILevelPlacer.uiLevels[activeUILevel].UpdateTexts();
            UILevelPlacer.uiLevels[activeUILevel].UpdateStars();
        }

        public void PlayLevel()
        {
            //level can be set
            SoundManager.ButtonClicked();
            if (LevelManager.LevelExists(activeUILevel))
            {
                LevelManager.activeLevel = activeUILevel;
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