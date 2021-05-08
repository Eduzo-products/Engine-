using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ExplodeComponent : MonoBehaviour
{
    public Transform local;
    public Slider range, speed;
    public Transform parent, objectExplodePrefab;
    //public List<ObjectExplode> explodeList = new List<ObjectExplode>();

    //public GameObject explodeButtonReference;
    public Toggle explode;
    public Button editExplode;
    public GameObject explodeButton;
    public TMP_InputField txtSpeed, txtRange;

    void Start()
    {
        ScriptsManager.OnCloseClicked += CloseProject;
        explode.onValueChanged.AddListener(delegate
        {
            ExplodeFunc(explode);
        });

        editExplode.onClick.AddListener(() => { EditExplode(); });

        valueChangeMaxRange();
        valueChangeSpeed();
    }

    private void CloseProject()
    {
        //Debug.Log("closedddddddddddddddddddddd");
        RemoveComponent();
    }

    public void ResetValue()
    {
        explode.isOn = false;
        speed.value = 0.1f;
        range.value = 4;
        txtSpeed.text = "0.1";
        txtRange.text = "4";
    }

    private void OnDestroy()
    {
        ScriptsManager.OnCloseClicked -= CloseProject;
    }


    public void ExplodeFunc(Toggle toggle)
    {
        ScriptsManager.Instance.isExplodable = toggle.isOn;
        editExplode.interactable = toggle.isOn ? true : false;
        CreateButton(toggle.isOn, "Explode");
    }
    public void EditExplode()
    {
        PopupManager.ButtonNameAction += PopupManager.Instance.OpenButtonProperty;
        PopupManager.ButtonNameAction(explodeButton.GetComponentInChildren<ControlFlowButton>().givenName, explodeButton);
    }

    public void CreateButton(bool canCreate, string type)
    {
        if (canCreate)
        {
            if (explodeButton == null)
            {
                explodeButton = CreateButton(type);
                if (!ScriptsManager.Instance.dsButtons.Contains(explodeButton))
                    ScriptsManager.Instance.dsButtons.Add(explodeButton);
            }
            else
            {
                explodeButton.SetActive(true);
                if (!ScriptsManager.Instance.dsButtons.Contains(explodeButton))
                    ScriptsManager.Instance.dsButtons.Add(explodeButton);
            }
        }
        else
        {
            explodeButton.SetActive(false);
            ScriptsManager.Instance.dsButtons.Remove(explodeButton);
        }

    }

    private GameObject CreateButton(string type)
    {
        PopupManager.uniqueButtonID++;
        Debug.Log("....Buttton created..");
        GameObject buttonSceneItem = Instantiate(PopupManager.Instance.buttonSceneItem);
        buttonSceneItem.transform.SetParent(ScriptsManager.Instance.scenePropertyRect);
        ObjectTransformComponent objectTransformComponent = buttonSceneItem.GetComponent<ObjectTransformComponent>();
        ControlFlowButton controlFlowButton = buttonSceneItem.GetComponentInChildren<ControlFlowButton>();


        buttonSceneItem.transform.position = new Vector3(1.5f, -1.0f, 0.0f);
        controlFlowButton.referencedTo = AssignButton.Explode.ToString();

        objectTransformComponent.SetTransform(buttonSceneItem.transform.position, buttonSceneItem.transform.eulerAngles, buttonSceneItem.transform.localScale);
        buttonSceneItem.name = controlFlowButton.givenName = controlFlowButton.alternateName = type;
        controlFlowButton.ApplyChanges();

        buttonSceneItem.GetComponentInChildren<Button>().onClick.AddListener(() => { ExplodeEffect(); });

        return buttonSceneItem;
    }

    public void ExplodeEffect()
    {
        //foreach (ObjectExplode item in explodeList)
        //{
        //    if (item.modelFunction.canExplode)
        //    {
        //        if (!item.modelFunction.isInExplodedView && !item.modelFunction.isMoving)
        //        {
        //            item.modelFunction.updateSettings();
        //        }

        //        item.modelFunction.ToggleExplodedView();
        //    }
        //}
        if (ScriptsManager.Instance.CurrentSelectedModel != null)
        {
            ThreeDModelFunctions modelFunction = ScriptsManager.Instance.CurrentSelectedModel.GetComponent<ThreeDModelFunctions>();
            modelFunction.explosionDistance = range.value;
            modelFunction.explosionSpeed = speed.value;
            ControlFlowButton btn = explodeButton.GetComponentInChildren<ControlFlowButton>();

            ScriptsManager.Instance.errorMessage.text = modelFunction.childMeshRenderers.Count > 1 ? "" : "This model cannot be exploded.";

            if (modelFunction.isInExplodedView)
            {
                if (btn.isClicked)
                { 
                    btn.OnUserClick();
                }
            }
            else
            {
                if (!btn.isClicked)
                {
                    btn.OnUserClick();
                }
            }

            modelFunction.updateSettings();

            modelFunction.ToggleExplodedView();
        }

    }

    public void OnExplodeDelegate(PointerEventData eventData)
    {
        // ExplodeEffect();
    }

    public void RemoveComponent()
    {
        //if (explodeButtonReference != null)
        //{
        //    explodeButtonReference.GetComponentInChildren<NewButton>().customButton.dropReference.ClearReference();
        //    explodeButtonReference = null;
        //}

        ScriptsManager.Instance.featureScript.explosion.sprite = ScriptsManager.Instance.featureScript.explosionSprites[0];
        ScriptsManager.Instance.isExplodable = false;

        ClearExplodeList();

        Transform explodeObj = ScriptsManager.Instance.scenePropertyRect.Find("Explode Effect");

        if (explodeObj != null)
        {
            Destroy(explodeObj.gameObject);
        }

        gameObject.SetActive(false);
        ScriptsManager.Instance.currentObjectName.text = "";
        //ScriptsManager.Instance.dsButtons.Remove(explodeButton);
        Destroy(explodeButton);
        ResetValue();

    }

    public void ResetExplodeView()
    {
        //if (explodeList.Count > 0)
        //{
        //    foreach (ObjectExplode item in explodeList)
        //    {
        //        if (item.modelFunction.isInExplodedView)
        //        {
        //            item.modelFunction.ToggleExplodedView();
        //        }
        //    }
        //}
    }

    public void ClearExplodeList()
    {
        //if (explodeList.Count > 0)
        //{
        //    foreach (ObjectExplode item in explodeList)
        //    {
        //        if (item.modelFunction.isInExplodedView)
        //        {
        //            item.modelFunction.ToggleExplodedView();
        //        }
        //        item.modelFunction.canExplode = false;
        //        Destroy(item.gameObject);
        //    }

        //    explodeList.Clear();
        //}
    }

    public void valueChangeMaxRange()
    {
        //for (int i = 0; i < explodeList.Count; i++)
        //{
        //    explodeList[i].modelFunction.explosionDistance = range.value;
        //    explodeList[i].modelFunction.explosionSpeed = speed.value;
        //    explodeList[i].modelFunction.updateSettings();
        //}

        //if (local != null)
        //{
        //    local.GetComponent<ThreeDModelFunctions>().explosionDistance = range.value;
        //    local.GetComponent<ThreeDModelFunctions>().updateSettings();
        //}
        txtRange.text = range.value.ToString("00.00");
    }

    public void valueChangeSpeed()
    {
        //for (int i = 0; i < explodeList.Count; i++)
        //{
        //    explodeList[i].modelFunction.explosionDistance = range.value;
        //    explodeList[i].modelFunction.explosionSpeed = speed.value;
        //    explodeList[i].modelFunction.updateSettings();

        //}

        //if (local != null)
        //{
        //    local.GetComponent<ThreeDModelFunctions>().explosionSpeed = (speed.value * 0.1f);
        //    local.GetComponent<ThreeDModelFunctions>().updateSettings();
        //}
        txtSpeed.text = speed.value.ToString("00.00");
    }

    public void OnSpeedInputValueChanged()
    {
        if (!string.IsNullOrWhiteSpace(txtSpeed.text))
        {
            float inputValue = Helper.Instance.RestrictInputValue(txtSpeed, 0.1f, 1.0f);
            speed.value = inputValue;
        }

    }

    public void OnRangeInputValueChanged()
    {
        if (!string.IsNullOrWhiteSpace(txtRange.text))
        {
            float inputValue = Helper.Instance.RestrictInputValue(txtRange, 1f, 7f);
            range.value = inputValue;
        }

    }
}