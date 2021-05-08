using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using RuntimeInspectorNamespace;
using UnityEngine.EventSystems;

public class ButtonPropertyControlFlow : PopupBase, IPopupManager, IButtonProperty
{
    // private GameObject sceneHierarchyItem;
    private GameObject buttonObject;
    private Image button;
    private Sprite rectangleSprite;
    private Sprite curvedSprite;
    private TextMeshProUGUI buttonText;
    private ControlFlowButton controlFlowButton;
    private GameObject errorMessage;
    private TMP_Dropdown type;
    public TMP_InputField nameField, alternateNameField;
    private TMP_InputField widthField, heightField, fontSizeField;
    private Image normalColor, highlightColor, pressedColor;
    private Image textNormalColor;
    private Button confirmButton, resetButton, cancelButton;
    private Image previewButton;
    private TextMeshProUGUI previewButtonText;
    private bool isClicked = false;

    //delegate for the name change on the scene line item
    public static Action<string, GameObject> delegateToSendNameItem;

    public ButtonPropertyControlFlow(ButtonPopup popUpVariables, string name, GameObject buttonObject)
    {
        this.buttonObject = buttonObject;
        //   this.sceneHierarchyItem = sceneHierarchyItem;
        button = buttonObject.GetComponentInChildren<Image>();
        buttonText = buttonObject.GetComponentInChildren<TextMeshProUGUI>();
        rectangleSprite = popUpVariables.rectangleSprite;
        curvedSprite = popUpVariables.curvedSprite;
        controlFlowButton = buttonObject.GetComponentInChildren<ControlFlowButton>();
        controlFlowButton.givenName = name;
        controlFlowButton.uniqueID = PopupManager.uniqueButtonID;
        popup = popUpVariables.popup;
        type = popUpVariables.type;
        nameField = popUpVariables.nameField;
        alternateNameField = popUpVariables.alternateNameField;
        widthField = popUpVariables.widthField;
        heightField = popUpVariables.heightField;
        fontSizeField = popUpVariables.fontSizeField;
        normalColor = popUpVariables.normalColor;
        highlightColor = popUpVariables.highlightColor;
        pressedColor = popUpVariables.pressedColor;
        textNormalColor = popUpVariables.textNormalColor;
        confirmButton = popUpVariables.confirmButton;
        cancelButton = popUpVariables.cancelButton;
        resetButton = popUpVariables.resetButton;
        previewButton = popUpVariables.previewButton;
        previewButtonText = popUpVariables.previewButtonText;
        errorMessage = popUpVariables.errorMessage;

        type.value = controlFlowButton.type;
        widthField.text = controlFlowButton.width.ToString();
        heightField.text = controlFlowButton.height.ToString();
        fontSizeField.text = controlFlowButton.fontSize.ToString();
        normalColor.color = controlFlowButton.normalColor;
        highlightColor.color = controlFlowButton.highlightColor;
        pressedColor.color = controlFlowButton.pressedColor;
        textNormalColor.color = controlFlowButton.textColor;

        confirmButton.onClick.AddListener(() => { Confirmation(); });
        cancelButton.onClick.AddListener(() => { Cancellation(); });
        type.onValueChanged.AddListener(delegate { OnTypeChange(); });
        nameField.onValueChanged.AddListener(delegate { OnNameFieldChange(); });
        widthField.onValueChanged.AddListener(delegate { OnWidthChange(); });
        heightField.onValueChanged.AddListener(delegate { OnHeightChange(); });
        fontSizeField.onValueChanged.AddListener(delegate { OnFontSizeChange(); });
        normalColor.GetComponent<Button>().onClick.AddListener(() => { OnNormalColorChange(); });
        highlightColor.GetComponent<Button>().onClick.AddListener(() => { OnHighlightColorChange(); });
        pressedColor.GetComponent<Button>().onClick.AddListener(() => { OnPressedColorChange(); });
        textNormalColor.GetComponent<Button>().onClick.AddListener(() => { OnTextNormalColorChange(); });
        previewButton.GetComponent<Button>().onClick.AddListener(() => { OnButtonClicked(); });
        resetButton.onClick.AddListener(() => { DefaultValues(); });

        nameField.text = buttonText.text = previewButtonText.text = name;
        alternateNameField.text = controlFlowButton.alternateName;
        SetPreviewButton();
        if (ScriptsManager.Instance.projectTypeDropdown.value.Equals(1))
        {
            alternateNameField.transform.parent.gameObject.SetActive(false);
        }
        else
        {
            alternateNameField.transform.parent.gameObject.SetActive(true);
        }
    }

