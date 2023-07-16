using Sirenix.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurgerBox : BoardObject, IWorkStation,IItemFactory,IUiObject
{
    // Worker
    public Worker Worker { get; set; }
    [field: SerializeField] public Transform WorkingPoint { get; set; }
    public Vector3Int WorkingCell { get => BoardManager.GetCellPos(WorkingPoint.position); }

    public string ItemName => "Burger";
    [SerializeField] int level = 1;
    public int Level { get => level; set => level = value; }
    /*
    private void Awake()
    {
        UiObjectManager.Instance.OnUiObjectActive += OnUiObjectActiveHandler;
    }
    */
    public void OnUiObjectActiveHandler(IUiObject uiObject)
    {
        if (Equals(uiObject))
        {
            ShowUiObject();
        }
        else
        {
            HideUiObject();
        }
    }

    private void Start()
    {
        WorkStationRegistry.Instance.AddWorkStation(this);
        UiObjectManager.Instance.AddUiObject(this);
        HideUiObject();
    }

    public GameObject UiObject;
    public void ShowUiObject()
    {
        UiObject.SetActive(true);
    }

    public void HideUiObject()
    {
        UiObject.SetActive(false);
    }
}
