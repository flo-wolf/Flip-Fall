using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Displays a gird in the editor onto which objects and verticies can snap to.
/// </summary>
namespace FlipFall.Editor
{
    [ExecuteInEditMode]
    public class GridOverlay : MonoBehaviour
    {
        public Camera editorCam;
        public static GridOverlay _instance;

        public bool snapToGrid = true;
        public bool showGrid = true;
        public bool showMain = true;
        public bool showSub = true;

        public float largeStep = 100F;
        public float smallStep = 50F;

        public Color mainColor = new Color(0f, 1f, 0f, 1f);
        private Color gridColorBackup;

        public float fadeTime = 0.5F;

        // lower left view corner
        [HideInInspector]
        public Vector2 start;

        [HideInInspector]
        // upper right view corner
        public Vector2 end;

        // generated line material
        public Material lineMaterial;

        // Use this for initialization
        private void Start()
        {
            if (_instance == null)
                _instance = this;

            //end = new Vector2(start.x + viewSizeX, start.y + viewSizeY);

            //print(viewSizeX + " - " + viewSizeY + " - " + start + " - " + end);
            mainColor.a = 0.7F;

            gridColorBackup = mainColor;
            StartCoroutine(cFadeIn());
            Main.onSceneChange.AddListener(SceneChanged);
        }

        private void SceneChanged(Main.Scene s)
        {
            StartCoroutine(cFadeOut());
        }

        private IEnumerator cFadeIn()
        {
            float t = 0F;
            Color fadedGridColor = gridColorBackup;
            fadedGridColor.a = 0;
            while (t < 1.0f)
            {
                t += Time.deltaTime * (Time.timeScale / fadeTime);
                mainColor = Color.Lerp(fadedGridColor, gridColorBackup, t);
                yield return 0;
            }
            yield break;
        }

        private IEnumerator cFadeOut()
        {
            float t = 0F;
            Color fadedGridColor = gridColorBackup;
            fadedGridColor.a = 0;
            while (t < 1.0f)
            {
                t += Time.deltaTime * (Time.timeScale / fadeTime);
                mainColor = Color.Lerp(gridColorBackup, fadedGridColor, t);
                yield return 0;
            }
            yield break;
        }

        // swtich grid active/deactive
        public static void Active(bool b)
        {
            if (b)
            {
                _instance.showGrid = true;
            }
            else
            {
                _instance.showGrid = false;
            }
        }

        private void CreateLineMaterial()
        {
            //// Unity has a built-in shader that is useful for drawing
            //// simple colored things.
            //var shader = Shader.Find("UI/Default");
            //lineMaterial = new Material(shader);
            ////lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            ////// Turn on alpha blending
            //lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            //lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            ////// Turn backface culling off
            ////lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            ////// Turn off depth writes
            lineMaterial.SetInt("_ZWrite", 1);
            lineMaterial.SetInt("_ZTest", (int)UnityEngine.Rendering.CompareFunction.LessEqual);
        }

        private void OnDrawGizmos()
        {
            OnPostRender();
        }

        private void CalcStart()
        {
            start = editorCam.ViewportToWorldPoint(new Vector3(0, 0, 1));
            end = editorCam.ViewportToWorldPoint(new Vector3(1, 1, 1));

            // adjust grid for worldspace snapping
            start.x = smallStep * Mathf.Round(start.x / smallStep);
            start.y = smallStep * Mathf.Round(start.y / smallStep);
            end.x = smallStep * Mathf.Round(end.x / smallStep);
            end.y = smallStep * Mathf.Round(end.y / smallStep);

            // widen grid to allow grid-dissappearing-free camera movements
            start.x -= smallStep;
            start.y -= smallStep;
            end.x += smallStep;
            end.y += smallStep;
        }

        private void OnPostRender()
        {
            if (showGrid && smallStep > 10)
            {
                CalcStart();
                CreateLineMaterial();
                lineMaterial.SetPass(0);

                GL.Begin(GL.LINES);

                if (showMain && largeStep != 0)
                {
                    GL.Color(mainColor);

                    // add here LargeStep lines
                }

                if (showSub)
                {
                    GL.Color(mainColor);
                    // SmallStep Lines
                    for (float j = start.y; j - smallStep <= end.y; j += smallStep)
                    {
                        // Horizontal Lines
                        GL.Vertex3(start.x, j, 500);
                        GL.Vertex3(end.x, j, 500);

                        // Vertical Lines
                        for (float i = 0; i <= end.x - start.x; i += smallStep)
                        {
                            if (j <= end.y)
                            {
                                GL.Vertex3(start.x + i, j, 500);
                                GL.Vertex3(start.x + i, j + smallStep, 500);
                            }
                        }
                    }
                }
                GL.End();
            }
        }
    }
}