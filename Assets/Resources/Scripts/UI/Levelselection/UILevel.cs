using FlipFall.Audio;
using FlipFall.Levels;
using FlipFall.Progress;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace FlipFall.UI
{
    public class UILevel : MonoBehaviour
    {
        public int id;

        // stars to unlock on scene entering
        public enum StarsToUnlock { none, star1, star12, star123, star2, star23, star3 }

        [HideInInspector]
        public StarsToUnlock starsToUnlock = StarsToUnlock.none;

        //Stars
        public GameObject Star1;
        public GameObject Star2;
        public GameObject Star3;

        //Times
        public Text timeSecText;
        public Text timeMilText;
        public Text topTimeSecText;
        public Text topTimeMilText;
        public Text failsCount;

        private Animator animator;

        private int starScore = -1;
        private Highscore highscore;

        [HideInInspector]
        public bool createdByLevelswitch = false;

        public void Start()
        {
            animator = GetComponent<Animator>();

            highscore = ProgressManager.GetProgress().highscores.Find(x => x.levelId == id);
            if (highscore != null && highscore.starCount >= 0)
            {
                starScore = highscore.starCount;
            }
            else
            {
                starScore = 0;
            }

            Main.onSceneChange.AddListener(SceneChanged);

            UpdateUILevel();

            if (createdByLevelswitch)
                animator.SetTrigger("levelswitch");
            else
                animator.SetTrigger("fadein");
        }

        private void SceneChanged(Main.ActiveScene s)
        {
            FadeOut();
            createdByLevelswitch = false;
        }

        public void FadeOut()
        {
            animator.SetTrigger("fadeout");
        }

        public void UpdateUILevel()
        {
            highscore = ProgressManager.GetProgress().highscores.Find(x => x.levelId == id);
            //levelNumberText.text = id.ToString();

            UpdateFails(highscore);

            SetNewStars();
            UpdateTimes();
        }

        // a new highscore got created, rising the current amount of stars => amimate those coming in new
        private void SetNewStars()
        {
            Debug.Log("starScore " + starScore + " starsToUnlock " + starsToUnlock);

            if (starsToUnlock != StarsToUnlock.none)
            {
                animator.SetTrigger("playInstantHide");
                StartCoroutine(cStarsRecieve());
            }
            else
            {
                ActivateStarImages();
                if (!createdByLevelswitch)
                    PlayFade();
            }
        }

        private void ActivateStarImages()
        {
            switch (starScore)
            {
                case 1:
                    Star1.SetActive(true);
                    Star2.SetActive(false);
                    Star3.SetActive(false);
                    break;

                case 2:
                    Star1.SetActive(true);
                    Star2.SetActive(true);
                    Star3.SetActive(false);
                    break;

                case 3:
                    Star1.SetActive(true);
                    Star2.SetActive(true);
                    Star3.SetActive(true);
                    break;

                default:
                    Star1.SetActive(false);
                    Star2.SetActive(false);
                    Star3.SetActive(false);
                    break;
            }
        }

        // fade in the newly reached stars one by one
        private IEnumerator cStarsRecieve()
        {
            Star1.SetActive(false);
            Star2.SetActive(false);
            Star3.SetActive(false);

            float delay = 0.5F;
            // star 1 is new
            if (starsToUnlock == StarsToUnlock.star1)
            {
                StarsFade(1);
            }
            else if (starsToUnlock == StarsToUnlock.star12)
            {
                StarsFade(1);
                yield return new WaitForSeconds(delay);
                StarsFade(2);
            }
            else if (starsToUnlock == StarsToUnlock.star123)
            {
                StarsFade(1);
                yield return new WaitForSeconds(delay / 2);
                StarsFade(2);
                yield return new WaitForSeconds(delay / 2);
                StarsFade(3);
            }
            else if (starsToUnlock == StarsToUnlock.star2)
            {
                Star1.SetActive(true);
                StarsFade(2);
            }
            else if (starsToUnlock == StarsToUnlock.star23)
            {
                Star1.SetActive(true);
                StarsFade(2);
                yield return new WaitForSeconds(delay);
                StarsFade(3);
            }
            else if (starsToUnlock == StarsToUnlock.star3)
            {
                Star1.SetActive(true);
                Star2.SetActive(true);
                StarsFade(3);
            }

            starsToUnlock = StarsToUnlock.none;

            yield return new WaitForSeconds(delay / 2);

            ActivateStarImages();
            PlayFade();

            yield break;
        }

        // called by UILevel - fades in a star (int : 1-3)
        public void StarsFade(int starId)
        {
            Debug.Log("starsFade " + starId);
            switch (starId)
            {
                case 1:
                    Star1.SetActive(true);
                    UILevelselectionManager.StarShake();
                    animator.SetTrigger("star1");
                    SoundManager.PlayStarGetSound();
                    break;

                case 2:
                    Star2.SetActive(true);
                    UILevelselectionManager.StarShake();
                    animator.SetTrigger("star2");
                    SoundManager.PlayStarGetSound();
                    break;

                case 3:
                    Star3.SetActive(true);
                    UILevelselectionManager.StarShake();
                    animator.SetTrigger("star3");
                    SoundManager.PlayStarGetSound();
                    break;
            }
        }

        // fade in the paly button
        public void PlayFade()
        {
            animator.SetTrigger("playFade");
            Button b = GetComponentInChildren<Button>();
            if (b != null)
                b.gameObject.SetActive(true);
        }

        public void UpdateFails(Highscore h)
        {
            if (h != null)
            {
                failsCount.text = string.Format("{0:0000}", highscore.fails);
            }
            else
                failsCount.text = "0000";
        }

        public void UpdateTimes()
        {
            // Debug.Log("UILevel Updatetexts of id: " + id);

            if (UILevelMatchesLevel())
            {
                double topTime = LevelManager.prefabLevels.Find(x => x.id == id).presetTime;

                // Preset top time seconds
                string topSec = ((int)topTime).ToString();
                topTimeSecText.text = topSec;

                // top time milseconds
                string topMilSec = string.Format("{0:0.00}", topTime);
                topMilSec = topMilSec.Substring(topMilSec.IndexOf(".") + 1);
                topTimeMilText.text = topMilSec;

                Highscore h = ProgressManager.GetProgress().highscores.Find(x => x.levelId == id);
                if (h != null && h.bestTime > 0)
                {
                    double bestTime = h.bestTime;
                    //Debug.Log("UpdateTexts() h.bestTime " + bestTime);

                    // Personal best seconds
                    string bestTimeString = ((int)bestTime).ToString();
                    timeSecText.text = bestTimeString;

                    //Debug.Log("UpdateTexts() bestTimeString " + bestTimeString);

                    // Personal best milseconds
                    string milSecs = string.Format("{0:0.00}", bestTime);
                    milSecs = milSecs.Substring(milSecs.IndexOf(".") + 1);
                    timeMilText.text = milSecs;
                }
                else
                {
                    timeSecText.text = "--";
                    timeMilText.text = "--";
                }
            }
        }

        public bool UILevelMatchesLevel()
        {
            if (LevelManager.LevelExists(id, false))
            {
                return true;
            }
            return false;
        }
    }
}