using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Controls scene switches, preserves data upon scene switching
/// </summary>

public class Main : MonoBehaviour
{
    public enum Scene { startup, welcome, home, levelselection, game, settings, editor, shop }
    public float sceneSwitchDuration = 0.5F;

    //public

    private void Start()
    {
        DontDestroyOnLoad(this);
    }

    public void SetScene(Scene newScene)
    {
        switch (newScene)
        {
            case Scene.startup:
                SceneManager.LoadScene("Startup");
                break;

            case Scene.welcome:
                SceneManager.LoadScene("Welcome");
                break;

            case Scene.home:
                SceneManager.LoadScene("Home");
                break;

            case Scene.levelselection:
                SceneManager.LoadScene("Levelselection");
                break;

            case Scene.game:
                SceneManager.LoadScene("Game");
                break;
        }
    }

    private void Update()
    {
    }
}