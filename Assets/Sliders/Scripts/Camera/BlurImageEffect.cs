using System.Collections;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Image Effects/Blur_MOD")]
public class BlurImageEffect : MonoBehaviour
{
    public int iterations = 2;
    public static float i = 0;
    public bool fadeIn = false;
    public float fadeInSpeed = 2.0f;
    public float maxBlur = 3.0f;
    public float blurSpread = 0.0f;
    public Shader blurShader = null;
    private static Material m_Material = null;

    protected Material material
    {
        get
        {
            if (m_Material == null)
            {
                m_Material = new Material(blurShader);
                m_Material.hideFlags = HideFlags.DontSave;
            }
            return m_Material;
        }
    }

    protected void OnDisable()
    {
        if (m_Material)
        {
            DestroyImmediate(m_Material);
        }
    }

    private void FixedUpdate()
    {
        if (fadeIn)
        {
            blurSpread = i;
            i += fadeInSpeed * Time.smoothDeltaTime;
        }
        if (i >= maxBlur)
            i = maxBlur;
    }

    protected void Start()
    {
        if (!blurShader || !material.shader.isSupported)
        {
            enabled = false;
            return;
        }
    }

    public void FourTapCone(RenderTexture source, RenderTexture dest, int iteration)
    {
        float off = 0.5f + iteration * blurSpread;
        Graphics.BlitMultiTap(source, dest, material,
            new Vector2(-off, -off),
            new Vector2(-off, off),
            new Vector2(off, off),
            new Vector2(off, -off)
        );
    }

    private void DownSample4x(RenderTexture source, RenderTexture dest)
    {
        float off = 1.0f;
        Graphics.BlitMultiTap(source, dest, material,
            new Vector2(-off, -off),
            new Vector2(-off, off),
            new Vector2(off, off),
            new Vector2(off, -off)
        );
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        RenderTexture buffer = RenderTexture.GetTemporary(source.width / 4, source.height / 4, 0);
        RenderTexture buffer2 = RenderTexture.GetTemporary(source.width / 4, source.height / 4, 0);

        DownSample4x(source, buffer);

        bool oddEven = true;
        for (int i = 0; i < iterations; i++)
        {
            if (oddEven)
                FourTapCone(buffer, buffer2, i);
            else
                FourTapCone(buffer2, buffer, i);
            oddEven = !oddEven;
        }
        if (oddEven)
            Graphics.Blit(buffer, destination);
        else
            Graphics.Blit(buffer2, destination);

        RenderTexture.ReleaseTemporary(buffer);
        RenderTexture.ReleaseTemporary(buffer2);
    }
}