    private void SetPreviewButton()
    {
        RectTransform previewRectTransform = previewButton.GetComponent<RectTransform>();
        float height = heightField.text.Length > 0 ? float.Parse(heightField.text) : 0.0f;
        float width = widthField.text.Length > 0 ? float.Parse(widthField.text) : 0.0f;
        previewButtonText.fontSize = fontSizeField.text.Length > 0 ? float.Parse(fontSizeField.text) : 0.0f;
        previewButtonText.color = textNormalColor.color;
        previewRectTransform.sizeDelta = new Vector2(width, height);
        previewButton.color = normalColor.color;

        ColorBlock colorBlock = previewButton.GetComponent<Button>().colors;
        colorBlock.normalColor = normalColor.color;
        colorBlock.highlightedColor = highlightColor.color;
        colorBlock.pressedColor = pressedColor.color;
        previewButton.GetComponent<Button>().colors = colorBlock;
    }

    public override void OpenPopUp()
    {
        popup.SetActive(true);
        OnTypeChange();
    }

    public override void ClosePopUp()
    {
        popup.SetActive(false);
        errorMessage.SetActive(false);
        DefaultValues();
        ColorPicker.Instance.Close();
        RemoveAllListeners();

        //delegate for editing the name and details of the button (remove)
        delegateToSendNameItem = null;
    }

    public void Cancellation()
    {
        ClosePopUp();
    }

    public void Confirmation()
    {
        ApplyChanges();
        ClosePopUp();
    }

    private void DefaultValues()
    {
        type.value = 1;
        if (ScriptsManager.Instance.projectTypeDropdown.value == 1)
        {
            nameField.text = "Scene";
        }
        else
        {
            nameField.text = alternateNameField.text = "Button";
        }
        widthField.text = "160";
        heightField.text = "30";
        fontSizeField.text = "18";
        isClicked = false;
        previewButton.color = normalColor.color = new Color32(0xFF, 0xFF, 0xFF, 0xFF);
        highlightColor.color = new Color32(0xCA, 0xCA, 0xCA, 0xFF);
        pressedColor.color = new Color32(0x9A, 0x9A, 0x9A, 0xFF);
        textNormalColor.color = new Color32(0x00, 0x00, 0x00, 0xFF);

        ColorBlock colorBlock = previewButton.GetComponent<Button>().colors;
        colorBlock.normalColor = normalColor.color;
        colorBlock.highlightedColor = highlightColor.color;
        colorBlock.pressedColor = pressedColor.color;
        previewButton.GetComponent<Button>().colors = colorBlock;

        previewButtonText.color = textNormalColor.color;
    }

    private void ApplyChanges()
    {
        if (nameField.text.Length > 0)
        {
            controlFlowButton.type = type.value;
            buttonObject.name = controlFlowButton.givenName = nameField.text;
            controlFlowButton.alternateName = alternateNameField.text;
            controlFlowButton.width = widthField.text.Length > 0 ? float.Parse(widthField.text) : 0.0f;
            controlFlowButton.height = heightField.text.Length > 0 ? float.Parse(heightField.text) : 0.0f;
            controlFlowButton.fontSize = fontSizeField.text.Length > 0 ? float.Parse(fontSizeField.text) : 0.0f;
            controlFlowButton.normalColor = normalColor.color;
            controlFlowButton.highlightColor = highlightColor.color;
            controlFlowButton.pressedColor = pressedColor.color;
            controlFlowButton.textColor = textNormalColor.color;

            if (ScriptsManager.Instance.projectTypeDropdown.value == 1)
            {
                //need to send the altered name to the particular line item on the scene panel (there is a null reference error with the below line, will resolve later)
                delegateToSendNameItem(nameField.text, buttonObject);
            }
            controlFlowButton.ApplyChanges();
        }
        else
        {
            errorMessage.SetActive(true);
        }
    }

    private void RemoveAllListeners()
    {
        confirmButton.onClick.RemoveAllListeners();
        cancelButton.onClick.RemoveAllListeners();
        type.onValueChanged.RemoveAllListeners();
        nameField.onValueChanged.RemoveAllListeners();
        widthField.onValueChanged.RemoveAllListeners();
        heightField.onValueChanged.RemoveAllListeners();
        fontSizeField.onValueChanged.RemoveAllListeners();
        normalColor.GetComponent<Button>().onClick.RemoveAllListeners();
        highlightColor.GetComponent<Button>().onClick.RemoveAllListeners();
        pressedColor.GetComponent<Button>().onClick.RemoveAllListeners();
        textNormalColor.GetComponent<Button>().onClick.RemoveAllListeners();
        previewButton.GetComponent<Button>().onClick.RemoveAllListeners();
        resetButton.onClick.RemoveAllListeners();
    }

