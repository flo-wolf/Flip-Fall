using Impulse.Levels;
using Impulse.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Impulse.UI
{
    public class UILevelPlacer : MonoBehaviour
    {
        public static GameObject placingParent;
        public GameObject cacheParent;

        //NO NOT SET THIS, its a reference to the prefab needed for instantiation
        private static GameObject uiLevelPrefab;
        public static UILevelPlacer _instance;

        //Stored uiLevels
        public static List<UILevel> uiLevels;

        //Amount of levels shown next to the selected one
        private int unfocusedAmount;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
                return;
            }
            _instance = this;

            //DontDestroyOnLoad(this);
            placingParent = this.gameObject;
        }

        private void OnEnable()
        {
            //first level
        }

        private void Start()
        {
            uiLevelPrefab = (GameObject)Resources.Load("Prefabs/UI/UILevel");

            //LevelManager.onLevelChange.AddListener(PlaceUILevel);
            uiLevels = CreateUILevels();
            PlaceUILevel(UILevelselectionManager.activeUILevel);
            PlaceSurroundingLevels(UILevelselectionManager.activeUILevel);
        }

        public static List<UILevel> CreateUILevels()
        {
            List<UILevel> createdUILevels = new List<UILevel>();
            for (int i = Constants.firstLevel; i <= Constants.lastLevel; i++)
            {
                UILevel l = CreateUILevel(i);
                Debug.Log("CreateUILevels() uilevel " + l.id);
                if (l.id != null)
                {
                    createdUILevels.Add(l);
                }
            }

            return createdUILevels;
        }

        public static UILevel CreateUILevel(int id)
        {
            UILevel uiLevel = null;
            try
            {
                uiLevel = Instantiate(uiLevelPrefab.GetComponent<UILevel>(), new Vector3(0, 0, 0), Quaternion.identity);
                uiLevel.id = id;
                uiLevel.gameObject.name = "Cached UILevel " + id;
                uiLevel.gameObject.transform.SetParent(_instance.cacheParent.transform);
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
            }
            return uiLevel;
        }

        //Place the rotating Levels that are currently not in focus around the given level by id
        private static void PlaceSurroundingLevels(int id)
        {
            //uiLevels.Find(x => x.id == id - 1).transform.localScale =
        }

        public static void PlaceUILevel(int id)
        {
            UILevel l = uiLevels.Find(x => x.id == id);
            //foreach (UILevel uil in uiLevels)
            //{
            //    Debug.Log("Placelevel all levels: " + uil.id);
            //}
            //Debug.Log("PlaceUILevel id: " + id + " uilevel: " + l);
            //if (l == null || l.id != id)
            //{
            //    Debug.LogError("UI level about to be placed is null");
            //}
            if (l != null && !IsPlaced(l.id))
            {
                DestroyExistingUILevels();
                l = (UILevel)Instantiate(uiLevels.Find(x => x.id == id), new Vector3(0, 0, 0), Quaternion.identity);
                l.gameObject.transform.SetParent(placingParent.transform);
                l.gameObject.transform.localPosition = new Vector3(0, 0, 0);
                Debug.Log("[UILevelPlacer]: Place(): Level " + uiLevels.Find(x => x.id == id).id + " placed.");
            }
            else
            {
                //Debug.Log("[UILevelPlacer]: Place(): Cant place Level " + uiLevels[id].id + ", it already exists in the scene!");
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