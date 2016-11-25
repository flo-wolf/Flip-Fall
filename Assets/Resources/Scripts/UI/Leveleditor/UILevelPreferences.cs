using FlipFall.Editor;
using FlipFall.LevelObjects;
using FlipFall.Levels;
using FlipFall.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Colotrils what preference menus get to be shown and handles the preference saving/discarding
/// </summary>

public class UILevelPreferences : MonoBehaviour
{
    public static UILevelPreferences _instance;
    public static LevelData preferenceData;
    public static bool menuOpen;
    private static Animator animator;

    public UIPreferenceMenu spawnMenu;
    public UIPreferenceMenu finishMenu;
    public UIPreferenceMenu turretMenu;
    public UIPreferenceMenu portalMenu;
    public UIPreferenceMenu spikeMenu;
    public UIPreferenceMenu attractorMenu;
    public UIPreferenceMenu speedStripMenu;
    public UIPreferenceMenu openedMenu;

    private void Start()
    {
        menuOpen = false;
        if (_instance == null)
        {
            _instance = this;
            animator = GetComponent<Animator>();
            preferenceData = LevelEditor.CreateLevelData();
        }
    }

    public static void DisableAll()
    {
        if (_instance != null)
        {
            animator.SetTrigger("hideAll");
        }
    }

    public static UIPreferenceMenu ShowMenu()
    {
        menuOpen = true;
        LevelObject.ObjectType type = LevelEditor.selectedObject.objectType;
        Debug.Log("type " + type + " - " + _instance);
        if (_instance != null)
        {
            UndoManager.AddUndoPoint();
            switch (type)
            {
                case LevelObject.ObjectType.turret:
                    UITurretMenu.Activate(LevelEditor.CreateLevelData());
                    UIPreferenceMenu.onPreferenceChange.Invoke(_instance.turretMenu);
                    animator.SetTrigger("showTurret");
                    Debug.Log("treeehehee " + _instance.turretMenu);
                    return _instance.turretMenu;

                case LevelObject.ObjectType.portal:
                    return _instance.portalMenu;

                case LevelObject.ObjectType.attractor:
                    return _instance.attractorMenu;

                case LevelObject.ObjectType.speedStrip:
                    return _instance.speedStripMenu;

                case LevelObject.ObjectType.finish:
                    return _instance.finishMenu;

                case LevelObject.ObjectType.spawn:
                    return _instance.spawnMenu;
            }
            Debug.Log("HERE");
        }
        return null;
    }

    public void LeaveUnsaved()
    {
        animator.SetTrigger("hideAll");
        menuOpen = false;
        UndoManager.Undo();
    }

    public void LeaveSaved()
    {
        menuOpen = false;
        animator.SetTrigger("hideAll");
    }

    public UIPreferenceMenu HideMenu()
    {
        menuOpen = false;
        if (_instance != null)
        {
            switch (LevelEditor.selectedObject.objectType)
            {
                case LevelObject.ObjectType.turret:
                    turretMenu.gameObject.SetActive(false);
                    //_instance.animator.SetTrigger("showTriggerMenu");
                    return turretMenu;

                case LevelObject.ObjectType.portal:
                    return portalMenu;

                case LevelObject.ObjectType.attractor:
                    return attractorMenu;

                case LevelObject.ObjectType.speedStrip:
                    return speedStripMenu;

                case LevelObject.ObjectType.finish:
                    return finishMenu;

                case LevelObject.ObjectType.spawn:
                    return spawnMenu;
            }
        }
        return null;
    }

    // obsolete
    public UIPreferenceMenu GetPreference(LevelObject.ObjectType type)
    {
        UIPreferenceMenu menu = null;
        switch (type)
        {
            case LevelObject.ObjectType.turret:
                return turretMenu;

            case LevelObject.ObjectType.portal:
                return portalMenu;

            case LevelObject.ObjectType.attractor:
                return attractorMenu;

            case LevelObject.ObjectType.speedStrip:
                return speedStripMenu;

            case LevelObject.ObjectType.finish:
                return finishMenu;

            case LevelObject.ObjectType.spawn:
                return spawnMenu;
        }
        return menu;
    }
}