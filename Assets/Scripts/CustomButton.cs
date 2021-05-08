using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

/// <summary>
/// Created by Jeffri.
/// </summary>
public enum ButtonType
{
    ColorTint, Image, ImageText, None
}

public class CustomButton : MonoBehaviour
{
    #region Cloud_Related_Variables
    [Header("<Cloud Related Variables>")]
    public ButtonType buttonType;
    public AttributesManager attributesManager;
    public string referencedTo = "", path = "";
    public GameObject buttonObj;

    public int ID = 0;
    public new string name = "Button";
    public string alternateName = "Button";
    public string normalImageURL = "";
    public string highlightImageURL = "";
    public string pressedImageURL = "";
    public bool isAllocated = false;

    public float width = 0.0f, height = 0.0f, fontSize = 0.0f;
    public Image image = null;
    public TMP_Text imageText;

    public Color normalColor, highlightColor, pressedColor;
    public Color textNormal, textHighlight, textDefault;
    #endregion

    #region Scene_Related_Variables
    [Header("<Scene Related Variables>")]
    public string customButtonName = "";
    public Image buttonParent = null;
    public Image thisButton = null;
    public TMP_Text nameText = null;

    public Color parentNormalColor, parentHighlightColor, parentPressedColor;
    public Color nameNormalColor, nameHighlightColor;

    [Header("Button Image")]
    public Sprite normalImage;
    public Sprite highlightImage;
    public Sprite pressedImage;
    public Sprite defaultSprite;

    public string normalImageText;
    public string highLightImageText;
    public string pressedImageText;

    public CustomButtons customButtons;
    #endregion

    public EventTrigger.Entry entry;

    private void Start()
    {
        InitAndSubscribeDelegate();
    }

    public void InitAndSubscribeDelegate()
    {
        textDefault = nameText.color;
    }

    public void OnDeleteButtons()
    {
        DeleteButton();
    }

    public void HighlightButton()
    {
        buttonParent.color = parentHighlightColor;
    }

    public void DehighlightButton()
    {
        buttonParent.color = parentNormalColor;
    }

    public void Clicked()
    {
        DehighlightButton();
    }

    public void DeletePopUp()
    {
        ScriptsManager.Instance.isDeletable = true;
        ScriptsManager.Instance.confirmationPanel.confirmationPanel.SetActive(true);
        ScriptsManager.Instance.confirmationPanel.confirmationTitleText.text = "Delete GameObject?";
        ScriptsManager.Instance.confirmationPanel.submitButtonText.text = "Delete";
        ScriptsManager.Instance.confirmationPanel.confirmationTextFieldText.text = "Do you want to delete the button?";
        ScriptsManager.Instance.confirmationPanel.cancelButtonText.text = "Cancel";
        ScriptsManager.Instance.confirmationPanel.confirmationIcon.sprite = Helper.Instance.PopUpSprite("Delete");

        ScriptsManager.Instance.objecttoBeDeleted = buttonObj.transform;
    }

    public void DeleteButton()
    {
        ButtonCustomizationManager.Instance.prefabsList.Remove(this);

        Destroy(buttonObj);
        Destroy(gameObject);

        ButtonCustomizationManager.Instance.uniqueID--;
        ButtonCustomizationManager.Instance.SwitchInfoPanel();
    }

    public void OpenAttributesPanel()
    {
        ButtonCustomizationManager.Instance.OpenAttributesPanel();

        attributesManager.selectedButton = this;
        attributesManager.alternateNameField.text = attributesManager.nameField.text = attributesManager.buttonText.text = imageText.text;

        AssignAttributeValues();
    }

    protected internal void AssignAttributeValues()
    {
        attributesManager.buttonTypeDropdown.value = (int)buttonType;

        attributesManager.widthField.text = width.ToString();
        attributesManager.heightField.text = height.ToString();
        attributesManager.fontField.text = fontSize.ToString();
        attributesManager.nameField.text = attributesManager.buttonText.text = name;
        attributesManager.alternateNameField.text = alternateName;
        attributesManager.normalColorField.color = attributesManager.normalColor = normalColor;
        attributesManager.highlightColorField.color = attributesManager.highlightColor = highlightColor;
        attributesManager.pressedColorField.color = attributesManager.pressedColor = pressedColor;
        attributesManager.textNormalColorField.color = attributesManager.textNormalColor = textNormal;
        attributesManager.textHighlightColorField.color = attributesManager.textHighlightColor = textHighlight;
        attributesManager.buttonImage.color = attributesManager.normalColor;
        attributesManager.buttonText.color = attributesManager.textNormalColor;
        attributesManager.normalImage.sprite = normalImage;
        attributesManager.highlightImage.sprite = highlightImage;
        attributesManager.pressedImage.sprite = pressedImage;
        attributesManager.normalImageText.text = normalImageText;
        attributesManager.highLightImageText.text = highLightImageText;
        attributesManager.pressedImageText.text = pressedImageText;
        attributesManager.buttonText.gameObject.SetActive(false);
        attributesManager.normalImageComponent.imageURLField.text = attributesManager.normalImageComponent.url = normalImageURL;
        attributesManager.highlightImageComponent.imageURLField.text = attributesManager.highlightImageComponent.url = highlightImageURL;
        attributesManager.pressedImageComponent.imageURLField.text = attributesManager.pressedImageComponent.url = pressedImageURL;

        switch (attributesManager.buttonTypeDropdown.value)
        {
            case 0:
                nameText.gameObject.SetActive(true);
                attributesManager.buttonText.gameObject.SetActive(true);
                attributesManager.buttonImage.sprite = thisButton.sprite = defaultSprite;
                attributesManager.buttonImage.color = thisButton.color = normalColor;
                attributesManager.buttonText.color = nameText.color = textNormal;
                break;
            case 1:
                attributesManager.buttonImage.sprite = thisButton.sprite = normalImage;
                break;
            case 2:
                nameText.gameObject.SetActive(true);
                attributesManager.buttonText.gameObject.SetActive(true);
                attributesManager.buttonImage.sprite = thisButton.sprite = normalImage;
                attributesManager.buttonText.color = nameText.color = textNormal;
                break;
        }
    }
}