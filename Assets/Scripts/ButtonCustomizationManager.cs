using RuntimeInspectorNamespace;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Created by Jeffri. 
/// </summary>
public enum AssignButton
{
    ModelRotate, ModelScale, GestureMove, GestureRotate,
    GestureScale, Explode, Video, Labels, Batching, Browser,
    Animation, Back, Audio, TextPanel, Button, NewAnimation, AnimationControlFlow,
    ButtonControlFlowTrigger, WalkThrough, Reset, Other
}

public class ButtonCustomizationManager : MonoBehaviour
{
    public static ButtonCustomizationManager Instance = null;

    public bool audioAdded = false;
    public bool videoAdded = false;
    public bool moveAdded = false;
    public bool rotateAdded = false;
    public bool scaleAdded = false;
    public bool explodeAdded = false;
    public bool browserAdded = false;
    public bool animationAdded = false;
    public bool batchingAdded = false;

    [Header("<Transform References>")]
    public Transform parentTransform;
    public Transform mainPanel, attributesPanel;

    [Header("<GameObject References>")]
    public AttributesManager attributesManager;

    public GameObject buttonCustomization;
    public GameObject previewPrefab;
    public GameObject buttonPrefab;
    public GameObject infoPanel;
    public GameObject buttonListPanel;

    public TMP_Dropdown buttonType;

    [Header("<Field References>")]
    public GameObject buttonTextReference;
    public GameObject nameReference;
    public GameObject alternateNameReference;
    public GameObject buttonColorReference;
    public GameObject buttonImageReference;
    public GameObject textColorReference;
    public GameObject fontReference;

    [Header("<List of custom buttons>")]
    [SerializeField] protected internal List<CustomButton> prefabsList = new List<CustomButton>();

