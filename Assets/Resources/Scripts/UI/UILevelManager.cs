using System.Collections;
using UnityEngine;

public class UILevelManager : MonoBehaviour
{
    public static UILevelManager _instance;

    // Use this for initialization
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
        _instance.gameObject.SetActive(false);
    }
}