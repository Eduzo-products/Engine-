using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
[Serializable]
public class SceneObjectLineItem : LineItem, ILineItem
{


    public short ID { get => id; }

    [SerializeField]
    GameObject contentTypeGameObj;
    [SerializeField]
    ActionStates contentType;

    public ActionStates ThisClassAction
    {

        get => contentType;
    }

  //  [SerializeField]
   // GameObject labelButtObj;

    public GameObject ContentTypeGameObj { get => contentTypeGameObj; private set => contentTypeGameObj = value; }

    public SceneObjectLineItem(short _belongingSceneId, ActionStates _contentType, Sprite _contentSprite, GameObject _contentTypeGameObj,
        GameObject lineitem, GameObject _insForAddSceneObjs, GameObject _closeButtProp, TextMeshProUGUI _titleProp)
    {
        ControlFlowManagerV2.Instance.getSceneLineItemPropertyObjects.propertyPanel.SetActive(false);
        id = _belongingSceneId;
        contentType = _contentType;
        thislineItemObj = lineitem;
      //  labelButtObj = _labelButtObj;
        closeButtProp = _closeButtProp;
        titleProp = _titleProp;
        contentTypeGameObj = _contentTypeGameObj;

        //add this method to the butt trigger event (model hierarchy panel)
        if (contentType == ActionStates.Model)
        {

            lineitem.transform.GetChild(3).GetComponent<Button>().onClick.AddListener(() => { EnableModelHierarchy(); });
        }

        selectButt = thislineItemObj.transform.GetChild(0).GetComponent<Button>();
        transistionButt = selectButt.colors;
        //OnClick - func on the lineitem
        selectButt.onClick.AddListener(() => { Select(); });
        //for deleting the object 
        lineitem.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(() => { ControlFlowManagerV2.Instance.ConfirmDeleteSceneObjPopUp(this.contentTypeGameObj); });
        //if content type is model ,enable the hieracrchy icon on the line item
        //set the content type icon on left of line item
        //get the gameobj to set the sprite 
        lineitem.transform.GetChild(4).GetComponent<Image>().sprite = _contentSprite;
        //remove the instruction once the item is added
        _insForAddSceneObjs.SetActive(false);

        //set content type as name 
        lineitem.transform.GetChild(1).GetComponent<TextMeshProUGUI>().SetText(_contentType.ToString() + " Object");
        Select();

    }

    protected override void EnableModelHierarchy()
    {
        base.EnableModelHierarchy();
    }


    public void RemoveDependies()
    {
        if (contentType == ActionStates.Model)
            SetLabelButtVisiablity(false);

        if (ControlFlowManagerV2.currentSelectedSceneObjLineItem == this)
        {
            ControlFlowManagerV2.currentSelectedSceneObjLineItem = null;
        }
        ControlFlowManagerV2.Instance.sceneObjLineItems.Remove(this);
    }

    public override void Select()
    {

        if (ControlFlowManagerV2.currentSelectedSceneObjLineItem != null)
            ControlFlowManagerV2.currentSelectedSceneObjLineItem.DeSelect();

        //ignore double press on the button by checking the button sprite state
        if (transistionButt.normalColor == transistionButt.highlightedColor)
            return;

        base.Select();

        //change the scene property to scene object property 
        base.ChangeTextProp("Scene Object Properties");
        //enable the object and heading change ... with close butt
        base.SetCloseButtVisiblity(true);
        ContentTypeComponent(contentType, true);

        ControlFlowManagerV2.currentSelectedSceneObjLineItem = this;

        ScriptsManager.Instance.runtimeHierarchy.Refresh();
        //enable the transform pivot axis of the object
        ScriptsManager.Instance.runtimeHierarchy.Select(contentTypeGameObj.transform);

        Enable();
    }

    private void SetLabelButtVisiablity(bool v)
    {

       // labelButtObj.GetComponent<Image>().color = raycasthitting.enable_label_flag ? Color.blue : Color.white;

        //enable label butt if the object type is model
       // labelButtObj.SetActive(v);



        //enable hierarchy butt if the object type is model
        thislineItemObj.transform.GetChild(3).gameObject.SetActive(v);

    }


    /// <summary>
    ///enable the content required property ( i.e == text component of the object on the right)
    /// </summary>
    /// <param name="action"></param>
    /// <param name="vool"></param>
    public void ContentTypeComponent(ActionStates action, bool vool)
    {
        switch (action)
        {
            //  case ActionStates.None:
            //      return ;
            case ActionStates.Model:
                contentTypeGameObj.GetComponent<ThreeDModelFunctions>().thisSceneObjLineItem = this;
                SetLabelButtVisiablity(vool);
                //disable/enable label butt if the object type is not model
                break;
            case ActionStates.Video:
                contentTypeGameObj.GetComponent<NoTransform>().thisSceneObjLineItem = this;
                SetLabelButtVisiablity(false);

                ScriptsManager.Instance.videoPropertyObj.SetActive(vool);
                break;
            case ActionStates.Audio:
                contentTypeGameObj.GetComponent<NoTransform>().thisSceneObjLineItem = this;
                SetLabelButtVisiablity(false);
                ScriptsManager.Instance.audioPropertyObj.SetActive(vool);
                break;
            case ActionStates.TextToSpeech:
                SetLabelButtVisiablity(false);
                ScriptsManager.Instance.textToSpeechPropertyObj.SetActive(vool);
                break;
            case ActionStates.TextPanel:
                contentTypeGameObj.GetComponent<TextContentCanvasHandler>().thisSceneObjLineItem = this;
                SetLabelButtVisiablity(false);
                ScriptsManager.Instance.textPropertyObj.SetActive(vool);
                break;
           // case ActionStates.Image:

               // contentTypeGameObj.GetComponent<TextContentCanvasHandler>().thisSceneObjLineItem = this;
               // SetLabelButtVisiablity(false);
              //  ScriptsManager.Instance.textPropertyObj.SetActive(vool);

            //    break;
        }


    }



    public override void DeSelect()
    {
        base.DeSelect();

        //get unselect obj code from runtimehierarchy (it hapns as expected without getting it, so analysis further)

        //disable the content required property ( i.e == text component of the object)
        ContentTypeComponent(contentType, false);
        //remove from current slection stack
        ControlFlowManagerV2.currentSelectedSceneObjLineItem = null;

    }

    public override void Delete()
    {
        //destroy line item
        //remove from the list of scene objects from scene line item and also from the list on the control flow script
        //destroy related object in 3d space
        //disable property 
        //disable label icon and model hierarchy icon
        //null the current line item selection
        //close the property tab, if this line is selcted


        RemoveDependies();
        if (ScriptsManager.Instance.controlFlowUI.activeSelf)
        {
            if (contentType == ActionStates.Model)
            {
                ControlFlowManagerV2.Instance.DeleteLabel(contentTypeGameObj);
            }
            if (contentTypeGameObj != null)
            {
                //destroy the gameObj in the scene
                DeleteItemByDestroy(contentTypeGameObj);
            }
        }
        //ScriptsManager.Instance.OnObjectDelete(contentTypeGameObj.transform);
        base.Delete();

    }




    public void Disable()
    {
       
        //disable the apprapraite object in scene
        contentTypeGameObj.SetActive(false);


    }

    public void Enable()
    {
        //enable the apprapraite object in scene
        contentTypeGameObj.SetActive(true);

        if (contentTypeGameObj.GetComponent<TextContentCanvasHandler>())
        {
            ScriptsManager.Instance.runtimeHierarchy.Select(contentTypeGameObj.transform);
        }
    }
}