using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Created By Jeffri
/// </summary>
public class ExistingProjectButton : MonoBehaviour
{
    [Header ("<------- Runtime References ------->")]
    public ExistingProjectManager projectManager;
    public int buttonID;
    public string projectName;
    public Color normalColor, selectedColor;

    [Header ("<------- Manual References ------->")]
    public Image buttonImage;
    public TMP_Text buttonName, status, lastModified;

    public void ButtonColors ( )
    {
        for (int p = 0; p < projectManager.buttons.Count; p++)
        {
            if (projectManager.buttons [p].GetComponent<ExistingProjectButton> ( ).buttonID == buttonID)
            {
                projectManager.buttons [p].GetComponent<ExistingProjectButton> ( ).buttonImage.color = selectedColor;
                projectManager.selectedProjectName = projectName;
                projectManager.submitButton.interactable = true;
            }
            else
            {
                projectManager.buttons [p].GetComponent<ExistingProjectButton> ( ).buttonImage.color = normalColor;
            }
        }
    }
}