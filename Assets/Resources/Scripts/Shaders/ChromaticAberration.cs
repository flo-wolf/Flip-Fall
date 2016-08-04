using System.Collections;
using UnityEngine;

//Sript copied/stolen, link at the end of the script
[ExecuteInEditMode]
public class ChromaticAberration : MonoBehaviour
{
    [Range(0f, 15f)]
    public float duration = 5f;                                 // How long the ChromoAbb effect will last

    public float speedMulti;                                    // Multiplier to increase the speed at which the effect happens

    [Header("Drag ChromoAbb Shader from Shaders Folder here")]  // Non-Field (Header Label Only)
    public Shader chromeAbShader;                              // Reference to the ChromoAbb Shader(drag from Assets/Shaders folder)

    private float ChromaticAberation = 1.0f;                   // This float is the value sent to the shader
    private float minTime = 0f;                                 // This is the minTime or starting time of the effect (leave at 0f)
    private float elapsed = 0f;                                 // This is the value modified by the coroutines. float ChromaticAbberation gets elapsed's value
    private Material curMaterial;                               // The currMaterial is the Chroma Ab Shader Material(hidden)
    private float maxTime;                                      // The maxTime that is used for ChromoAbb Effect(set vie code below)

    private Material material                                           // Accessor for material/currMaterial.
    {
        get
        {
            if (curMaterial == null)
            {
                curMaterial = new Material(chromeAbShader);
                curMaterial.hideFlags = HideFlags.HideAndDontSave;
            }
            return curMaterial;
        }
    }

    // Use this for initialization
    private void Start()
    {
        //if image effects are not supported disable this monobehaviour and Return
        if (!SystemInfo.supportsImageEffects)
        {
            enabled = false;
            return;
        }
        //set the initial value of the private float variable "maxTime" to the same as duration;
        maxTime = duration;
    }

    private void OnRenderImage(RenderTexture srcText, RenderTexture destText)
    {
        //if chromaticAbberation shader has been assigned in the inspector, it will then send the ChromaticAbberation var to _AberrationOffset property on the shader.
        //We use the Coroutines to adjust the "elapsed" float var over time.  we then assign the "elapsed" var to the "ChromaticAbberation" var so that when
        //StartAbberation() is called this behaviour keeps the "ChromaticAbberation" var the same value as the "elapsed" var.  So each pass through the while loops change the value.
        if (chromeAbShader != null)
        {
            //set property on shader... to our float value if chromeAbbShader != null.
            material.SetFloat("_AberrationOffset", ChromaticAberation);
            Graphics.Blit(srcText, destText, material);
        }
        else
        {
            Graphics.Blit(srcText, destText);
        }
    }

    private IEnumerator OverTime(/*float waitTime*/)
    {
        //"elapsed" time gets assigned "minTime"(the private "minTime" that is set to 0f).
        elapsed = minTime;
        //"duration" gets assigned "maxTime"(the "maxTime" we set in the inspector for the effects length).
        duration = maxTime;

        //while our "elapsed" time is less than our "duration"... do these things...
        while (elapsed < duration)
        {
            //increase "elapsed" by Time.deltaTime Multiplied by "speedMulti"
            elapsed += Time.deltaTime * speedMulti;

            //yield return null to finish execution this frame, and we will come back here again next frame and run the above
            //again as long as the while loop is still true/
            yield return null;
        }
        //If after Starting this coroutine, setting the value of "elapsed" to the "minTime", and we are
        //done with the while loop we used to Increase "elapsed". Now we just set "elapsed" to "maxTime"(since it
        //should be about there already anyway), and yield return StartCoroutine("UnderTime")...

        //After this OverTime coroutine has increased "elapsed" and the red,blue,yellow channels are spread out, we start the other
        //coroutine to bring them back down to zero.  Then every time OnRenderImage() gets called it has a new/updated value("elapsed")
        //to assign to the chroma ab property on the shader.

        //elapsed = maxTime;

        yield return StartCoroutine("UnderTime");
    }

    private IEnumerator UnderTime(/*float waitTime*/)
    {
        //"elapsed" time gets assigned "maxTime"(the maxTime we set in the inspector for the effect to run).
        elapsed = maxTime;

        //"duration" gets assigned "minTime"(the private field at the top that is given the value of 0f;
        duration = minTime;

        //while our "elapsed" time is greater than our "duration"... do these things...
        while (elapsed > duration)
        {
            //reduce "elapsed" by Time.deltaTime Multiplied by "speedMulti"
            elapsed -= Time.deltaTime * speedMulti;

            //yield return null to finish execution this frame, and we will come back here again next frame and run the above
            //as long as the while loop is still true/
            yield return null;
        }
        //if the after Starting this coroutine, and after setting the value of "elapsed" to the "maxTime", and we are
        //done with the while loop, then set "elapsed" to "minTime" (0f), and yield return null. We are done.

        //elapsed = minTime;

        yield return null;
    }

    // Update is called once per frame
    private void Update()
    {
        //Assigns the "elapsed" float to the "ChromaticAberation" float every frame.
        //The "ChromaticAbberation" float is then fed to the ImageEffect Shaders "_ChromAb" property
        //in the OnRenderTexture() Function;
        ChromaticAberation = elapsed;
    }

    public void StartAberration()
    {
        //stop coroutines for good measure
        StopAllCoroutines();
        //start the coroutine Overtime(which starts UnderTime(comes back))
        StartCoroutine("OverTime");
    }

    // This function is called when the behaviour becomes disabled or inactive
    public void OnDisable()
    {
        //if the curMaterial variable is active, and does not equal null
        if (curMaterial)
        {
            //then since this object is being disabled we call DestroyImmediate.  Since this code is set to execute in edit mode,
            //and because the delayed destroy is never reached, we use DestroyImmediate Instead... per DestroyImmediate() Docs
            // http://docs.unity3d.com/ScriptReference/Object.DestroyImmediate.html
            DestroyImmediate(curMaterial);
        }
    }
}