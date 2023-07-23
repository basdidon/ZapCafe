using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UiMenuGameplayController : PanelControl
{
    [SerializeField] PanelControl buildMenuPanel;
    public override string Key => "MenuGameplay";

    protected override void Awake()
    {
        base.Awake();
        List <VisualElement> btnList = new() 
        {
            Root.Q<VisualElement>("build-menu-btn"),
            Root.Q<VisualElement>("worker-menu-btn"),
        };

        btnList[0].RegisterCallback<ClickEvent>(evt=> GotoBuildMenu());
        btnList[1].RegisterCallback<ClickEvent>(evt => Debug.Log("worker-btn"));

        btnList.ForEach(btn => SetClickAnimation(btn, mainColor, onMouseDownColor));
    }

    public void GotoBuildMenu()
    {
        Hide();
        UiEvents.instance.DisplayUiTriggerEvent("BuildMenu");
    }
}