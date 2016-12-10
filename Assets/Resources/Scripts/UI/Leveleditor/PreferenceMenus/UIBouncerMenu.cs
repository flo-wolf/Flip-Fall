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

    private static Bouncer bouncer;

    private void Start()
    {
        _instance = this;
        onPreferenceChange.AddListener(PreferenceChanged);

        rotationSlider.value = (int)LevelEditor.selectedObject.transform.rotation.eulerAngles.z;

        bouncer = LevelEditor.selectedObject.GetComponent<Bouncer>();

        if (bouncer != null)
        {
            bounceForceSlider.value = bouncer.forceAdd;
        }

        started = true;
    }

    private void OnDisable()
    {
        started = false;
    }

    public static void Activate(LevelData levelData)
    {
        if (_instance != null)
        {
            _instance.rotationSlider.value = (int)LevelEditor.selectedObject.transform.rotation.eulerAngles.z;
            bouncer = LevelEditor.selectedObject.GetComponent<Bouncer>();

            if (bouncer != null)
            {
                _instance.bounceForceSlider.value = bouncer.forceAdd;
            }
        }
        editData = levelData;
        started = true;
    }

    public void RotationSliderChanged(Slider s)
    {
        if (started)
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
        //transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * damping);
    }

    public void BounceForceSliderChanged(Slider s)
    {
        int v = (int)s.value;

        if (bouncer != null)
            bouncer.forceAdd = v;
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