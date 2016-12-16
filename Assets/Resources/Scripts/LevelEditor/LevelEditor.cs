using FlipFall.Audio;
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
        public enum EditorMode { select, edit, place, portalLink }

        public static EditorMode editorMode = EditorMode.select;

        // current level getting edited
        public static LevelData editLevel;

        public static LevelObject selectedObject;

        public static Mesh selectedMesh;

        public static bool changesAreSaved;

        public static bool inventoryDragged = false;

        private void Start()
        {
            inventoryDragged = false;
            changesAreSaved = true;

            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
                return;
            }
            _instance = this;

            editorMode = EditorMode.select;

            if (editLevel != null)
            {
                LevelPlacer._instance.Place(editLevel);
                UndoManager.AddSavePoint(editLevel);
            }
        }

        // saves the changes made to the currently placed generated level to the .levelData format
        public static void SaveLevel()
        {
            // save it
            editLevel = CreateLevelData();
            UndoManager.AddSavePoint(editLevel);
        }

        // takes the current placed levelObjects and serializes it into the levelData format
        public static LevelData CreateLevelData()
        {
            if (LevelPlacer.generatedLevel != null && editLevel != null)
            {
                print("CreateLevelData");
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
                if (!posVerts.Equals(editLevel.moveVerticies))
                {
                    Debug.Log("Different Vertices");
                }
                l.moveVerticies = posVerts;
                l.moveTriangles = level.moveArea.meshFilter.mesh.triangles;

                // save spawn
                Vector3 spawnPos = level.spawn.transform.localPosition;
                l.objectData.spawnPosition = new Position2(spawnPos.x, spawnPos.y);

                // save finish
                Vector3 finishPos = level.finish.transform.localPosition;
                l.objectData.finishPosition = new Position2(finishPos.x, finishPos.y);

                // save turrets
                foreach (Turret t in level.turrets)
                {
                    Position2 turretPosition = new Position2(t.transform.localPosition.x, t.transform.localPosition.y);
                    TurretData td = new TurretData(turretPosition);
                    td.rotation = new Position3(t.transform.rotation.eulerAngles.x, t.transform.rotation.eulerAngles.y, t.transform.rotation.eulerAngles.z);
                    td.shotDelay = t.shotDelay;
                    td.shotSpeed = t.shotSpeed;
                    td.startupDelay = t.startupDelay;
                    td.constantFire = t.constantFire;
                    l.objectData.turretData.Add(td);
                }

                // save attractors
                foreach (Attractor a in level.attractors)
                {
                    Position2 aPosition = new Position2(a.transform.localPosition.x, a.transform.localPosition.y);
                    AttractorData ad = new AttractorData(aPosition);
                    ad.pullStrength = (int)a.maxPullForce;
                    ad.radius = (int)a.pullRadius;
                    l.objectData.attractorData.Add(ad);
                }

                // save portals
                foreach (Portal p in level.portals)
                {
                    Position2 pPosition = new Position2(p.transform.localPosition.x, p.transform.localPosition.y);
                    PortalData pd = new PortalData(pPosition, p.portalID);
                    pd.portalID = p.portalID;
                    pd.linkedPortalID = p.linkedPortalID;
                    l.objectData.portalData.Add(pd);
                }

                // save speedstrips
                foreach (SpeedStrip s in level.speedStrips)
                {
                    Position2 pPosition = new Position2(s.transform.localPosition.x, s.transform.localPosition.y);
                    SpeedStripData sd = new SpeedStripData(pPosition);
                    sd.rotation = new Position3(s.transform.rotation.eulerAngles.x, s.transform.rotation.eulerAngles.y, s.transform.rotation.eulerAngles.z);
                    sd.pushStrength = (int)s.accelSpeed;
                    l.objectData.speedStripData.Add(sd);
                }

                foreach (Bouncer b in level.bouncers)
                {
                    Position2 bPosition = new Position2(b.transform.localPosition.x, b.transform.localPosition.y);
                    BouncerData bd = new BouncerData(bPosition);
                    bd.rotation = new Position3(b.transform.rotation.eulerAngles.x, b.transform.rotation.eulerAngles.y, b.transform.rotation.eulerAngles.z);
                    bd.bounciness = b.bounciness;
                    bd.width = b.width;
                    l.objectData.bouncerData.Add(bd);
                }

                return l;
            }
            return null;
        }

        // change the current selected object and controll outline/handler/delete button display
        public static void SetSelectedObject(LevelObject newSelected)
        {
            // the new selection will be null, thus deselect whatever is selected
            if (newSelected == null && !Handle.vertGettingSelected)
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

                SoundManager.PlayLightWobble(0.6F);
                editorMode = EditorMode.select;
            }
            // replace the current selection with a new one
            else if (!Handle.vertGettingSelected)
            {
                // there was an object sleected already
                if (selectedObject != null)
                {
                    selectedObject.SetOutlineVisible(false);
                    // if the movearea got selected activate the handles
                    if (selectedObject.objectType == LevelObject.ObjectType.moveArea)
                    {
                        VertHandler._instance.OnDisable();
                    }
                }

                editorMode = EditorMode.edit;
                selectedObject = newSelected;
                selectedObject.SetOutlineVisible(true);

                // if the movearea got selected activate the handles
                if (newSelected.objectType == LevelObject.ObjectType.moveArea)
                {
                    VertHandler.showHandles = true;
                    UILevelEditor.DeleteShow(false);
                }
                // the new object is not a movearea, activate the delete button (movearea delete button gets displayed when vertices get selected)
                else if (newSelected.objectType != LevelObject.ObjectType.spawn && newSelected.objectType != LevelObject.ObjectType.finish)
                {
                    UILevelEditor.DeleteShow(true);
                    SoundManager.PlayLightWobble();
                }
            }
        }

        public static bool TryTestLevel()
        {
            editLevel = CreateLevelData();
            Game.gameType = Game.GameType.testing;
            Main.SetScene(Main.ActiveScene.game);
            return true;
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