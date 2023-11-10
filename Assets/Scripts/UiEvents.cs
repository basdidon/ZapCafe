using System;
using UnityEngine;

public class UiEvents : MonoBehaviour
{
    public static UiEvents instance;

    private void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    public event Action<string> OnDisplayUi;
    public void DisplayUiTriggerEvent(string key)
    {
        OnDisplayUi?.Invoke(key);
    }

    public event Action<string> OnHideUi;
    public void HideUiTriggerEvent(string key)
    {
        OnHideUi?.Invoke(key);
    }
}
