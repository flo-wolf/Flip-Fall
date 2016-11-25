using FlipFall.LevelObjects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Base Class to define Preference Menues, which inherit from this class (UITurretMenu etc.)
/// Takes the current select object and modifies its properties
/// </summary>
public class UIPreferenceMenu : MonoBehaviour
{
    public static PreferenceActivateEvent onPreferenceChange = new PreferenceActivateEvent();

    public class PreferenceActivateEvent : UnityEvent<UIPreferenceMenu> { }

    // Use this for initialization
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }
}