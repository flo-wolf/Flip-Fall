using Impulse.Levels;
using Impulse.Progress;
using Impulse.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Impulse.UI
{
    public class UILevelPlacer : MonoBehaviour
    {
        public static UILevelPlacer _instance;
        public static GameObject placingParent;
        public static UILevel placedLevel;
        public static List<UILevelNumber> placedLevelNumbers = new List<UILevelNumber>();

        public GameObject numberParent;

        //NO NOT SET THIS, its a reference to the prefab needed for instantiation
        private static GameObject uiLevelPrefab;
        private static GameObject uiLevelNumberPrefab;

        //Stored uiLevels
        //public static List<UILevel> uiLevels;

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
            uiLevelPrefab = (GameObject)Resources.Load("Prefabs/UI/UILevel");
            uiLevelNumberPrefab = (GameObject)Resources.Load("Prefabs/UI/UILevelNumber");
        }

        private void Start()
        {
            placedLevelNumbers = new List<UILevelNumber>();
            PlaceUILevel(UILevelselectionManager.activeUILevel, false);
            PlaceLevelNumbers(placedLevel.id);
            placedLevel.UpdateUILevel();
        }

        // First place of all levelnumbers on entering the scene. id == number to place around
        private static void PlaceLevelNumbers(int id)
        {
            Debug.Log("PlaceLevelNumbers(int id) " + id);
            for (int i = 2; i >= -2; i--)
            {
                int y = -i;
                if (UILevelMatchesLevel(id + i) && !placedLevelNumbers.Any(x => x.id == (id + i)) && !placedLevelNumbers.Any(x => x.position == i) && ProgressManager.GetProgress().lastUnlockedLevel >= id + i)
                {
                    //Debug.Log("matches: " + i);
                    UILevelNumber nbr = (UILevelNumber)Instantiate(uiLevelNumberPrefab.GetComponent<UILevelNumber>(), Vector3.zero, Quaternion.identity);
                    nbr.position = i;
                    nbr.id = id + i;
                    nbr.gameObject.transform.parent = _instance.numberParent.transform;
                    nbr.gameObject.transform.localPosition = Vector3.zero;
                    placedLevelNumbers.Add(nbr);
                }
                else if (UILevelMatchesLevel(id + y) && !placedLevelNumbers.Any(x => x.id == (id + y)) && !placedLevelNumbers.Any(x => x.position == y) && ProgressManager.GetProgress().lastUnlockedLevel >= id + y)
                {
                    //Debug.Log("matches: " + y);
                    UILevelNumber nbr = (UILevelNumber)Instantiate(uiLevelNumberPrefab.GetComponent<UILevelNumber>(), Vector3.zero, Quaternion.identity);
                    nbr.position = y;
                    nbr.id = id + y;
                    nbr.gameObject.transform.parent = _instance.numberParent.transform;
                    nbr.gameObject.transform.localPosition = Vector3.zero;
                    placedLevelNumbers.Add(nbr);
                }
            }
        }

        public static void PlaceLevelNumber(int id, int position)
        {
            if (position < 3 && position > -3)
            {
                if (UILevelMatchesLevel(id) && !placedLevelNumbers.Any(x => x.id == id) && ProgressManager.GetProgress().lastUnlockedLevel >= id)
                {
                    UILevelNumber nbr = (UILevelNumber)Instantiate(uiLevelNumberPrefab.GetComponent<UILevelNumber>(), Vector3.zero, Quaternion.identity);
                    nbr.position = position;
                    nbr.id = id;
                    nbr.gameObject.transform.parent = _instance.numberParent.transform;
                    nbr.gameObject.transform.localPosition = Vector3.zero;
                    placedLevelNumbers.Add(nbr);
                }
                else
                    Debug.Log("Couldn't place UILevelNumber");
            }
        }

        public static bool UILevelMatchesLevel(int ID)
        {
            if (LevelManager.LevelExists(ID))
                return true;
            return false;
        }

        public static void PlaceUILevel(int id, bool levelSwitch)
        {
            if (!IsPlaced(id))
            {
                DestroyExistingUILevels();
                UILevel l = (UILevel)Instantiate(uiLevelPrefab.GetComponent<UILevel>(), new Vector3(0, 0, 0), Quaternion.identity);
                l.id = id;
                l.gameObject.transform.SetParent(placingParent.transform);
                l.createdByLevelswitch = levelSwitch;
                //l.gameObject.transform.localPosition = new Vector3(0, 0, 0);
                l.gameObject.transform.localPosition = new Vector3(0, uiLevelPrefab.transform.position.y, 0);
                placedLevel = l;
            }
            else
            {
                Debug.Log("[UILevelPlacer]: Place(): Cant place UILevel " + id + ", it already exists in the scene!");
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

        //public static List<UILevel> CreateUILevels()
        //{
        //    List<UILevel> createdUILevels = new List<UILevel>();
        //    for (int i = Constants.firstLevel; i <= Constants.lastLevel; i++)
        //    {
        //        UILevel l = CreateUILevel(i);
        //        //Debug.Log("CreateUILevels() uilevel " + l.id);
        //        if (l.id != null)
        //        {
        //            createdUILevels.Add(l);
        //        }
        //    }

        //    return createdUILevels;
        //}

        //public static UILevel CreateUILevel(int id)
        //{
        //    UILevel uiLevel = null;
        //    try
        //    {
        //        uiLevel = Instantiate(uiLevelPrefab.GetComponent<UILevel>(), new Vector3(0, 0, 0), Quaternion.identity);
        //        uiLevel.id = id;
        //        uiLevel.gameObject.name = "Cached UILevel " + id;
        //        uiLevel.gameObject.transform.SetParent(_instance.cacheParent.transform);
        //    }
        //    catch (UnityException e)
        //    {
        //        Debug.Log(e);
        //    }

        //    if (uiLevel == null)
        //    {
        //        Debug.Log("LevelLoader: Levelprefab could not be found.");
        //        return null;
        //    }
        //    else
        //    {
        //    }
        //    return uiLevel;
        //}
    }
}