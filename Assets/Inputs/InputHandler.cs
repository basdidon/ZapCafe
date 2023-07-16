using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    /*
    public static InputHandler Instance { get; private set; }
    Inputs inputs;
    public LayerMask objectMask;

    public Transform hitDebug;

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
            RaycastHit2D[] hits = new RaycastHit2D[10];
            int n_hits = Physics2D.RaycastNonAlloc(ray.origin, ray.direction,hits,20f,objectMask.value);
            Debug.DrawRay(ray.origin, ray.direction*20f, Color.black, 5f);
            Transform topMostHit = null;
            for(int i= 0; i < n_hits; i++)
            {
                if (hits[i].transform.CompareTag("WorkStation"))
                {
                    Debug.Log(hits[i].transform.name+" y-> "+hits[i].transform.position.y +" hit at : "+hits[i].point);
                    if(topMostHit == null)
                    {
                        topMostHit = hits[i].transform;
                    }
                    else if(topMostHit.transform.position.y > hits[i].transform.position.y)
                    {
                        Debug.Log("a");
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
                    UiObjectManager.Instance.OnUiObjectActive?.Invoke(uiObject);
                    uiObject.ShowUiObject();
                    hitDebug.position = topMostHit.position;
                }
            }
            else
            {
                Debug.Log("null");
                UiObjectManager.Instance.OnUiObjectActive?.Invoke(null);
            }

    }
        };*/
}
