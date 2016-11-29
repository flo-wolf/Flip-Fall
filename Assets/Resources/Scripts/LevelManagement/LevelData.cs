using FlipFall.Levels;
using FlipFall.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// This is the serialized version of a level, in the Memento Pattern.
/// It contains all the information needed to reproduce a level.
/// Reproduction is handled by the LevelLoader and LevelPlacer scripts.
/// </summary>

namespace FlipFall.Levels
{
    [Serializable]
    public class LevelUpdateEvent : UnityEvent { }

    [Serializable]
    public class LevelData
    {
        public static LevelUpdateEvent onLevelUpdate = new LevelUpdateEvent();

        public bool custom;             // was this level user created and can it thus editable?
        public int id;                  // identification
        public double presetTime;       // best time
        public string title;            // name
        public string author;           // author, by default "FlipFall"

        public Position2[] moveVerticies;
        public int[] moveTriangles;

        // Spawn
        public Position2 spawnPosition;

        // Finish
        public Position2 finishPosition;

        // Turrets
        public List<TurretData> turretData;

        // Attractors
        public List<AttractorData> attractorData;

        // Portals
        public List<PortalData> portalData;

        // SpeedStrips
        public List<SpeedStripData> speedStripData;

        public LevelData(int _id)
        {
            id = _id;
            presetTime = -1;
            title = "Custom Level";
            author = "FlipFall";
            moveVerticies = new Position2[0];
            spawnPosition = new Position2(0, 58);
            finishPosition = new Position2(0, -63);
            turretData = new List<TurretData>();
            portalData = new List<PortalData>();
            speedStripData = new List<SpeedStripData>();
            attractorData = new List<AttractorData>();
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