using FlipFall.Editor;
using FlipFall.LevelObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// OBSOLETE: Switched since v0.2.38 from consumables to non-consumable building supplies.
/// Currently these values are ignored.
///
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

        public List<LevelObject.ObjectType> unlockedLevelObjects;

        // a table containing the list of available levelObjects aswell as their owned amount
        //public Hashtable levelObjects = new Hashtable();

        public Inventory()
        {
            unlockedLevelObjects = new List<LevelObject.ObjectType>();
            //levelObjects.Add(LevelObject.ObjectType.speedStrip, 5);
            //levelObjects.Add(LevelObject.ObjectType.turret, 5);
            //levelObjects.Add(LevelObject.ObjectType.portal, 5);
            //levelObjects.Add(LevelObject.ObjectType.attractor, 5);
            //levelObjects.Add(LevelObject.ObjectType.bouncer, 5);
        }

        // adds a value to the given entry in the inventory, if it doesnt exist, it gets created. Can be used to substract.
        public void Add(LevelObject.ObjectType type, int value)
        {
            // add the value to the existing value
            if (!unlockedLevelObjects.Contains(type))
            {
                unlockedLevelObjects.Add(type);
            }
            onInventoryChange.Invoke();
        }

        // removes an entry from the inventory
        public void Remove(LevelObject.ObjectType type)
        {
            if (unlockedLevelObjects.Contains(type))
            {
                unlockedLevelObjects.Remove(type);
            }
            onInventoryChange.Invoke();
        }

        public bool Contains(LevelObject.ObjectType type)
        {
            if (unlockedLevelObjects.Contains(type))
                return true;
            return false;
        }

        public static Inventory CreateCopy(Inventory inv)
        {
            Inventory inventoryCopy = new Inventory();
            List<LevelObject.ObjectType> newList = new List<LevelObject.ObjectType>();

            foreach (LevelObject.ObjectType key in inv.unlockedLevelObjects)
            {
                newList.Add(key);
            }
            inventoryCopy.unlockedLevelObjects = newList;
            return inventoryCopy;
        }
    }
}