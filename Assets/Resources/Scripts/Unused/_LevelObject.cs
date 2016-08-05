using System.Collections.Generic;
using UnityEngine;

//LevelObjects can either be logic items (triggers, start, end), shapes (custom mesh, boxes, triangles, circles), or enemies(spikes, chainsaws, lasers)

namespace Sliders.Unused
{
    public class LevelObject : MonoBehaviour
    {
        public enum LevelObjectType { spawn, finish, shape }

        public LevelObjectType levelObjectType;

        public void Start()
        {
        }

        public void UpdateMesh()
        {
        }

        //Draw triangle shape
        public void createShape()
        {
        }
    }
}

/*
--------------------------------------------------------------
        public LevelObject(Level level, TYPE type)
        {
            if (type == TYPE.Shape)
            {
                type = TYPE.Shape;
                levelMesh = new Mesh();
                parentLevel = level;
                level.Add(this);
            }
            else if (type == TYPE.Logic)
            {
                type = TYPE.Logic;
                levelMesh = new Mesh();
                parentLevel = level;
                level.Add(this);
            }
            else if (type == TYPE.Enemy)
            {
                type = TYPE.Enemy;
                levelMesh = new Mesh();
                parentLevel = level;
                level.Add(this);
            }
        }

        public void Create()
        {
        }

        public void Add()
        {
            //gameobject erstellen
            levelObject.AddComponent<MeshCollider>();
            levelObject.AddComponent<MeshFilter>();
        }

        public Level GetParentLevel()
        {
            return parentLevel;
        }

        public void SetParentLevel(Level newLevel)
        {
            newLevel.Add(this);
            parentLevel.RemoveLevelObject(this);
        }

        public void Remove()
        {
            //Only works when LevelObject is a MonoBehviour (obviously)
            //Destroy(this.gameObject);
        }
        */