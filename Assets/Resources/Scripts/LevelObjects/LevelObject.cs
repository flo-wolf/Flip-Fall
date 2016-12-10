using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlipFall.LevelObjects
{
    [Serializable]
    public class LevelObject : MonoBehaviour
    {
        public enum ObjectType { moveArea, spawn, finish, turret, portal, attractor, speedStrip, spike, bouncer }
        public ObjectType objectType;

        public GameObject OutlineGameObject;

        private Material m;

        public void SetOutlineVisible(bool visible)
        {
            if (visible && objectType != ObjectType.moveArea)
            {
                MeshRenderer mr = GetComponent<MeshRenderer>();
                if (mr != null)
                {
                    m = mr.material;
                    MeshRenderer mrOutline = OutlineGameObject.GetComponent<MeshRenderer>();
                    if (mrOutline != null)
                    {
                        Material mOutline = mrOutline.material;

                        // set the outline color to half of the rgb color of the levelobject
                        Color mColor = m.color;
                        mColor.b = mColor.b / 2;
                        mColor.g = mColor.g / 2;
                        mColor.r = mColor.r / 2;
                        mColor.a = mColor.a / 2;
                        mOutline.SetColor("Tint", m.color);

                        // the outline should render behind the levelobject
                        mOutline.renderQueue = m.renderQueue - 1;
                    }
                }
                OutlineGameObject.SetActive(true);
            }
            else if (objectType != ObjectType.moveArea)
                OutlineGameObject.SetActive(false);
        }
    }
}