using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DynamicScrollFitter : MonoBehaviour
{
    public float contentElementHeight;
    public RectTransform content;
    public VerticalLayoutGroup layoutGroup;

    private RectOffset padding;
    private Vector2 newScale;

    // Use this for initialization
    private void Start()
    {
    }

    private void OnGUI()
    {
        newScale = content.sizeDelta;
        newScale.y = layoutGroup.padding.bottom + ((contentElementHeight + (layoutGroup.spacing)) * transform.childCount);
        content.sizeDelta = newScale;
    }
}