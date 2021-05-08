using System;
using UnityEngine;

//Using this script for control flow alone. Created on 4-9-2020.
public class PopupManager : Singleton<PopupManager>
{
    public static int uniqueButtonID;
    public GameObject buttonHierarchyItem, buttonSceneItem;
    public static Action<string, GameObject> ButtonNameAction;
    public CreateScenePopup createSceneVariables;
    public ButtonPopup buttonPopUpVariables;
    public SceneObjectsPopup sceneObjectsPopup;

    public void OpenSceneNamePopup()
    {
        CreateScene createScene = new CreateScene(createSceneVariables, buttonHierarchyItem, buttonSceneItem);
        createScene.OpenPopUp();
        ButtonNameAction += OpenButtonProperty;
    }

    public void OpenButtonProperty(string name, GameObject button)
    {
        ButtonPropertyControlFlow buttonPropertyControlFlow = new ButtonPropertyControlFlow(buttonPopUpVariables, name, button);
        buttonPropertyControlFlow.OpenPopUp();
        ButtonNameAction -= OpenButtonProperty;
    }

    public void OpenSceneObjectsPopup()
    {
        SceneObjects sceneObjects = new SceneObjects(sceneObjectsPopup);
        sceneObjects.OpenPopUp();
    }

    private void Start()
    {
        ScriptsManager.OnCloseClicked += EmptyButtnAction;
    }

    private void EmptyButtnAction() => ButtonNameAction = null;
   }