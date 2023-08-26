using UnityEngine;
using UnityEngine.UIElements;

public class UiObjectManager : MonoBehaviour
{
    public static UiObjectManager Instance { get; private set; }
    Inputs inputs;
    public LayerMask objectMask;
    public Transform TransformToFollow;

    public Vector2 newPosition;

    // Ui Doc
    private VisualElement root;
    private TextElement nameText;
    private TextElement levelText;
    private TextElement timeText;
    private TextElement priceText;
    private TextElement costText;
    private Button upgradeButton;

    private Camera m_MainCamera;

    // events
    public System.Action ClickUpgradeBtn;

    private void OnEnable() => inputs.Enable();
    private void OnDisable() => inputs.Disable();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        inputs = new Inputs();

        inputs.Gameplay.Tap.performed += delegate
        {
            Debug.Log("Tap performed");
            var touchPos = inputs.Gameplay.TouchPosition.ReadValue<Vector2>();
            if (UItoolkitRayCastBlocker.IsPointerOverBlockers(touchPos))
            {
                return;
            }
            var ray = Camera.main.ScreenPointToRay(touchPos);

            RaycastHit2D[] hits = new RaycastHit2D[10];
            int n_hits = Physics2D.RaycastNonAlloc(ray.origin, ray.direction, hits, 20f);
            Transform topMostHit = null;

            for (int i = 0; i < n_hits; i++)
            {
                Debug.Log(hits[i].transform.name);
                if (hits[i].transform.CompareTag("WorkStation"))
                {
                    Debug.Log(hits[i].transform.name + " y-> " + hits[i].transform.position.y + " hit at : " + hits[i].point);
                    if (topMostHit == null)
                    {
                        topMostHit = hits[i].transform;
                    }
                    else if (topMostHit.transform.position.y > hits[i].transform.position.y)
                    {
                        topMostHit = hits[i].transform;
                    }

                }
            }

            if (topMostHit != null)
            {
                var uiObject = topMostHit.GetComponent<IUiObject>();
                if (uiObject == null)
                {
                    Debug.LogError($"{topMostHit.transform.name} doesn't have component IUiObject.");
                }
                else
                {
                    uiObject.ShowUiObject();
                }
            }
            else
            {
                HidePanel();
            }

        };
    }

    private void Start()
    {
        m_MainCamera = Camera.main;

        if (TryGetComponent(out UIDocument uiDoc))
        {
            uiDoc.panelSettings.referenceResolution = new Vector2Int(Screen.width, Screen.height);

            root = uiDoc.rootVisualElement.Q("ObjectInfo");
            nameText = root.Q<Label>("NameText");
            levelText = root.Q<Label>("LevelText");
            timeText = root.Q<Label>("TimeText");
            priceText = root.Q<Label>("PriceText");
            costText = root.Q<Label>("CostText");
            upgradeButton = root.Q<Button>("UpgradeButton");

            upgradeButton.clicked += delegate { ClickUpgradeBtn?.Invoke(); };
        }

        if (root == null)
            Debug.Log("m_bar null");

        HidePanel();
    }
    /*
    private void UpgradeButton_clicked()
    {
        Debug.Log("btn clicked");
    }*/

    public void SetPosition()
    {
        newPosition = RuntimePanelUtils.CameraTransformWorldToPanel(root.panel, TransformToFollow.position, m_MainCamera);
        newPosition.x -= root.layout.width / 2;
        newPosition.y -= root.layout.height / 2;

        // check overflow Ui
        newPosition.x = Mathf.Clamp(newPosition.x, 0f, Screen.width - root.layout.width);
        newPosition.y = Mathf.Clamp(newPosition.y, 0f, Screen.height - root.layout.height);

        root.transform.position = newPosition;
    }

    private void LateUpdate()
    {
        if (root != null)
        {
            SetPosition();
        }
    }

    public void DisplayFactoryPanel(Vector2 position, string name, int level, float time, float price, float cost)
    {
        Debug.Log("Display");
        nameText.text = name;
        levelText.text = level.ToString();
        timeText.text = $"{time} s";
        priceText.text = price.ToString();
        costText.text = cost.ToString();

        TransformToFollow.position = position;
        root.style.display = DisplayStyle.Flex;
    }

    public void HidePanel()
    {
        root.style.display = DisplayStyle.None;
    }
}