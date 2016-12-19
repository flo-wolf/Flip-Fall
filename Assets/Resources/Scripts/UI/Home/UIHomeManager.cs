using FlipFall;
using FlipFall.Audio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///  Manages all UI Elements of the Home Scene.
/// </summary>

namespace FlipFall.UI
{
    public class UIHomeManager : MonoBehaviour
    {
        public static UIHomeManager _instance;

        // Animations

        public Animator animator;
        public Animation startupAnimation;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
                return;
            }
            _instance = this;
            Main.onStartup.AddListener(AppStartup);

            Main.onSceneChange.AddListener(SceneChanged);
        }

        private void SceneChanged(Main.ActiveScene s)
        {
            //animator.SetTrigger("fadeout");
        }

        private void AppStartup()
        {
            startupAnimation["fadeFromBlack"].speed = 0.5F;
            startupAnimation.Play("fadeFromBlack");
        }

        private void Start()
        {
            //FadeIn();
        }

        public void AchievementButton()
        {
            Main.SetScene(Main.ActiveScene.achievements);
            animator.SetTrigger("achievements");
            SoundManager.ButtonClicked();
        }

        public void LevelSelectButton()
        {
            Main.SetScene(Main.ActiveScene.levelselection);
            animator.SetTrigger("play");
            SoundManager.ButtonClicked();
        }

        public void TutorialButton()
        {
            Main.SetScene(Main.ActiveScene.tutorial);
            animator.SetTrigger("howto");
            SoundManager.ButtonClicked();
        }

        public void SettingsButton()
        {
            Main.SetScene(Main.ActiveScene.settings);
            animator.SetTrigger("settings");
            SoundManager.ButtonClicked();
        }

        public void EditorButton()
        {
            Main.SetScene(Main.ActiveScene.editor);
            animator.SetTrigger("editor");
            SoundManager.ButtonClicked();
        }

        public void CreditsButton()
        {
            Main.SetScene(Main.ActiveScene.credits);
            animator.SetTrigger("fadeout");
            SoundManager.ButtonClicked();
        }

        public void ShopButton()
        {
            Main.SetScene(Main.ActiveScene.shop);
            animator.SetTrigger("shop");
            SoundManager.ButtonClicked();
        }

        public void GoProButton()
        {
            Main.SetScene(Main.ActiveScene.gopro);
            animator.SetTrigger("fadeout");
            SoundManager.ButtonClicked();
        }

        public void ShareButton()
        {
            Debug.Log("share");
            SoundManager.ButtonClicked();
            ShareText();
        }

        public void RateButton()
        {
            Debug.Log("rate");
            SoundManager.ButtonClicked();
            Application.OpenURL("market://details?id=com.florianwolf.flipfall");
        }

        private string subject = "Flip Fall";
        private string body = "Beat this. https://play.google.com/store/apps/details?id=com.florianwolf.flipfall";

        public void ShareText()
        {
#if UNITY_EDITOR
            // do nothing in unity

            //execute the below lines if being run on a Android device
#elif UNITY_ANDROID
            //Reference of AndroidJavaClass class for intent
            AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
            //Reference of AndroidJavaObject class for intent
            AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");

            //call setAction method of the Intent object created
            intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
            //set the type of sharing that is happening
            intentObject.Call<AndroidJavaObject>("setType", "text/plain");
            //add data to be passed to the other activity i.e., the data to be sent
            intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_SUBJECT"), subject);
            intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), body);
            //get the current activity
            AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");

            //start the activity by sending the intent data
            AndroidJavaObject jChooser = intentClass.CallStatic<AndroidJavaObject>("createChooser", intentObject, "1 share = 1 worldpeace");
            currentActivity.Call("startActivity", jChooser);
#endif
        }
    }
}