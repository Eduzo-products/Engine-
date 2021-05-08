using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class Helper : Singleton<Helper>
{
    public List<ConfirmationPanelIcons> iconSprites = new List<ConfirmationPanelIcons>();

    /// <summary>
    /// Use this method to change the sprite on the confirmation panel and pop up panel.
    /// Parameter is case sensitive.
    /// </summary>
    /// <param name="typeOfComponent"></param>
    /// <returns></returns>
    public Sprite PopUpSprite(string typeOfComponent)
    {
        if (typeOfComponent != "")
        {
            int count = iconSprites.Count;

            if (count > 0)
            {
                for (int p = 0; p < count; p++)
                {
                    if (iconSprites[p].iconName.ToLower() == typeOfComponent.ToLower())
                    {
                        return iconSprites[p].iconSprite;
                    }
                }
            }
        }

        return null;
    }

    /// <summary>
    /// To restrict inputfield value till given float values.
    /// </summary>
    /// <param name="inputField"></param>
    /// <returns></returns>
    public float RestrictInputValue(TMP_InputField inputField, float minValue, float maxValue)
    {
        float inputValue = 0;
        bool isSucessful = float.TryParse(inputField.text, out inputValue);
        inputValue = (!isSucessful) ? (inputValue * 0.0f) : inputValue;

        float restrict = Mathf.Clamp(inputValue, minValue, maxValue);
        inputField.text = inputValue.ToString();

        return restrict;
    }

    /// <summary>
    /// To restrict inputfield value till given int values.
    /// </summary>
    /// <param name="inputField"></param>
    /// <returns></returns>
    public int RestrictInputValue(TMP_InputField inputField, int minValue, int maxValue)
    {
        int inputValue = 0;
        bool isSucessful = int.TryParse(inputField.text, out inputValue);
        inputValue = (!isSucessful) ? (inputValue * 0) : inputValue;

        int restrict = Mathf.Clamp(inputValue, minValue, maxValue);
        inputField.text = inputValue.ToString();

        return restrict;
    }

    public IEnumerator SetRefTemp(string path, Action<GameObject> objFetched = null)
    {
        GameObject obj = null;
        yield return new WaitUntil(() => obj = SearchAndSetObjectOnSequence(path));

        objFetched(obj);
    }

    public GameObject SearchAndSetObjectOnSequence(string path)
    {
        return GameObject.Find(path);
    }
}