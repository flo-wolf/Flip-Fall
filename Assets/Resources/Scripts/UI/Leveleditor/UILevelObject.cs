using FlipFall.LevelObjects;
using FlipFall.Progress;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace FlipFall.UI
{
    public class UILevelObject : MonoBehaviour
    {
        public static ItemSelectEvent onItemSelect = new ItemSelectEvent();

        public class ItemSelectEvent : UnityEvent<UILevelObject> { }

        public LevelObject.ObjectType objectType;
        public Toggle selectToogle;
        public Text amountText;

        private int amount = 0;

        // Use this for initialization
        private void Start()
        {
            UpdateAmount();
            Inventory.onInventoryChange.AddListener(UpdateAmount);
            onItemSelect.AddListener(OnItemSelect);
        }

        private void OnItemSelect(UILevelObject obj)
        {
            if (obj != this)
            {
                selectToogle.isOn = false;
            }
        }

        private void UpdateAmount()
        {
            amount = ProgressManager.GetProgress().unlocks.inventory.GetAmount(objectType);
            if (amount > 0)
            {
                selectToogle.interactable = true;
                amountText.text = amount.ToString();
            }
            if (amount <= 0)
            {
                amountText.text = "0";
                selectToogle.interactable = false;
                // change icon to gray if amount is zero
            }
        }

        // this toogle got clicked
        public void SelectClick(Toggle t)
        {
            if (amount > 0 && t.isOn)
            {
                t.isOn = true;
                //selectToogle.interactable = false;
                onItemSelect.Invoke(this);
            }
            //else
            //{
            //    t.isOn = false;
            //    selectToogle.interactable = false;
            //}
        }
    }
}