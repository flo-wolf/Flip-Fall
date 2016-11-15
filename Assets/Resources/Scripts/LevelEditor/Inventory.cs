﻿using FlipFall.LevelObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// This class gets saved in the current progress.
/// It contains a hashtable of all bought levelObjects that can be placed inside the editor.
/// The inventroy will get displayed in the editor screen.
/// </summary>

namespace FlipFall.Progress
{
    [Serializable]
    public class Inventory
    {
        public static InventoryChangeEvent onInventoryChange = new InventoryChangeEvent();

        public class InventoryChangeEvent : UnityEvent { }

        // a table containing the list of available levelObjects aswell as their owned amount
        private Hashtable levelObjects = new Hashtable();

        public Inventory()
        {
            levelObjects.Add(LevelObject.ObjectType.spawn, 2);
            levelObjects.Add(LevelObject.ObjectType.finish, 2);
            levelObjects.Add(LevelObject.ObjectType.speedStrip, 1);
            levelObjects.Add(LevelObject.ObjectType.turret, 1);
        }

        // adds a value to the given entry in the inventory, if it doesnt exist, it gets created. Can be used to substract.
        public void Add(LevelObject.ObjectType type, int value)
        {
            // add the value to the existing value
            if (levelObjects.Contains(type))
            {
                int oldValue = (int)levelObjects[type];
                levelObjects[type] = oldValue + value;
            }
            // the entry doesnt exist yet, create a new one with a guaranteed positive value
            else if (value > 0)
            {
                levelObjects.Add(type, value);
            }

            onInventoryChange.Invoke();
        }

        // removes an entry from the inventory
        public void Remove(LevelObject.ObjectType type)
        {
            if (levelObjects.Contains(type))
            {
                levelObjects.Remove(type);
            }
            onInventoryChange.Invoke();
        }

        // returns the owned amount of a certian levelObject, if the levelObject is not listed -1 gets returned;
        public int GetAmount(LevelObject.ObjectType type)
        {
            if (levelObjects.Contains(type))
            {
                return (int)levelObjects[type];
            }
            else
                return -1;
        }
    }
}