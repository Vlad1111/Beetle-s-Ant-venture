using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuBehaviour : MonoBehaviour
{
    public static MenuBehaviour Instance;
    private void Awake()
    {
        Instance = this;
    }

    public Transform menuParent;

    public Transform settingsTab;
    public Transform instruction;
    public Transform creditsTab;
    public Transform quitGameTab;

    public Image endCutscene;

    private void Start()
    {
        if(menuParent.gameObject.activeSelf)
            menuParent.gameObject.SetActive(false);
        ShowSettings();
        Cursor.visible = false;
    }

    public bool IsMenuOpened()
    {
        return menuParent.gameObject.activeSelf;
    }

    public void ToggleMenuOnOff()
    {
        menuParent.gameObject.SetActive(!menuParent.gameObject.activeSelf);
        Cursor.visible = menuParent.gameObject.activeSelf;
    }

    private void CloseAllTabs()
    {
        settingsTab.gameObject.SetActive(false);
        creditsTab.gameObject.SetActive(false);
        instruction.gameObject.SetActive(false);
        quitGameTab.gameObject.SetActive(false);
    }

    public void ShowSettings()
    {
        CloseAllTabs();
        settingsTab.gameObject.SetActive(true);
    }

    public void ShowInstructions()
    {
        CloseAllTabs();
        instruction.gameObject.SetActive(true);
    }

    public void ShowCredits()
    {
        CloseAllTabs();
        creditsTab.gameObject.SetActive(true);
    }

    public void ShowQuitGamePopup()
    {
        CloseAllTabs();
        quitGameTab.gameObject.SetActive(true);
    }

    public void QuiteGame()
    {
        Application.Quit();
    }

    private float endCutsceneAlpha = -1f;
    public void ShowEndCutscene()
    {
        endCutsceneAlpha = 0;
        endCutscene.gameObject.SetActive(true);
    }

    private void Update()
    {
        if(endCutsceneAlpha >= 0f && endCutsceneAlpha < 1)
        {
            endCutsceneAlpha += Time.deltaTime;
            if (endCutsceneAlpha > 1)
                endCutsceneAlpha = 1;

            endCutscene.color = new Color(0, 0, 0, endCutsceneAlpha);
        }
    }
}
