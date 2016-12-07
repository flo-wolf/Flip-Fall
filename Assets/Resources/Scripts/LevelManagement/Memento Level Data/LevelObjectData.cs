using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlipFall.Levels
{
    [Serializable]
    public class LevelObjectData
    {
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

        public LevelObjectData()
        {
            spawnPosition = new Position2(20, 100);
            finishPosition = new Position2(20, -20);
            turretData = new List<TurretData>();
            portalData = new List<PortalData>();
            speedStripData = new List<SpeedStripData>();
            attractorData = new List<AttractorData>();
        }
    }
}