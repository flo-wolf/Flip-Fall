using FlipFall.Theme;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamColorSetter : MonoBehaviour
{
    public static CamColorSetter _instance;

    // Use this for initialization
    private void Start()
    {
        _instance = this;
        BgColorUpdate();
    }

    // Update is called once per frame
    public static void BgColorUpdate()
    {
        if (_instance != null)
            _instance.GetComponent<Camera>().backgroundColor = ThemeManager.theme.backgorundColor;
    }
}