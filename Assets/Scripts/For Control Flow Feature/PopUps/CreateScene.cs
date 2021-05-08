using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateScene : PopupBase, IPopupManager, ICustomInstantiate
{
    private TMP_InputField nameField;
    private GameObject buttonHierarchy, buttonScene;
    private Button confirmationButton, cancellationButton;

    public CreateScene(CreateScenePopup popUpVariables, GameObject buttonHierarchy, GameObject buttonScene)
    {
        popup = popUpVariables.popup;
        nameField = popup.GetComponentInChildren<TMP_InputField>();
        this.confirmationButton = popUpVariables.confirmButton;
        this.cancellationButton = popUpVariables.cancelButton;
        this.buttonHierarchy = buttonHierarchy;
        this.buttonScene = buttonScene;

        nameField.onValueChanged.AddListener(delegate { OnNameFieldChange(); });
        this.confirmationButton.onClick.AddListener(() => { Confirmation(); });
        this.cancellationButton.onClick.AddListener(() => { Cancellation(); });

        //for line item Creation in the scene vertical panel (saravanan)
        PopupManager.ButtonNameAction += ControlFlowManagerV2.Instance.CreateScene;
    }

    public override void OpenPopUp()
    {
        popup.SetActive(true);
        nameField.Select();
    }

    public override void ClosePopUp()
    {
        RemoveAllListeners();
        popup.SetActive(false);
        confirmationButton.interactable = false;
        nameField.text = "";
    }

    public void Confirmation()
    {

        //code from saravanan..  in order to avoid the scene name duplication
        if (ControlFlowManagerV2.Instance.sceneLineItems.Any(s => s.Name().Equals(nameField.text)))
        {
            XR_studio_ControlFlow.flowDefault.Instance.SetMissingMessage("A scene exists with same name, please give some other name.");
            return;
        }

        //GameObject buttonHierarchyItem = Instantiate<GameObject>(buttonHierarchy);
        PopupManager.uniqueButtonID++;

        GameObject buttonSceneItem = Instantiate<GameObject>(buttonScene);
        buttonSceneItem.transform.SetParent(ScriptsManager.Instance.scenePropertyRect);
        buttonSceneItem.GetComponent<ObjectTransformComponent>();
        buttonSceneItem.GetComponent<ObjectTransformComponent>().SetTransform(buttonSceneItem.transform.position, buttonSceneItem.transform.eulerAngles, buttonSceneItem.transform.localScale);
        ScriptsManager.Instance.dsButtons.Add(buttonSceneItem);
       // ScriptsManager.Instance.runtimeHierarchy.Refresh();
       // ScriptsManager.Instance.runtimeHierarchy.Select(buttonSceneItem.transform);

        PopupManager.ButtonNameAction(nameField.text, buttonSceneItem);
        ClosePopUp();
    }

    public void Cancellation() => ClosePopUp();


    private void RemoveAllListeners()
    {
        nameField.onValueChanged.RemoveAllListeners();
        confirmationButton.onClick.RemoveAllListeners();
        cancellationButton.onClick.RemoveAllListeners();

        //for line item Creation in the scene vertical panel (saravanan)
        PopupManager.ButtonNameAction -= ControlFlowManagerV2.Instance.CreateScene;
    }

    private void OnNameFieldChange()
    {
        if (nameField != null)
        {
            confirmationButton.interactable = nameField.text.Length > 0 ? true : false;
        }
    }

    public T Instantiate<T>(GameObject objectToInstantiate) where T : Object
    {
        T requestedGameObject = Object.Instantiate(objectToInstantiate) as T;
        requestedGameObject.name = nameField.text;
        return requestedGameObject;
    }
}