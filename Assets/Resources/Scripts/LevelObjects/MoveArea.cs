using FlipFall.Levels;
using FlipFall.Theme;
using System.Collections;
using UnityEngine;

namespace FlipFall.LevelObjects
{
    public class MoveArea : LevelObject
    {
        public MeshFilter meshFilter;
        private PolygonCollider2D poly2D;
        private MeshRenderer mr;

        private void Awake()
        {
            objectType = ObjectType.moveArea;
            meshFilter = GetComponent<MeshFilter>();
            mr = GetComponent<MeshRenderer>();
            mr.material.SetColor("_Color", ThemeManager.theme.moveZoneColor);
            mr.material.SetFloat("_SliceAmount", 1F);

            Main.onSceneChange.AddListener(SceneChanged);
            StartCoroutine(cReverseDissolveLevel(LevelManager._instance.DissolveLevelDuration));
        }

        private void SceneChanged(Main.Scene s)
        {
            DissolveLevel();
        }

        private void UpdateColliders()
        {
        }

        public void DissolveLevel()
        {
            mr = GetComponent<MeshRenderer>();
            StartCoroutine(cDissolveLevel(LevelManager._instance.DissolveLevelDuration));
        }

        // updates the shader's slice amount each frame to fade out the level. Also fades the alpha color value.
        private IEnumerator cDissolveLevel(float duration)
        {
            if (mr != null)
            {
                yield return new WaitForSeconds(0.1F);
                Material m = mr.material;

                // begin color - full alpha
                Color c = m.GetColor("_Color");

                // end begin color - no alpha
                Color ca = new Color(c.r, c.g, c.b, 0F);

                float t = 0F;
                while (t < 1.0f)
                {
                    //float alpha = Mathf.Lerp(0.0, 1.0, lerp);
                    t += Time.deltaTime * (Time.timeScale / duration);
                    m.SetColor("_Color", Color.Lerp(c, ca, t));
                    m.SetFloat("_SliceAmount", t);
                    Debug.Log("Color: " + m.color);
                    yield return 0;
                }
            }
            else
                Debug.Log("Dissolving Level failed, moveAreaGo MeshRenderer not found.");
            yield break;
        }

        private IEnumerator cReverseDissolveLevel(float duration)
        {
            if (mr != null)
            {
                yield return new WaitForSeconds(0.1F);
                Material m = mr.material;
                float t = 0F;
                while (t < 1.0f)
                {
                    t += Time.deltaTime * (Time.timeScale / duration);
                    m.SetFloat("_SliceAmount", 1 - t);
                    yield return 0;
                }
            }
            else
                Debug.Log("Dissolving Level failed, moveAreaGo MeshRenderer not found.");
            yield break;
        }
    }
}