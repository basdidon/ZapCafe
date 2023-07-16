using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public static InputHandler Instance { get; private set; }
    Inputs inputs;
    LayerMask WorkStationMask;


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

        ContactFilter2D contactFilter = new();
        contactFilter.layerMask = WorkStationMask;

        inputs = new Inputs();
        inputs.Gameplay.Tap.performed += delegate {
            Debug.Log("Tap performed");
            var touchPos = inputs.Gameplay.TouchPosition.ReadValue<Vector2>();
            var ray = Camera.main.ScreenPointToRay(touchPos);
            RaycastHit2D[] hits = new RaycastHit2D[10];
            int n_hits = Physics2D.Raycast(ray.origin, ray.direction, contactFilter, hits);
            for(int i= 0; i < n_hits; i++)
            {
                Debug.Log(hits[i].transform.name);
            }

        };
    }
}
