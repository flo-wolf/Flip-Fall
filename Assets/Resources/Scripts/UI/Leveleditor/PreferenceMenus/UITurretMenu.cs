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
    public Slider fireRateSlider;

    private static Turret turret;

    // Use this for initialization
    private void Start()
    {
        _instance = this;
        onPreferenceChange.AddListener(PreferenceChanged);
        rotationSlider.value = (int)LevelEditor.selectedObject.transform.rotation.eulerAngles.z;

        turret = LevelEditor.selectedObject.GetComponent<Turret>();
        if (turret != null)
        {
            fireRateSlider.value = turret.shotDelay;
        }
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

        // set the selectet object's rotation to the angle given by the slider
        Vector3 lookPos = transform.position;
        lookPos.y = 0;

        Quaternion rotation = LevelEditor.selectedObject.transform.rotation;
        rotation = Quaternion.Euler(0, 0, v);
        LevelEditor.selectedObject.transform.rotation = rotation;

        //transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * damping);
    }

    public void FireRateSliderChanged(Slider s)
    {
        float v = s.value;

        if (turret != null)
            turret.shotDelay = v;
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