    //Miscellaneous variables.
    public ScrollRect scrollRect;
    public int uniqueID;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        SubscribeDelegate();
    }

    private void OnDestroy()
    {
        ScriptsManager.OnCloseClicked -= OnDeleteButtons;
        PublishContent.OnSaveContentClick -= OnSaveButtons;
    }

    public void SubscribeDelegate()
    {
        PublishContent.OnSaveContentClick += OnSaveButtons;
        ScriptsManager.OnCloseClicked += OnDeleteButtons;
    }

    private void OnSaveButtons()
    {
        if (prefabsList.Count > 0)
        {
            for (int p = 0; p < prefabsList.Count; p++)
            {
                prefabsList[p].customButtons = new CustomButtons();

                prefabsList[p].customButtons.ID = prefabsList[p].ID;
                prefabsList[p].customButtons.name = prefabsList[p].name;
                prefabsList[p].customButtons.alternateName = prefabsList[p].alternateName;
                prefabsList[p].customButtons.customButtonName = prefabsList[p].customButtonName;
                prefabsList[p].customButtons.width = prefabsList[p].width;
                prefabsList[p].customButtons.height = prefabsList[p].height;
                prefabsList[p].customButtons.fontSize = prefabsList[p].fontSize;
                prefabsList[p].customButtons.normalColor = prefabsList[p].normalColor;
                prefabsList[p].customButtons.highlightColor = prefabsList[p].highlightColor;
                prefabsList[p].customButtons.pressedColor = prefabsList[p].pressedColor;
                prefabsList[p].customButtons.textNormal = prefabsList[p].textNormal;
                prefabsList[p].customButtons.textHighlight = prefabsList[p].textHighlight;
                prefabsList[p].customButtons.referencedTo = prefabsList[p].referencedTo;
                prefabsList[p].customButtons.path = prefabsList[p].path;
                prefabsList[p].customButtons.normalImageURL = prefabsList[p].normalImageURL;
                prefabsList[p].customButtons.highlightImageURL = prefabsList[p].highlightImageURL;
                prefabsList[p].customButtons.pressedImageURL = prefabsList[p].pressedImageURL;
                prefabsList[p].customButtons.buttonType = (int)prefabsList[p].buttonType;

                ScriptsManager._toolContent.customButtons.Add(prefabsList[p].customButtons);
            }
        }
    }

    private void OnDeleteButtons()
    {
        if (prefabsList.Count > 0)
        {
            for (int p = 0; p < prefabsList.Count; p++)
            {
                Destroy(prefabsList[p].buttonObj);
                Destroy(prefabsList[p].gameObject);
                uniqueID--;
            }
            uniqueID = 0;
            prefabsList.Clear();
        }
    }

    public void OpenButtonsPanel()
    {
        ScriptsManager.Instance.transformGizmo.ManipulationControl();
        ScriptsManager.Instance.enableDisableComponent.Reset();
        buttonCustomization.SetActive(true);
        mainPanel.gameObject.SetActive(true);

        SwitchInfoPanel();
    }

    public void CloseButtonsPanel()
    {
        buttonCustomization.SetActive(false);
        mainPanel.gameObject.SetActive(false);
        Battlehub.RTCommon.Pointer.isSceneWindowEnter = true;
    }

    public void OpenAttributesPanel()
    {
        attributesPanel.gameObject.SetActive(true);
        mainPanel.gameObject.SetActive(false);
    }

    public void CloseAttributesPanel()
    {
        attributesManager.ResetImageURL();
        attributesPanel.gameObject.SetActive(false);
        mainPanel.gameObject.SetActive(true);
    }

    public void OnButtonTypeChange()
    {
        ColorPicker.Instance.Close();

        nameReference.SetActive(false);
        buttonTextReference.SetActive(false);
        alternateNameReference.SetActive(false);
        buttonColorReference.SetActive(false);
        buttonImageReference.SetActive(false);
        textColorReference.SetActive(false);
        fontReference.SetActive(false);

        switch (buttonType.value)
        {
            case 0:
                nameReference.SetActive(true);
                buttonTextReference.SetActive(true);
                alternateNameReference.SetActive(true);
                buttonColorReference.SetActive(true);
                textColorReference.SetActive(true);
                fontReference.SetActive(true);
                break;
            case 1:
                buttonImageReference.SetActive(true);
                break;
            case 2:
                nameReference.SetActive(true);
                buttonTextReference.SetActive(true);
                alternateNameReference.SetActive(true);
                buttonImageReference.SetActive(true);
                textColorReference.SetActive(true);
                fontReference.SetActive(true);
                break;
            case 3:
                nameReference.SetActive(true);
                buttonTextReference.SetActive(true);
                fontReference.SetActive(true);
                break;
        }
    }

    public Dictionary<int, NewButton> dict = new Dictionary<int, NewButton>();

    /// <summary>
    /// Use this method for creating new custom buttons.
    /// </summary>
    public void CreateNewButton()
    {
        if (uniqueID < 30)
        {
            uniqueID++;
            GameObject prefabButton = Instantiate(previewPrefab, parentTransform);
            CustomButton cb = prefabButton.GetComponent<CustomButton>();

            GameObject button = Instantiate(buttonPrefab, ScriptsManager.Instance.objectCollection.transform);
            prefabButton.name = button.name = cb.customButtonName = $"Custom Button ({uniqueID})";
            button.GetComponentInChildren<EventTrigger>().enabled = false;
            button.AddComponent<ObjectTransformComponent>();
            button.GetComponent<ObjectTransformComponent>().SetTransform(button.transform.position, button.transform.localRotation.eulerAngles, button.transform.localScale);
            button.GetComponent<ObjectTransformComponent>().currentTransform.objectFullPath = ScriptsManager.Instance.GetObjectFullPath(button);
            button.GetComponent<ObjectTransformComponent>().currentTransform.objectName = button.name;
            button.transform.localPosition = ScriptsManager.Instance.objectCollection.transform.position;

            cb.buttonObj = button;
            cb.nameText.text = cb.name = cb.alternateName = "Button";
            cb.attributesManager = attributesManager;
            cb.image = button.GetComponentInChildren<Image>();
            cb.ID = uniqueID;
            cb.imageText = button.GetComponentInChildren<TMP_Text>();
            cb.path = button.GetComponent<ObjectTransformComponent>().currentTransform.objectFullPath;

            NewButton newButton = button.GetComponentInChildren<NewButton>();
            newButton.normalName = newButton.alternateName = newButton.buttonText.text = cb.name;
            newButton.dragRefID = cb.ID;
            newButton.customButton = cb;
            newButton.buttonType = cb.buttonType;
            newButton.eventTrigger = button.GetComponentInChildren<EventTrigger>();
            prefabsList.Add(cb);
            if (!dict.ContainsKey(newButton.customButton.ID))
                dict.Add(newButton.customButton.ID, newButton);
            SwitchInfoPanel();
        }
    }

    internal void SwitchInfoPanel()
    {
        if (parentTransform.childCount <= 0)
        {
            infoPanel.SetActive(true);
            buttonListPanel.SetActive(false);
        }
        else
        {
            infoPanel.SetActive(false);
            buttonListPanel.SetActive(true);
        }
    }
}