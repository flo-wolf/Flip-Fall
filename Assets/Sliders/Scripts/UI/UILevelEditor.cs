using Sliders.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Sliders.UI
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
                dropdownValueChangedHandler(dropdown);
            });
        }

        private void dropdownValueChangedHandler(Dropdown target)
        {
            LevelManager.SetLevel(target.value);
        }

        public void LoadLevels()
        {
            LevelManager.PlaceActiveLevel();
            UpdateDropdown();
        }

        public void SaveLevels()
        {
            //LevelManager.save();
        }

        public void NewLevel()
        {
            UpdateDropdown();
        }

        public void RemoveLevel()
        {
            UpdateDropdown();
        }

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

        #endregion Public Methods
    }
}