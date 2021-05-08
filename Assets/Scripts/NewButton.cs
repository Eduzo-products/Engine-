using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System;

/// <summary>
/// Created by Jeffri.
/// </summary>
public class NewButton : MonoBehaviour
//, IDragHandler, IEndDragHandler, IBeginDragHandler
{
    private GameObject draggedItem;
    [SerializeField] internal bool isClicked = false;

    public ButtonType buttonType;
    public GameObject parentObject;
    public Image buttonImage;
    public TMP_Text buttonText;
    public string normalName = "", alternateName = "";
    public float distance, distanceMultiplier;
    public Color32 normalColor, highlightColor, pressedColor, textNormalColor, textHighlightColor;
    public GameObject itemDragPrefab;

    public int dragRefID;
    public CustomButton customButton;
    public EventTrigger eventTrigger;

    public Sprite normalImage;
    public Sprite highlightImage;
    public Sprite pressedImage;
    public Sprite defaultSprite;
    public bool OnAnimation;

    private Coroutine routine;

    public void OnPointerEnter()
    {
        if (!isClicked)
        {
            HighlightButtonColor();
        }
        else
        {
            NormalButtonColor();
        }
    }

    public void OnPointerExit()
    {
        if (!isClicked)
        {
            NormalButtonColor();
        }
        else
        {
            HighlightButtonColor();
        }
    }

    public void OnPointerUp()
    {
        PressedButtonColor();
    }

    public void OnPointerDown()
    {
        if (OnAnimation)
        {
            return;
        }

        isClicked = !isClicked;
        buttonText.text = isClicked ? alternateName : normalName;

        PressedButtonColor();
    }

    public void OnPointerClicked()
    {
        OnPointerDown();

        routine = StartCoroutine(InvokePointerEvent((endRoutine) =>
          {
              if (endRoutine)
              {
                  StopCoroutine(routine);
              }
          }));
    }

    private IEnumerator InvokePointerEvent(Action<bool> endRoutine)
    {
        yield return new WaitForSeconds(0.09f);
        OnPointerExit();

        endRoutine(true);
    }

    private void NormalButtonColor()
    {
        switch (buttonType)
        {
            case ButtonType.ColorTint:
                buttonText.gameObject.SetActive(true);
                buttonImage.color = normalColor;
                buttonText.color = textNormalColor;
                buttonImage.sprite = defaultSprite;
                break;
            case ButtonType.Image:
                buttonText.gameObject.SetActive(false);
                buttonImage.sprite = normalImage;
                break;
            case ButtonType.ImageText:
                buttonText.gameObject.SetActive(true);
                buttonImage.sprite = normalImage;
                buttonText.color = textNormalColor;
                break;
        }
    }

    private void HighlightButtonColor()
    {
        switch (buttonType)
        {
            case ButtonType.ColorTint:
                buttonText.gameObject.SetActive(true);
                buttonImage.color = highlightColor;
                buttonText.color = textHighlightColor;
                buttonImage.sprite = defaultSprite;
                break;
            case ButtonType.Image:
                buttonText.gameObject.SetActive(false);
                buttonImage.sprite = highlightImage;
                break;
            case ButtonType.ImageText:
                buttonText.gameObject.SetActive(true);
                buttonImage.sprite = highlightImage;
                buttonText.color = textHighlightColor;
                break;
        }
    }

    private void PressedButtonColor()
    {
        switch (buttonType)
        {
            case ButtonType.ColorTint:
                buttonText.gameObject.SetActive(true);
                buttonImage.color = pressedColor;
                buttonText.color = textHighlightColor;
                buttonImage.sprite = defaultSprite;
                break;
            case ButtonType.Image:
                buttonText.gameObject.SetActive(false);
                buttonImage.sprite = pressedImage;
                break;
            case ButtonType.ImageText:
                buttonText.gameObject.SetActive(true);
                buttonImage.sprite = pressedImage;
                buttonText.color = textHighlightColor;
                break;
            case ButtonType.None:
                break;
        }
    }

    public void TypeOfButton()
    {
        switch (buttonType)
        {
            case ButtonType.ColorTint:
                buttonImage.sprite = defaultSprite;
                buttonText.gameObject.SetActive(true);
                break;
            case ButtonType.Image:
                buttonImage.sprite = normalImage;
                buttonText.gameObject.SetActive(false);
                break;
            case ButtonType.ImageText:
                buttonImage.sprite = normalImage;
                buttonText.gameObject.SetActive(true);
                break;
            case ButtonType.None:
                break;
        }
    }

    //public void OnDrag(PointerEventData eventData)
    //{
    //    Vector3 mousePosition = Input.mousePosition;
    //    mousePosition.z = distance * distanceMultiplier;
    //    Vector3 temp = Camera.main.ScreenToWorldPoint(mousePosition);
    //    draggedItem.transform.position = new Vector3(temp.x, temp.y, temp.z + 3.138f) / draggedItem.GetComponent<Canvas>().scaleFactor;
    //}

    //public void OnEndDrag(PointerEventData eventData)
    //{
    //    Destroy(draggedItem);
    //}

    //public void OnBeginDrag(PointerEventData eventData)
    //{
    //    GameObject dragItem = Instantiate(itemDragPrefab);
    //    draggedItem = dragItem;
    //    draggedItem.GetComponentInChildren<TMP_Text>().text = normalName;
    //}
}