using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlipFall.LevelObjects
{
    [Serializable]
    public class LevelObject : MonoBehaviour
    {
        public enum ObjectType { moveArea, spawn, finish, turret, portal, attractor, speedStrip, spike }
        public ObjectType objectType;
    }
}