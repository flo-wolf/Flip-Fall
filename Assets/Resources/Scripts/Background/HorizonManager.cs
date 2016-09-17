using Impulse.Progress;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HorizonManager : MonoBehaviour
{
    public enum Skin { red, blackWhite, orange, sunset, blue, green, rainbow }

    //skin currently active
    public static Skin skin;

    //skin prefab references
    public GameObject sunset;

    private void Start()
    {
        DontDestroyOnLoad(this);
        SetSkin(GetLastSkin());
    }

    // get last skin from progress
    public Skin GetLastSkin()
    {
        Skin lastSkin = Skin.sunset;
        return lastSkin;
    }

    // checks if the skin is equipable/unlocked
    public bool IsUnlocked(Skin newSkin)
    {
        return true;

        if (ProgressManager.GetProgress().unlocks.skins.Any(x => x == newSkin))
            return true;
        return false;
    }

    public void SetSkin(Skin newSkin)
    {
        Debug.Log("1");
        if (newSkin != skin && IsUnlocked(newSkin))
        {
            GameObject go = null;

            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }

            Debug.Log("2");

            switch (newSkin)
            {
                case Skin.sunset:
                    go = Instantiate(sunset, new Vector3(0f, 0f, 0f), Quaternion.identity);
                    go.gameObject.transform.parent = transform;
                    go.gameObject.transform.localPosition = Vector3.zero;
                    break;

                default:
                    break;
            }
        }
        skin = newSkin;
    }
}