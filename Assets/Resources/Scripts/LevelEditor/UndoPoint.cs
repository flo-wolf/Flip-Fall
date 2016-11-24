using FlipFall.Levels;
using FlipFall.Progress;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A Point
/// </summary>
namespace FlipFall.Editor
{
    [Serializable]
    public class UndoPoint
    {
        public int id;
        public Inventory inventory;
        public LevelData levelData;

        public UndoPoint(int _id, LevelData _levelData, Inventory _inventory)
        {
            id = _id;
            levelData = _levelData;
            inventory = _inventory;
        }
    }
}