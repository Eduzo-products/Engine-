using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ObjectExplode : MonoBehaviour
{
    public Transform model;
    public string nameOfGameObject;
    public TMP_Text nameField;
    public Toggle toggle;
    public ThreeDModelFunctions modelFunction;
    public ExplodeComponent explodeComponent;
    public bool isExplodable = false;

    public void OnValueChanged()
    {
        if (toggle.isOn)
        {
            isExplodable = true;
            modelFunction.canExplode = isExplodable;
            modelFunction.updateSettings();
            //if (!explodeComponent.explodeList.Contains(this))
            //{
            //    explodeComponent.explodeList.Add(this);
            //}
        }
        else
        {
            //if (explodeComponent.explodeList.Contains(this))
            //{
            //    explodeComponent.explodeList.Remove(this);
            //}
            isExplodable = false;
            modelFunction.canExplode = isExplodable;
        }
    }

    public void ExplodeSelected()
    {
        if (isExplodable)
        {
            modelFunction.ToggleExplodedView();
        }
    }
}