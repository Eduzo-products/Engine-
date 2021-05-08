using Crosstales.RTVoice.Tool;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LabelWithTextToSpeechProperty : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField labelTextField;
    [SerializeField]
    public TMP_InputField speakTextField;
    [SerializeField]
    public SpeechText speechManager;
    public Label_Publish label_Publish;
    public TextMeshProUGUI playTTSButtonText;

    // Start is called before the first frame update
    void Start()
    {
        if (speechManager == null)
            speechManager = GetComponent<SpeechText>();
        //PublishContent.OnSaveContentClick += onSaveContent;
        ScriptsManager.OnCloseClicked += CloseProject;
    }

    private void CloseProject()
    {
        RemoveComponent();
    }


    private void OnDestroy()
    {
        // PublishContent.OnSaveContentClick -= onSaveContent;
        ScriptsManager.OnCloseClicked -= CloseProject;
    }

    public void OnLabelTextValueChanged()
    {
        if (labelTextField.text.Length > 0)
        {
            label_Publish.textObj.text = label_Publish.labelWithTTS.labelText = labelTextField.text;
        }
        else
        {
            label_Publish.textObj.text = "Enter label name";
            label_Publish.labelWithTTS.labelText = "";
        }
    }

    public void OnSpeechTextValueChanged()
    {
        label_Publish.labelWithTTS.textToSpeechDetails.text = speakTextField.text;
    }

    public void SetPropertyValue()
    {
        labelTextField.text = label_Publish.labelWithTTS.labelText;
        speakTextField.text = label_Publish.labelWithTTS.textToSpeechDetails.text;
    }

    public void OnSamplePlayClick()
    {
        speechManager.CurrentText = speakTextField.text;

        if (speechManager.CurrentText.Length > 0)
        {
            if (!speechManager.Source.isPlaying)
            {
                speechManager.Speak();
                playTTSButtonText.text = "stop";
                StartCoroutine(StopTTS());
            }
            else
            {
                speechManager.Silence();
                playTTSButtonText.text = "play";
                StopCoroutine(StopTTS());
            }
        }
    }

    public void RemoveComponent()
    {
        gameObject.SetActive(false);
        speechManager.Silence();
        playTTSButtonText.text = "play";
        StopCoroutine(StopTTS());
    }

    private IEnumerator StopTTS()
    {
        yield return new WaitUntil(() => speechManager.Source.isPlaying);

        while (speechManager.Source.isPlaying)
        {
            yield return null;
        }
        playTTSButtonText.text = "play";
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
