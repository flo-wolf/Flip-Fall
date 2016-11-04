using FlipFall.Levels;
using FlipFall.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// This is the serialized version of a level, this is the Memento Pattern.
/// It contains all the information needed to reproduce a level.
/// </summary>

namespace FlipFall.Levels
{
    [Serializable]
    public class LevelUpdateEvent : UnityEvent { }

    [Serializable]
    public class LevelData
    {
        public static LevelUpdateEvent onLevelUpdate = new LevelUpdateEvent();

        public int id;
        public double presetTime;
        public string title;
        public string author;

        public Position2[] moveVerticies;

        // Spawn
        public Position3 spawnPosition;

        // Finish
        public Position3 finishPosition;

        // Turret
        public List<TurretData> turretDatas;

        // Attractor

        public LevelData(int _id)
        {
            id = _id;
            presetTime = -1;
            title = "homo";
            author = "User";
            moveVerticies = new Position2[0];
            spawnPosition = new Position3(0, 0, 0);
            finishPosition = new Position3(0, 0, 0);
            turretDatas = new List<TurretData>();
            // default level values in here
        }
    }
}

/// <summary>
/// The vector3's can not be serialized (weird) so here is my extention
/// </summary>
[Serializable]
public class Position2
{
    public float x = 0;
    public float y = 0;

    public Position2(Vector2 vector)
    {
        x = vector.x;
        y = vector.y;
    }

    public Position2(Transform transform)
    {
        x = transform.position.x;
        y = transform.position.y;
    }

    public Position2(float posX, float posY)
    {
        x = posX;
        y = posY;
    }
}

[Serializable]
public class Position3
{
    public float x = 0;
    public float y = 0;
    public float z = 0;

    public Position3(Vector3 vector)
    {
        x = vector.x;
        y = vector.y;
        z = vector.z;
    }

    public Position3(Transform transform)
    {
        x = transform.position.x;
        y = transform.position.y;
        z = transform.position.z;
    }

    public Position3(float posX, float posY, float posZ)
    {
        x = posX;
        y = posY;
        z = posZ;
    }
}