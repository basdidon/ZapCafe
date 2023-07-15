using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public static InputHandler Instance { get; private set; }
    Inputs inputs;

    private void OnEnable() => inputs.Enable();
    private void OnDisable() => inputs.Disable();

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

        inputs = new Inputs();
        inputs.Gameplay.Tap.performed += delegate {
            Debug.Log("Tap performed");
            var touchPos = inputs.Gameplay.TouchPosition.ReadValue<Vector2>();
            var ray = Camera.main.ScreenPointToRay(touchPos);
            //Physics2D.Raycast(ray.origin, ray.direction, 10f,);
        };
    }
}
