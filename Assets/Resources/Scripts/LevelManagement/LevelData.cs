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

        public string checksum;

        public int id;                  // identification
        public bool custom;             // was this level user created and is it thus editable?
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

            checksum = GenerateChecksum();
        }

        // generate a checksum out of all levelobjects contained in the leveldata, to verify no objects have been added or removed
        public string GenerateChecksum()
        {
            string check = "turretData" + turretData.Count + "portalData" + portalData.Count + "speedStripData" + speedStripData.Count + "attractorData" + attractorData.Count;
            return Md5Sum(check);
        }

        public string Md5Sum(string strToEncrypt)
        {
            System.Text.UTF8Encoding ue = new System.Text.UTF8Encoding();
            byte[] bytes = ue.GetBytes(strToEncrypt);

            // encrypt bytes
            System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] hashBytes = md5.ComputeHash(bytes);

            // Convert the encrypted bytes back to a string (base 16)
            string hashString = "";

            for (int i = 0; i < hashBytes.Length; i++)
            {
                hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
            }

            return hashString.PadLeft(32, '0');
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