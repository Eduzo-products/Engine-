using System.Collections;
using System.Collections.Generic;
using RuntimeInspectorNamespace;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

/// <summary>
/// Created by Jeffri.
/// </summary>
public class AttributesManager : MonoBehaviour
{
    [Header("<Runtime References>")]
    public CustomButton selectedButton;

    //Button.
    [Header("<Button>")]
    public RectTransform buttonRect;
    public Image buttonImage;
    public TMP_Text buttonText;

    //Input fields.
    [Header("<Input Fields>")]
    public TMP_InputField nameField;
    public TMP_InputField alternateNameField;

    //Size.
    [Header("<Size Fields>")]
    public TMP_InputField widthField;
    public TMP_InputField heightField;
    public TMP_InputField fontField;

    //Button color.
    [Header("<Button Color Fields>")]
    public Image normalColorField;
    public Image highlightColorField;
    public Image pressedColorField;

    //Text color.
    [Header("<Text Color Fields>")]
    public Image textNormalColorField;
    public Image textHighlightColorField;

    //Colors
    [Header("Button Colors")]
    public Color32 normalColor;
    public Color32 highlightColor;
    public Color32 pressedColor;

    [Header("Text Colors")]
    public Color32 textNormalColor;
    public Color32 textHighlightColor;

    [Header("Button Image")]
    public Image normalImage;
    public Image highlightImage;
    public Image pressedImage;

    public TMP_Text normalImageText;
    public TMP_Text highLightImageText;
    public TMP_Text pressedImageText;

    public ImageComponent normalImageComponent, highlightImageComponent, pressedImageComponent;

    public TMP_Dropdown buttonTypeDropdown;

    [Header("Misc. References")]
    public Sprite defaultSprite;

    public void OnButtonTypeChange()
    {
        switch (buttonTypeDropdown.value)
        {
            case 0:
                buttonImage.color = normalColorField.color;
                buttonText.color = textNormalColorField.color;
                buttonImage.sprite = defaultSprite;
                break;
            case 1:
                buttonImage.color = Color.white;
                buttonImage.sprite = normalImage.sprite;
                break;
            case 2:
                buttonImage.color = Color.white;
                buttonImage.sprite = normalImage.sprite;
                buttonText.color = textNormalColorField.color;
                break;
            case 3:
                break;
        }
    }

    public void OnNameValueChange()
    {
        buttonText.text = nameField.text;
    }

    public void OnWidthValueChange()
    {
        if (widthField.text != "")
        {
            float width = Helper.Instance.RestrictInputValue(widthField, 0.0f, 360.0f);
            float widthChange = Mathf.Clamp(width, 0.0f, 360.0f);
            widthField.text = widthChange.ToString();
            buttonRect.sizeDelta = new Vector2(widthChange, buttonRect.rect.height);
        }
    }

    public void OnHeightValueChange()
    {
        if (heightField.text != "")
        {
            float height = Helper.Instance.RestrictInputValue(heightField, 0.0f, 360.0f);
            heightField.text = height.ToString();
            buttonRect.sizeDelta = new Vector2(buttonRect.rect.width, height);
        }
    }

    public void OnFontSizeChange()
    {
        if (fontField.text != "")
        {
            float fontSize = Helper.Instance.RestrictInputValue(fontField, 0.0f, 360.0f);
            fontField.text = fontSize.ToString();
            buttonText.fontSize = fontSize;
        }
    }

    public void OnButtonNormalColorChange()
    {
        ScriptsManager.Instance.transformGizmo.ManipulationControl();
        ColorPicker.Instance.Show(ButtonNormalChange, normalColor);
    }

    private void ButtonNormalChange(Color32 color)
    {
        buttonImage.color = color;
        normalColor = color;
        normalColorField.color = color;
    }

    public void OnButtonHighlightColorChange()
    {
        ScriptsManager.Instance.transformGizmo.ManipulationControl();
        ColorPicker.Instance.Show(ButtonHighlightColor, highlightColor);
    }

    private void ButtonHighlightColor(Color32 color)
    {
        highlightColor = color;
        highlightColorField.color = color;
    }

    public void OnButtonPressedColorChange()
    {
        ScriptsManager.Instance.transformGizmo.ManipulationControl();
        ColorPicker.Instance.Show(ButtonPressedColor, pressedColor);
    }

    private void ButtonPressedColor(Color32 color)
    {
        pressedColor = color;
        pressedColorField.color = color;
    }

    public void OnTextNormalColorChange()
    {
        ScriptsManager.Instance.transformGizmo.ManipulationControl();
        ColorPicker.Instance.Show(TextNormalColor, textNormalColor);
    }

    private void TextNormalColor(Color32 color)
    {
        buttonText.color = color;
        textNormalColorField.color = color;
    }

