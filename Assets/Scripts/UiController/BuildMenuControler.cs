using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class WorkStationListEntryController
{
    VisualElement button;
    VisualElement img;
    Label priceLabel;
    Label nameLabel;

    WorkStationData workStationData;
    public WorkStationData WorkStationData 
    {
        get => workStationData;
        set
        {
            workStationData = value;
            img.style.backgroundImage = new StyleBackground(workStationData.sprite);
            priceLabel.text = $"{workStationData.price} C";
            nameLabel.text = workStationData.name;
        }
    }

    //This function retrieves a reference to the 
    //character name label inside the UI element.
    public void SetVisualElement(VisualElement visualElement)
    {
        button = visualElement.Q<VisualElement>("btn");
        img = visualElement.Q<VisualElement>("img");
        priceLabel = visualElement.Q<Label>("price-label");
        nameLabel = visualElement.Q<Label>("name-label");

        button.RegisterCallback<ClickEvent>(evt => {
            UiEvents.instance.HideUiTriggerEvent("BuildMenu");
            UiEvents.instance.DisplayUiTriggerEvent("BuildMode");
            BuildModeUiController.Instance.WorkStationData = WorkStationData;
            Debug.Log($"Enter build mode : {nameLabel.text}");
        });
    }
}

public class BuildMenuControler : PanelControl
{
    public override string Key => "BuildMenu";

    [SerializeField] List<WorkStationData> WorkStationDataSet;

    // UXML template for list entries
    [SerializeField] VisualTreeAsset ListEntryTemplate;

    // UI element references
    VisualElement workStationListView;


    protected override void Awake()
    {
        base.Awake();
        WorkStationDataSet = new();
        WorkStationDataSet.AddRange(Resources.LoadAll<WorkStationData>("WorkStationDataSet"));

        workStationListView = Root.Q("build-menu-panel");

        WorkStationDataSet.ForEach(data => {
            var clone = ListEntryTemplate.Instantiate();
            SetClickAnimation(clone.Q<VisualElement>("btn"), mainColor, onMouseDownColor);
            var newListEntryLogic = new WorkStationListEntryController();
            //clone.userData = newListEntryLogic;
            newListEntryLogic.SetVisualElement(clone);
            newListEntryLogic.WorkStationData = data;
            workStationListView.Add(clone);
        });

        workStationListView.RegisterCallback<ClickEvent>(evt => Debug.Log("clicked"));
    }

    private void Start()
    {
        Hide();
    }
}