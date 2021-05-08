

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaticSequence : XR_studio_ControlFlow
{

    /// <summary>
    /// A skybox to the scene that stays from beginning to end of the control flow, just idle 
    /// </summary>
    public Skybox currentControlFlowSkybox;
    /// <summary>
    /// A Background that stays from beginning to end of the control flow, just idle 
    /// </summary>
    public Sprite currentControlFlowBackground;
    /// <summary>
    /// A gameobject (model or something) that stays from beginning to end of the control flow, just idle 
    /// </summary>
    public Dictionary<GameObject, AssetDetails> currentStaticModels = new Dictionary<GameObject, AssetDetails>();


    //  public List<Button> 



    /// <summary>
    /// This sequence would stay in scene all time 
    /// (e.g) Menu Buttons to trigger ,Skybox or background of the complete scene and idle gameobject that stays till end like a Heading
    /// 
    /// </summary>
    public StaticSequence(Skybox toSkybox, Sprite toBG, Dictionary<GameObject, AssetDetails> models)
    {
        currentControlFlowSkybox = toSkybox;
        currentControlFlowBackground = toBG;
        currentStaticModels = models;
    }

    public void FireStaticSequence()
    {
        if (currentControlFlowBackground != null)
        {
            //assign the given background sprite to the control flow structure 
        }


        if (currentControlFlowSkybox != null)
        {
            //assign the given background skybox to the control flow structure 
        }


        foreach (KeyValuePair<GameObject, AssetDetails> modelDetail in currentStaticModels)
        {

            StartCoroutine(ScriptsManager.Instance.GetAssetBundle(modelDetail.Value, modelDetail.Value.FileName, null,
       (objectDownloaded) =>
       {
           //storing it in the sequence obj parent set itself
           objectDownloaded.transform.SetParent(sequenceObjsParent.transform);

           //isAllActionDownloaded += 1;

           // g = objectDownloaded;

       }));
        }

    }
}

class GFG
{

    // Returns index of x if it is present in arr[], 
    // else return -1 
    static int binarySearch(String[] arr, String x)
    {
        int l = 0, r = arr.Length - 1;
        while (l <= r)
        {
            int m = l + (r - l) / 2;

            int res = x.CompareTo(arr[m]);

            // Check if x is present at mid 
            if (res == 0)
                return m;

            // If x greater, ignore left half 
            if (res > 0)
                l = m + 1;

            // If x is smaller, ignore right half 
            else
                r = m - 1;
        }

        return -1;
    }
}