using FlipFall.Editor;
using FlipFall.LevelObjects;
using FlipFall.Levels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBouncerMenu : UIPreferenceMenu
{
    public static UIBouncerMenu _instance;
    private static bool started = false;

    public LevelObject.ObjectType objectType = LevelObject.ObjectType.speedStrip;
    private static LevelData editData;
    public Slider rotationSlider;
    public Slider bounceForceSlider;
    public Slider widthSlider;

    private static Bouncer bouncer;

    private void Start()
    {
        _instance = this;
        onPreferenceChange.AddListener(PreferenceChanged);
        started = false;

        if (LevelEditor.selectedObject != null && LevelEditor.selectedObject.objectType == objectType && rotationSlider.IsActive() && bounceForceSlider.IsActive())
        {
            rotationSlider.value = (int)LevelEditor.selectedObject.transform.rotation.eulerAngles.z;
            bouncer = LevelEditor.selectedObject.GetComponent<Bouncer>();

            if (bouncer != null)
            {
                widthSlider.value = bouncer.width;
                bounceForceSlider.value = bouncer.bounciness;
            }
        }
    }

    public static void Activate(LevelData levelData)
    {
        started = false;
        if (_instance != null)
        {
            _instance.rotationSlider.value = (int)LevelEditor.selectedObject.transform.rotation.eulerAngles.z;
            bouncer = LevelEditor.selectedObject.GetComponent<Bouncer>();

            if (bouncer != null)
            {
                _instance.widthSlider.value = bouncer.width;
                _instance.bounceForceSlider.value = bouncer.bounciness;
            }
        }
        editData = levelData;
    }

    public void RotationSliderChanged(Slider s)
    {
        if (s.IsActive() && started)
        {
            float v = s.value;

            // snap the slider to 22,5 steps
            float lowerThanThis = 11.25F;
            for (float i = 0; i <= 360; i += 22.5F)
            {
                if (v <= lowerThanThis)
                {
                    v = i;
                    break;
                }
                else
                    lowerThanThis = i + 11.25F;
            }

            // set the selectet object's rotation to the angle given by the slider
            Vector3 lookPos = transform.position;
            lookPos.y = 0;

            Quaternion rotation = LevelEditor.selectedObject.transform.rotation;
            rotation = Quaternion.Euler(0, 0, v);
            LevelEditor.selectedObject.transform.rotation = rotation;
        }
        started = true;
        //transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * damping);
    }

    public void BounceForceSliderChanged(Slider s)
    {
        if (started)
        {
            if (bouncer != null)
                bouncer.SetBounciness((int)Mathf.Round(s.value));
        }
        started = true;
    }

    public void WidthSliderChanged(Slider s)
    {
        if (started)
        {
            bouncer.SetWidth((int)Mathf.Round(s.value));
        }
        started = true;
    }

    private void PreferenceChanged(UIPreferenceMenu menu)
    {
        if (menu != _instance && gameObject.activeSelf == true)
        {
            gameObject.SetActive(false);
            started = false;
        }
    }
}