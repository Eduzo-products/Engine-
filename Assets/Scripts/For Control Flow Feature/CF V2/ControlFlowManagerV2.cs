using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ControlFlowManagerV2 : MonoBehaviour
{

    public static ControlFlowManagerV2 Instance;
    public GameObject instructionMsg, scenePropertyPanel, sceneObjTitle;


    //Control flow procedure state machine code 
    public static Action<List<SceneSequence>> startControlFlowAction;


    public GameObject sceneLineitemPrefab, sceneObjLineItemParent, addIconSceneObjs, insForAddSceneObjs, sceneObjLineItemPrefab;
    public Transform sceneLineItemParent;

    public List<SceneLineItem> sceneLineItems = new List<SceneLineItem>();

    //this variable shouldnt be here.. remove it later
    public List<SceneObjectLineItem> sceneObjLineItems = new List<SceneObjectLineItem>();


    public void RemoveInstruction() => instructionMsg.SetActive(false);


    public SceneLineItemPropertyObjects getSceneLineItemPropertyObjects;
    //cureent selected scene in the space
    public static SceneLineItem currentSelectedSceneLineItem;
    //cureent selected scene object in the space
    public static SceneObjectLineItem currentSelectedSceneObjLineItem;

    public static bool confirmDeleteSceneLineItem;
    public static bool confirmDeleteSceneObjLineItem;

    //in order to destroy line items from scene list and obj list
    public GameObject ObjTobeDeleted;
    [SerializeField]
    GameObject closeButtButt;

    [SerializeField]
    TextMeshProUGUI titleProp;

    //   public static Action<GameObject, ActionStates> contentTypeSceneObj;


    //this is the obj for disabling the scene name in the hieracrhy manually 
    public Transform hierarchyContentObj;

    public List<Transform> ObjListOutOfObjectCollection = new List<Transform>();

    [SerializeField]
    GameObject camFrustrumObj;//indicationg the camera view port .. (i.e) the objects which are inside would be visible on publish application 

    public Sprite[] ContentSprites;
    [SerializeField]
    private short sceneObjLineItemsCount, sceneLineItemsCount;

    //label close buttons for the preview, in order to disable at preview and re-enable again 
    List<Transform> removeBtnsInLabelList = new List<Transform>();
    //for use of exiting prject data manipualtion, using it in order to parent the scene object.. will submerge the variable later
    //  public List<VerticalLayoutGroup> verticalLayoutsContentParent = new List<VerticalLayoutGroup>();


    private void Awake()
    {

        Instance = this;
        SceneLineItem.DeleteLineItemDelegate += RemoveItemFromtheSceneList;
        //contentTypeSceneObj += CreateSeneObjectLineItem;
    }


    public void AddCreateSceneFuncFromDelegate() => PopupManager.ButtonNameAction += CreateScene;
    public void RemoveCreateSceneFuncFromDelegate() => PopupManager.ButtonNameAction -= CreateScene;

    // public void AddEditSceneFunc() => PopupManager.ButtonNameAction += EditScene;
    //  public void RemoveEditSceneFunc() => PopupManager.ButtonNameAction -= EditScene;

    private void Start()
    {

        //   Invoke("MakeHierarchyWorksForUs", 2f);
        //  SubscribeToDelegate();
    }

    public void SubscribeToDelegate()
    {
        ScriptsManager.OnCloseClicked += CloseProject;
        PublishContent.OnSaveContentClick += OnSaveProject;
    }

    public void EnableCameraFrustum(bool v)
    {

        camFrustrumObj.SetActive(v);

    }

    /// <summary>
    /// delete label with model on the scene object
    /// </summary>
    /// <param name="obj"></param>
    public void DeleteLabel(GameObject obj)
    {
        ScriptsManager.Instance.objecttoBeDeleted = obj.transform;
        ScriptsManager.Instance.DeleteObject();



    }





    /// <summary>
    /// Reset all data members and member functions 
    /// </summary>
    private void CloseProject()
    {
        StopAllCoroutines();


        foreach (var item in sceneLineItems)
        {
            item.Delete();
        }

        sceneLineItemsCount = sceneObjLineItemsCount = 0;
        ScriptsManager.Instance.DeactivateAllComponent();
        sceneLineItems.Clear();
        sceneObjLineItems.Clear();
        PublishContent.OnSaveContentClick -= OnSaveProject;
        removeBtnsInLabelList.Clear();
        currentSelectedSceneLineItem = null;
        currentSelectedSceneObjLineItem = null;

        ObjListOutOfObjectCollection.Clear();

        // verticalLayoutsContentParent.Clear();

    }

    void OnSaveProject()
    {

        for (int i = 0; i < sceneLineItems.Count; i++)
        {
            //sceneLineItems[i].GameObjInSpace.GetComponentInChildren<ControlFlowButton>().OnSave();
            sceneLineItems[i].OnSave();
            Debug.Log("<color=green> checking save button </color>" + sceneLineItems[i]);
        }


    }





    public void CreateScene(string name, GameObject obj)
    {
        //if (hierarchyContentObj != null && hierarchyContentObj.GetChild(0).GetChild(0).gameObject != null)
        //    hierarchyContentObj.GetChild(0).GetChild(0).gameObject.SetActive(false);

        RemoveInstruction();
        CreateSeneLineItem(name, obj);

        RemoveCreateSceneFuncFromDelegate();
    }



    #region Rewrite the code (section -- delete the content )

    public void ConfirmDeleteScenePopUp(GameObject obj)
    {
        ObjTobeDeleted = obj;
        confirmDeleteSceneLineItem = true;
        ScriptsManager.Instance.confirmationPanel.confirmationPanel.SetActive(true);
        ScriptsManager.Instance.confirmationPanel.confirmationTitleText.text = "Delete Scene";
        ScriptsManager.Instance.confirmationPanel.submitButtonText.text = "Delete";
        ScriptsManager.Instance.confirmationPanel.confirmationTextFieldText.text = "Do you want to delete the scene line item?";
        ScriptsManager.Instance.confirmationPanel.cancelButtonText.text = "Cancel";
        ScriptsManager.Instance.confirmationPanel.confirmationIcon.sprite = Helper.Instance.PopUpSprite("Delete");
    }

    public void ConfirmDeleteSceneObjPopUp(GameObject obj)
    {
        confirmDeleteSceneObjLineItem = true;
        ObjTobeDeleted = obj;
        //  ScriptsManager.Instance.OnObjectDelete(obj.transform);

        ScriptsManager.Instance.confirmationPanel.confirmationPanel.SetActive(true);
        ScriptsManager.Instance.confirmationPanel.confirmationTitleText.text = "Delete Scene Object";
        ScriptsManager.Instance.confirmationPanel.submitButtonText.text = "Delete";
        ScriptsManager.Instance.confirmationPanel.confirmationTextFieldText.text = "Are you sure you want to delete this object?";
        ScriptsManager.Instance.confirmationPanel.cancelButtonText.text = "Cancel";
        ScriptsManager.Instance.confirmationPanel.confirmationIcon.sprite = Helper.Instance.PopUpSprite("Delete");
    }



    public void DeleteMethodForSceneObjLineItem(GameObject ObjToDeleted = null)
    {
        foreach (var item in sceneObjLineItems)
        {
            if (item.ContentTypeGameObj.Equals(ObjToDeleted))
            {
                currentSelectedSceneLineItem.objList.Remove(item);
                sceneObjLineItems.Remove(item);
                item.Delete();
                break;
            }
        }
        ReEnableThePropertiesAfterDeleting();
    }

    public void DeleteMethodForSceneLineItem()
    {
        foreach (var item in sceneLineItems)
        {
            if (item.GameObjInSpace.Equals(ObjTobeDeleted))
            {
                sceneLineItems.Remove(item);

                item.Delete();

                break;
            }
        }
        // ReEnableThePropertiesAfterDeleting();
    }

    //re structure this code block of method later (not right way of dong)
    public void ReEnableThePropertiesAfterDeleting()
    {
        ScriptsManager.Instance.DeactivateAllComponent();

        if (currentSelectedSceneLineItem != null)
        {
            currentSelectedSceneLineItem.Select();
            //currentSelectedSceneLineItem.DeSelect();
        }
        if (currentSelectedSceneObjLineItem != null)
        {
            //  currentSelectedSceneObjLineItem.DeSelect();
            currentSelectedSceneObjLineItem.Select();
        }
    }

    #endregion
    public void EditScene(string name, GameObject obj)
    {


        //    NameTheScneObjTitle(name);


    }

    public void PutObjsOutOfObjectCollection()
    {
        ThreeDModelFunctions[] childTransforms = ScriptsManager.Instance.objectCollection.transform.GetComponentsInChildren<ThreeDModelFunctions>(true);

        for (int i = 0; i < childTransforms.Length; i++)
        {
            if (ScriptsManager.Instance.projectTypeDropdown.value == 1)
            {
                if (!childTransforms[i].gameObject.activeSelf)
                {
                    ModelOutOfObjectCollection(childTransforms[i].transform);
                }
            }
            //else if (ScriptsManager.Instance.projectTypeDropdown.value == 0)
            //{
            //    ModelOutOfObjectCollection(childTransforms[i].transform);
            //}

        }
    }

    private void ModelOutOfObjectCollection(Transform transform)
    {
        ObjListOutOfObjectCollection.Add(transform);
        transform.SetParent(null);
        transform.gameObject.SetActive(false);
    }




    public void CloseButtProperty()
    {
        //disable the property heading and the close butt

        //select the scene line item object
        currentSelectedSceneLineItem.Select();



    }

    private void CreateSeneLineItem(string name, GameObject obj)
    {
        #region loading the data for opening existing project 
        SceneLineItemData tempSetUpSceneData = null;
        Properties tempSetUpProp = null;
        if (ScriptsManager._toolContent != null && ScriptsManager._toolContent.sceneLineItemsToSave != null)
        {

            var tempList = ScriptsManager._toolContent.sceneLineItemsToSave;
            if (tempList.Count != 0)
            {

                tempSetUpSceneData = tempList.Where(s => s.sceneUnqiueId.Equals(name)).FirstOrDefault();
                //print(tempSetUpSceneData.sceneUnqiueId);
            }

        }
        if (tempSetUpSceneData == null)
        {
            //having unique id for the scene line items
            sceneLineItemsCount += (short)sceneLineItems.Count;
            tempSetUpProp = new Properties();
        }
        else
        {
            sceneLineItemsCount = tempSetUpSceneData._idNumeric;
            tempSetUpProp = tempSetUpSceneData._thisSceneProperty;
        }
        #endregion

        //temp prop is for loading project scene item 
        SceneLineItem lineItem = new SceneLineItem(name, Instantiate(sceneLineitemPrefab, sceneLineItemParent),
            obj, sceneObjLineItemParent, sceneObjTitle, sceneLineItemsCount, insForAddSceneObjs, addIconSceneObjs,
            closeButtButt, titleProp, tempSetUpProp);

        sceneLineItems.Add(lineItem);


    }
    /// <summary>
    /// creating scene object line item with sending the apporpriate related objects to fulfill its functionality
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="action"></param>
    public void CreateSeneObjectLineItem(GameObject obj, ActionStates action)
    {
        Transform parentObj;
        short tempId;
        CheckAndReturnParentTransform(obj, (item) =>
        {

            //having unique id for the scene object line items
            // sceneObjLineItemsCount += sceneObjLineItems.Count;

            if (item == null)
            {
                parentObj = currentSelectedSceneLineItem.ContentHolderSceneObjList.transform;
                tempId = currentSelectedSceneLineItem.ID;
            }
            else
            {
                parentObj = item.ContentHolderSceneObjList.transform;
                tempId = item.ID;
            }

            SceneObjectLineItem lineItem = new SceneObjectLineItem(tempId, action, GetSpriteFromAction(action), obj,
            Instantiate(sceneObjLineItemPrefab, parentObj), insForAddSceneObjs, closeButtButt, titleProp);

            sceneObjLineItems.Add(lineItem);

            if (item == null)
                currentSelectedSceneLineItem.objList.Add(lineItem);
            else
                item.objList.Add(lineItem);

        });
    }

    /// <summary>
    /// for managing data from existing project
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="belongsToThisScene"></param>
    /// <returns></returns>
    private void CheckAndReturnParentTransform(GameObject obj, Action<SceneLineItem> belongsToThisScene)
    {
        if (ScriptsManager._toolContent != null && ScriptsManager._toolContent.sceneLineItemsToSave != null)
        {


            for (int i = 0; i < ScriptsManager._toolContent.sceneLineItemsToSave.Count; i++)
            {
                for (int j = 0; j < ScriptsManager._toolContent.sceneLineItemsToSave[i].sceneObjList.Count; j++)
                {

                    if (ScriptsManager._toolContent.sceneLineItemsToSave[i].sceneObjList[j].objName == obj.name)
                    {
                        //for (int k = 0; k < verticalLayoutsContentParent.Count; k++)
                        //{
                        //    if (verticalLayoutsContentParent[k].name == ScriptsManager._XRStudioContent.sceneLineItemsToSave[i].sceneUnqiueId + " ObjectsContentList")
                        //    { return verticalLayoutsContentParent[k].transform; }
                        //}
                        for (int l = 0; l < sceneLineItems.Count; l++)
                        {
                            if (sceneLineItems[l].ID == ScriptsManager._toolContent.sceneLineItemsToSave[i].sceneObjList[j].id)
                            {
                                belongsToThisScene(sceneLineItems[l]);
                                return;
                            }
                        }
                    }

                    //else
                    //{
                    //    print("no name with right ObjectsContentList");
                    //    belongsToThisScene(null);
                    //    continue;
                    //}
                }
            }

            belongsToThisScene(null);

        }
        else
        {
            belongsToThisScene(null);
        }
    }

    public void EnableDisableModelHierarchy(bool boolean)
    {

        if (ScriptsManager.Instance.projectTypeDropdown.value == 1)
        {
            ScriptsManager.Instance.controlFlowUI.SetActive(boolean);
        }
        else if (ScriptsManager.Instance.projectTypeDropdown.value == 0)
        {
            ScriptsManager.Instance.modelHierarchyPanel.SetActive(boolean);

        }
        if (boolean)
        {
            PutBackObjsToObjectCollection();
            raycasthitting.enable_label_flag = false;

        }
    }

    private void PutBackObjsToObjectCollection()
    {
        for (int i = 0; i < ObjListOutOfObjectCollection.Count; i++)
        {
            ObjListOutOfObjectCollection[i].SetParent(ScriptsManager.Instance.objectCollection.transform);


            // if (ScriptsManager.Instance.projectTypeDropdown.value == 0)
            //     ObjListOutOfObjectCollection[i].gameObject.SetActive(true);

        }
        ObjListOutOfObjectCollection.Clear();
    }


    private Sprite GetSpriteFromAction(ActionStates action)
    {
        switch (action)
        {
            //case ActionStates.None:
            //    return ContentSprites[0];

            case ActionStates.Model:
                return ContentSprites[1];
            case ActionStates.Video:
                return ContentSprites[3];
            case ActionStates.Audio:
                return ContentSprites[0];
            case ActionStates.TextToSpeech:
                return ContentSprites[4];
          //  case ActionStates.Image:
           //     return ContentSprites[5];
            case ActionStates.TextPanel:
                return ContentSprites[2];

        }
        return null;
    }


    //take this method to main script later 
    public void DisableFunctionalityForPreview()
    {


        if (raycasthitting.enable_label_flag)
            ScriptsManager.Instance.enableDisableComponent.enableAddLabel();


        var tempLabels = ScriptsManager.Instance.GetComponent<raycasthitting>().labels;

        print(removeBtnsInLabelList.Count);

        if (removeBtnsInLabelList.Count != tempLabels.Count)
        {
            removeBtnsInLabelList.Clear();
            for (int i = 0; i < tempLabels.Count; i++)
            {

                removeBtnsInLabelList.Add(tempLabels[i].transform.GetChild(0).GetChild(2));
            }

            //  EnableFunctionalityForPreview(false);
        }
        else
        {
            // EnableFunctionalityForPreview(false);

        }
    }

    /// <summary>
    /// label close button stuff
    /// </summary>
    /// <param name="bol"></param>
    public void EnableFunctionalityForPreview(bool bol)
    {
        for (int i = 0; i < removeBtnsInLabelList.Count; i++)
        {
            removeBtnsInLabelList[i].gameObject.SetActive(bol);
        }
    }



    /// <summary>
    /// data to control flow sequencial process logic (there is same code below ,need to make it to a single method)
    /// </summary>
    public void SendSceneContentToXR_studioControlFlow()
    {


        DisableFunctionalityForPreview();
        startControlFlowAction(SaveMultiScene());
    }


    /// <summary>
    /// used for grp the data in order to feed control flow algorithm 
    /// </summary>
    /// <returns></returns>
    internal List<SceneSequence> SaveMultiScene()
    {
        List<SceneSequence> allSequence = new List<SceneSequence>();
        for (int i = 0; i < sceneLineItems.Count; i++)
        {
            SceneSequence tempSeq = new SceneSequence();



            foreach (var j in sceneLineItems[i].objList)
            {
                XR_Studio_Action tempAction = new XR_Studio_Action();


                tempAction.thisAction = j.ThisClassAction;

                switch (tempAction.thisAction)
                {
                    case ActionStates.None:
                        break;
                    case ActionStates.Model:
                        ThreeDModelFunctions threeDModel = j.ContentTypeGameObj.GetComponent<ThreeDModelFunctions>();
                        tempAction.assetDetails = threeDModel.objDetails;


                        List<string> str = threeDModel.GetTotalChildrenForControlFlowRef().str;
                        List<bool> strBool = threeDModel.GetTotalChildrenForControlFlowRef().strBool;
                        Dictionary<string, bool> dict = threeDModel.GetTotalChildrenForControlFlowRef().dict;


                        tempAction.assetDetails.childName = str;

                        tempAction.assetDetails.childActiveState = strBool;

                        tempAction.assetDetails.childVisibility = dict;


                        break;
                    case ActionStates.Video:
                        AddVideo tempvideo = j.ContentTypeGameObj.GetComponent<NoTransform>().ThisObjVideoProperties();
                        tempvideo.ObjectTransform = j.ContentTypeGameObj.GetComponent<ObjectTransformComponent>().currentTransform;
                        tempAction.videoDetails = tempvideo;
                        break;
                    case ActionStates.Audio:
                        AddAudio tempAudio = j.ContentTypeGameObj.GetComponent<NoTransform>().ThisObjAudioProperties();
                        tempAction.audioDetails = tempAudio;
                        tempAction.audioClip = j.ContentTypeGameObj.GetComponent<NoTransform>().audioClip;
                        break;
                    //case ActionStates.TextToSpeech:

                    //    break;
                    //case ActionStates.Animation:
                    //    break;
                    case ActionStates.TextPanel:
                        tempAction.textContent = j.ContentTypeGameObj.GetComponent<TextContentCanvasHandler>().textContentCurrentObjDetails;
                        tempAction.textContent.transform = j.ContentTypeGameObj.GetComponent<ObjectTransformComponent>().currentTransform;
                        break;
                    //case ActionStates.Button:
                    //    break;
                    default:
                        break;
                }
                tempSeq.actionsSequenceControlFlow.Add(tempAction);
            }

            if (sceneLineItems[i].thisScenePropertyProperty.playAfterLaunch || sceneLineItems[i].thisScenePropertyProperty.continueWithPreScne)
            {
                tempSeq.triggersSequenceControlFlow.Add(new XR_Studio_Trigger()
                {
                    thisTrigger = TypeOfTriggers.Continue
                    //  buttonObj = sceneLineItems[i].GameObjInSpace.GetComponentInChildren<ControlFlowButton>()
                });
            }

            if (sceneLineItems[i].thisScenePropertyProperty.playOnClick)
            {
                var tempStats = sceneLineItems[i].GameObjInSpace.GetComponentInChildren<ControlFlowButton>();
                tempSeq.triggersSequenceControlFlow.Add(new XR_Studio_Trigger()
                {
                    thisTrigger = TypeOfTriggers.Button,


                    buttonObj = new DSButton
                    {
                        type = tempStats.type,
                        uniqueID = tempStats.uniqueID,
                        width = tempStats.width,
                        height = tempStats.height,
                        fontSize = tempStats.fontSize,
                        normalColor = tempStats.normalColor,
                        highlightColor = tempStats.highlightColor,
                        pressedColor = tempStats.pressedColor,
                        textColor = tempStats.textColor,
                        givenName = tempStats.givenName,
                        alternateName = tempStats.alternateName,
                        referencedTo = tempStats.referencedTo
                    }


                });
            }


            allSequence.Add(tempSeq);
        }
        return allSequence;
    }

    public TypeOfTriggers BoolToTrigger(SceneLineItem lineItem)
    {
        if (lineItem.thisScenePropertyProperty.playAfterLaunch || lineItem.thisScenePropertyProperty.continueWithPreScne)
        {
            return TypeOfTriggers.Continue;
        }
        else
        {
            return TypeOfTriggers.Button;

        }

    }



    //public TextContent GetTextContent(SceneObjectLineItem lineItemObj)
    //{
    //    TextContent txt = new TextContent();

    //    TextContentComponent[] textTemps = Resources.FindObjectsOfTypeAll<TextContentComponent>();

    //    for (int i = 0; i < textTemps.Length; i++)
    //    {
    //        if (textTemps[i].name.Equals("Property_" + lineItemObj.ContentTypeGameObj.name))
    //        {
    //            txt.titleText = textTemps[i].titleTextInput.text;
    //            txt.text = textTemps[i].textInput.text;
    //            txt.contentWidth = float.Parse(textTemps[i].contentWidthInput.text);
    //            txt.titleFontSize = int.Parse(textTemps[i].titleFontSize.text);
    //            txt.fontSize = int.Parse(textTemps[i].fontSizeInput.text);
    //            txt.fontColor = textTemps[i].fontColorValue;
    //            txt.bgColor = textTemps[i].panelBGColorValue;
    //            txt.panelName = textTemps[i].panelProperty.name;
    //            txt.transform = textTemps[i].panelProperty.GetComponent<ObjectTransformComponent>().currentTransform;


    //            txt.enableTTS = textTemps[i].enableTTSToggle.isOn;

    //        }
    //    }


    //    return txt;
    //}


    void RemoveItemFromtheSceneList(SceneLineItem item)
    {

        sceneLineItems.Remove(item);
        //sceneLineItems
    }



}



