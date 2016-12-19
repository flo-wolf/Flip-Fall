using FlipFall.Audio;
using FlipFall.Levels;
using FlipFall.Progress;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages all UI Elements of the respective scene
/// </summary>

namespace FlipFall.UI
{
    public class UIGoProManager : MonoBehaviour
    {
        //public enum UIState { levelSelection, home, settings, game, title, shop, editor, credits, buyPro }
        //public static UIState uiState;
        public static UIGoProManager _instance;

        public Animator animator;
        //private buy

        private void Start()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
                return;
            }
            _instance = this;

            Main.onSceneChange.AddListener(SceneChanging);
            InAppBilling.onProBuy.AddListener(ProBought);

            if (!InAppBilling.ProIsOwned())
            {
                Debug.Log("is not owned");
                animator.SetTrigger("fadeBuy");
            }
            else
            {
                Debug.Log("is owned");
                animator.SetTrigger("fadeOwned");
            }
            // check if the item is bought or not and then fade the correct ui in

            // else: animator.SetTrigger("fadeThanks");
        }

        private void SceneChanging(Main.ActiveScene scene)
        {
            animator.SetTrigger("fadeout");
        }

        public void HomeButtonClicked()
        {
            SoundManager.ButtonClicked();
            Main.SetScene(Main.ActiveScene.home);
        }

        public void RewardedVideoButton()
        {
            SoundManager.ButtonClicked();
            Main.ShowRewardedVideo();
        }

        public void ProButtonClicked()
        {
            // pro not bought yet
            if (!IsProUnlocked())
            {
                // try to buy
                if (InAppBilling.BuyPro())
                {
                    // change scene to a "thank you" notice
                }
            }
            SoundManager.ButtonClicked();
        }

        // in app billing listener when pro gets bought
        private void ProBought()
        {
            Debug.Log("SUCCESSFULL PRO BUY");
            animator.ResetTrigger("buy");
            animator.SetTrigger("buy");
        }

        private static bool IsProUnlocked()
        {
            //checks
            return false;
        }
    }
}