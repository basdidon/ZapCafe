using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class UItoolkitRayCastBlocker : MonoBehaviour
{
    VisualElement root;
    static List<UItoolkitRayCastBlocker> AllRayCastBlockers = new();
    public Rect contectRect;
    public Rect blockingArea;

    private void OnEnable()
    {
        root = GetComponent<UIDocument>().rootVisualElement[0];

        AllRayCastBlockers.Add(this);
    }

    private void OnDisable()
    {
        AllRayCastBlockers.Remove(this);
    }

    private bool IsPointerOverBlocker(Vector3 mousePosWorld)
    {
        Vector2 mousePosPanel = RuntimePanelUtils.CameraTransformWorldToPanel(root.panel, mousePosWorld, Camera.main);

        Rect layout = root.layout;
        Vector3 pos = root.transform.position;
        blockingArea = new(pos.x, pos.y, layout.width, layout.height);
        
        if (mousePosPanel.x <= blockingArea.xMax && mousePosPanel.x >= blockingArea.xMin && mousePosPanel.y <= blockingArea.yMax && mousePosPanel.y >= blockingArea.yMin)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    public static bool IsPointerOverBlockers(Vector2 mousePosScreen)
    {
        var mousePosWorld = Camera.main.ScreenToWorldPoint(mousePosScreen);
        foreach (var blocker in AllRayCastBlockers)
        {
            if (blocker.IsPointerOverBlocker(mousePosWorld))
            {
                return true;
            }
        }
        return false;
    }
}