    public void OnTextHighlightColorChange()
    {
        ScriptsManager.Instance.transformGizmo.ManipulationControl();
        ColorPicker.Instance.Show(TextHighlightColor, textHighlightColor);
    }

    private void TextHighlightColor(Color32 color)
    {
        textHighlightColorField.color = color;
    }

    public void SavePopUp()
    {
        ScriptsManager.Instance.saveButton = true;
        ScriptsManager.Instance.confirmationPanel.confirmationPanel.SetActive(true);
        ScriptsManager.Instance.confirmationPanel.confirmationTitleText.text = "Save Changes?";
        ScriptsManager.Instance.confirmationPanel.submitButtonText.text = "Save";
        ScriptsManager.Instance.confirmationPanel.confirmationTextFieldText.text = "Do you want to save the changes?";
        ScriptsManager.Instance.confirmationPanel.cancelButtonText.text = "Cancel";
        ScriptsManager.Instance.confirmationPanel.confirmationIcon.sprite = Helper.Instance.PopUpSprite("CustomButton");
    }

    public void ResetPopUp()
    {
        ScriptsManager.Instance.resetButton = true;
        ScriptsManager.Instance.confirmationPanel.confirmationPanel.SetActive(true);
        ScriptsManager.Instance.confirmationPanel.confirmationTitleText.text = "Reset Changes?";
        ScriptsManager.Instance.confirmationPanel.submitButtonText.text = "Reset";
        ScriptsManager.Instance.confirmationPanel.confirmationTextFieldText.text = "Do you want to reset the changes?";
        ScriptsManager.Instance.confirmationPanel.cancelButtonText.text = "Cancel";
        ScriptsManager.Instance.confirmationPanel.confirmationIcon.sprite = Helper.Instance.PopUpSprite("CustomButton");
    }

    public void CancelPopUp()
    {
        ScriptsManager.Instance.cancelButton = true;
        ScriptsManager.Instance.confirmationPanel.confirmationPanel.SetActive(true);
        ScriptsManager.Instance.confirmationPanel.confirmationTitleText.text = "Cancel Editing?";
        ScriptsManager.Instance.confirmationPanel.submitButtonText.text = "Yes";
        ScriptsManager.Instance.confirmationPanel.confirmationTextFieldText.text = "Do you want to cancel editing the button?";
        ScriptsManager.Instance.confirmationPanel.cancelButtonText.text = "No";
        ScriptsManager.Instance.confirmationPanel.confirmationIcon.sprite = Helper.Instance.PopUpSprite("CustomButton");
    }

    public void SaveChanges()
    {
        ScriptsManager.Instance.runtimeHierarchy.Refresh();

        selectedButton.buttonType = (ButtonType)buttonTypeDropdown.value;

        if (selectedButton.buttonType == ButtonType.ColorTint || selectedButton.buttonType == ButtonType.ImageText)
        {
            selectedButton.name = nameField.text;
        }
        else
        {
            string buttonName = selectedButton.normalImageURL.Split('/')[selectedButton.normalImageURL.Split('/').Length - 1].Split('.')[0];
            selectedButton.name = buttonName;
        }

        if (widthField.text != "")
        {
            selectedButton.width = float.Parse(widthField.text);
        }
        else
        {
            selectedButton.width = 0.0f;
        }

        if (heightField.text != "")
        {
            selectedButton.height = float.Parse(heightField.text);
        }
        else
        {
            selectedButton.height = 0.0f;
        }

        if (fontField.text != "")
        {
            selectedButton.fontSize = float.Parse(fontField.text);
        }
        else
        {
            selectedButton.fontSize = 0.0f;
        }

        selectedButton.buttonObj.name = selectedButton.name + " " + selectedButton.ID;
        selectedButton.customButtonName = selectedButton.buttonObj.name;
        selectedButton.alternateName = alternateNameField.text;
        selectedButton.normalColor = normalColorField.color;
        selectedButton.highlightColor = highlightColorField.color;
        selectedButton.pressedColor = pressedColorField.color;
        selectedButton.textNormal = textNormalColorField.color;
        selectedButton.textHighlight = textHighlightColorField.color;
        selectedButton.defaultSprite = defaultSprite;
        selectedButton.image.color = normalColorField.color;
        selectedButton.imageText.color = textNormalColorField.color;
        selectedButton.imageText.fontSize = selectedButton.fontSize;
        selectedButton.normalImage = normalImage.sprite;
        selectedButton.highlightImage = highlightImage.sprite;
        selectedButton.pressedImage = pressedImage.sprite;
        selectedButton.normalImageText = normalImageText.text;
        selectedButton.highLightImageText = highLightImageText.text;
        selectedButton.pressedImageText = pressedImageText.text;
        selectedButton.nameText.gameObject.SetActive(false);
        selectedButton.buttonObj.GetComponent<ObjectTransformComponent>().currentTransform.objectFullPath = ScriptsManager.Instance.GetObjectFullPath(selectedButton.buttonObj);
        selectedButton.path = selectedButton.buttonObj.GetComponent<ObjectTransformComponent>().currentTransform.objectFullPath;

        switch (selectedButton.buttonType)
        {
            case ButtonType.ColorTint:
                selectedButton.nameText.gameObject.SetActive(true);
                selectedButton.thisButton.sprite = selectedButton.defaultSprite;
                selectedButton.thisButton.color = selectedButton.normalColor;
                selectedButton.nameText.color = selectedButton.textNormal;
                break;
            case ButtonType.Image:
                selectedButton.thisButton.sprite = selectedButton.normalImage;
                break;
            case ButtonType.ImageText:
                selectedButton.nameText.gameObject.SetActive(true);
                selectedButton.thisButton.sprite = selectedButton.normalImage;
                selectedButton.nameText.color = selectedButton.textNormal;
                break;
        }

        selectedButton.nameText.text = selectedButton.imageText.text = nameField.text;

        selectedButton.buttonObj.GetComponentInChildren<RectTransform>().sizeDelta = new Vector2(selectedButton.width, selectedButton.height);

        NewButton newButton = selectedButton.buttonObj.GetComponentInChildren<NewButton>();
        newButton.normalName = nameField.text;
        newButton.alternateName = alternateNameField.text;
        newButton.normalColor = normalColorField.color;
        newButton.highlightColor = highlightColorField.color;
        newButton.pressedColor = pressedColorField.color;
        newButton.textNormalColor = textNormalColorField.color;
        newButton.textHighlightColor = textHighlightColorField.color;
        newButton.customButton = selectedButton;
        newButton.buttonType = selectedButton.buttonType;
        newButton.normalImage = selectedButton.normalImage;
        newButton.highlightImage = selectedButton.highlightImage;
        newButton.pressedImage = selectedButton.pressedImage;
        newButton.defaultSprite = selectedButton.defaultSprite;
        newButton.dragRefID = selectedButton.ID;

        newButton.TypeOfButton();
       
        ScriptsManager.Instance.ButtonAttributeChanged(newButton);
    }