    private void OnNameFieldChange()
    {
        if (buttonObject != null)
        {
            previewButtonText.text = nameField.text;
        }
    }

    public void OnTypeChange()
    {
        if (buttonObject != null)
        {
            switch (type.value)
            {
                case 0:
                    previewButton.sprite = curvedSprite;
                    break;
                case 1:
                    previewButton.sprite = rectangleSprite;
                    break;
            }
        }
    }

    public void OnWidthChange()
    {
        if (buttonObject != null && widthField.text.Length > 0)
        {
            float width = Helper.Instance.RestrictInputValue(widthField, 0.0f, 360.0f);
            float widthChange = Mathf.Clamp(width, 0.0f, 360.0f);
            widthField.text = widthChange.ToString();
            previewButton.GetComponent<RectTransform>().sizeDelta = new Vector2(widthChange, previewButton.GetComponent<RectTransform>().rect.height);
        }
    }

    public void OnHeightChange()
    {
        if (buttonObject != null && heightField.text.Length > 0)
        {
            float height = Helper.Instance.RestrictInputValue(heightField, 0.0f, 360.0f);
            float heightChange = Mathf.Clamp(height, 0.0f, 244.0f);
            heightField.text = heightChange.ToString();
            previewButton.GetComponent<RectTransform>().sizeDelta = new Vector2(previewButton.GetComponent<RectTransform>().rect.width, heightChange);
        }
    }

    public void OnFontSizeChange()
    {
        if (fontSizeField.text != "")
        {
            float fontSize = Helper.Instance.RestrictInputValue(fontSizeField, 0.0f, 360.0f);
            fontSizeField.text = fontSize.ToString();
            previewButtonText.fontSize = fontSize;
        }
    }

    public void OnNormalColorChange()
    {
        ScriptsManager.Instance.transformGizmo.ManipulationControl();
        ColorPicker.Instance.Show(ButtonNormalColor, normalColor.color);
    }

    private void ButtonNormalColor(Color32 color)
    {
        if (buttonObject != null)
        {
            normalColor.color = previewButton.color = color;

            ColorBlock colorBlock = previewButton.GetComponent<Button>().colors;
            colorBlock.normalColor = color;
            previewButton.GetComponent<Button>().colors = colorBlock;
        }
    }

    public void OnHighlightColorChange()
    {
        ScriptsManager.Instance.transformGizmo.ManipulationControl();
        ColorPicker.Instance.Show(ButtonHighlightColor, highlightColor.color);
    }

    private void ButtonHighlightColor(Color32 color)
    {
        if (buttonObject != null)
        {
            highlightColor.color = color;

            ColorBlock colorBlock = previewButton.GetComponent<Button>().colors;
            colorBlock.highlightedColor = color;
            previewButton.GetComponent<Button>().colors = colorBlock;
        }
    }

    public void OnPressedColorChange()
    {
        ScriptsManager.Instance.transformGizmo.ManipulationControl();
        ColorPicker.Instance.Show(ButtonPressedColor, pressedColor.color);
    }

    private void ButtonPressedColor(Color32 color)
    {
        if (buttonObject != null)
        {
            pressedColor.color = color;

            ColorBlock colorBlock = previewButton.GetComponent<Button>().colors;
            colorBlock.pressedColor = color;
            previewButton.GetComponent<Button>().colors = colorBlock;
        }
    }

    public void OnTextNormalColorChange()
    {
        ScriptsManager.Instance.transformGizmo.ManipulationControl();
        ColorPicker.Instance.Show(TextNormalColor, textNormalColor.color);
    }

    private void TextNormalColor(Color32 color)
    {
        if (buttonObject != null)
        {
            textNormalColor.color = previewButtonText.color = color;
        }
    }

    public void OnButtonClicked()
    {
        isClicked = !isClicked;
        switch (isClicked)
        {
            case true:
                previewButton.color = pressedColor.color;
                if (ScriptsManager.Instance.projectTypeDropdown.value.Equals(0))
                {
                    previewButtonText.text = alternateNameField.text;
                }
                break;
            case false:
                previewButton.color = normalColor.color;
                if (ScriptsManager.Instance.projectTypeDropdown.value.Equals(0))
                {
                    previewButtonText.text = nameField.text;
                }
                break;
        }
        ScriptsManager.Instance.eventSystem.SetSelectedGameObject(null);
    }
}