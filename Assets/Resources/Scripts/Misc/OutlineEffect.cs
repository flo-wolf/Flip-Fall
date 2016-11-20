using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class OutlineEffect : MonoBehaviour
{
    private List<Outline> outlines = new List<Outline>();

    public Camera sourceCamera;
    public Camera outlineCamera;

    public float lineThickness = 4f;
    public float lineIntensity = .5f;

    public Color lineColor0 = Color.red;
    public Color lineColor1 = Color.green;
    public Color lineColor2 = Color.blue;
    public bool flipY = false;
    public bool additiveRendering = true;
    public float alphaCutoff = .5f;

    private Material outline1Material;
    private Material outline2Material;
    private Material outline3Material;
    private Material outlineEraseMaterial;
    private Shader outlineShader;
    private Shader outlineBufferShader;
    private Material outlineShaderMaterial;
    private RenderTexture renderTexture;

    private Material GetMaterialFromID(int ID)
    {
        if (ID == 0)
            return outline1Material;
        else if (ID == 1)
            return outline2Material;
        else
            return outline3Material;
    }

    private Material CreateMaterial(Color emissionColor)
    {
        Material m = new Material(outlineBufferShader);
        m.SetColor("_Color", emissionColor);
        m.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        m.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        m.SetInt("_ZWrite", 0);
        m.DisableKeyword("_ALPHATEST_ON");
        m.EnableKeyword("_ALPHABLEND_ON");
        m.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        m.renderQueue = 3000;
        return m;
    }

    private void Start()
    {
        CreateMaterialsIfNeeded();
        UpdateMaterialsPublicProperties();

        if (sourceCamera == null)
        {
            sourceCamera = GetComponent<Camera>();

            if (sourceCamera == null)
                sourceCamera = Camera.main;
        }

        if (outlineCamera == null)
        {
            GameObject cameraGameObject = new GameObject("Outline Camera");
            cameraGameObject.transform.parent = sourceCamera.transform;
            outlineCamera = cameraGameObject.AddComponent<Camera>();
        }

        renderTexture = new RenderTexture(sourceCamera.pixelWidth, sourceCamera.pixelHeight, 16, RenderTextureFormat.Default);
        UpdateOutlineCameraFromSource();
    }

    private void OnDestroy()
    {
        renderTexture.Release();
        DestroyMaterials();
    }

    private void OnPreCull()
    {
        if (renderTexture.width != sourceCamera.pixelWidth || renderTexture.height != sourceCamera.pixelHeight)
        {
            renderTexture = new RenderTexture(sourceCamera.pixelWidth, sourceCamera.pixelHeight, 16, RenderTextureFormat.Default);
            outlineCamera.targetTexture = renderTexture;
        }
        UpdateMaterialsPublicProperties();
        UpdateOutlineCameraFromSource();

        if (outlines != null)
        {
            for (int i = 0; i < outlines.Count; i++)
            {
                if (outlines[i] != null)
                {
                    outlines[i].originalMaterial = outlines[i].GetComponent<Renderer>().sharedMaterial;
                    outlines[i].originalLayer = outlines[i].gameObject.layer;

                    if (outlines[i].eraseRenderer)
                        outlines[i].GetComponent<Renderer>().sharedMaterial = outlineEraseMaterial;
                    else
                        outlines[i].GetComponent<Renderer>().sharedMaterial = GetMaterialFromID(outlines[i].color);

                    if (outlines[i].GetComponent<Renderer>() is MeshRenderer)
                    {
                        outlines[i].GetComponent<Renderer>().sharedMaterial.mainTexture = outlines[i].originalMaterial.mainTexture;
                    }

                    outlines[i].gameObject.layer = LayerMask.NameToLayer("Outline");
                }
            }
        }

        outlineCamera.Render();

        if (outlines != null)
        {
            for (int i = 0; i < outlines.Count; i++)
            {
                if (outlines[i] != null)
                {
                    if (outlines[i].GetComponent<Renderer>() is MeshRenderer)
                        outlines[i].GetComponent<Renderer>().sharedMaterial.mainTexture = null;

                    outlines[i].GetComponent<Renderer>().sharedMaterial = outlines[i].originalMaterial;
                    outlines[i].gameObject.layer = outlines[i].originalLayer;
                }
            }
        }
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        outlineShaderMaterial.SetTexture("_OutlineSource", renderTexture);
        Graphics.Blit(source, destination, outlineShaderMaterial);
    }

    private void CreateMaterialsIfNeeded()
    {
        if (outlineShader == null)
            outlineShader = Resources.Load<Shader>("OutlineEffect/OutlineShader");
        if (outlineBufferShader == null)
            outlineBufferShader = Resources.Load<Shader>("OutlineEffect/OutlineBufferShader");
        if (outlineShaderMaterial == null)
        {
            outlineShaderMaterial = new Material(outlineShader);
            outlineShaderMaterial.hideFlags = HideFlags.HideAndDontSave;
            UpdateMaterialsPublicProperties();
        }
        if (outlineEraseMaterial == null)
            outlineEraseMaterial = CreateMaterial(new Color(0, 0, 0, 0));
        if (outline1Material == null)
            outline1Material = CreateMaterial(new Color(1, 0, 0, 0));
        if (outline2Material == null)
            outline2Material = CreateMaterial(new Color(0, 1, 0, 0));
        if (outline3Material == null)
            outline3Material = CreateMaterial(new Color(0, 0, 1, 0));

        outline1Material.SetFloat("_AlphaCutoff", alphaCutoff);
        outline2Material.SetFloat("_AlphaCutoff", alphaCutoff);
        outline3Material.SetFloat("_AlphaCutoff", alphaCutoff);
    }

    private void DestroyMaterials()
    {
        DestroyImmediate(outlineShaderMaterial);
        DestroyImmediate(outlineEraseMaterial);
        DestroyImmediate(outline1Material);
        DestroyImmediate(outline2Material);
        DestroyImmediate(outline3Material);
        outlineShader = null;
        outlineBufferShader = null;
        outlineShaderMaterial = null;
        outlineEraseMaterial = null;
        outline1Material = null;
        outline2Material = null;
        outline3Material = null;
    }

    private void UpdateMaterialsPublicProperties()
    {
        if (outlineShaderMaterial)
        {
            outlineShaderMaterial.SetFloat("_LineThicknessX", lineThickness / 1000);
            outlineShaderMaterial.SetFloat("_LineThicknessY", lineThickness / 1000);
            outlineShaderMaterial.SetFloat("_LineIntensity", lineIntensity);
            outlineShaderMaterial.SetColor("_LineColor1", lineColor0);
            outlineShaderMaterial.SetColor("_LineColor2", lineColor1);
            outlineShaderMaterial.SetColor("_LineColor3", lineColor2);
            if (flipY)
                outlineShaderMaterial.SetInt("_FlipY", 1);
            else
                outlineShaderMaterial.SetInt("_FlipY", 0);
            if (!additiveRendering)
                outlineShaderMaterial.SetInt("_Dark", 1);
            else
                outlineShaderMaterial.SetInt("_Dark", 0);
        }
    }

    private void UpdateOutlineCameraFromSource()
    {
        outlineCamera.CopyFrom(sourceCamera);
        outlineCamera.renderingPath = RenderingPath.Forward;
        outlineCamera.backgroundColor = new Color(0.0f, 0.0f, 0.0f, 0.0f);
        outlineCamera.clearFlags = CameraClearFlags.SolidColor;
        outlineCamera.cullingMask = LayerMask.GetMask("Outline");
        outlineCamera.rect = new Rect(0, 0, 1, 1);
        outlineCamera.enabled = true;
        outlineCamera.targetTexture = renderTexture;
    }

    public void AddOutline(Outline outline)
    {
        if (!outlines.Contains(outline))
        {
            outlines.Add(outline);
        }
    }

    public void RemoveOutline(Outline outline)
    {
        outlines.Remove(outline);
    }
}