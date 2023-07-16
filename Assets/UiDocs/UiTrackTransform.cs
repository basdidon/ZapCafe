using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

// ref https://forum.unity.com/threads/uitoolkit-world-space-support-status.1306908/
public class UITrackTransform : MonoBehaviour
{
    public Transform TransformToFollow;

    // Ui Doc
    private VisualElement root;
    //private TextElement nameText;

    private Camera m_MainCamera;
    private void Start()
    {
        m_MainCamera = Camera.main;
        if (TryGetComponent(out UIDocument uiDoc))
        {
            root = uiDoc.rootVisualElement.Q("ObjectInfo");
            //nameText = root.Q<Label>("NameText") ;
        }

        if (root == null)
            Debug.Log("m_bar null");

        SetPosition();
        //nameText.text = "DonutBox";
    }

    public void SetPosition()
    {
        Vector2 newPosition = RuntimePanelUtils.CameraTransformWorldToPanel(root.panel, TransformToFollow.position, m_MainCamera);
        newPosition.x -= root.layout.width / 2;
        root.transform.position = newPosition;
    }

    private void LateUpdate()
    {
        if(root != null)
        {
            SetPosition();
        }
    }
}

