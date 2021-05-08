using System;
using System.Collections.Generic;
using UnityEngine;
using XrStudio_ControlFlow;

public enum RequestStatus { Success, Failed };

public class GeneralResponseData
{
    public RequestStatus requestStatus;
    public string responseMsg;
}

[Serializable]
public class XR_Studio_Action
{
    public int id;
    public ActionStates thisAction;
    public float durationOfAction;

    public bool thisActionEnded = false;
    public bool thisActionStarted = false;
    public bool isThisActionLive = false;

    public AssetDetails assetDetails = null;
    public TextContent textContent = null;
    public AddAudio audioDetails = null;
    public AddVideo videoDetails = null;
    public CustomButtons customButton = null;
    public AudioClip audioClip;
   // public ImageComponent 
}
[Serializable]
public class SceneSequence
{
    public List<XR_Studio_Action> actionsSequenceControlFlow = new List<XR_Studio_Action>();
    public List<XR_Studio_Trigger> triggersSequenceControlFlow = new List<XR_Studio_Trigger>();
}

[Serializable]
public enum TypeOfTriggers
{

    None, Button, Continue, OnLoad
}

[Serializable]
public class XR_Studio_Trigger
{
    public TypeOfTriggers thisTrigger;
    //assign if available button settings... to button as trigger
    public DSButton buttonObj;
    public bool isTriggered;
    public int id;

   // public GameObject buttObj;

}



[Serializable]
public enum ActionStates
{
    None, Model, Video, Audio, TextToSpeech, Animation,/* Image,*/TextPanel, Button

}

/// <summary>
/// data post downloaded ,stored inside the scene
/// </summary>
[Serializable]
public class SequenceContentDownloaded
{
    //Saving the respective action number and its asset
    public Dictionary<short, GameObject> assetData = new Dictionary<short, GameObject>();
    //turn to list of audio in future
    public AudioClip audioClip;

    public Dictionary<GameObject, string> videoUrl = new Dictionary<GameObject, string>();

    public float sequenceLengthInSeconds;




}

public class ActionVideoClass : IAction
{
    public int actionLength { get; set; }
    public int actionNum { get; set; }
    public GameObject Obj { get => _obj; set => _obj = value; }
    public string Url { get => url; set => url = value; }

    GameObject _obj;

    string url;

    public ActionVideoClass(int actionLength, int actionNum, GameObject obj, string url)
    {
        this.actionLength = actionLength;
        this.actionNum = actionNum;
        Obj = obj;
        Url = url;
    }

    public void PlayAction()
    {

    }

    public void Download()
    {


    }

}
public class ActionModelClass : IAction
{
    public int actionLength { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public int actionNum { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public GameObject Obj { get => _obj; set => _obj = value; }

    GameObject _obj;

    public ActionModelClass(int actionLength, int actionNum, GameObject obj)
    {
        this.actionLength = actionLength;
        this.actionNum = actionNum;
        Obj = obj;
    }

    public void PlayAction()
    {
        Obj.SetActive(false);
    }


}

