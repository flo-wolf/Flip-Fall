using FlipFall.Editor;
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
        public Animation anim;

        private int amount = 0;

        // Use this for initialization
        private void Start()
        {
            FadeIn();
            UpdateAmount();
            Inventory.onInventoryChange.AddListener(UpdateAmount);
            onItemSelect.AddListener(OnItemSelect);
            Main.onSceneChange.AddListener(FadeOut);
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

        public void Place()
        {
            amount = ProgressManager.GetProgress().unlocks.inventory.GetAmount(objectType);
            if (amount > 0)
            {
                ProgressManager.GetProgress().unlocks.inventory.Add(objectType, amount - 1);
            }
            UpdateAmount();
        }

        // this toogle got clicked
        public void SelectClick(Toggle t)
        {
            if (amount > 0 && t.isOn)
            {
                t.isOn = true;
                //selectToogle.interactable = false;
                LevelEditor.editorMode = LevelEditor.EditorMode.place;
                onItemSelect.Invoke(this);
            }
            else if (!t.isOn)
            {
                LevelEditor.editorMode = LevelEditor.EditorMode.select;
            }
        }

        private void FadeIn()
        {
            anim["fadeIn"].speed = 1.0f;
            anim.Play("fadeIn");
        }

        private void FadeOut(Main.Scene s)
        {
            anim["fadeIn"].speed = -1.0f;
            anim.Play("fadeIn");
        }
    }
}