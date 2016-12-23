using FlipFall.Levels;
using FlipFall.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// This is the serialized version of a level, in the memento pattern.
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

        public string levelChecksum;

        public int id;                  // identification
        public bool custom;             // was this level user created and is it thus editable?
        public double presetTime;       // best time
        public string title;            // name
        public string author;           // author, by default "FlipFall"

        public Position2[] moveVerticies;
        public int[] moveTriangles;

        // a collection of all levelobjects in the scene including all of their attributes
        public LevelObjectData objectData;

        public string objectChecksum;

        public LevelData(int _id)
        {
            id = _id;
            presetTime = 99;
            title = "Custom Level";
            author = "FlipFall";
            moveVerticies = new Position2[0];
            objectData = new LevelObjectData();
            objectChecksum = GenerateObjectChecksum();
        }

        // checksum getting generated out of the amount of objects in the scene
        public string GenerateObjectChecksum()
        {
            string check = "turretData" + objectData.turretData.Count + "portalData" + objectData.portalData.Count + "speedStripData" + objectData.speedStripData.Count + "attractorData" + objectData.attractorData.Count;
            return Md5Sum(check);
        }

        // full object checksum, checks if any changes were made to the level - includes id, time, movearea and objects.
        public string GenerateLevelChecksum()
        {
            string jsonLevelData = JsonUtility.ToJson(objectData) + JsonUtility.ToJson(moveVerticies) + JsonUtility.ToJson(moveTriangles) + id + presetTime + custom;
            return Md5Sum(jsonLevelData);
        }

        // creates an MD5 Hash out of an input string
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