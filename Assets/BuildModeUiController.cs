using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BuildModeUiController : PanelControl
{
    public override string Key => "BuildMode";

    protected override void Awake()
    {
        base.Awake();
        List<VisualElement> btnList = new() {
            Root.Q<VisualElement>("confirm-btn"),
            Root.Q<VisualElement>("rotate-btn"),
            Root.Q<VisualElement>("cancle-btn"),
        };

        btnList.ForEach(btn => SetClickAnimation(btn, mainColor, onMouseDownColor));

        btnList[0].RegisterCallback<ClickEvent>(evt => OnConfirm(evt));
        btnList[1].RegisterCallback<ClickEvent>(evt => OnRatate(evt));
        btnList[2].RegisterCallback<ClickEvent>(evt => OnCancle(evt));

    }

    private void Start()
    {
        Hide();
    }

    protected override void Display()
    {
        base.Display();
        TileOverlay.Instance.Active();
    }

    protected override void Hide()
    {
        base.Hide();
        TileOverlay.Instance.Deactive();    
    }

    public void OnConfirm(ClickEvent clickEvent)
    {
        Hide();
        Debug.Log("confirm");
        UiEvents.instance.DisplayUiTriggerEvent("MenuGameplay");
    }

    public void OnRatate(ClickEvent clickEvent)
    {
        Debug.Log("rotate");
    }

    public void OnCancle(ClickEvent clickEvent)
    {
        Hide();
        Debug.Log("cancle");
        UiEvents.instance.DisplayUiTriggerEvent("BuildMenu");
    }


}
