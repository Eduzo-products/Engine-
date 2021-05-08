using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class LabelWithAudioProperty : MonoBehaviour
{
    //private AudioSource audioSource;
    public AudioClip audioClip;
    public TMP_InputField inputLabelText;
    public TMP_InputField inputAudioClipPath;
    public Label_Publish label_Publish;
    public bool isPlaying = false;
    public TextMeshProUGUI playButtonText;

    private Coroutine coroutine;
    // [SerializeField]
    // private TMP_InputField inputStartTime;
    // [SerializeField]
    // private TMP_InputField inputEndTime;

    // Start is called before the first frame update
    void Start()
    {
        ScriptsManager.OnCloseClicked += CloseProject;
    }

    private void CloseProject()
    {
        RemoveComponent();
    }

    public void OnLabelValueChange()
    {
        if (inputLabelText.text.Length > 0)
        {
            label_Publish.textObj.text = label_Publish.labelWithAudio.labelText = inputLabelText.text;
        }
        else
        {
            label_Publish.textObj.text = "Enter label name";
            label_Publish.labelWithAudio.labelText = "";
        }
    }

    public void OnAudioURLChange()
    {
        label_Publish.labelWithAudio.audioDetails.audioURL = inputAudioClipPath.text;
    }

    public void SetPropertyValue()
    {
        inputLabelText.text = label_Publish.labelWithAudio.labelText;
        inputAudioClipPath.text = label_Publish.labelWithAudio.audioDetails.audioURL;
    }

    private void OnDestroy()
    {
        ScriptsManager.OnCloseClicked -= CloseProject;
    }

    public void PlayPushSample()
    {
        isPlaying = !isPlaying;

        if (isPlaying)
        {
            ScriptsManager.Instance.audioManager.clip = audioClip;
            ScriptsManager.Instance.audioManager.Play();
            playButtonText.text = "stop";
            coroutine = StartCoroutine(EndOfAudio());
        }
        else
        {
            ScriptsManager.Instance.audioManager.Stop();
            playButtonText.text = "play";
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
        }
    }

    private IEnumerator EndOfAudio()
    {
        yield return new WaitUntil(() => ScriptsManager.Instance.audioManager.isPlaying);
        while (ScriptsManager.Instance.audioManager.isPlaying)
        {
            yield return null;
        }
        playButtonText.text = "play";
    }

    public IEnumerator DownloadAudio(string path)
    {
        var uwr = UnityWebRequestMultimedia.GetAudioClip(path, AudioType.WAV);
        DownloadFromServer.Instance.DownloadingProgress(uwr);
        yield return uwr.SendWebRequest();

        if (uwr.isNetworkError || uwr.isHttpError)
        {
            //     Debug.LogError(uwr.error);
            yield break;
        }

        audioClip = DownloadHandlerAudioClip.GetContent(uwr);
        //// use audio clip
        //if (inputStartTime.text == "" || inputStartTime.text == "." || inputStartTime.text == "-")
        //    inputStartTime.text = "0";
        //if (inputEndTime.text == "" || inputEndTime.text == "." || inputEndTime.text == "-")
        //    inputEndTime.text = audioClip.length.ToString();
        //else if (float.Parse(inputEndTime.text) > audioClip.length)
        //    inputEndTime.text = audioClip.length.ToString();
        ScriptsManager.Instance.audioManager.clip = audioClip;
        ScriptsManager.Instance.audioManager.Play();
    }

    public void RemoveComponent()
    {
        gameObject.SetActive(false);
        playButtonText.text = "play";
        ScriptsManager.Instance.audioManager.Stop();
        audioClip = null;
    }

    public void closeButton()
    {
        ScriptsManager.Instance.runtimeHierarchy.Deselect();
        ScriptsManager.Instance.transformComponent.SetActive(false);
        List<GameObject> labelGobject = ScriptsManager.Instance.GetComponent<raycasthitting>().labels;
        for (int i = 0; i < labelGobject.Count; i++)
        {
            if (labelGobject[i].name == label_Publish.gameObject.name)
            {

                Destroy(labelGobject[i]);
                ScriptsManager.Instance.GetComponent<raycasthitting>().remove(i);
                ScriptsManager.Instance.GetComponent<raycasthitting>().removeLableObject(i);
                break;
            }
        }
        //label_Publish.gameObject.name = "";
        ScriptsManager.Instance.GetComponent<Label_components>().labelComp.SetActive(false);
        RemoveComponent();
    }

    
}
