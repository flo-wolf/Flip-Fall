using FlipFall.LevelObjects;
using FlipFall.Levels;
using FlipFall.UI;
using System.Collections;
using System.IO;
using UnityEngine;

namespace FlipFall.Editor
{
    public class LevelEditor : MonoBehaviour
    {
        //look at tileeditor sctipt to get an idea how a level editor could be build up like
        public static LevelEditor _instance;

        // select: select game objects/groups of objects to edit
        // move: move the current selected object. If the object is the movearea, vertices will get moved
        // tool: depending on the selection each selected object can have it's own tool
        //       the tool mode tool can have its own sub-modes
        public enum EditorMode { select, edit, place }

        public static EditorMode editorMode = EditorMode.select;

        // current level getting edited
        public static LevelData editLevel;

        public static LevelObject selectedObject;

        public static Mesh selectedMesh;

        public static bool changesAreSaved;

        private void Start()
        {
            changesAreSaved = true;

            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
                return;
            }
            _instance = this;

            editorMode = EditorMode.select;

            if (editLevel != null)
                LevelPlacer._instance.PlaceCustom(editLevel);

            selectedObject = LevelPlacer.generatedLevel.moveArea;
        }

        // saves the changes made to the currently placed generated level to the .levelData format
        public static void SaveLevel()
        {
            if (editLevel != null)
            {
                LevelDataMono level = LevelPlacer.generatedLevel;
                LevelData l = new LevelData(level.levelData.id);

                // save level info
                l.author = level.levelData.author;
                l.id = level.levelData.id;
                l.presetTime = level.levelData.presetTime;
                l.title = level.levelData.title;
                l.custom = level.levelData.custom;

                // save moveArea mesh
                Vector3[] verts = level.moveArea.meshFilter.mesh.vertices;
                Position2[] posVerts = new Position2[verts.Length];
                for (int i = 0; i < verts.Length; i++)
                {
                    posVerts[i] = new Position2(verts[i].x, verts[i].y);
                }
                l.moveVerticies = posVerts;
                l.moveTriangles = level.moveArea.meshFilter.mesh.triangles;

                // save spawn
                Vector3 spawnPos = level.spawn.transform.localPosition;
                l.spawnPosition = new Position2(spawnPos.x, spawnPos.y);

                print("spawnpos saving: " + spawnPos);

                // save finish
                Vector3 finishPos = level.finish.transform.localPosition;
                l.finishPosition = new Position2(finishPos.x, finishPos.y);
                print("finish saving: " + finishPos);

                // save turrets
                foreach (Turret t in level.turrets)
                {
                    Position2 turretPosition = new Position2(t.transform.localPosition.x, t.transform.localPosition.y);
                    TurretData td = new TurretData(turretPosition);
                    td.shotDelay = t.shotDelay;
                    td.shotSpeed = t.shotSpeed;
                    td.startupDelay = t.startupDelay;
                    td.constantFire = t.constantFire;
                    l.turretData.Add(td);
                }

                // save attractors
                foreach (Attractor a in level.attractors)
                {
                    Position2 aPosition = new Position2(a.transform.localPosition.x, a.transform.localPosition.y);
                    AttractorData ad = new AttractorData(aPosition);
                    l.attractorData.Add(ad);
                }

                // save portals
                foreach (Portal p in level.portals)
                {
                    Position2 pPosition = new Position2(p.transform.localPosition.x, p.transform.localPosition.y);
                    PortalData pd = new PortalData(pPosition);
                    l.portalData.Add(pd);
                }

                // save speedstrips
                foreach (SpeedStrip s in level.speedStrips)
                {
                    Position2 pPosition = new Position2(s.transform.localPosition.x, s.transform.localPosition.y);
                    SpeedStripData sd = new SpeedStripData(pPosition);
                    l.speedStripData.Add(sd);
                }

                // save it
                LevelLoader.SaveCustomLevel(l);
            }
        }

        // change the current selected object and controll outline/handler/delete button display
        public static void SetSelectedObject(LevelObject newSelected)
        {
            // deselect the current selection
            if (newSelected == null)
            {
                // if the movearea was selected deactivate the handles
                if (selectedObject.objectType == LevelObject.ObjectType.moveArea)
                {
                    VertHandler.showHandles = false;
                }

                if (selectedObject != null)
                    selectedObject.SetOutlineVisible(false);
                selectedObject = null;

                UILevelEditor.DeleteShow(false);

                editorMode = EditorMode.select;
            }
            // replace the current selection with a new one
            else
            {
                if (selectedObject != null)
                {
                    selectedObject.SetOutlineVisible(false);
                    // if the movearea got selected activate the handles
                    if (selectedObject.objectType == LevelObject.ObjectType.moveArea)
                    {
                        VertHandler._instance.OnDisable();
                    }
                }

                // if the movearea got selected activate the handles
                if (newSelected.objectType == LevelObject.ObjectType.moveArea)
                {
                    VertHandler.showHandles = true;
                    UILevelEditor.DeleteShow(false);
                }
                // the new object is not a movearea, activate the delete button (movearea delete button gets displayed when vertices get selected)
                else
                {
                    UILevelEditor.DeleteShow(true);
                }

                editorMode = EditorMode.edit;
                selectedObject = newSelected;
                selectedObject.SetOutlineVisible(true);
            }
        }

        public static void SetState(EditorMode mode)
        {
            editorMode = mode;
        }

        // loads a level by its id
        public void LoadLevel(string id)
        {
        }
    }
}