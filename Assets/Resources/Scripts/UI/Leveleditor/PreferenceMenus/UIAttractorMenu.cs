using FlipFall.Editor;
using FlipFall.LevelObjects;
using FlipFall.Levels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIAttractorMenu : UIPreferenceMenu
{
    public static UIAttractorMenu _instance;
    private static bool started = false;

    public LevelObject.ObjectType objectType = LevelObject.ObjectType.speedStrip;
    private static LevelData editData;
    public Slider radiusSlider;
    public Slider pullSlider;

    private static Attractor attractor;

    private void Start()
    {
        _instance = this;
        onPreferenceChange.AddListener(PreferenceChanged);

        radiusSlider.value = (int)LevelEditor.selectedObject.transform.lossyScale.x / 2;

        attractor = LevelEditor.selectedObject.gameObject.GetComponent<Attractor>();
        if (attractor != null)
            pullSlider.value = attractor.maxPullForce;
    }

    public static void Activate(LevelData levelData)
    {
        editData = levelData;
        started = true;
        attractor = LevelEditor.selectedObject.gameObject.GetComponent<Attractor>();
    }

    // change the attractors size based on the slider input
    public void RadiusSliderChanged(Slider s)
    {
        float v = s.value;

        Debug.Log(v);

        // snap the slider to 5-unit-steps
        float lowerThanThis = 2.5F;
        for (float i = 0; i <= 360; i += 5F)
        {
            if (v < lowerThanThis)
            {
                v = i;
                break;
            }
            else
                lowerThanThis = i + 2.5F;
        }

        // set the selectet objects size to fit the given radius
        if (attractor != null)
        {
            attractor.pullRadius = v;
            attractor.SetScale();
        }

        // LevelEditor.selectedObject.transform.lossyScale.Set(scale.x, scale.y, scale.z);
    }

    // change pull strength based on slider value ( s.value = ~2000)
    public void PullSliderChanged(Slider s)
    {
        float v = s.value;
        if (attractor != null)
            attractor.maxPullForce = v;
    }

    private void PreferenceChanged(UIPreferenceMenu menu)
    {
        if (menu != _instance && gameObject.activeSelf == true)
        {
            gameObject.SetActive(false);
        }
    }
}