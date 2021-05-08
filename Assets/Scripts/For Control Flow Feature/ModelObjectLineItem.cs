using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;





[Serializable]
public class ModelObjectLineItem : LineItem, ILineItem
{
    public short ID { get => id; }

    [SerializeField]
    GameObject modelGameObj;


    [SerializeField]
    GameObject visibilityIcon;

    Button modelVisibilityButt { get; set; }


    Sprite enableSprite, disableSprite;



    public ModelObjectLineItem(GameObject modelGameObj, GameObject lineitem)
    {
        this.modelGameObj = modelGameObj;
        thislineItemObj = lineitem;
        //OnClick - for enabling label function and object collection right after disable the model list of items 
        lineitem.transform.GetChild(3).GetComponent<Button>().onClick.AddListener(() => { EnableModelHierarchy(); });

        selectButt = thislineItemObj.transform.GetChild(0).GetComponent<Button>();
        transistionButt = selectButt.colors;

        //OnClick - select func on the lineitem
        selectButt.onClick.AddListener(() => { Select(); });

        //set model name to the line item
        lineitem.transform.GetChild(1).GetComponent<TextMeshProUGUI>().SetText(modelGameObj.name);

        //OnClick - for deleting the object
        lineitem.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(() => { ScriptsManager.Instance.OnObjectDelete(this.modelGameObj.transform); });


        modelVisibilityButt = lineitem.transform.GetChild(4).GetComponent<Button>();

        //sprite indication for enabling the model in the scene
        enableSprite = modelVisibilityButt.image.sprite;
        //sprite indication that model is disabled in the scene
        disableSprite = modelVisibilityButt.spriteState.pressedSprite;

        modelVisibilityButt.onClick.AddListener(() => { EnableDisableModelGameObj(); });

        Select();

        modelGameObj.GetComponent<ThreeDModelFunctions>().thisModelLineItem = this;


    }

    public GameObject ModelGameObj { get => modelGameObj; private set => modelGameObj = value; }

    protected override void EnableModelHierarchy()
    {
        //enable label functionality globally (simple project content type)
        base.EnableModelHierarchy();
        // modelGameObj.transform.SetParent(ScriptsManager.Instance.objectCollection.transform);

        //modelGameObj.SetActive(true);
    }

    public override void Delete()
    {

        RemoveDependies();
        //deletes the line item
        base.Delete();
    }


    //the body of this method can be made in script manager itself, keeping it here inorder to maintain consistency 
    public void RemoveDependies()
    {
        //clear labels data and delete it, if model has any
        ScriptsManager.Instance.objecttoBeDeleted = modelGameObj.transform;
        ScriptsManager.Instance.DeleteObject();

        if (ScriptsManager.currentSelectedModelObjectLineItem == this)
        {
            ScriptsManager.currentSelectedModelObjectLineItem = null;
        }


    }

    public override void Select()
    {

        if (ScriptsManager.currentSelectedModelObjectLineItem != null)
            ScriptsManager.currentSelectedModelObjectLineItem.DeSelect();

        //ignore double press on the button by checking the button sprite state
        if (transistionButt.normalColor == transistionButt.highlightedColor)
            return;

        base.Select();

        ScriptsManager.currentSelectedModelObjectLineItem = this;

        ScriptsManager.Instance.runtimeHierarchy.Refresh();

        //enable the transform pivot axis of the object
        ScriptsManager.Instance.runtimeHierarchy.Select(modelGameObj.transform);


    }

    public override void DeSelect()
    {
        base.DeSelect();

        //get unselect obj code from runtimehierarchy (it hapns as expected without getting it, so analysis further)

        //disable the content required property ( i.e == text component of the object)
        //   ContentTypeComponent(contentType, false);

        //remove from current slection stack
        ScriptsManager.currentSelectedModelObjectLineItem = null;

    }


    public void EnableDisableModelGameObj()
    {
        Debug.Log("check tis line for the button");
        if (modelVisibilityButt.image.sprite == enableSprite)
        {

            //disable the apprapraite object in scene
            modelGameObj.SetActive(false);
            modelVisibilityButt.image.sprite = disableSprite;
        }
        else if (modelVisibilityButt.image.sprite == disableSprite)
        {
            //enable the apprapraite object in scene
            modelGameObj.SetActive(true);
            modelVisibilityButt.image.sprite = enableSprite;

        }


    }






}

