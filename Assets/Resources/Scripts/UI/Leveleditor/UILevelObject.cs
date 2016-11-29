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

        public static UILevelObject currentSelectedObject;

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
                if (selectToogle.isOn)
                {
                    selectToogle.isOn = false;
                }
                // change icon to gray if amount is zero
            }
        }

        // this toogle got clicked
        public void SelectClick(Toggle t)
        {
            if (!UIObjectlPreferences.menuOpen)
            {
                if (amount > 0 && t.isOn)
                {
                    t.isOn = true;
                    //selectToogle.interactable = false;
                    LevelEditor.editorMode = LevelEditor.EditorMode.place;
                    currentSelectedObject = this;
                    onItemSelect.Invoke(this);
                }
                else if (!t.isOn && currentSelectedObject.selectToogle == t)
                {
                    LevelEditor.editorMode = LevelEditor.EditorMode.select;
                }
                Debug.Log("editormode: " + LevelEditor.editorMode);
            }
        }

        private void FadeIn()
        {
            anim["fadeIn"].speed = 1.0f;
            anim.Play("fadeIn");
        }

        private void FadeOut(Main.Scene s)
        {
            print("fadeout");
            anim["fadeIn"].speed = -1.0f;
            anim.Play("fadeIn");
        }
    }
}