using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public abstract class PanelControl : MonoBehaviour
{
    public VisualElement Root { get; set; }
    public abstract string Key { get; }

    #region ButtoonAnimate
    [Header("buttonColor")]
    public Color mainColor;
    public Color onMouseDownColor;

    public void SetClickAnimation(VisualElement _v, Color _mainColor, Color _onMouseDownColor)
    {
        _v.RegisterCallback<MouseDownEvent>(evt => _v.style.backgroundColor = new StyleColor(_onMouseDownColor));
        _v.RegisterCallback<MouseUpEvent>(evt => _v.style.backgroundColor = new StyleColor(_mainColor));
    }
    #endregion

    protected virtual void Awake()
    {
        if (TryGetComponent(out UIDocument uiDoc))
        {
            Root = uiDoc.rootVisualElement;
        }

        UiEvents.instance.OnDisplayUi += OnDisplayUi;
        UiEvents.instance.OnHideUi += OnHideUi;
    }

    protected virtual void Display() => Root.style.display = DisplayStyle.Flex;
    protected virtual void Hide() => Root.style.display = DisplayStyle.None;

    #region Events handler
    private void OnDisplayUi(string _key) 
    {
        if (Key != _key)
            return;

        Display();
    }
    private void OnHideUi(string _key)
    {
        if (Key != _key)
            return;

        Hide();
    }
    #endregion
}
