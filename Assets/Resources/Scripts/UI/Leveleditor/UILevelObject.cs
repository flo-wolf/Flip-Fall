using FlipFall.Audio;
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
        public Image lockedImage;
        public Animation anim;

        private bool owned = false;

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
            owned = ProgressManager.GetProgress().unlocks.inventory.Contains(objectType);
            Debug.Log("owned " + objectType + "? " + owned);
            if (owned)
            {
                selectToogle.interactable = true;
                lockedImage.gameObject.SetActive(false);
            }
            else
            {
                lockedImage.gameObject.SetActive(true);
                selectToogle.interactable = false;
                if (selectToogle.isOn)
                    selectToogle.isOn = false;
            }
        }

        // this toogle got changed
        public void SelectClick(Toggle t)
        {
            if (!UIObjectPreferences.menuOpen)
            {
                if (owned && t.isOn)
                {
                    t.isOn = true;
                    //selectToogle.interactable = false;
                    LevelEditor.editorMode = LevelEditor.EditorMode.place;
                    SoundManager.PlayLightWobble();
                    currentSelectedObject = this;
                    onItemSelect.Invoke(this);
                }
                else if (!t.isOn && currentSelectedObject.selectToogle == t)
                {
                    if (LevelEditor.editorMode == LevelEditor.EditorMode.place)
                        SoundManager.PlayLightWobble(0.6F);
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

        private void FadeOut(Main.ActiveScene s)
        {
            print("fadeout");
            anim["fadeIn"].speed = -1.0f;
            anim.Play("fadeIn");
        }
    }
}