using Impulse.Progress;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HorizonManager : MonoBehaviour
{
    public static HorizonManager _instance;

    public enum Skin { unset, childhood, forrest, rainbow, sunset, silver, gold, darkness, ocean, toxic }

    //skin currently active
    public static Skin skin = Skin.unset;
    public Skin defaultSkin;

    //skin prefab references
    public GameObject[] skins;

    private void Start()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        _instance = this;

        DontDestroyOnLoad(this);
        SetSkin(GetLastSkin());
    }

    // get last skin from progress
    public Skin GetLastSkin()
    {
        Skin lastSkin = ProgressManager.GetProgress().settings.skin;
        if (skin == Skin.unset)
        {
            lastSkin = defaultSkin;
            Debug.Log("def: " + defaultSkin);
        }
        return lastSkin;
    }

    // checks if the skin is equipable/unlocked
    public static bool IsUnlocked(Skin newSkin)
    {
        return true;

        // NOT CALLED CURRENTLY, UNLOCKS NOT GETTING CHECKED!
        if (ProgressManager.GetProgress().unlocks.skins.Any(x => x == newSkin))
            return true;
        return false;
    }

    public static void SetSkin(Skin newSkin)
    {
        if (newSkin != skin && IsUnlocked(newSkin))
        {
            GameObject go = null;

            foreach (GameObject g in _instance.skins)
            {
                if (g.GetComponent<Horizon>().skin == newSkin)
                {
                    foreach (Transform child in _instance.transform)
                    {
                        Destroy(child.gameObject);
                    }
                    go = Instantiate(g, new Vector3(0f, 0f, 0f), Quaternion.identity);
                    go.gameObject.transform.parent = _instance.transform;
                    go.gameObject.transform.localPosition = Vector3.zero;
                    skin = newSkin;
                    ProgressManager.GetProgress().settings.skin = skin;
                    break;
                }
            }
        }
    }
}