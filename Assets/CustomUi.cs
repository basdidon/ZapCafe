using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CustomUi : MonoBehaviour
{
    VisualElement Root { get; set; }
    [SerializeField] VisualTreeAsset mainMenu;

    private void Awake()
    {
        if(TryGetComponent(out UIDocument uiDoc))
        {
            Root = uiDoc.rootVisualElement;            
        }

        //var clone = mainMenu.Instantiate();
        VisualElement v = new();
        Label l = new("hehe");
        Root.Add(v);
        Root.Add(l);
        
        /*
        var clone2 = mainMenu.Instantiate();
        Root.Add(clone2);*/
        
    }

}
