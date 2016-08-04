using System.Collections;
using UnityEngine;

[ExecuteInEditMode]
public class RGBSplit : MonoBehaviour
{
    #region Variables

    public Shader curShader;
    public float ChromaticAbberation = 1.0f;
    private Material curMaterial;

    #endregion Variables

    #region Properties

    private Material material
    {
        get
        {
            if (curMaterial == null)
            {
                curMaterial = new Material(curShader);
                curMaterial.hideFlags = HideFlags.HideAndDontSave;
            }
            return curMaterial;
        }
    }

    #endregion Properties

    // Use this for initialization
    private void Start()
    {
        if (!SystemInfo.supportsImageEffects)
        {
            enabled = false;
            return;
        }
    }

    private void OnRenderImage(RenderTexture sourceTexture, RenderTexture destTexture)
    {
        if (curShader != null)
        {
            material.SetFloat("_AberrationOffset", ChromaticAbberation);
            Graphics.Blit(sourceTexture, destTexture, material);
        }
        else
        {
            Graphics.Blit(sourceTexture, destTexture);
        }
    }

    // Update is called once per frame
    private void Update()
    {
    }

    private void OnDisable()
    {
        if (curMaterial)
        {
            DestroyImmediate(curMaterial);
        }
    }
}