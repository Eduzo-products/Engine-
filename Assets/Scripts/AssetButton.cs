using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AssetButton : MonoBehaviour
{
    public Transform arrowCircle = null;
    public GameObject syncObject = null;

    public Button button;
    public Image image;
    public TMP_Text text;

    [Header("Button Colors")]
    public Color normalColor;
    public Color highlightColor, pressedColor, disabledColor;

    [Header("Text Colors")]
    public Color textNormal;
    public Color textHighlight;

    public void HighlightButton()
    {
        if (button.interactable)
        {
            image.color = highlightColor;
            text.color = textHighlight;
        }
    }

    public void DehighlightButton()
    {
        if (button.interactable)
        {
            image.color = normalColor;
            text.color = textNormal;
        }
    }

    public void Clicked()
    {
        if (button.interactable)
        {
            image.color = pressedColor;
            text.color = textNormal;
        }
    }
}