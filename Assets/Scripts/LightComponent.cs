using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RuntimeInspectorNamespace;

/// <summary>
/// Created by Jeffri. 30-1-2020.
/// </summary>
public class LightComponent : MonoBehaviour
{
    public AddLights addLights;
    public new Light light;
    public NoTransform noTransform;
    public TMP_Dropdown lightDropdown;
    public GameObject range, angle;

    public Image colorField;
    public TMP_InputField intensityField, indirectMultiplierField, angleField, rangeField;
    public Slider angleSlider;

    private void Start()
    {
        SubscribeDelegate();
    }

    private void OnDestroy()
    {
        ScriptsManager.OnCloseClicked -= OnCloseProject;
    }

    public void SubscribeDelegate()
    {
        ScriptsManager.OnCloseClicked += OnCloseProject;
    }

    private void OnCloseProject()
    {
        addLights.lightsCount = 0;
        RemoveComponent();
    }

    public void OnTypeChange()
    {
        range.SetActive(false);
        angle.SetActive(false);

        switch (lightDropdown.value)
        {
            case 0:
                noTransform.light.type = LightType.Spot;
                range.SetActive(true);
                angle.SetActive(true);
                break;
            case 1:
                noTransform.light.type = LightType.Directional;
                break;
            case 2:
                noTransform.light.type = LightType.Point;
                range.SetActive(true);
                break;
        }

        noTransform.lightValue = lightDropdown.value;
        if (gameObject.activeSelf)
            StartCoroutine(Enable());
    }

    public void OnRangeFieldChange()
    {
        if (lightDropdown.value != 1)
        {
            if (rangeField.text != "")
            {
                float inputValue = Helper.Instance.RestrictInputValue(rangeField, 0.0f, 999.0f);
                noTransform.rangeValue = light.range = inputValue;
            }
            else
            {
                noTransform.rangeValue = light.range = 0.0f;
            }
        }
    }

    public void OnSliderValueChange()
    {
        if (lightDropdown.value == 0)
        {
            noTransform.angleValue = light.spotAngle = angleSlider.value;
            angleField.text = angleSlider.value.ToString("00.00");
        }
    }

    public void OnSliderFieldChange()
    {
        if (lightDropdown.value == 0)
        {
            if (angleField.text != "")
            {
                float inputValue = Helper.Instance.RestrictInputValue(angleField, 0.0f, 179.0f);
                noTransform.angleValue = light.spotAngle = angleSlider.value = inputValue;
            }
            else
            {
                noTransform.angleValue = light.spotAngle = angleSlider.value = 0.0f;
            }
        }
    }

    public void OnIntensityChange()
    {
        if (intensityField.text != "")
        {
            float inputValue = Helper.Instance.RestrictInputValue(intensityField, 0.0f, 999.0f);

            noTransform.intensity = light.intensity = inputValue;
        }
        else
        {
            noTransform.intensity = light.intensity = 0.0f;
        }
    }

    public void OnEnable()
    {
        StartCoroutine(Enable());
    }

    IEnumerator Enable()
    {
        RectTransform[] temp = GetComponentsInChildren<RectTransform>(true);
        for (int i = 0; i < temp.Length; i++)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(temp[i].GetComponent<RectTransform>());
        }

        yield return null;

        for (int i = 0; i < temp.Length; i++)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(temp[i].GetComponent<RectTransform>());
        }
    }

    public void OnIndirectMultiplierChange()
    {
        if (indirectMultiplierField.text != "")
        {
            float inputValue = Helper.Instance.RestrictInputValue(indirectMultiplierField, 0.0f, 999.0f);
            noTransform.indirectMultiplier = light.bounceIntensity = inputValue;
        }
        else
        {
            noTransform.indirectMultiplier = light.bounceIntensity = 0.0f;
        }
    }

    public void ChangeLightColor()
    {
        ScriptsManager.Instance.transformGizmo.ManipulationControl();
        ColorPicker.Instance.Show(OnLightColorChange, colorField.color);
    }

    private void OnLightColorChange(Color32 color32)
    {
        noTransform.colorValue = light.color = colorField.color = color32;
    }

    public void RemoveComponent()
    {
        ColorPicker.Instance.Close();
        ScriptsManager.Instance.transformComponent.SetActive(false);
        colorField.color = Color.white;
        addLights.lightsCount--;

        if (noTransform)
        {
            Destroy(noTransform.gameObject);
        }

        ScriptsManager.Instance.sceneLight.SetActive(true);
        ScriptsManager.Instance.currentObjectName.text = "";
        ScriptsManager.Instance.featureScript.lightImg.sprite = ScriptsManager.Instance.featureScript.lightSprites[0];
        gameObject.SetActive(false);
    }
}