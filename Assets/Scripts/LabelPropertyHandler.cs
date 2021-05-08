using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// Added by Periyasamy on 13/9/2019 for Label feature
/// </summary>
/// 
public class LabelPropertyHandler : MonoBehaviour
{
    public Text displayLebelText;
    public TMP_InputField labelInput;
    public string objName;
    public TMP_Text labelCompDispText;
    public List<GameObject> labelGobject;
    // Start is called before the first frame update
    void Start()
    {
        ScriptsManager.OnCloseClicked += CloseProject;
    }

    private void CloseProject()
    {
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        ScriptsManager.OnCloseClicked -= CloseProject;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnLabelTextChange()
    {
        //displayLebelText.text = labelInput.text;
        ScriptsManager.Instance.GetComponent<Label_components>().ApplyTextChange(labelInput.text);
    }

    public void closeButton()
    {
        //StartCoroutine(StopTransition());
    }

   
}
