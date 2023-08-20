using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TextSpawner : MonoBehaviour
{
    public static TextSpawner Instance { get; private set; }
    public UIDocument uIDoc;
    VisualElement root;

    public VisualTreeAsset textEventAsset;

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

        uIDoc = GetComponent<UIDocument>();
        root = uIDoc.rootVisualElement;
    }

    public void SpawnText(string message,Vector3 targetPos)
    {
        StartCoroutine(TextFloatAnimation(message, targetPos, 1f));
    }

    IEnumerator TextFloatAnimation(string message, Vector3 startWorldAt, float range)
    {
        var textEvent = textEventAsset.Instantiate();
        textEvent.Q<Label>("Message").text = message;
        root.Add(textEvent);
        
        Vector3 startPanelAt = RuntimePanelUtils.CameraTransformWorldToPanel(root.panel, startWorldAt, Camera.main);
        Vector3 endPanelAt = RuntimePanelUtils.CameraTransformWorldToPanel(root.panel, startWorldAt + Vector3.up * range, Camera.main);
        float duration = 2f;
        float timeElapsed = 0f;

        textEvent.transform.position =  startPanelAt;


        while (timeElapsed < duration)
        {
            textEvent.transform.position = Vector3.Lerp(startPanelAt, endPanelAt, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        root.Remove(textEvent);
    }
}
