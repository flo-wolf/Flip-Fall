using FlipFall.Theme;
using System.Collections;
using UnityEngine;

namespace FlipFall.LevelObjects
{
    public class Spawn : LevelObject
    {
        public bool facingLeftOnSpawn;
        private Animation anim;

        private void Start()
        {
            anim = GetComponent<Animation>();
            if (anim != null && Main.currentScene == Main.Scene.game)
            {
                anim.Play("spawnFadeOut");
            }
            objectType = ObjectType.spawn;
            MeshRenderer mr = GetComponent<MeshRenderer>();
            if (mr != null)
                mr.material.SetColor("_Color2", ThemeManager.theme.speedstripColor);
            else
                Debug.LogError("No MeshRenderer attached to the Spawn, can't set the color.");
        }

        public Vector3 GetPosition()
        {
            return transform.position;
        }
    }
}