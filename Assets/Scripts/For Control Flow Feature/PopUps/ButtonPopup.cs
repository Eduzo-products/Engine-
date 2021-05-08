using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

[Serializable]
public struct ButtonPopup
{
    public GameObject popup;
    public GameObject errorMessage;
    public Sprite rectangleSprite;
    public Sprite curvedSprite;
    public Button confirmButton;
    public Button cancelButton;
    public TMP_Dropdown type;
    public TMP_InputField nameField, alternateNameField;
    public TMP_InputField widthField, heightField, fontSizeField;
    public Image normalColor, highlightColor, pressedColor;
    public Image textNormalColor;
    public Button resetButton;
    public Image previewButton;
    public TextMeshProUGUI previewButtonText;
}
