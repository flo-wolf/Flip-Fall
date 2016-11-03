﻿using FlipFall.Editor;
using Impulse;
using Impulse.Audio;
using Impulse.Levels;
using Impulse.Progress;
using Impulse.Theme;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// The UI version of a product sold in the Shop Menu.
/// Is always in sync with a ProductInfo, which gets stored in the current progres with the same id.
/// </summary>

[SerializeField]
public class UIEditorLevel : MonoBehaviour
{
    /// <summary>
    /// Can you buy this multiple times (building-tools) or just once and forever?
    /// </summary>
    public enum LevelType { recieved, created };

    public enum LevelStatus { playable, nonPlayable };

    public Level level;

    [Header("UIEditorLevel Components")]
    public Text titleText;
    public Button editButton;
    public Button playButton;

    // prevent changes to toogle.isOn via script from invoking the tooglechange call and executing the Equip mothod
    private bool unvalidToogleCall = false;

    // corresponding product information data, stored in progress save-file
    private ProductInfo productInfo;

    public void Edit()
    {
        LevelEditor.editLevel = level;
        Main.SetScene(Main.Scene.editor);
    }

    // set texts to fit inspector variables
    public void UpdateTexts()
    {
        titleText.text = level.title;
        if (level.title == "")
        {
            titleText.text = "Custom Level";
        }
    }

    // set toggles to fit values gathered from the progress file
    public void UpdateButtons()
    {
        if (IsPlayable())
        {
            //playButton.gameObject.SetActive(true);
        }
    }

    // the level should be playable in theory, since it contains all elements a level needs for playing
    // ... check if impossible by simulating it by pathfinding
    private bool IsPlayable()
    {
        bool hasSpawnOnMoveArea = false;
        bool hasFinishOnMoveArea = false;

        if (level.GetComponentInChildren<Spawn>(true))
        {
            hasSpawnOnMoveArea = true;
        }
        if (level.GetComponentInChildren<Finish>(true))
        {
            hasFinishOnMoveArea = true;
        }

        return hasSpawnOnMoveArea && hasFinishOnMoveArea;
    }
}