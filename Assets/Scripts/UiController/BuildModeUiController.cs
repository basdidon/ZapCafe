using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using System;

public class BuildModeUiController : PanelControl
{
    public static BuildModeUiController Instance;
    public override string Key => "BuildMode";

    public UIDocument uIDoc;
    VisualElement root;

    IsometricDirections direction = IsometricDirections.NegativeX;

    [SerializeField] Transform buildPreview;
    SpriteRenderer SpriteRenderer { get; set; }

    [SerializeField] Transform workingCellPreview;
    SpriteRenderer WorkingCellRenderer { get; set; }
    [field: SerializeField] public Sprite WorkingCellOverlaySprite { get; private set; }

    // Input
    [field: SerializeField] public InputActionReference TouchPosInputRef { get; set; }
    InputAction TouchPosAction => TouchPosInputRef.action;

    Grid MainGrid => BoardManager.Instance.MainGrid;

    [SerializeField] Transform WorkStationsTransform;
    WorkStationData workStationData;
    public WorkStationData WorkStationData { get => workStationData;
        set
        {
            workStationData = value;
            SpriteRenderer.sprite = WorkStationData.GetPreviewSprite(direction);
        }
    }

    //  ui element refs
    VisualElement buildMenuPanel;
    Button confirmBtn;
    Button rotateBtn;
    Button cancleBtn;

    [field: SerializeField] Vector2 Offset { get; set; }
    [field: SerializeField] Vector2 CenterOffset { get; set; }

    protected override void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        base.Awake();

        root = uIDoc.rootVisualElement;

        buildMenuPanel = Root.Q<VisualElement>("build-menu-panel");
        confirmBtn = Root.Q<Button>("confirm-btn");
        rotateBtn = Root.Q<Button>("rotate-btn");
        cancleBtn = Root.Q<Button>("cancle-btn");

        confirmBtn.RegisterCallback<ClickEvent>(evt => OnConfirm(evt));
        rotateBtn.RegisterCallback<ClickEvent>(evt => OnRatate(evt));
        cancleBtn.RegisterCallback<ClickEvent>(evt => OnCancle(evt));

        if (buildPreview.TryGetComponent(out SpriteRenderer spriteRenderer))
        {
            SpriteRenderer = spriteRenderer;
        }

        if (workingCellPreview.TryGetComponent(out SpriteRenderer renderer))
        {
            WorkingCellRenderer = renderer;
        }
    }

    Vector3Int previewCell;
    Vector3Int PreviewCell {
        get => previewCell;
        set
        {
            if (previewCell != value)
            {
                previewCell = value;

                SpriteRenderer.color = BoardManager.Instance.IsBuildableCell(previewCell) ? Color.green : Color.red;
                WorkingCellRenderer.color = BoardManager.Instance.IsBuildableCell(WorkingCell)?Color.green:Color.red;

                //confirmBtn.style.display = IsBuildable ? DisplayStyle.Flex : DisplayStyle.None;
                confirmBtn.SetEnabled(IsBuildable);

                buildPreview.position = MainGrid.GetCellCenterWorld(PreviewCell);
                workingCellPreview.position = MainGrid.GetCellCenterWorld(WorkingCell);

                CenterOffset = -new Vector2(buildMenuPanel.resolvedStyle.width, buildMenuPanel.resolvedStyle.height) / 2 + Offset;
                buildMenuPanel.transform.position = RuntimePanelUtils.CameraTransformWorldToPanel(root.panel, buildPreview.position, Camera.main) + CenterOffset;

                if (buildPreview.gameObject.activeSelf == false)
                    buildPreview.gameObject.SetActive(true);

                if (workingCellPreview.gameObject.activeSelf == false)
                    workingCellPreview.gameObject.SetActive(true);
            }
        }
    }

    Vector3Int WorkingCell => previewCell + WorkStationData.GetWorkingCellLocal(direction);

    private void SetPreviewPosition(InputAction.CallbackContext ctx)
    {
        if (UItoolkitRayCastBlocker.IsPointerOverBlockers(ctx.ReadValue<Vector2>()))
        {
            Debug.Log("raycast blocked");
            return;
        }

        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(ctx.ReadValue<Vector2>());
        var cellPos = MainGrid.WorldToCell(worldPoint);

        if (BoardManager.Instance.WorkerArea.HasTile(cellPos))
            PreviewCell = cellPos;
    }

    private void Start()
    {
        Hide();
    }

    protected override void Display()
    {
        base.Display();
        TileOverlay.Instance.Active();
        TouchPosAction.Enable();
        TouchPosAction.performed += SetPreviewPosition;
        direction = IsometricDirections.NegativeX;
    }

    protected override void Hide()
    {
        base.Hide();
        TileOverlay.Instance.Deactive();
        buildPreview.gameObject.SetActive(false);
        workingCellPreview.gameObject.SetActive(false);
        TouchPosAction.Disable();
        TouchPosAction.performed -= SetPreviewPosition;
    }

    public void OnConfirm(ClickEvent clickEvent)
    {
        if (!LevelManager.Instance.TrySpend(WorkStationData.Price))
        {
            Debug.LogWarning("Coin not enough.");
            return;
        }

        Hide();
        Debug.Log("confirm");
        var go = new GameObject(workStationData.name);
        go.transform.position = buildPreview.position;
        go.transform.parent = WorkStationsTransform;
        go.AddComponent<SpriteRenderer>();
        var sortingGroup = go.AddComponent<SortingGroup>();
        sortingGroup.sortingLayerName = "Object";

        if (WorkStationData.name != "Bar")
        {
            var itemFactory = go.AddComponent<ItemFactory>();
            itemFactory.Initialize(WorkStationData, direction);
        }
        else
        {
            throw new System.NotImplementedException();
        }

        UiEvents.instance.DisplayUiTriggerEvent("MenuGameplay");
    }

    public void OnRatate(ClickEvent clickEvent)
    {
        direction += 1;
        if (Enum.GetNames(typeof(IsometricDirections)).Length <= (int)direction)
        {
            direction = 0;
        }

        SpriteRenderer.sprite = WorkStationData.GetPreviewSprite(direction);
        WorkingCellRenderer.color = BoardManager.Instance.IsBuildableCell(WorkingCell) ? Color.green : Color.red;
        workingCellPreview.position = MainGrid.GetCellCenterWorld(WorkingCell);

        //confirmBtn.style.display = IsBuildable ? DisplayStyle.Flex : DisplayStyle.None;
        confirmBtn.SetEnabled(IsBuildable);

        Debug.Log("rotate");
    }

    public void OnCancle(ClickEvent clickEvent)
    {
        Hide();
        Debug.Log("cancle");
        UiEvents.instance.DisplayUiTriggerEvent("BuildMenu");
    }

    bool IsBuildable => BoardManager.Instance.IsBuildableCell(previewCell) && BoardManager.Instance.IsBuildableCell(WorkingCell);
}
