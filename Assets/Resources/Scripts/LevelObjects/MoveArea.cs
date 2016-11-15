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

        private IEnumerator cDissolveLevel(float duration)
        {
            if (mr != null)
            {
                yield return new WaitForSeconds(0.1F);
                Material m = mr.material;
                float t = 0F;
                while (t < 1.0f)
                {
                    t += Time.deltaTime * (Time.timeScale / duration);
                    m.SetFloat("_SliceAmount", t);
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