using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
[Serializable]
public class SceneLineItem : LineItem, ILineItem, ICustomInstantiate
{
    [SerializeField]
    GameObject gameObjInSpace;
    [SerializeField]
    Properties thisSceneProperty;

    /// <summary>
    /// scene objects for this scene, list of content items (to avoid dulicate elements, have used hashset)
    /// </summary>
    public HashSet<SceneObjectLineItem> objList = new HashSet<SceneObjectLineItem>();

    GameObject contentHolderSceneObjList, addIconSceneObjs;
    private GameObject insForAddSceneObjs;
    GameObject sceneObjTitle;

    //Color normalColor = new Color(255,255,255), highlightColor = new Color(0,0,0);

    public static Action<SceneLineItem> DeleteLineItemDelegate;

    public Properties thisScenePropertyProperty { get => thisSceneProperty; }//set => thisSceneProperty=value; }


    public GameObject ContentHolderSceneObjList { get => contentHolderSceneObjList; private set => contentHolderSceneObjList = value; }
    public GameObject GameObjInSpace { get => gameObjInSpace; private set => gameObjInSpace = value; }

    public short ID { get => id; }

    public SceneLineItem(string _name, GameObject _thislineItemObj, GameObject obj, GameObject sceneObjLineItemParent,
        GameObject _sceneObjTitle, short count, GameObject _insforAddObjs, GameObject _addIconSceneObjs, 
        GameObject _closeButtProp,TextMeshProUGUI _titleProp, Properties _properties = null)
    {
        name = _name;
        thislineItemObj = _thislineItemObj;
        gameObjInSpace = obj;
        closeButtProp = _closeButtProp;
        titleProp = _titleProp;
        sceneObjTitle = _sceneObjTitle;
        id = count;
        addIconSceneObjs = _addIconSceneObjs;
        insForAddSceneObjs = _insforAddObjs;

        Properties.RemoveAllListenerOnToggle();
        Properties.ResetValues();
       
        thisSceneProperty = _properties != null ? _properties : new Properties();

        selectButt = thislineItemObj.transform.GetChild(0).GetComponent<Button>();
        transistionButt = selectButt.colors;
        //OnClick func on the lineitem
        selectButt.onClick.AddListener(() => { Select(); });
        //OnClick Func for the edit butt on the line item
        thislineItemObj.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() => { Edit(); });
        //OnClick Func for the delete butt on the line item
        thislineItemObj.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(() => { ControlFlowManagerV2.Instance.ConfirmDeleteScenePopUp(gameObjInSpace); });

        contentHolderSceneObjList = Instantiate<GameObject>(sceneObjLineItemParent);
        contentHolderSceneObjList.SetActive(true);
        insForAddSceneObjs.SetActive(true);

