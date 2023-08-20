using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UiMenuGameplayController : PanelControl
{
    [SerializeField] PanelControl buildMenuPanel;
    public override string Key => "MenuGameplay";

    Button buildMenuBtn;
    Button workerMenuBtn;

    protected override void Awake()
    {
        base.Awake();
        buildMenuBtn = Root.Q<Button>("build-menu-btn");
        workerMenuBtn = Root.Q<Button>("worker-menu-btn");

        buildMenuBtn.RegisterCallback<ClickEvent>(evt=> GotoBuildMenu());
        workerMenuBtn.RegisterCallback<ClickEvent>(evt => Debug.Log("worker-btn"));
    }

    public void GotoBuildMenu()
    {
        Hide();
        UiEvents.instance.DisplayUiTriggerEvent("BuildMenu");
    }
}