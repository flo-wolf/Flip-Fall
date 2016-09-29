using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ToggleSpriteSwap : MonoBehaviour
{
    public Toggle targetToggle;
    public Sprite selectedSprite;

    // Use this for initialization
    private void Start()
    {
        targetToggle.toggleTransition = Toggle.ToggleTransition.None;
        targetToggle.onValueChanged.AddListener(OnTargetToggleValueChanged);
    }

    private void OnTargetToggleValueChanged(bool newValue)
    {
        Image targetImage = targetToggle.targetGraphic as Image;
        if (targetImage != null)
        {
            if (newValue)
            {
                targetImage.overrideSprite = selectedSprite;
            }
            else {
                targetImage.overrideSprite = null;
            }
        }
    }
}