        Select();
        ButtonPropertyControlFlow.delegateToSendNameItem += NameTheScneObjTitle;
    }




    public GameObject GetRefObjInSpace()
    {
        return gameObjInSpace;
    }



    /// <summary>
    /// choose which triggers should be shown for the particular scene
    /// </summary>
    public void SetPropertyStats()
    {
        bool checkStartScene;

        if (ControlFlowManagerV2.Instance.sceneLineItems.Count != 0)
        {
            checkStartScene = (this == ControlFlowManagerV2.Instance.sceneLineItems[0]) ? true : false;
        }
        else
        {
            checkStartScene = true;

        }
        ControlFlowManagerV2.Instance.getSceneLineItemPropertyObjects.continueWithPreScneToggle.gameObject.SetActive(!checkStartScene);

        ControlFlowManagerV2.Instance.getSceneLineItemPropertyObjects.playAfterLaunchToggle.gameObject.SetActive(checkStartScene);

    }

    public void OnClickLineItem()
    {
        //click on the line item
        Select();
        SetPropertyStats();
    }





    public override void Delete()
    {
        RemoveProperties();
        RemoveDependies();

        base.Delete();
    }

    public override string Name()
    {
        return base.Name();
    }

    public void RemoveDependies()
    {

        DeleteAllSceneObjs();

        ScriptsManager.Instance.dsButtons.Remove(gameObjInSpace);
        DeleteItemByDestroy(gameObjInSpace);

        if (ControlFlowManagerV2.currentSelectedSceneLineItem == this)
        {
            SetContentTitle("Scene Objects");
            addIconSceneObjs.SetActive(false);
            ControlFlowManagerV2.currentSelectedSceneLineItem = null;
        }

        if (contentHolderSceneObjList.activeSelf)
            ScriptsManager.Instance.DeactivateAllComponent();
    }

    public void RemoveProperties()
    {
        //toggle belongs to the scene trigger which is a property so having remove listeners here
        Properties.RemoveAllListenerOnToggle();

        SetPropertyTitle("Properties");



    }




    public void DeleteAllSceneObjs()
    {

        //enable the scene in order to call delete the model objects inside the scene
        EnableSceneObjListItems();

        //call delete function in all objs of sceneObject list
        foreach (var item in objList)
        {
            item.Delete();
        }

        DeleteItemByDestroy(contentHolderSceneObjList);
        

    }


    /// <summary>
    /// deselect works only when another scene line item is pressed
    /// </summary>
    public override void DeSelect()
    {
        base.DeSelect();

        SetContentTitle("Scene Objects");
        //thisSceneProperty.SetToggleValueToBool();
        // thisSceneProperty.RemoveAllListenerOnToggle();

        DisableSceneObjListItems();
        ScriptsManager.Instance.runtimeHierarchy.Deselect();

        SetPropertyTitle("Properties");
        //disable Scene property 
        ControlFlowManagerV2.Instance.getSceneLineItemPropertyObjects.propertyPanel.SetActive(false);

        addIconSceneObjs.SetActive(false);
        // insForAddSceneObjs.SetActive(false);
        ControlFlowManagerV2.currentSelectedSceneLineItem = null;

    }


    public override void Select()
    {
        if (ControlFlowManagerV2.currentSelectedSceneLineItem != null) //&& ControlFlowManagerV2.currentSelectedSceneLineItem !=this)
            ControlFlowManagerV2.currentSelectedSceneLineItem.DeSelect();


        if (ControlFlowManagerV2.currentSelectedSceneObjLineItem != null)
            ControlFlowManagerV2.currentSelectedSceneObjLineItem.DeSelect();

        Properties.RemoveAllListenerOnToggle();

        //ignore double press on the button by checking the button sprite state
        if (transistionButt.normalColor == transistionButt.highlightedColor)
            return;

        base.Select();

        EnableSceneObjListItems();
        NameTheScneObjTitle(name, gameObjInSpace);
        ScriptsManager.Instance.runtimeHierarchy.Refresh();
        ScriptsManager.Instance.runtimeHierarchy.Select(gameObjInSpace.transform);

        //Enable Scene property 
        ControlFlowManagerV2.Instance.getSceneLineItemPropertyObjects.propertyPanel.SetActive(true);

        SetPropertyTitle("Scene Properties");
        SetPropertyStats();
        thisSceneProperty.SetBoolValueToToggle();
        thisSceneProperty.SetValueChangeDelegateSubscriber();

        addIconSceneObjs.SetActive(true);

        //assign the current selection
        ControlFlowManagerV2.currentSelectedSceneLineItem = this;

    }








    public void SetPropertyTitle(string v)
    {

        //change the scene property to scene object property 
        base.ChangeTextProp(v);
        //enable the object and heading change ... with close butt
        base.SetCloseButtVisiblity(false);
    }
    public void EnableSceneObjListItems()
    {

        contentHolderSceneObjList.SetActive(true);
        //call endable func in the object items to enable in 3d space and scene
        foreach (var item in objList)
        {
            item.Enable();
        }
    }

    public void DisableSceneObjListItems()
    {
        //call disable func in the object items to disable in 3d space and scene
        foreach (var item in objList)
        {
            item.Disable();
        }


        contentHolderSceneObjList.SetActive(false);

    }


    public void Edit()
    {
        ButtonPropertyControlFlow buttonPropertyControlFlow = new ButtonPropertyControlFlow(PopupManager.Instance.buttonPopUpVariables, name, gameObjInSpace);
        buttonPropertyControlFlow.OpenPopUp();
        ButtonPropertyControlFlow.delegateToSendNameItem += NameTheScneObjTitle;

    }


    private void NameTheScneObjTitle(string _name, GameObject objButtOnSpace)
    {
        if (objButtOnSpace != gameObjInSpace)
            return;

        thislineItemObj.transform.GetChild(3).GetComponent<TextMeshProUGUI>().SetText(_name);
        //set the name of scene object line item 
        name = _name;

        objButtOnSpace.name = _name;

        //set heading for content 
        SetContentTitle(name + " Scene Objects");
        ScriptsManager.Instance.currentObjectName.text = _name;

        //  ButtonPropertyControlFlow.delegateToSendNameItem -= NameTheScneObjTitle;

    }


    public void SetContentTitle(string text)
    {
        if (contentHolderSceneObjList.activeSelf)
            sceneObjTitle.GetComponent<TextMeshProUGUI>().SetText(text);
    }

    public T Instantiate<T>(GameObject gameObj) where T : UnityEngine.Object
    {
        T requestedGameObject = UnityEngine.Object.Instantiate(gameObj, gameObj.transform.parent) as T;
        requestedGameObject.name = name + " ObjectsContentList";
        return requestedGameObject;
    }

    /// <summary>
    /// save the data from the class to json
    /// </summary>
    public void OnSave()
    {
        //add more variables according to your need
        var tempData = new SceneLineItemData
        {
            _thisSceneProperty = thisSceneProperty,
            sceneUnqiueId = name,
            _idNumeric = id

        };

        foreach (var item in objList)
        {
            tempData.sceneObjList.Add(new SceneObjectLineItemData { objName = item.ContentTypeGameObj.name, id = item.ID });
        }

        ScriptsManager._toolContent.sceneLineItemsToSave.Add(tempData);


    }

}
