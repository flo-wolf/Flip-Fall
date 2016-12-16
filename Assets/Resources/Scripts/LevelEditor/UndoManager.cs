using FlipFall.LevelObjects;
using FlipFall.Levels;
using FlipFall.Progress;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Holds copies of the last few different versions of the level and its corresponding inventory.
/// Can be used to switch to any of the stored versions of the level/inventory.
/// Can be used to switch to the last savepoint.
/// </summary>

namespace FlipFall.Editor
{
    public class UndoManager : MonoBehaviour
    {
        public static NewSavePointEvent onSavePointChange = new NewSavePointEvent();

        public class NewSavePointEvent : UnityEvent { }

        // the id of the current UndoPoint
        public static int undoId = 0;

        // contains a LevelData and
        public static List<UndoPoint> undoPoints;
        public static UndoPoint savePoint;

        private static int maxUndoSaves = 10; // setting menu option

        private void Start()
        {
            undoPoints = new List<UndoPoint>();
            savePoint = null;
            undoId = 0;
        }

        // set the level to the last savepoint (not the last undo-point, two different things!)
        public static bool RestoreSavePoint()
        {
            if (savePoint != null)
            {
                // make the last savepoint the freshest undo point
                undoId = savePoint.id;

                List<UndoPoint> keepUndoPoints = new List<UndoPoint>();

                // delete everything after the savepoint i.e. only keep those before the savepoint
                foreach (UndoPoint undo in undoPoints)
                {
                    if (undo.id <= undoId)
                    {
                        keepUndoPoints.Add(undo);
                    }
                }
                undoPoints = keepUndoPoints;

                ProgressManager.GetProgress().unlocks.SetInventory(savePoint.inventory);
                LevelLoader.SaveCustomLevel(savePoint.levelData);
                return true;
            }
            return false;
        }

        // jump to the last undo step if it exists
        public static bool Undo()
        {
            if (!UndoExists(undoId))
            {
                undoId -= 2;
                AddUndoPoint();
            }

            bool undoExists = UndoExists(undoId - 1);
            Debug.Log("Undo - Undo to Step " + (undoId - 1) + " exists: " + undoExists);
            if (undoExists)
            {
                undoId -= 1;
                if (RestoreUndoPoint(undoId))
                {
                    return true;
                }
            }
            return false;
        }

        // jump to the next undo step if it exists
        public static bool Redo()
        {
            Debug.Log("Redo - Overwriting setp " + undoId);
            if (UndoExists(undoId + 1))
            {
                undoId += 1;

                if (RestoreUndoPoint(undoId))
                {
                    return true;
                }
            }
            return false;
        }

        // does the undo point exists in the hashtables?
        private static bool UndoExists(int id)
        {
            if (undoPoints.Any(x => x.id == id))
                return true;
            return false;
        }

        // set the level to a certian undopoint
        private static bool RestoreUndoPoint(int id)
        {
            UndoPoint undoPoint = undoPoints.Find(x => x.id == id);
            Debug.Log("RestoreUndoPoint " + id);
            if (undoPoint != null)
            {
                ProgressManager.GetProgress().unlocks.SetInventory(undoPoint.inventory);
                VertHandler._instance.DestroyHandles();
                LevelPlacer._instance.Place(undoPoint.levelData);
                LevelEditor.editLevel = undoPoint.levelData;
                return true;
            }
            return false;
        }

        // adds a savepoint that can be retrieved with RetrieveSavePoint()
        public static void AddSavePoint(LevelData levelData)
        {
            // add the value to the existing value
            undoId += 1;
            Inventory inventory = Inventory.CreateCopy(ProgressManager.GetProgress().unlocks.inventory);
            savePoint = new UndoPoint(undoId, levelData, inventory);
            AddUndoPoint();
            onSavePointChange.Invoke();
            LevelLoader.SaveCustomLevel(levelData);
        }

        // adds an undo point, a copy of the current editotlevel, to the hastable and throws the oldest one away if neccessary.
        // also adds an inventory savepoint that corresponds to the editorlevel getting saved
        public static void AddUndoPoint()
        {
            LevelData levelData = LevelEditor.CreateLevelData();
            Inventory inventory = Inventory.CreateCopy(ProgressManager.GetProgress().unlocks.inventory);

            undoId += 1;
            UndoPoint oldUndoPoint = undoPoints.Find(x => x.id == undoId);

            //Debug.Log("AddUndoPoint " + undoId + "inventory turrets: " + inventory.GetAmount(LevelObjects.LevelObject.ObjectType.turret));

            // there is already an undo point with the id we want to add
            if (oldUndoPoint != null)
            {
                // remove that old fucker (jk)
                undoPoints.Remove(oldUndoPoint);
            }

            // add the new undo Point
            UndoPoint newUndoPoint = new UndoPoint(undoId, levelData, inventory);
            undoPoints.Add(newUndoPoint);
            onSavePointChange.Invoke();

            // if there are too many saves, delete the last one
            if (undoPoints.Count > maxUndoSaves)
            {
                // find the lowest value key
                int lowest = 1000000;

                foreach (UndoPoint undo in undoPoints)
                {
                    if (undo.id < lowest)
                        lowest = undo.id;
                }
                undoPoints.Remove(undoPoints.Find(x => x.id == lowest));
            }

            // Debug.Log("Added AddUndoPoint " + undoId + "inventory turrets: " + undoPoints.Find(x => x.id == undoId).inventory.GetAmount(LevelObjects.LevelObject.ObjectType.turret));
        }
    }
}