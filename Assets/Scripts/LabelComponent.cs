using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LabelComponent : MonoBehaviour
{
    public GameObject labelSceneReference;
    public GameObject labelButtonReference;

    private void Start()
    {
        ScriptsManager.OnCloseClicked += RemoveComponent;
    }

    private void OnDestroy()
    {
        ScriptsManager.OnCloseClicked -= RemoveComponent;
    }

    public void RemoveComponent()
    {
        Destroy(labelSceneReference);
        gameObject.SetActive(false);
        ScriptsManager.Instance.currentObjectName.text = "";
    }
}