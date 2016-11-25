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
        Vector3 lookPos = transform.position;
        lookPos.y = 0;

        Quaternion rotation = LevelEditor.selectedObject.transform.rotation;
        rotation = Quaternion.Euler(0, 0, s.value);
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