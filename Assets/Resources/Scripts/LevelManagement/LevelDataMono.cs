using FlipFall.Cam;
using FlipFall.LevelObjects;
using FlipFall.Progress;
using FlipFall.Theme;
using FlipFall.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Level class. When a level gets loaded all moveArea meshes will be combined into one object, the moveAreaGo.
/// Is referenced in the LevelPlacer.placedLevel.
/// </summary>

namespace FlipFall.Levels
{
    public class LevelDataMono : MonoBehaviour
    {
        public static LevelUpdateEvent onGameStateChange = new LevelUpdateEvent();

        public class LevelUpdateEvent : UnityEvent<Level> { }

        public LevelData levelData;
        public MoveArea moveArea;
        public Spawn spawn;
        public Finish finish;

        private void Awake()
        {
        }

        public void OnEnable()
        {
            // reset the dissolve shader to zero
        }

        private void FixedUpdate()
        {
        }

        public void AddObject(GameObject go)
        {
        }

        public void RemoveObject(GameObject go)
        {
        }
    }
}