using System.Collections;
using UnityEngine;

/// <summary>
/// UI Elements displayed right after the player dies. They disappear as soon as the scoreScreen is displayed.
/// </summary>

public class UIScoreScreen : MonoBehaviour
{
    public static UIScoreScreen _instance;

    private void Awake()
    {
        _instance = this;
    }

    public static void Show()
    {
        _instance.gameObject.SetActive(true);
    }

    public static void Hide()
    {
        _instance.gameObject.SetActive(true);
    }
}