using FlipFall.Background;
using FlipFall.Progress;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Manages the color theme of the game.
/// </summary>

namespace FlipFall.Theme
{
    public class ThemeManager : MonoBehaviour
    {
        public static ThemeManager _instance;

        public enum Skin { unset, burgundy, forrest, rainbow, sunset, silver, gold, darkness, ocean, toxic, hailstone, cherry, riptide, coconut }

        //skin currently active
        public static Skin skin = Skin.unset;
        public static Theme theme = new Theme();
        public Skin defaultSkin;

        public GameObject themePrefab;

        //skin prefab references
        public Theme[] themes;

        private static bool started = false;

        private void Start()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
                return;
            }
            _instance = this;

            if (!started)
            {
                SetSkin(GetLastSkin());
                //SetSkin(ProgressManager.GetProgress().unlocks.currentSkin);
            }
            DontDestroyOnLoad(this);
            started = true;
        }

        //private void OnLevelWasLoaded()
        //{
        //    GameObject bgCam = GameObject.FindGameObjectWithTag("BackgroundCam");
        //    bgCam.GetComponent<Camera>().backgroundColor = theme.backgorundColor;
        //}

        // get last skin from progress
        public Skin GetLastSkin()
        {
            Skin lastSkin = ProgressManager.GetProgress().unlocks.currentSkin;
            if (lastSkin == Skin.unset)
            {
                lastSkin = defaultSkin;
                Debug.Log("def: " + defaultSkin);
            }
            return lastSkin;
        }

        // checks if the skin is equipable/unlocked
        public static bool IsUnlocked(Skin newSkin)
        {
            if (ProgressManager.GetProgress().unlocks.unlockedThemes.Any(x => x == newSkin) || newSkin == _instance.defaultSkin)
                return true;
            return false;
        }

        public static void SetSkin(Skin newSkin)
        {
            Debug.Log("setskin");
            if (IsUnlocked(newSkin) && theme.horizonSkin != newSkin)
            {
                GameObject go = null;

                foreach (Theme t in _instance.themes)
                {
                    if (t.horizonSkin == newSkin)
                    {
                        ProgressManager.GetProgress().unlocks.currentSkin = newSkin;
                        skin = newSkin;

                        //Destroy(theme.gameObject);
                        theme = new Theme();

                        foreach (Transform child in _instance.transform)
                        {
                            Destroy(child.gameObject);
                        }
                        go = Instantiate(_instance.themePrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
                        go.gameObject.transform.parent = _instance.transform;
                        go.gameObject.transform.localPosition = Vector3.zero;

                        // set theme element to fitting theme
                        Theme compTheme = go.GetComponent<Theme>();
                        compTheme = t;

                        // set all quad materials to fitting color
                        MeshRenderer[] mrs = go.GetComponentsInChildren<MeshRenderer>();
                        foreach (MeshRenderer mr in mrs)
                        {
                            WaveSetter wave = mr.GetComponent<WaveSetter>();
                            int id = wave.id;
                            switch (id)
                            {
                                case 1:
                                    mr.material.color = t.horizonColor1;
                                    break;

                                case 2:
                                    mr.material.color = t.horizonColor2;
                                    break;

                                case 3:
                                    mr.material.color = t.horizonColor3;
                                    break;

                                case 4:
                                    mr.material.color = t.horizonColor4;
                                    break;

                                case 5:
                                    mr.material.color = t.horizonColor5;
                                    break;
                            }
                        }
                        theme = t;

                        CamColorSetter.BgColorUpdate();

                        Debug.Log("Skin " + skin + " was set");
                        ProgressManager.SaveProgressData();
                        break;
                    }
                }
            }
            else
            {
                Debug.Log("Skin is not unlocked");
            }
        }
    }
}