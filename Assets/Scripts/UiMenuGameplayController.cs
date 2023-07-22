using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UiMenuGameplayController : PanelControl
{
    [SerializeField] PanelControl buildMenuPanel;
    VisualElement btn;

    protected override void Awake()
    {
        base.Awake();
        btn = Root.Q<VisualElement>("build-menu-btn");
        //btn.RegisterCallback<ClickEvent>(evt=>evt.currentTarget);
    }

    public void GotoBuildMenu()
    {
        btn.style.backgroundColor = new StyleColor(Color.black);
        /*
        Hide();
        buildMenuPanel.Display();
        */
    }
}