using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;

public class BuildModeUiController : PanelControl
{
    public static BuildModeUiController Instance;
    public override string Key => "BuildMode";

    [SerializeField] Transform buildPreview;
    [SerializeField] InputActionReference actionReference;
    [SerializeField] Grid MainGrid { get; set; }
    SpriteRenderer SpriteRenderer { get; set; }
    [SerializeField] Transform WorkStationsTransform;
    WorkStationData workStationData;
    public WorkStationData WorkStationData { get => workStationData;
        set 
        {
            workStationData = value;
            SpriteRenderer.sprite = workStationData.sprite;
        }
    }

    protected override void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        base.Awake();
        List<VisualElement> btnList = new() {
            Root.Q<VisualElement>("confirm-btn"),
            Root.Q<VisualElement>("rotate-btn"),
            Root.Q<VisualElement>("cancle-btn"),
        };

        MainGrid = FindObjectOfType<Grid>();

        btnList.ForEach(btn => SetClickAnimation(btn, mainColor, onMouseDownColor));

        btnList[0].RegisterCallback<ClickEvent>(evt => OnConfirm(evt));
        btnList[1].RegisterCallback<ClickEvent>(evt => OnRatate(evt));
        btnList[2].RegisterCallback<ClickEvent>(evt => OnCancle(evt));

        if(buildPreview.TryGetComponent(out SpriteRenderer spriteRenderer))
        {
            SpriteRenderer = spriteRenderer;
        }
    }

    private void SetPreviewPosition(InputAction.CallbackContext ctx)
    {
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(ctx.ReadValue<Vector2>());
        var cellPos = MainGrid.WorldToCell(worldPoint);

        if (!TileOverlay.Instance.TilesPos.Contains(cellPos))
            return;

        buildPreview.position = MainGrid.GetCellCenterWorld(cellPos);

        if (buildPreview.gameObject.activeSelf == false)
            buildPreview.gameObject.SetActive(true);
    }

    private void Start()
    {
        Hide();
    }

    protected override void Display()
    {
        base.Display();
        TileOverlay.Instance.Active();
        actionReference.action.Enable();
        actionReference.action.performed += SetPreviewPosition;
    }

    protected override void Hide()
    {
        base.Hide();
        TileOverlay.Instance.Deactive();
        buildPreview.gameObject.SetActive(false);
        actionReference.action.Disable();
        actionReference.action.performed -= SetPreviewPosition;
    }

    public void OnConfirm(ClickEvent clickEvent)
    {
        if(!LevelManager.Instance.TrySpend(workStationData.price))
        {
            Debug.LogWarning("Coin not enough.");
            return;
        }

        Hide();
        Debug.Log("confirm");
        Instantiate(workStationData.prefab, buildPreview.position, Quaternion.identity, WorkStationsTransform);
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
