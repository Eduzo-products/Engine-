using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddLights : MonoBehaviour
{
   // public GameObject lightPrefab;
    public int lightsCount;

    public void CreateLights()
    {
        ScriptsManager.Instance.sceneLight.SetActive(false);
        ScriptsManager.Instance.enableDisableComponent.Reset();
        ScriptsManager.Instance.gameObject.GetComponent<Label_components>().ClearComp();
        //ScriptsManager.Instance.lightComponent.SetActive(true);
        //ScriptsManager.Instance.featureScript.lightImg.sprite = ScriptsManager.Instance.featureScript.lightSprites[1];
        LightComponent componentLight = ScriptsManager.Instance.lightComponent.GetComponent<LightComponent>();
        if (lightsCount < 1 && componentLight.noTransform==null)
        {
            lightsCount++;

            

            
            componentLight.addLights = this;

            GameObject light = Instantiate(ScriptsManager.Instance.lightPrefab, ScriptsManager.Instance.scenePropertyRect);
            int lightPresentcount = 0;
            char alreadyLightCount = '0';
            for (int i = 0; i < ScriptsManager.Instance.scenePropertyRect.transform.childCount; i++)
            {
                if (ScriptsManager.Instance.scenePropertyRect.transform.GetChild(i).GetComponent<NoTransform>())
                {
                    if (ScriptsManager.Instance.scenePropertyRect.transform.GetChild(i).GetComponent<NoTransform>().propertyType == TypesOfProperty.Light)
                    {
                        char[] array1 = ScriptsManager.Instance.scenePropertyRect.transform.GetChild(i).name.ToCharArray();
                        lightPresentcount++;
                        alreadyLightCount = array1[array1.Length - 2];
                    }
                }
            }
            if (lightPresentcount == 0)
            {
                light.name = $"Light ({1})";

            }
            else
            {
                if (alreadyLightCount == '1')
                {
                    light.name = $"Light ({2})";
                }
                else
                {
                    light.name = $"Light ({1})";
                }
            }
            light.AddComponent<NoTransform>();

            NoTransform noTransform = light.GetComponent<NoTransform>();

            noTransform.propertyType = TypesOfProperty.Light;
            noTransform.lightComponent = componentLight;
            noTransform.componentObject = light;
            noTransform.light = noTransform.componentObject.GetComponent<Light>();
            noTransform.light.type = LightType.Directional;
            noTransform.lightValue = (int)noTransform.light.type;
            noTransform.colorValue = noTransform.light.color;
            noTransform.intensity = noTransform.light.intensity;
            noTransform.indirectMultiplier = noTransform.light.bounceIntensity;
            noTransform.angleValue = noTransform.light.spotAngle;
            noTransform.rangeValue = noTransform.light.range;

            componentLight.noTransform = noTransform;
            componentLight.light = noTransform.componentObject.GetComponent<Light>();
            noTransform.GetLightPath(noTransform.light.gameObject);
            //ScriptsManager.Instance.enableDisableComponent.Reset();
            ScriptsManager.Instance.runtimeHierarchy.Refresh();
            //ScriptsManager.Instance.runtimeHierarchy.Select(light.transform);
            ScriptsManager.Instance.lightComponent.SetActive(true);
            ScriptsManager.Instance.featureScript.lightImg.sprite = ScriptsManager.Instance.featureScript.lightSprites[1];

            if (lightsCount == 1)
            {
                ScriptsManager.Instance.AddErrorMessage("Cannot add more than one light in the scene.");
            }
        }else
        {
            
           // LightComponent componentLight = ScriptsManager.Instance.lightComponent.GetComponent<LightComponent>();
            ScriptsManager.Instance.runtimeHierarchy.Refresh();
            ScriptsManager.Instance.lightComponent.SetActive(true);
            ScriptsManager.Instance.featureScript.lightImg.sprite = ScriptsManager.Instance.featureScript.lightSprites[1];

        }
       
    }
}