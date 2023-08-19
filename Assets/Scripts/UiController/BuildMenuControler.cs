using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BuildMenuControler : PanelControl
{
    public override string Key => "BuildMenu";

    [SerializeField] List<WorkStationData> WorkStationDataSet;

    // UXML template for list entries
    [SerializeField] VisualTreeAsset ListEntryTemplate;

    // UI element references
    ListView listView;

    protected override void Awake()
    {
        base.Awake();
        WorkStationDataSet = new();
        WorkStationDataSet.AddRange(Resources.LoadAll<WorkStationData>("WorkStationDataSet"));

        //workStationListView = Root.Q<ListView>("build-menu-listview");
        listView = Root.Q<ListView>();
        listView.makeItem = () =>
         {
             var clone = ListEntryTemplate.Instantiate();
             var newListEntryLogic = new WorkStationListEntryController();
             clone.userData = newListEntryLogic;
             newListEntryLogic.SetVisualElement(clone);
             return clone;
         };

        // Set up bind function for a specific list entry
        listView.bindItem = (item, index) =>
        {
            (item.userData as WorkStationListEntryController).WorkStationData = WorkStationDataSet[index];// .SetCharacterData(AllCharacters[index]);
        };

        // Set a fixed item height
        listView.fixedItemHeight = 500;

        // Set the actual item's source list/array
        listView.itemsSource = WorkStationDataSet;

    }
    /*
    private void Start()
    {
        Hide();
    }*/
}

public class WorkStationListEntryController
{
    VisualElement purchaseBtn;
    VisualElement img;
    Label nameTxt;
    Label priceTxt;
    Label descriptionTxt;

    WorkStationData workStationData;
    public WorkStationData WorkStationData
    {
        get => workStationData;
        set
        {
            workStationData = value;
            img.style.backgroundImage = new StyleBackground(workStationData.Sprite);
            nameTxt.text = workStationData.name;
            descriptionTxt.text = workStationData.Description;
            priceTxt.text = $"{workStationData.Price} C";
        }
    }

    //This function retrieves a reference to the 
    //character name label inside the UI element.
    public void SetVisualElement(VisualElement visualElement)
    {
        purchaseBtn = visualElement.Q<VisualElement>("purchase-btn");
        img = visualElement.Q<VisualElement>("img");
        nameTxt = visualElement.Q<Label>("name-txt");
        descriptionTxt = visualElement.Q<Label>("description-txt");
        priceTxt = visualElement.Q<Label>("price-txt");
        /*
        purchaseBtn.RegisterCallback<ClickEvent>(evt => {
            UiEvents.instance.HideUiTriggerEvent("BuildMenu");
            UiEvents.instance.DisplayUiTriggerEvent("BuildMode");
            BuildModeUiController.Instance.WorkStationData = WorkStationData;
            Debug.Log($"Enter build mode : {nameTxt.text}");
        });*/
    }
}