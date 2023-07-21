using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public abstract class PanelControl : MonoBehaviour
{
    public VisualElement Root { get; set; }

    protected virtual void Awake()
    {
        if (TryGetComponent(out UIDocument uiDoc))
        {
            Root = uiDoc.rootVisualElement;
        }
    }

    public void Display() => Root.style.display = DisplayStyle.Flex;
    public void Hide() => Root.style.display = DisplayStyle.None;
}
