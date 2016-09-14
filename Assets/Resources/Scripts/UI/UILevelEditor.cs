using Impulse.Levels;
using Impulse.Progress;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Impulse.UI
{
    public class UILevelEditor : MonoBehaviour
    {
        public Text levelInfoText;
        public Dropdown dropdown;
        public Level editorLevel;

        #region Public Methods

        public void Start()
        {
            LoadLevels();

            //listen for level selection changes in the dropdown menu
            dropdown.onValueChanged.AddListener(delegate
            {
            });
        }

        public void LoadLevels()
        {
            //UpdateDropdown();
        }

        public void SaveLevels()
        {
            //LevelManager.save();
        }

        public void NewLevel()
        {
            //UpdateDropdown();
        }

        public void RemoveLevel()
        {
            //UpdateDropdown();
        }

        /*
        public void UpdateDropdown()
        {
            List<Dropdown.OptionData> optionDataList = new List<Dropdown.OptionData>();
            foreach (Level _level in LevelManager.loadedLevels)
            {
                Dropdown.OptionData od = new Dropdown.OptionData();
                od.text = _level.id.ToString();
                optionDataList.Add(od);
            }
            dropdown.ClearOptions();
            if (optionDataList.Count != 0)
            {
                dropdown.AddOptions(optionDataList);
                dropdown.value = LevelManager.activeLevel.id;
            }
            dropdown.RefreshShownValue();
        }
        */

        #endregion Public Methods
    }
}