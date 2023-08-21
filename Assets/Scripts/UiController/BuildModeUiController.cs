using System.Collections.Generic;
using UnityEngine;
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
    GameObject previewWorkingCell;
    
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
        if(Instance != null && Instance != this)
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

        if(buildPreview.TryGetComponent(out SpriteRenderer spriteRenderer))
        {
            SpriteRenderer = spriteRenderer;
        }
    }

    Vector3Int previewCell;
    Vector3Int PreviewCell {
        get => previewCell;
        set
        {
            if(previewCell != value)
            {
                previewCell = value;

                if (BoardManager.Instance.IsBuildableCell(previewCell))
                {
                    SpriteRenderer.color = Color.green;
                    confirmBtn.style.display = DisplayStyle.Flex;
                }
                else
                {
                    SpriteRenderer.color = Color.red;
                    confirmBtn.style.display = DisplayStyle.None;
                }
                /*
                if (!BoardManager.Instance.IsBuildableCell(PreviewCell) || !BoardManager.Instance.IsBuildableCell(PreviewCell + WorkStationData.WorkingCellLocal))
                    return;
                */
                buildPreview.position = MainGrid.GetCellCenterWorld(PreviewCell);

                CenterOffset = -new Vector2(buildMenuPanel.resolvedStyle.width, buildMenuPanel.resolvedStyle.height) / 2 + Offset;
                buildMenuPanel.transform.position = RuntimePanelUtils.CameraTransformWorldToPanel(root.panel, buildPreview.position, Camera.main) + CenterOffset;

                if (buildPreview.gameObject.activeSelf == false)
                    buildPreview.gameObject.SetActive(true);
            }
        }
    }

    private void SetPreviewPosition(InputAction.CallbackContext ctx)
    {
        if (UItoolkitRayCastBlocker.IsPointerOverBlockers(ctx.ReadValue<Vector2>()))
        {
            Debug.Log("raycast blocked");
            return;
        }

        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(ctx.ReadValue<Vector2>());
        var cellPos = MainGrid.WorldToCell(worldPoint);

        if(BoardManager.Instance.WorkerArea.HasTile(cellPos))
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
        TouchPosAction.Disable();
        TouchPosAction.performed -= SetPreviewPosition;
    }

    public void OnConfirm(ClickEvent clickEvent)
    {
        if(!LevelManager.Instance.TrySpend(WorkStationData.Price))
        {
            Debug.LogWarning("Coin not enough.");
            return;
        }

        Hide();
        Debug.Log("confirm");
        Instantiate(WorkStationData.Prefab, buildPreview.position, Quaternion.identity, WorkStationsTransform);
        UiEvents.instance.DisplayUiTriggerEvent("MenuGameplay");
    }

    public void OnRatate(ClickEvent clickEvent)
    {
        direction += 1;
        if (Enum.GetNames(typeof(IsometricDirections)).Length <= (int) direction)
        {
            direction = 0;
        }
        SpriteRenderer.sprite = WorkStationData.GetPreviewSprite(direction);
        Debug.Log("rotate");
    }

    public void OnCancle(ClickEvent clickEvent)
    {
        Hide();
        Debug.Log("cancle");
        UiEvents.instance.DisplayUiTriggerEvent("BuildMenu");
    }
}
