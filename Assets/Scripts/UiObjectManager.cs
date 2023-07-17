using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public interface IUiObject
{
    void ShowUiObject();
    void HideUiObject();
}

public class UiObjectManager : MonoBehaviour
{
    public static UiObjectManager Instance { get; private set; }
    Inputs inputs;
    public LayerMask objectMask;

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
                    var offset =  Vector3.up * 3 + topMostHit.transform.position;
                    UITrackTransform.Instance.DisplayFactoryPanel(offset,"a",1,1,1,1);
                }
            }
            else
            {
                /*
                Debug.Log("null");
                OnUiObjectActive?.Invoke(null);
                */
            }
            
        };
    }

    public void AddUiObject(IUiObject uiObject)
    {
        //uiObjects.Add(uiObject);
    }
}
