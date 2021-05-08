
using TMPro;
using UnityEngine;
using UnityEngine.UI;
[System.Serializable]
public class LineItem
{

    [SerializeField]
    protected string name;
    [SerializeField]
    protected GameObject thislineItemObj;
    [SerializeField]
    protected short id;

    [SerializeField]
    protected Button selectButt;

    [SerializeField]
    protected ColorBlock transistionButt;

    [SerializeField]
    protected GameObject closeButtProp;
    [SerializeField]
    protected TextMeshProUGUI titleProp;

    public virtual void Delete() => DeleteLineItem();


    void DeleteLineItem()
    {
        DeleteItemByDestroy(thislineItemObj);


    }

    public void DeleteItemByDestroy(GameObject item)
    {
        Object.Destroy(item);


    }

    public virtual string Name()
    {
        return name;
    }

    public virtual void DeSelect()
    {
        //set sprite to normal color //set normal color to button sprite
        transistionButt.normalColor = new Color(1, 1, 1, 1);
        //reassign color states to the button 
        if (selectButt != null)
            selectButt.colors = transistionButt;
    }


    protected virtual void ChangeTextProp(string text)
    {
        titleProp.SetText(text);
    }

    protected virtual void SetCloseButtVisiblity(bool bol) => closeButtProp.SetActive(bol);

    public virtual void Select()
    {
        //set highlight color to normal color of button sprite
        transistionButt.normalColor = transistionButt.highlightedColor;
        //reassign color states to the button 
        if (selectButt != null)
            selectButt.colors = transistionButt;
    }


    protected virtual void EnableModelHierarchy()
    {
        //enable the object collection for the model (just disable the controlflow object - thats why false is sent as parameter)
        ControlFlowManagerV2.Instance.EnableDisableModelHierarchy(false);
        ControlFlowManagerV2.Instance.PutObjsOutOfObjectCollection();
        ScriptsManager.Instance.enableDisableComponent.enableAddLabel();

    }
}

