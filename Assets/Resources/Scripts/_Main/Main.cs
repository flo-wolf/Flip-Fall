using admob;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using Impulse.Progress;
using Impulse.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms;

/// <summary>
/// Main Application Controller. Switches scenes upon calling SetScene() after a delay, giving other classes a timeframe
/// to save or initiate fade-in and fade-outs, since the scene switch is called after a delay post-event invoke.
/// </summary>

namespace Impulse
{
    public class Main : MonoBehaviour
    {
        public static Main _instance;

        /// <summary>
        /// Indicates which scene is curretly active -  switch with SetScene()
        /// </summary>
        public enum Scene { welcome, home, levelselection, tutorial, game, settings, editor, shop, achievements }
        public static Scene currentScene;

        public float sceneSwitchDelay = 0.5F;

        public static StartupEvent onStartup = new StartupEvent();
        public static SceneChangeEvent onSceneChange = new SceneChangeEvent();

        public static AchievementEvent onAchievementUnlock = new AchievementEvent();

        // avoid multiple awake calls due to DontDestroyOnLoad
        public static bool started = false;

        // ads
        public static int adCooldownCounter = 0;
        public static int adEveryFinish = 4;
        public static bool adInQue = false;

        private void Awake()
        {
            if (!started)
            {
                // Activate the Google Play Games platform
                PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().EnableSavedGames().Build();

                PlayGamesPlatform.InitializeInstance(config);
                PlayGamesPlatform.DebugLogEnabled = true;
                PlayGamesPlatform.Activate();

                // initialize progress (replace by google save)
                ProgressManager.LoadProgressData();

                // Initialize Ads
                Admob.Instance().initAdmob("ca-app-pub-2906510767249222/2074269594", "ca-app-pub-2906510767249222/2353471190");//admob id with format ca-app-pub-279xxxxxxxx/xxxxxxxx
                //Admob.Instance().setTesting(true);

                currentScene = Scene.home;
            }

            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
                return;
            }
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        private void Start()
        {
            if (!started)
            {
                // initialize ads
                RequestInterstitial();

                onStartup.Invoke();
                started = true;
            }
        }

        public static void SetScene(Scene newScene)
        {
            ProgressManager.SaveProgressData();
            currentScene = newScene;
            onSceneChange.Invoke(newScene);
            switch (newScene)
            {
                case Scene.welcome:
                    if (SceneManager.GetActiveScene().name != "Welcome")
                        _instance.StartCoroutine(_instance.cSetScene("Welcome"));
                    break;

                case Scene.home:
                    _instance.StartCoroutine(_instance.cSetScene("Home"));
                    break;

                case Scene.shop:
                    if (SceneManager.GetActiveScene().name != "Shop")
                        _instance.StartCoroutine(_instance.cSetScene("Shop"));
                    break;

                case Scene.levelselection:
                    if (SceneManager.GetActiveScene().name != "Levelselection")
                        _instance.StartCoroutine(_instance.cSetScene("Levelselection"));
                    break;

                case Scene.tutorial:
                    if (SceneManager.GetActiveScene().name != "Tutorial")
                        _instance.StartCoroutine(_instance.cSetScene("Tutorial"));
                    break;

                case Scene.game:
                    if (SceneManager.GetActiveScene().name != "Game")
                        _instance.StartCoroutine(_instance.cSetScene("Game"));
                    break;

                case Scene.settings:
                    if (SceneManager.GetActiveScene().name != "Settings")
                        _instance.StartCoroutine(_instance.cSetScene("Settings"));
                    break;

                case Scene.achievements:
                    if (SceneManager.GetActiveScene().name != "Achievements")
                    {
#if UNITY_EDITOR
                        _instance.StartCoroutine(_instance.cSetScene("Home"));
#elif UNITY_ANDROID
                        Social.localUser.Authenticate((bool success) =>
                        {
                            if (success)
                            {
                                _instance.StartCoroutine(_instance.cSetSceneAchievement());
                            }
                            else
                            {
                                SetScene(Scene.home);
                            }
                        });
#endif
                    }
                    break;

                case Scene.editor:
                    if (ProgressManager.GetProgress().proVersion)
                    {
                        if (SceneManager.GetActiveScene().name != "Editor")
                            _instance.StartCoroutine(_instance.cSetScene("Editor"));
                    }
                    else
                    {
                        if (SceneManager.GetActiveScene().name != "EditorUnlock")
                            _instance.StartCoroutine(_instance.cSetScene("EditorUnlock"));
                    }
                    break;
            }
        }

        private IEnumerator cSetScene(string sceneName)
        {
            AsyncOperation ao = SceneManager.LoadSceneAsync(sceneName);
            ao.allowSceneActivation = false;
            yield return new WaitForSeconds(sceneSwitchDelay);

            // Ad management
            // a level was just won - check if displaying an ad is possible
            if (sceneName == "Levelselection" && UILevelselectionManager.enterType == UILevelselectionManager.EnterType.finished)
            {
                adCooldownCounter++;
                if (adCooldownCounter % adEveryFinish == 0 || adInQue)
                {
                    if (ShowInterstitial())
                    {
                        adCooldownCounter = 0;
                        adInQue = false;
                    }
                    else
                    {
                        adInQue = true;
                    }
                }
            }

            //display the scene
            ao.allowSceneActivation = true;

            yield break;
        }

        private IEnumerator cSetSceneAchievement()
        {
            yield return new WaitForSeconds(sceneSwitchDelay);

            // show default achievements ui, placeholder
            Social.ShowAchievementsUI();
            SetScene(Scene.home);

            yield break;
        }

        private void OnApplicationQuit()
        {
            ProgressManager.SaveProgressData();
            PlayGamesPlatform.Instance.SignOut();
        }

        // Listening for Android-back-key presses
        private void Update()
        {
            if (started)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    if (currentScene == Scene.home)
                        Application.Quit();
                    else if (currentScene == Scene.game)
                        SetScene(Scene.levelselection);
                    else
                        SetScene(Scene.home);
                }
#if UNITY_EDITOR
                else if (Input.GetKeyDown(KeyCode.Return))
                {
                    if (currentScene == Scene.levelselection && UILevelselectionManager._instance != null)
                    {
                        UILevelselectionManager._instance.PlayLevel();
                    }
                }
#endif
            }
        }

        //banner: ca-app-pub-2906510767249222/2074269594
        //interstitial: ca-app-pub-2906510767249222/2353471190
        public static void RequestInterstitial()
        {
            Admob.Instance().loadInterstitial();
        }

        public static bool ShowInterstitial()
        {
            if (Admob.Instance().isInterstitialReady())
            {
                Admob.Instance().showInterstitial();
                return true;
            }
            else
            {
                RequestInterstitial();
                if (Admob.Instance().isInterstitialReady())
                {
                    Admob.Instance().showInterstitial();
                    return true;
                }
            }
            return false;
        }

        public class StartupEvent : UnityEvent { }

        public class SceneChangeEvent : UnityEvent<Scene> { }

        public class AchievementEvent : UnityEvent { }
    }
}