using FlipFall.Editor;
using FlipFall.LevelObjects;
using FlipFall.Levels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPortalMenu : UIPreferenceMenu
{
    public static UIPortalMenu _instance;
    private static bool started = false;

    public LevelObject.ObjectType objectType = LevelObject.ObjectType.portal;
    private static LevelData editData;

    private static Portal portal;
    private static Portal newLinkPortal;

    private void Start()
    {
        _instance = this;
        onPreferenceChange.AddListener(PreferenceChanged);
    }

    public static void Activate(LevelData levelData)
    {
        editData = levelData;
        started = true;
        portal = LevelEditor.selectedObject.gameObject.GetComponent<Portal>();

        if (portal.linkedPortal != null)
            UIObjectPreferences.PortalHasLink(true);
        else
            UIObjectPreferences.PortalHasLink(false);
    }

    // prepare for selecting a portal
    public void SelectPortalButton()
    {
        UIObjectPreferences.SelectPortalLink();
    }

    // the portal to be linked has been selected
    public static void SelectLinkPortal(Portal p)
    {
        newLinkPortal = p;
        portal.Link(p);
        UIObjectPreferences.PortalLinkSelected();
    }

    private void PreferenceChanged(UIPreferenceMenu menu)
    {
        if (menu != _instance && gameObject.activeSelf == true)
        {
            gameObject.SetActive(false);
        }
    }
}