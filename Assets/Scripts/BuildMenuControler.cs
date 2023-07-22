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

    //This function retrieves a reference to the 
    //character name label inside the UI element.
    public void SetVisualElement(VisualElement visualElement)
    {
        button = visualElement.Q<VisualElement>("btn");
        img = visualElement.Q<VisualElement>("img");
        priceLabel = visualElement.Q<Label>("price-label");
        nameLabel = visualElement.Q<Label>("name-label");
    }

    //This function receives the character whose name this list 
    //element displays. Since the elements listed 
    //in a `ListView` are pooled and reused, it's necessary to 
    //have a `Set` function to change which character's data to display.

    public void SetCharacterData(WorkStationData workStationData)
    {
        img.style.backgroundImage = new StyleBackground(workStationData.Sprite);
        priceLabel.text = $"{workStationData.Price} C";
        nameLabel.text = workStationData.name;
    }
}

public class BuildMenuControler : PanelControl
{
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
            var newListEntryLogic = new WorkStationListEntryController();
            clone.userData = newListEntryLogic;
            newListEntryLogic.SetVisualElement(clone);
            newListEntryLogic.SetCharacterData(data);
            workStationListView.Add(clone);
        });

        workStationListView.RegisterCallback<ClickEvent>(evt => Debug.Log("clicked"));
    }

    private void Start()
    {
        Hide();
    }
}