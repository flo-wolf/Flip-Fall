using FlipFall;
using FlipFall.Theme;
using System;
using System.Collections;

using System.Collections.Generic;

using UnityEngine;

using UnityEngine.Events;

/// <summary>
/// Portal LevelObject.
/// The total number of portals in a Level has to be even, two always referencing each other.
/// A pair of portals is called twins.
/// When the Player hits a Portal he gets teleported to the referenced twin portal.
/// A Portal can either be a one way transition into another area,
/// It can also serve as a checkpoint upon entering.
/// </summary>
namespace FlipFall.LevelObjects
{
    public class Portal : MonoBehaviour
    {
        /// <summary>
        /// Portals can either allow traveling back through or deny it.
        /// </summary>
        public enum PortalType { oneway, tunnel, limitedTravel };

        public PortalType portalType;

        //if PortalType == limitedTravel
        public int travelMax;

        public Portal twinPortal;
        public bool active;
        //public bool possibleExit;

        private bool activeMemory;

        /// <summary>
        /// Containing portalIdle, portalUse
        /// </summary>
        public Animation anim;

        private void Start()
        {
            activeMemory = active;
            Game.onGameStateChange.AddListener(GameStateChanged);
            MeshRenderer mr = GetComponent<MeshRenderer>();
            if (mr != null)
                mr.material.SetColor("_Color", ThemeManager.theme.portalColor);
            else
                Debug.LogError("No MeshRenderer attached to the Portal, can't set the color.");
        }

        private void GameStateChanged(Game.GameState gs)
        {
            switch (gs)
            {
                case Game.GameState.playing:
                    active = activeMemory;
                    break;

                default:
                    break;
            }
        }
    }
}