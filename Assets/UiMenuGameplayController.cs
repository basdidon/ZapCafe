using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UiMenuGameplayController : PanelControl
{
    [SerializeField] PanelControl buildMenuPanel;

    protected override void Awake()
    {
        base.Awake();
        Root.Q<Button>("build-menu-btn").clicked += GotoBuildMenu;
    }

    public void GotoBuildMenu()
    {
        Hide();
        buildMenuPanel.Display();
    }
}