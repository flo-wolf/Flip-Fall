using FlipFall;
using FlipFall.Editor;
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
    public class Portal : LevelObject
    {
        /// <summary>
        /// Portals can either allow traveling back through or deny it.
        /// </summary>
        public enum PortalType { oneway, tunnel, limitedTravel };

        public PortalType portalType;

        //if PortalType == limitedTravel
        public int travelMax;

        public int portalID = -1;
        public int linkedPortalID = -1;

        public Portal linkedPortal;
        public bool active = true;
        //public bool possibleExit;

        // the portal the player currently travels from, aka start portal
        private static Portal startPortal;

        // the portal the player currently travels to, aka exit portal
        private static Portal exitPortal;

        // the players velocity on entering the portal
        private static Vector3 teleportVelocityMemory;

        private bool activeMemory;

        /// <summary>
        /// Containing portalIdle, portalUse
        /// </summary>
        public Animation anim;

        private void Start()
        {
            objectType = ObjectType.portal;
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

        // The player has entered this portal
        public void Enter()
        {
            Player.teleporting = true;
            if (active && this != exitPortal && linkedPortal != null)
            {
                if (linkedPortal.active)
                {
                    // save the velocity on entering the portal
                    Rigidbody2D playerBody = Player._instance.rBody;
                    teleportVelocityMemory = playerBody.velocity;

                    // set exitportal
                    exitPortal = linkedPortal;
                    startPortal = this;

                    // set the player's position to the exit position
                    Player._instance.transform.position = exitPortal.transform.position;
                }
            }
            Player.teleporting = false;
        }

        // the player exits this portal
        public void Exit()
        {
            if (this != startPortal && startPortal != null)
            {
                if (portalType == Portal.PortalType.oneway)
                    linkedPortal.active = false;
                else
                {
                    linkedPortal.active = true;
                    active = true;
                }

                exitPortal = null;
                startPortal = null;
            }
            Player.teleporting = false;
        }

        // link another portal to this one
        public void Link(Portal p)
        {
            if (p != null && p.portalID != portalID)
            {
                linkedPortal = p;
                linkedPortalID = p.portalID;
                linkedPortal.linkedPortalID = portalID;
                linkedPortal.linkedPortal = this;
            }
            else
                Debug.LogError("nah");
        }

        // unlick whatever portal is linked
        public void Unlink()
        {
            if (linkedPortal != null)
            {
                linkedPortal.linkedPortalID = -1;
                linkedPortal.linkedPortal = null;
                linkedPortalID = -1;
                linkedPortal = null;
            }
        }
    }
}