    public void OnPointerEnter()
    {
        switch (buttonTypeDropdown.value)
        {
            case 0:
                buttonImage.color = highlightColorField.color;
                buttonText.color = textHighlightColorField.color;
                break;
            case 1:
                buttonImage.sprite = highlightImage.sprite;
                break;
            case 2:
                buttonImage.sprite = highlightImage.sprite;
                buttonText.color = textHighlightColorField.color;
                break;
            case 3:
                Debug.Log("OnPointerEnter Case 3");
                break;
        }
    }

    public void OnPointerExit()
    {
        switch (buttonTypeDropdown.value)
        {
            case 0:
                buttonImage.color = normalColorField.color;
                buttonText.color = textNormalColorField.color;
                break;
            case 1:
                buttonImage.sprite = normalImage.sprite;
                break;
            case 2:
                buttonImage.sprite = normalImage.sprite;
                buttonText.color = textNormalColorField.color;
                break;
            case 3:
                Debug.Log("OnPointerExit Case 3");
                break;
        }
    }

    public void OnPointerDown()
    {
        switch (buttonTypeDropdown.value)
        {
            case 0:
                buttonImage.color = pressedColorField.color;
                buttonText.color = textHighlightColorField.color;
                break;
            case 1:
                buttonImage.sprite = pressedImage.sprite;
                break;
            case 2:
                buttonImage.sprite = pressedImage.sprite;
                buttonText.color = textHighlightColorField.color;
                break;
            case 3:
                Debug.Log("OnPointerDown Case 3");
                break;
        }
    }

    public void OnPointerUp()
    {
        OnPointerExit();
    }

    public void ResetValues()
    {
        buttonTypeDropdown.value = (int)ButtonType.ColorTint;
        buttonText.gameObject.SetActive(true);

        widthField.text = "160";
        heightField.text = "30";
        fontField.text = "18";
        alternateNameField.text = nameField.text = buttonText.text = "Button";
        normalColorField.color = normalColor = highlightColorField.color = pressedColorField.color = Color.white;
        textNormalColorField.color = textNormalColor = textNormalColor = textHighlightColorField.color = textHighlightColor = new Color32(0x32, 0x32, 0x32, 0xFF);
        buttonImage.color = normalColor;
        buttonText.color = textNormalColor;
        buttonImage.sprite = normalImage.sprite = highlightImage.sprite = pressedImage.sprite = defaultSprite;
        normalImageText.text = highLightImageText.text = pressedImageText.text = "";
        ResetImageURL();
    }

    public void ResetImageURL()
    {
        //normalImageComponent.imageURLField.text = highlightImageComponent.imageURLField.text = pressedImageComponent.imageURLField.text =
        //normalImageComponent.url = highlightImageComponent.url = pressedImageComponent.url = "";
    }
}