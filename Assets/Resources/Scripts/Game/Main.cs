using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/// <summary>
/// Main Application Controller. Switches scenes upon calling SetScene() after a delay, giving other classes a timeframe
/// to save or initiate fade-in and fade-outs, since the event is called without delay.
/// </summary>

namespace Impulse
{
    public class Main : MonoBehaviour
    {
        public enum Scene { startup, welcome, home, levelselection, game, settings, editor, shop }
        public float sceneSwitchDuration = 0.5F;

        public static Main _instance;

        public SceneChangeEvent onSceneChange = new SceneChangeEvent();

        private void Awake()
        {
            DontDestroyOnLoad(this);
            _instance = this;
        }

        public static void SetScene(Scene newScene)
        {
            _instance.onSceneChange.Invoke(newScene);
            switch (newScene)
            {
                case Scene.startup:
                    if (SceneManager.GetActiveScene().name != "Startup")
                        _instance.StartCoroutine(_instance.cSetScene("Startup"));
                    break;

                case Scene.welcome:
                    if (SceneManager.GetActiveScene().name != "Welcome")
                        _instance.StartCoroutine(_instance.cSetScene("Welcome"));
                    break;

                case Scene.home:
                    if (SceneManager.GetActiveScene().name != "Home")
                        _instance.StartCoroutine(_instance.cSetScene("Home"));
                    break;

                case Scene.levelselection:
                    if (SceneManager.GetActiveScene().name != "Levelselection")
                        _instance.StartCoroutine(_instance.cSetScene("Levelselection"));
                    break;

                case Scene.game:
                    if (SceneManager.GetActiveScene().name != "Game")
                        _instance.StartCoroutine(_instance.cSetScene("Game"));
                    break;
            }
        }

        private IEnumerator cSetScene(string sceneName)
        {
            yield return new WaitForSeconds(sceneSwitchDuration);
            SceneManager.LoadScene(sceneName);
            yield break;
        }

        public class SceneChangeEvent : UnityEvent<Scene> { }
    }
}