using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManipulationComponent : MonoBehaviour
{
    public Toggle move, rotate, scale;
    public Button editMove, editRotate, editScale;
    //[HideInInspector]
    public GameObject moveButton, rotateButton, scaleButton;

    private void Start()
    {
        move.onValueChanged.AddListener(delegate
       {
           MoveFunc(move);
       });
        rotate.onValueChanged.AddListener(delegate
       {
           RotateFunc(rotate);
       });
        scale.onValueChanged.AddListener(delegate
       {
           ScaleFunc(scale);
       });

        editMove.onClick.AddListener(() => { EditMove(); });
        editRotate.onClick.AddListener(() => { EditRotate(); });
        editScale.onClick.AddListener(() => { EditScale(); });

        //SubscribeToDelegate();
    }

    public void SubscribeToDelegate()
    {
        PublishContent.OnSaveContentClick += OnManipulationSave;
        ScriptsManager.OnCloseClicked += CloseProject;
    }

    public void UnSubscribeToDelegate()
    {
        PublishContent.OnSaveContentClick -= OnManipulationSave;
        ScriptsManager.OnCloseClicked -= CloseProject;
    }

    private void OnManipulationSave()
    {
        if (ScriptsManager.Instance.move)
        {
            ScriptsManager._toolContent.moveScript = ScriptsManager.Instance.move;
        }

        if (ScriptsManager.Instance.rotate)
        {
            ScriptsManager._toolContent.rotScript = ScriptsManager.Instance.rotate;
        }

        if (ScriptsManager.Instance.scale)
        {
            ScriptsManager._toolContent.scaleScript = ScriptsManager.Instance.scale;
        }
    }

    public void MoveFunc(Toggle toggle)
    {
        ScriptsManager.Instance.move = toggle.isOn;
        editMove.interactable = toggle.isOn ? true : false;
        CreateButton(toggle.isOn, "Move");
    }

    public void RotateFunc(Toggle toggle)
    {
        ScriptsManager.Instance.rotate = toggle.isOn;
        editRotate.interactable = toggle.isOn ? true : false;
        CreateButton(toggle.isOn, "Rotate");
    }

    public void ScaleFunc(Toggle toggle)
    {
        ScriptsManager.Instance.scale = toggle.isOn;
        editScale.interactable = toggle.isOn ? true : false;
        CreateButton(toggle.isOn, "Scale");
    }

    public void EditMove()
    {
        PopupManager.ButtonNameAction += PopupManager.Instance.OpenButtonProperty;
        PopupManager.ButtonNameAction(moveButton.GetComponentInChildren<ControlFlowButton>().givenName, moveButton);
    }

    public void EditRotate()
    {
        PopupManager.ButtonNameAction += PopupManager.Instance.OpenButtonProperty;
        PopupManager.ButtonNameAction(rotateButton.GetComponentInChildren<ControlFlowButton>().givenName, rotateButton);
    }

    public void EditScale()
    {
        PopupManager.ButtonNameAction += PopupManager.Instance.OpenButtonProperty;
        PopupManager.ButtonNameAction(scaleButton.GetComponentInChildren<ControlFlowButton>().givenName, scaleButton);
    }

    public void CreateButton(bool canCreate, string type)
    {
        switch (canCreate)
        {
            case true:
                switch (type)
                {
                    case "Move":
                        if (moveButton == null)
                        {
                            moveButton = CreateButton(type);
                        }
                        else
                        {
                            moveButton.SetActive(true);
                        }
                        break;
                    case "Rotate":
                        if (rotateButton == null)
                        {
                            rotateButton = CreateButton(type);
                        }
                        else
                        {
                            rotateButton.SetActive(true);
                        }
                        break;
                    case "Scale":
                        if (scaleButton == null)
                        {
                            scaleButton = CreateButton(type);
                        }
                        else
                        {
                            scaleButton.SetActive(true);
                        }
                        break;
                }
                break;
            case false:
                switch (type)
                {
                    case "Move":
                        moveButton.SetActive(false);
                        break;
                    case "Rotate":
                        rotateButton.SetActive(false);
                        break;
                    case "Scale":
                        scaleButton.SetActive(false);
                        break;
                }
                break;
        }
    }

    private GameObject CreateButton(string type)
    {
        PopupManager.uniqueButtonID++;

        GameObject buttonSceneItem = Instantiate(PopupManager.Instance.buttonSceneItem);
        buttonSceneItem.transform.SetParent(ScriptsManager.Instance.scenePropertyRect);
        ObjectTransformComponent objectTransformComponent = buttonSceneItem.GetComponent<ObjectTransformComponent>();
        ControlFlowButton controlFlowButton = buttonSceneItem.GetComponentInChildren<ControlFlowButton>();
        switch (type)
        {
            case "Move":
                buttonSceneItem.transform.position = new Vector3(-0.75f, -1.0f, 0.0f);
                controlFlowButton.referencedTo = AssignButton.GestureMove.ToString();
                break;
            case "Rotate":
                buttonSceneItem.transform.position = new Vector3(0.75f, -1.0f, 0.0f);
                controlFlowButton.referencedTo = AssignButton.GestureRotate.ToString();
                break;
            case "Scale":
                buttonSceneItem.transform.position = new Vector3(0.0f, -1.0f, 0.0f);
                controlFlowButton.referencedTo = AssignButton.GestureScale.ToString();
                break;
        }
        objectTransformComponent.SetTransform(buttonSceneItem.transform.position, buttonSceneItem.transform.eulerAngles, buttonSceneItem.transform.localScale);
        buttonSceneItem.name = controlFlowButton.givenName = controlFlowButton.alternateName = type;
        controlFlowButton.ApplyChanges();
        ScriptsManager.Instance.dsButtons.Add(buttonSceneItem);
        return buttonSceneItem;
    }

    private void CloseProject()
    {
        RemoveComponent();
    }

    public void RemoveComponent()
    {
        ScriptsManager.Instance.featureScript.manipulation.sprite = ScriptsManager.Instance.featureScript.manipulationSprites[0];
        ScriptsManager.Instance.isGesture = ScriptsManager.Instance.move = ScriptsManager.Instance.rotate = ScriptsManager.Instance.scale
            = move.isOn = rotate.isOn = scale.isOn = editMove.interactable = editRotate.interactable = editScale.interactable = false;

        if (ScriptsManager._toolContent != null)
        {
            ScriptsManager._toolContent.moveScript = ScriptsManager._toolContent.rotScript = ScriptsManager._toolContent.scaleScript = false;
        }

        Transform temp = ScriptsManager.Instance.scenePropertyRect.Find("Gestures");

        if (temp != null)
        {
            Destroy(temp.gameObject);
        }

        gameObject.SetActive(false);
        ScriptsManager.Instance.currentObjectName.text = "";
    }
}
