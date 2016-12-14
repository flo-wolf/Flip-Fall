using FlipFall.Audio;
using FlipFall.Levels;
using FlipFall.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles the UI Input and Animations for the LevelEditor Selection level settings menu.
/// </summary>

public class UILevelSettings : MonoBehaviour
{
    public static UILevelSettings _instance;
    private static LevelData editData;
    private static UIScrollElement editElement;
    public Animator animator;

    public InputField titleInput;
    public InputField authorInput;

    private void Start()
    {
        if (_instance == null)
            _instance = this;
    }

    public static void Show(LevelData levelData, UIScrollElement scrollElement)
    {
        editElement = scrollElement;
        editData = levelData;
        Debug.Log("title: " + levelData.title);
        _instance.titleInput.text = levelData.title;
        _instance.authorInput.text = levelData.author;
        _instance.animator.ResetTrigger("fadeIn");
        _instance.animator.ResetTrigger("fadeOut");
        _instance.animator.SetTrigger("fadeIn");
    }

    // leave the settings and restore the saved leveldata
    public void LeaveUnsavedButton()
    {
        animator.ResetTrigger("fadeIn");
        animator.ResetTrigger("fadeOut");
        animator.SetTrigger("fadeOut");
        SoundManager.ButtonClicked();

        StartCoroutine(cDelayedEditorLevelsShow());
    }

    // leave the settings and save all changes
    public void LeaveSaveButton()
    {
        animator.ResetTrigger("fadeIn");
        animator.ResetTrigger("fadeOut");
        animator.SetTrigger("fadeOut");
        SoundManager.ButtonClicked();

        editData.title = titleInput.text;
        editData.author = authorInput.text;
        LevelLoader.SaveCustomLevel(editData);
        LevelManager.customLevels = LevelLoader.LoadCustomLevels();
        DestroyImmediate(editElement.gameObject);
        UIEditorSelection._instance.LoadUIEditorLevels();
        //UIScrollFade._instance.UpdateScrollElements();
        StartCoroutine(cDelayedEditorLevelsShow());
    }

    private IEnumerator cDelayedEditorLevelsShow()
    {
        yield return new WaitForSeconds(0.25F);
        UIScrollFade._instance.UpdateScrollElements();
        yield break;
    }

    // the delete button got pressed, open the yes/no menu
    public void DeleteRequest()
    {
        animator.ResetTrigger("deleteRequest");
        animator.SetTrigger("deleteRequest");
        SoundManager.ButtonClicked();
    }

    // the duplicate button got pressed, open the yes/no menu
    public void DuplicateButton()
    {
        UIEditorSelection._instance.DuplicateLevel(editElement.GetComponent<UIEditorLevel>());
        animator.SetTrigger("fadeOut");
        SoundManager.ButtonClicked();
    }

    // yes got pressed, delete the level
    public void DeleteYes()
    {
        animator.ResetTrigger("deleteYes");
        animator.SetTrigger("deleteYes");
        animator.SetTrigger("fadeOut");
        SoundManager.ButtonClicked();

        DestroyImmediate(editElement.gameObject);
        LevelLoader.DeleteCustomLevel(editData);
        LevelManager.customLevels = LevelLoader.LoadCustomLevels();
        UIEditorSelection._instance.LoadUIEditorLevels();
        //UIScrollFade.scrollElements.Remove(editElement);
        //UIScrollFade._instance.UpdateScrollElements();
        UIScrollRectContentResizer.onContentChange.Invoke();
        StartCoroutine(cDelayedEditorLevelsShow());
    }

    // no got pressed, don't delete the level
    public void DeleteNo()
    {
        animator.ResetTrigger("deleteNo");
        animator.SetTrigger("deleteNo");
        SoundManager.ButtonClicked();
    }
}