using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;

public class BuildMenuControler : PanelControl
{
    public override string Key => "BuildMenu";

    [SerializeField] List<WorkStationData> WorkStationDataSet;

    // UXML template for list entries
    [SerializeField] VisualTreeAsset ListEntryTemplate;

    [field: SerializeField] int FixedItemHeight{ get; set; }

    // UI element references
    Button closeBtn;
    ListView listView;

    protected override void Awake()
    {
        base.Awake();
        WorkStationDataSet = new();
        WorkStationDataSet.AddRange(
            Resources.LoadAll<WorkStationData>("WorkStationDataSet")
            .OrderByDescending(item=>item.Price)
            .ThenBy(item=>item.name)
        );

        closeBtn = Root.Q<Button>("close-btn");
        closeBtn.RegisterCallback<ClickEvent>(evt => BackToMainMenu());

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
            WorkStationListEntryController _itemdata = item.userData as WorkStationListEntryController;
            _itemdata.WorkStationData = WorkStationDataSet[index];// .SetCharacterData(AllCharacters[index]);
            if(_itemdata.WorkStationData.Price == 0)
                _itemdata.Locked();
        };

        // Set a fixed item height
        listView.fixedItemHeight = FixedItemHeight;

        // Set the actual item's source list/array
        listView.itemsSource = WorkStationDataSet;

    }
    
    private void Start()
    {
        Hide();
    }

    void BackToMainMenu()
    {
        Hide();
        UiEvents.instance.DisplayUiTriggerEvent("MenuGameplay");
    }
}

public class WorkStationListEntryController
{
    // state panel
    VisualElement ActiveStatePanel;
    VisualElement LockedStatePanel;

    // active components
    Button purchaseBtn;
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
        // state panel
        ActiveStatePanel = visualElement.Q<VisualElement>("active");
        LockedStatePanel = visualElement.Q<VisualElement>("locked");

        // active components
        purchaseBtn = visualElement.Q<Button>("purchase-btn");
        img = visualElement.Q<VisualElement>("img");
        nameTxt = visualElement.Q<Label>("name-txt");
        descriptionTxt = visualElement.Q<Label>("description-txt");
        priceTxt = visualElement.Q<Label>("price-txt");

        // active state at default
        ActiveStatePanel.style.display = DisplayStyle.Flex;
        LockedStatePanel.style.display = DisplayStyle.None;
        
        // bind purchase button
        purchaseBtn.RegisterCallback<ClickEvent>(evt => {
            if (workStationData == null)
                return;

            UiEvents.instance.HideUiTriggerEvent("BuildMenu");
            UiEvents.instance.DisplayUiTriggerEvent("BuildMode");
            BuildModeUiController.Instance.WorkStationData = WorkStationData;
            Debug.Log($"Enter build mode : {nameTxt.text}");
        });
    }

    public void Locked()
    {
        ActiveStatePanel.style.display = DisplayStyle.None;
        LockedStatePanel.style.display = DisplayStyle.Flex;
    }
}