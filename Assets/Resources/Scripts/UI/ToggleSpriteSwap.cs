using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// Switches sprites when the pause/resume toggle gets clicked.
/// Default toogle sprite switches dont work in unscaled time, which is something we have on game pausing. This is thus the workaround.

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