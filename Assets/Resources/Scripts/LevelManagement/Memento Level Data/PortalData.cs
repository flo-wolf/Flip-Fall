using FlipFall.LevelObjects;
using FlipFall.Levels;
using FlipFall.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// This is the actual Progress Data that is saved and loaded.
/// It consists out of an array of scoreboars, each of them containing the scores for a level.
/// It also holds other variables important for the players progress like achievemnts or settings.
/// </summary>

namespace FlipFall.Levels
{
    [Serializable]
    public class PortalData
    {
        public int portalID;
        public int linkedPortalID;
        public Position2 position;
        public Portal.PortalType portalType;

        public bool active;

        public PortalData(Position2 pos, int id)
        {
            portalID = id = -1;
            linkedPortalID = -1;
            portalType = Portal.PortalType.oneway;
            position = pos;
            active = true;
        }
    }
}