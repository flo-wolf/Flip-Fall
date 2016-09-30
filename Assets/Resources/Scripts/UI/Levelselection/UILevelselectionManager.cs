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

        public class UILevelSwitchEvent : UnityEvent<int> { }

        //References
        public Animation fadeAnimation;
        public Animation uiLevelAnimation;
        public Animation homeAnimation;
        public Animation playButtonAnimation;

        //LevelNumber Animators
        private List<Animator> LevelNumberAnims;

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
            activeUILevel = ProgressManager.GetProgress().lastPlayedLevelID;
        }

        private void OnEnable()
        {
        }

        private void Start()
        {
            Main.onSceneChange.AddListener(SceneChanging);
            LevelManager.onLevelChange.AddListener(LevelChanging);
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
                UILevelPlacer.placedLevel.UpdateUILevel();
                UILevelDrag.UpdateDragObject();

                // move the numbers left, bringing the nextLevel into focus, removing those outside
                for (int i = UILevelPlacer.placedLevelNumbers.Count - 1; i >= 0; i--)
                {
                    UILevelPlacer.placedLevelNumbers[i].Left();
                }

                //try to place leftover levels
                for (int i = -2; i <= 2; i++)
                {
                    UILevelPlacer.PlaceLevelNumber(UILevelPlacer.placedLevel.id + i, i);
                }

                //// how many levels are currently allowed to exist
                //int maxAllowed = 5;
                //maxAllowed = UILevelPlacer.placedLevel.id

                //// add missing uiLevelNumbers if there are less than the maxAllowed number
                //if (UILevelPlacer.placedLevelNumbers.Count < maxAllowed)
                //{
                //}

                Debug.Log("NextLevel() currentUILevel.id " + activeUILevel);

                onUILevelSwitch.Invoke(activeUILevel);

                return true;
            }
            return false;
        }

        // display the last UILevel - if it exists and if it is unlocked, then place it
        public static bool LastLevel()
        {
            if (activeUILevel - 1 >= Constants.firstLevel)
            {
                activeUILevel--;
                UILevelPlacer.PlaceUILevel(activeUILevel);
                UILevelPlacer.placedLevel.UpdateUILevel();
                UILevelDrag.UpdateDragObject();

                // backwards iteration needed, because we might remove items during iterating from the collection
                for (int i = UILevelPlacer.placedLevelNumbers.Count - 1; i >= 0; i--)
                {
                    UILevelPlacer.placedLevelNumbers[i].Right();
                }

                //try to place leftover levels
                for (int i = -2; i <= 2; i++)
                {
                    UILevelPlacer.PlaceLevelNumber(UILevelPlacer.placedLevel.id + i, i);
                }

                //for(int i = 0; i<)

                Debug.Log("LastLevel() currentUILevel.id " + activeUILevel);
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

        public void HomeBtnClicked()
        {
            SoundManager.ButtonClicked();
            homeAnimation.Play("buttonClick");
            Main.SetScene(Main.Scene.home);
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