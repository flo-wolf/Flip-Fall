using admob;
using FlipFall.Progress;
using FlipFall.UI;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms;

/// <summary>
/// Main Application Controller. Switches scenes upon calling SetScene() after a delay, giving other classes a timeframe
/// to save or initiate fade-in and fade-outs, since the scene switch is called after a delay post-event invoke.
/// </summary>

namespace FlipFall
{
    public class Main : MonoBehaviour
    {
        public static Main _instance;

        /// <summary>
        /// Indicates which scene is curretly active -  switch with SetScene()
        /// </summary>
        public enum ActiveScene { welcome, home, levelselection, tutorial, game, settings, editor, shop, achievements, credits, gopro }
        public static ActiveScene currentScene;

        public float sceneSwitchDelay = 0.5F;

        public static StartupEvent onStartup = new StartupEvent();
        public static SceneChangeEvent onSceneChange = new SceneChangeEvent();

        public static AchievementEvent onAchievementUnlock = new AchievementEvent();

        // avoid multiple awake calls due to DontDestroyOnLoad
        public static bool started = false;

        // ads
        public static int adCooldownCounter = 0;
        public static int adEveryFinish = 4;
        public static int adEveryTest = 8;
        public static bool adInQue = false;

        public static bool switchingScene = false;

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

                currentScene = ActiveScene.home;

#if UNITY_ANDROID
                Social.localUser.Authenticate((bool success) =>
                {
                });
#endif
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

        public static void SetScene(ActiveScene newScene)
        {
            if (!switchingScene)
            {
                switchingScene = true;
                ProgressManager.SaveProgressData();
                currentScene = newScene;
                onSceneChange.Invoke(newScene);
                switch (newScene)
                {
                    case ActiveScene.welcome:
                        if (SceneManager.GetActiveScene().name != "Welcome")
                            _instance.StartCoroutine(_instance.cSetScene("Welcome"));
                        break;

                    case ActiveScene.home:
                        _instance.StartCoroutine(_instance.cSetScene("Home"));
                        break;

                    case ActiveScene.shop:
                        if (SceneManager.GetActiveScene().name != "Shop")
                            _instance.StartCoroutine(_instance.cSetScene("Shop"));
                        break;

                    case ActiveScene.levelselection:
                        if (SceneManager.GetActiveScene().name != "Levelselection")
                            _instance.StartCoroutine(_instance.cSetScene("Levelselection"));
                        break;

                    case ActiveScene.tutorial:
                        if (SceneManager.GetActiveScene().name != "Tutorial")
                            _instance.StartCoroutine(_instance.cSetScene("Tutorial"));
                        break;

                    case ActiveScene.gopro:
                        if (SceneManager.GetActiveScene().name != "GoPro")
                            _instance.StartCoroutine(_instance.cSetScene("GoPro"));
                        break;

                    case ActiveScene.game:
                        if (SceneManager.GetActiveScene().name != "Game")
                            _instance.StartCoroutine(_instance.cSetScene("Game"));
                        break;

                    case ActiveScene.settings:
                        if (SceneManager.GetActiveScene().name != "Settings")
                            _instance.StartCoroutine(_instance.cSetScene("Settings"));
                        break;

                    case ActiveScene.credits:
                        if (SceneManager.GetActiveScene().name != "Credits")
                            _instance.StartCoroutine(_instance.cSetScene("Credits"));
                        break;

                    case ActiveScene.achievements:
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
                                SetScene(ActiveScene.home);
                            }
                        });
#endif
                        }
                        break;

                    case ActiveScene.editor:
                        //if (ProgressManager.GetProgress().proVersion)
                        //{
                        if (SceneManager.GetActiveScene().name != "EditorSelection" && SceneManager.GetActiveScene().name != "Game")
                            _instance.StartCoroutine(_instance.cSetScene("EditorSelection"));
                        else
                            _instance.StartCoroutine(_instance.cSetScene("Leveleditor"));
                        // }
                        break;
                }
            }
        }

        private IEnumerator cSetScene(string sceneName)
        {
            Scene oldScene = SceneManager.GetActiveScene();
            Scene newScene = SceneManager.GetSceneByName(sceneName);
            AsyncOperation ao = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
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
            else if (sceneName == "Leveleditor" && Game.gameType == Game.GameType.testing)
            {
                Debug.Log("SetSCENE EDTTOROREOEOEO ---------------------");
                adCooldownCounter++;
                if (adCooldownCounter % adEveryTest == 0 || adInQue)
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
            switchingScene = false;
            //SceneManager.UnloadSceneAsync(oldScene);
            //SceneManager.SetActiveScene(newScene);

            //Debug.Log("LOOOOOOOOOOOOOOOOOOOl");

            //Scene nextScene = SceneManager.GetSceneByName(sceneName);
            //if (nextScene.IsValid())
            //{
            //    //yield return new WaitForEndOfFrame();
            //
            //    SceneManager.SetActiveScene(nextScene);
            //}

            //SceneManager.SetActiveScene(newScene);

            //SceneManager.UnloadSceneAsync(oldScene.buildIndex);

            //SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));

            yield break;
        }

        private IEnumerator cSetSceneAchievement()
        {
            yield return new WaitForSeconds(sceneSwitchDelay);

            // show default achievements ui, placeholder
            Social.ShowAchievementsUI();
            SetScene(ActiveScene.home);

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
                    if (currentScene == ActiveScene.home)
                        Application.Quit();
                    else if (currentScene == ActiveScene.game)
                        SetScene(ActiveScene.levelselection);
                    else
                        SetScene(ActiveScene.home);
                }
#if UNITY_EDITOR
                else if (Input.GetKeyDown(KeyCode.Return))
                {
                    if (currentScene == ActiveScene.levelselection && UILevelselectionManager._instance != null)
                    {
                        UILevelselectionManager._instance.PlayLevel();
                    }
                }
#endif
            }
        }

        public static string zoneId = "rewardedVideo";

        public static bool ShowRewardedVideo()
        {
            ShowOptions options = new ShowOptions();
            options.resultCallback = HandleShowResult;
            Advertisement.Show(zoneId, options);
            return true;
        }

        //banner: ca-app-pub-2906510767249222/2074269594
        //interstitial: ca-app-pub-2906510767249222/2353471190
        // rewarded video: ca-app-pub-2906510767249222/6294034790
        public static void RequestInterstitial()
        {
            if (!ProgressManager.GetProgress().proVersion)
            {
                Admob.Instance().loadInterstitial();
            }
        }

        public static bool ShowInterstitial()
        {
            if (!ProgressManager.GetProgress().proVersion)
            {
                Debug.Log("SHOWINTERSTITIAL");
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
            }
            return false;
        }

        private static void HandleShowResult(ShowResult result)
        {
            switch (result)
            {
                case ShowResult.Finished:
                    Debug.Log("Video completed. User rewarded some credits.");
                    ProgressManager.GetProgress().starsOwned += 1;
                    break;

                case ShowResult.Skipped:
                    Debug.LogWarning("Video was skipped.");
                    break;

                case ShowResult.Failed:
                    Debug.LogError("Video failed to show.");
                    break;
            }
        }

        public class StartupEvent : UnityEvent { }

        public class SceneChangeEvent : UnityEvent<ActiveScene> { }

        public class AchievementEvent : UnityEvent { }
    }
}