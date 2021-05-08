using Crosstales.RTVoice.Tool;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Crosstales.RTVoice;

public class TextToSpeechComponent : MonoBehaviour
{
    [SerializeField]
    public TMP_InputField speakTextField;
    [SerializeField]
    public SpeechText speechManager;
    public TextMeshProUGUI playTTSButtonText;

    private void CloseProject()
    {
        RemoveComponent();
    }

    private void onSaveContent()
    {
        //if (speakTextField.text != "")
        //{
        ScriptsManager._toolContent.textToSpeech.Add
            (new XRTextToSpeech
            {
                text = speakTextField.text,
                playOnStart = true
            });
        //}
    }

    public void SubscribeDelegates()
    {
        PublishContent.OnSaveContentClick += onSaveContent;
        ScriptsManager.OnCloseClicked += CloseProject;
    }

    public void UnsubscribeDelegates()
    {
        PublishContent.OnSaveContentClick -= onSaveContent;
        ScriptsManager.OnCloseClicked -= CloseProject;
    }

    private void OnDestroy()
    {
        UnsubscribeDelegates();
    }

    public void OnSamplePlayClick()
    {
        speechManager.Silence();
        if (speakTextField.text != "")
        {
            if (!Speaker.isSpeaking)
            {
                speechManager.CurrentText = speakTextField.text;
                speechManager.Speak();
            }
        }
    }

    public void RemoveComponent()
    {
        speechManager.Silence();
        ScriptsManager.Instance.featureScript.textToSpeech.sprite = ScriptsManager.Instance.featureScript.textToSpeechSprites[0];

        speechManager.CurrentText = speakTextField.text = "";
        Transform temp = ScriptsManager.Instance.scenePropertyRect.Find("TextToSpeech");

        if (temp != null)
        {
            Destroy(temp.gameObject);
        }

        gameObject.SetActive(false);
        ScriptsManager.Instance.currentObjectName.text = "";

        UnsubscribeDelegates();
    }
}
