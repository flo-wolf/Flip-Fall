using FlipFall.Editor;
using FlipFall.LevelObjects;
using FlipFall.Levels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITurretMenu : UIPreferenceMenu
{
    public static UITurretMenu _instance;
    private static bool started = false;

    public LevelObject.ObjectType objectType = LevelObject.ObjectType.turret;
    private static LevelData editData;
    public Slider rotationSlider;

    // Use this for initialization
    private void Start()
    {
        _instance = this;
        onPreferenceChange.AddListener(PreferenceChanged);
        rotationSlider.value = (int)LevelEditor.selectedObject.transform.rotation.eulerAngles.z;
    }

    public static void Activate(LevelData levelData)
    {
        editData = levelData;
        started = true;
    }

    public void RotationSliderChanged(Slider s)
    {
        float v = s.value;

        // snap the slider to 22,5 steps
        float lowerThanThis = 11.25F;
        for (float i = 0; i <= 360; i += 22.5F)
        {
            if (v < lowerThanThis)
            {
                v = i;
                break;
            }
            else
                lowerThanThis = i + 11.25F;
        }

        //// value needs snapping
        //if (v >= 0 && v <= 360 && v % (22.5F) != 0)
        //{
        //    if (v < 11.25)
        //        v = 0;
        //    else if (v < 37.5)
        //        v = 22.5F;
        //    else if (v < 56.25)
        //        v = 45;
        //    else if (v < 75)
        //        v = 67.5F;
        //    else if (v < 112.5)
        //        v = 90F;
        //    else if (v < 131.25)
        //        v = 112.5F;
        //    else if (v < 150)
        //        v = 135F;
        //    else if (v < 168.75)
        //        v = 157.5F;
        //    else if (v < 187.5)
        //        v = 180;
        //    else if (v < 37.5)
        //        v = 202.5F;
        //    else if (v < 37.5)
        //        v = 225F;
        //    else if (v < 37.5)
        //        v = 247.5F;
        //    else if (v < 37.5)
        //        v = 270F;
        //    else if (v < 37.5)
        //        v = 315F;
        //    else if (v < 37.5)
        //        v = 337.5F;
        //    else if (v < 37.5)
        //        v = 360F;
        //}

        // set the selectet object's rotation to the angle given by the slider
        Vector3 lookPos = transform.position;
        lookPos.y = 0;

        Quaternion rotation = LevelEditor.selectedObject.transform.rotation;
        rotation = Quaternion.Euler(0, 0, v);
        LevelEditor.selectedObject.transform.rotation = rotation;

        //transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * damping);
    }

    private void PreferenceChanged(UIPreferenceMenu menu)
    {
        if (menu != _instance && gameObject.activeSelf == true)
        {
            gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    private void Update()
    {
    }
}