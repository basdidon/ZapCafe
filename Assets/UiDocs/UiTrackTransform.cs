using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

// ref https://forum.unity.com/threads/uitoolkit-world-space-support-status.1306908/
public class UITrackTransform : MonoBehaviour
{
    public static UITrackTransform Instance { get; private set; }
    public Transform TransformToFollow;

    public Vector2 newPosition;

    public Vector2 panelSize;
    public Vector2 resolvePanelSize;
    public Vector2 myPanelSize;

    public Vector2 screenSize;
    public Vector2 referenceResolution;

    // Ui Doc
    private VisualElement root;
    private TextElement nameText;
    private TextElement levelText;
    private TextElement timeText;
    private TextElement priceText;
    private TextElement costText;

    private Camera m_MainCamera;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        m_MainCamera = Camera.main;
        if (TryGetComponent(out UIDocument uiDoc))
        {
            root = uiDoc.rootVisualElement.Q("ObjectInfo");
            nameText = root.Q<Label>("NameText") ;
            levelText = root.Q<Label>("LevelText");
            timeText = root.Q<Label>("TimeText");
            priceText = root.Q<Label>("PriceText");
            costText = root.Q<Label>("CostText");
            referenceResolution = uiDoc.panelSettings.referenceResolution;
        }

        if (root == null)
            Debug.Log("m_bar null");
        //HidePanel();
}

    public void SetPosition()
    {
        panelSize =  new Vector2(root.layout.width, root.layout.height);
        resolvePanelSize = new Vector2(root.resolvedStyle.width, root.resolvedStyle.height);
        myPanelSize = new Vector2(root.layout.width * (Screen.width / referenceResolution.x), root.layout.height * (Screen.height / referenceResolution.y));

        screenSize = new Vector2(Screen.width,Screen.height);
        var screenPoint = m_MainCamera.WorldToScreenPoint(TransformToFollow.position); //RuntimePanelUtils.CameraTransformWorldToPanel(root.panel, TransformToFollow.position, m_MainCamera);
        newPosition = screenPoint;
        newPosition.x -= panelSize.x /2;
        newPosition.y = referenceResolution.y - newPosition.y;

        // check overflow Ui
        newPosition.x = Mathf.Clamp(newPosition.x, 0f, referenceResolution.x - panelSize.x);
        newPosition.y = Mathf.Clamp(newPosition.y, 0f, referenceResolution.y - panelSize.y);
        
        //Debug.Log($"{newPosition} / {Screen.width} , {Screen.height}");

        root.transform.position = newPosition;
    }
    
    private void LateUpdate()
    {
        if(root != null)
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

