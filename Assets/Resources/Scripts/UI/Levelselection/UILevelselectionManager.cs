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
/// Manages all UI Elememnts of the levelselection scene
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
            activeUILevel = ProgressManager.GetProgress().lastPlayedLevelID;
            //Debug.Log(activeUILevel);
        }

        private void Start()
        {
            Main.onSceneChange.AddListener(SceneChanging);
            LevelManager.onLevelChange.AddListener(LevelChanging);

            UpdateLevelView();
            UILevelDrag.UpdateDragObject();
            //fadeAnimation.Play("fadeFromBlack");
            //uiLevelAnimation.Play("uiLevelselectionFadeIn");
        }

        // adjust UILevelID whenever the level changes
        private void LevelChanging(int newLevelID)
        {
            activeUILevel = newLevelID;
        }

        private void SceneChanging(Main.Scene scene)
        {
            if (scene == Main.Scene.game)
            {
                //currentUILevel = UILevelPlacer.LoadUILevel(ProgressManager.GetProgress().lastUnlockedLevel);
                //UpdateLevelView();
                //UILevelDrag.UpdateDragObject();
                //fadeAnimation.Play("fadeToBlack");
                //uiLevelAnimation.Play("uiLevelselectionFadeOut");
                //FadeOut();
            }
            else
            {
                //fadeAnimation.Play("fadeToBlack");
            }
        }

        // display the next UILevel - if it exists and if it is unlocked, then place it
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

        public static void NextWasUnlocked()
        {
            _instance.StartCoroutine(_instance.UnlockNext());
        }

        public IEnumerator UnlockNext()
        {
            // add delay here
            yield return new WaitForSeconds(0);
            NextLevel();
            yield break;
        }

        // display the last UILevel - if it exists and if it is unlocked, then place it
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

        // called on GameState.levelselection/finishscreen
        private void UpdateLevelView()
        {
            //Debug.Log("ACTIVEUILEVEL : " + activeUILevel);
            UILevelPlacer.uiLevels.Find(x => x.id == activeUILevel).UpdateTexts();
            UILevelPlacer.uiLevels.Find(x => x.id == activeUILevel).UpdateStars();
        }

        public void PlayLevel()
        {
            //level can be set
            SoundManager.ButtonClicked();
            if (LevelManager.LevelExists(activeUILevel))
            {
                LevelManager.SetLevel(activeUILevel);
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