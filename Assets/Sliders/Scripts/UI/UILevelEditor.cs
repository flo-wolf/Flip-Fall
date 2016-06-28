using Impulse.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Impulse.UI
{
    public class UILevelEditor : MonoBehaviour
    {
        public static List<LevelDataModel> levels = new List<LevelDataModel>();
        public Text levelInfoText;
        public Dropdown dropdown;

        private LevelDataModel level;

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
            Debug.Log("selected: " + target.value);
            level = levels[target.value];
            levelInfoText.text = "Level - " + level.id.ToString();
        }

        public void LoadLevels()
        {
            List<LevelDataModel> loadedLevels = new List<LevelDataModel>();
            levels = loadedLevels;
            UpdateDropdown();
        }

        public void SaveLevels()
        {
        }

        public void NewLevel()
        {
            level = new LevelDataModel();

            int newID = levels.Count;
            if (levels.Any(x => x.id != newID))
            {
                level.id = levels[levels.Count - 1].id + 1;
            }
            else
            {
                level.id = levels.Count + 1;
            }
            levels.Add(level);
            UpdateDropdown();
        }

        public void RemoveLevel()
        {
            if (levels.Contains(level))
            {
                int newID = levels.Count;
                if (newID > 0)
                {
                    Debug.Log(levels[level.id - 1].id);
                    levels.Remove(level);
                    level = levels[levels.Count - 1];
                }
                else if (levels.Count - 1 <= 0)
                {
                    levels.Remove(level);
                    NewLevel();
                }
            }
            UpdateDropdown();
        }

        public LevelDataModel GetLevel(int _id)
        {
            var model = levels[_id];
            return model;
        }

        public void SetLevel(int _id, LevelDataModel _level)
        {
            //Add a check if the id can be allocated to a level, if not, dont create a progress entry for a level that doesnt exist
            if (levels.Any(x => x.id == _id))
            {
                var model = levels.FirstOrDefault(x => x.id == _id);
                model = _level;
                model.updated = DateTime.UtcNow;
            }
            else
            {
                levels.Add(_level);
            }
        }

        public void UpdateDropdown()
        {
            List<Dropdown.OptionData> optionDataList = new List<Dropdown.OptionData>();
            foreach (LevelDataModel _level in levels)
            {
                Dropdown.OptionData od = new Dropdown.OptionData();
                od.text = _level.id.ToString();
                optionDataList.Add(od);
            }
            dropdown.ClearOptions();
            if (optionDataList.Count != 0)
            {
                dropdown.AddOptions(optionDataList);
                dropdown.value = level.id;
            }
            dropdown.RefreshShownValue();
        }

        public void RemoveLevelAt(int _id)
        {
            if (levels.Any(x => x.id == _id))
            {
                var model = levels.FirstOrDefault(x => x.id == _id);
                levels.Remove(model);
            }
        }

        public void SetTime(Double time)
        {
            level.SetTime(time);
        }

        public void SetName(String name)
        {
            level.SetName(name);
        }

        #endregion Public Methods
    }
}