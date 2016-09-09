using Impulse.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Impulse.UI
{
    public class UILevelPlacer : MonoBehaviour
    {
        public static GameObject placingParent;
        public static UILevelPlacer _instance;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
                return;
            }
            _instance = this;

            placingParent = this.gameObject;
        }

        private void Start()
        {
        }

        public static UILevel LoadUILevel(int id)
        {
            UILevel uiLevel = null;
            try
            {
                GameObject go = (GameObject)Resources.Load("Prefabs/UI/UILevel");
                uiLevel = go.GetComponent<UILevel>();
                uiLevel.id = id;
            }
            catch (UnityException e)
            {
                Debug.Log(e);
            }

            if (uiLevel == null)
            {
                Debug.Log("LevelLoader: Levelprefab could not be found.");
                return null;
            }
            else
            {
                PlaceUILevel(uiLevel);
            }
            return uiLevel;
        }

        private static void PlaceUILevel(UILevel uiLevel)
        {
            UILevel l = null;
            if (!IsPlaced(uiLevel.id))
            {
                DestroyExistingUILevels();
                l = (UILevel)Instantiate(uiLevel, new Vector3(0, 0, 0), Quaternion.identity);
                l.gameObject.transform.SetParent(placingParent.transform);
                l.gameObject.transform.localPosition = new Vector3(0, 0, 0);
                Debug.Log("[UILevelPlacer]: Place(): Level " + uiLevel.id + " placed.");
            }
            else
            {
                Debug.Log("[UILevelPlacer]: Place(): Cant place Level " + uiLevel.id + ", it already exists in the scene!");
            }
        }

        //change to getcomponents and cycle through that => performance
        private static bool IsPlaced(int _id)
        {
            UILevel[] uiLevels = placingParent.GetComponentsInChildren<UILevel>();
            foreach (UILevel l in uiLevels)
            {
                if (l.id == _id)
                    return true;
            }
            return false;
        }

        private static void DestroyExistingUILevels()
        {
            UILevel[] uiLevels = placingParent.GetComponentsInChildren<UILevel>();

            foreach (UILevel l in uiLevels)
            {
                Destroy(l.gameObject);
            }
        }
    }
}