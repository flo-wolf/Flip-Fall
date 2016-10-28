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
        //for passing an unlock when entering the levelselection
        public enum EnterType { unlockNext, finished, failed }
        public static EnterType enterType = EnterType.failed;

        public static UILevelselectionManager _instance;
        public static int activeUILevel;

        public static UILevelSwitchEvent onUILevelSwitch = new UILevelSwitchEvent();

        public class UILevelSwitchEvent : UnityEvent<int> { }

        //References
        public Animator animator;

        public float fadeOutTime = 1F;
        public float fadeInTime = 1F;
        public float nextLevelDelay = 2F;

        private static bool nextLevelGetsUnlocked = false;

        public static bool unlockNextLevel = false;
        public static bool unlockNextLevelPossible = true;

        private bool playPressed = false;

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

        private void Start()
        {
            Main.onSceneChange.AddListener(SceneChanging);
            LevelManager.onLevelChange.AddListener(LevelChanging);

            UILevelDrag.UpdateDragObject();

            if (unlockNextLevel)
            {
                NextWasUnlocked();
            }

            nextLevelGetsUnlocked = false;
            playPressed = false;

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
            animator.SetTrigger("fadeout");

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
                unlockNextLevel = false;
                Debug.Log("ProgressManager.GetProgress().lastUnlockedLevel " + ProgressManager.GetProgress().lastUnlockedLevel);
                activeUILevel++;
                ProgressManager.GetProgress().lastPlayedLevelID = activeUILevel;

                UILevelPlacer.newUnlocks = UILevel.StarsToUnlock.none;
                UILevelPlacer.PlaceUILevel(activeUILevel, true);

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
                unlockNextLevel = false;
                activeUILevel--;
                ProgressManager.GetProgress().lastPlayedLevelID = activeUILevel;

                UILevelPlacer.newUnlocks = UILevel.StarsToUnlock.none;
                UILevelPlacer.placedLevel.FadeOut();
                //delay this
                UILevelPlacer.PlaceUILevel(activeUILevel, true);
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
                return true;
            }
            return false;
        }

        public static void NextWasUnlocked()
        {
            _instance.StartCoroutine(_instance.UnlockNext());
        }

        // ADD LEVEL UNLOCK ANIMATIONS AND DELAY
        public IEnumerator UnlockNext()
        {
            // add delay here
            yield return new WaitForSeconds(nextLevelDelay);
            if (unlockNextLevel)
            {
                NextLevel();
                SoundManager.PlayLevelSwitchSound();
            }

            yield return new WaitForSeconds(3F);
            unlockNextLevel = false;
            yield break;
        }

        public void HomeBtnClicked()
        {
            unlockNextLevel = false;
            SoundManager.ButtonClicked();
            Main.SetScene(Main.Scene.home);
        }

        public void PlayLevel()
        {
            unlockNextLevel = false;
            Debug.Log(nextLevelGetsUnlocked);
            //level can be set
            if (!nextLevelGetsUnlocked && !playPressed)
            {
                SoundManager.ButtonClicked();
                if (LevelManager.LevelExists(activeUILevel))
                {
                    LevelManager.SetLevel(activeUILevel);
                    SoundManager.PlayCamTransitionSound();
                    Main.SetScene(Main.Scene.game);
                }
            }

            playPressed = true;
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

        public static void StarShake()
        {
            _instance.animator.SetTrigger("starShake");
        }
    }
}