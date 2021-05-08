using CommandUndoRedo;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
/// <summary>
/// tempory ... rewrite it again
/// </summary>
public class audioClass : ICommand
{

    [HideInInspector] public GameObject hierarchyReference;
    [HideInInspector] public GameObject linkHolder;

    public AudioClip audioClip;
    [SerializeField]
    public TMP_InputField inputAudioClipPath;
  
    public bool isPlaying;

    public AudioComponent audioComponent;

    public audioClass(GameObject hr, GameObject lh,  TMP_InputField ipacp, AudioComponent ac)
    {
        this.hierarchyReference = hr;
        this.linkHolder = lh;
        this.inputAudioClipPath = ipacp;
        this.audioComponent = ac;
    }

   

    public void StoreNewValues( AudioClip ac, bool isplaying)
    {
        this.audioClip = ac;
        this.isPlaying = isplaying;

    }

    public void Execute()
    {
        OnRedo();
    }

    public void UnExecute()
    {
        OnUndo();
    }

    private void OnUndo()
    {
        audioComponent.RemoveComponent();
    }

    private void OnRedo()
    {
        //create or enable the hieracrchy object
        //set the link and audio to the object
        ScriptsManager.Instance.enableDisableComponent.EnableDisableAudioFeature(true);
    }
}
