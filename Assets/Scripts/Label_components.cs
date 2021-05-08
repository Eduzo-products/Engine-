using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Video;

public class Label_components : MonoBehaviour
{
    // Start is called before the first frame update
    //public GameObject audComp;
    public GameObject vidComp;
    public GameObject labelComp;
    //public GameObject desComp;
    //public GameObject tex2SpeechComp;
    public Text display_Text;
    public TMP_Text inputfield;
    public TMP_Text Vinputfield;
    public Label_Publish labelPublish;
    public GameObject labelWithAudioProperty;
    public GameObject labelWithTextToSpeechProperty;
    public GameObject labelWithTextContentProperty;
    public TMP_InputField inputField_textchange;

    public void ClearComp()
    {
        vidComp.SetActive(false);
        labelComp.SetActive(false);
        if (ScriptsManager.Instance.videoPanel)
        {
            ScriptsManager.Instance.videoPanel.GetComponent<VideoPlayer>().Stop();
        }

        labelWithAudioProperty.SetActive(false);
        labelWithAudioProperty.GetComponent<LabelWithAudioProperty>().isPlaying = true;
        labelWithAudioProperty.GetComponent<LabelWithAudioProperty>().PlayPushSample();

        labelWithTextToSpeechProperty.SetActive(false);
        labelWithTextContentProperty.SetActive(false);

        ScriptsManager.Instance.transistionObject.SetActive(false);
        ScriptsManager.Instance.videoPanel = ScriptsManager.Instance.videoPanel1;

    }


    //user press on the label
    public void SelectComp(int index, Text displayText, Label_Publish labelObj)
    {
        ScriptsManager.Instance.currentObjectName.gameObject.SetActive(true);
        ScriptsManager.Instance.currentObjectName.text = labelObj.gameObject.name;
        labelComp.SetActive(true);
        labelComp.GetComponent<LabelPropertyHandler>().objName = labelObj.gameObject.name;
        labelComp.GetComponent<LabelPropertyHandler>().displayLebelText = displayText;
        labelPublish = labelObj;
        display_Text = displayText;

        if (labelPublish.descriptionPanelObj != null)
        {
            labelPublish.descriptionPanelObj.SetActive(false);
        }

        if (!displayText.text.Equals("Enter label name"))//|| !displayText.text.Equals(""))
        {
            display_Text.text = displayText.text;
            inputField_textchange.text = display_Text.text;
        }
        else if (displayText.text.Equals("Enter label name") || displayText.text.Equals(""))
        {
            inputField_textchange.text = "";
            display_Text.text = "Enter label name";
        }
        ScriptsManager.Instance.transistionObject.SetActive(false);

        switch (index)
        {
            case 0:
                labelComp.SetActive(true);
                display_Text = displayText;
                //display_Text.text = displayText.text;

                LabelPropertyHandler temp = labelComp.GetComponent<LabelPropertyHandler>();
                temp.objName = labelObj.gameObject.name;
                temp.displayLebelText = displayText;
                ScriptsManager.Instance.labelPropertyText.text = "Label";
                //temp.labelCompDispText.text = displayText.text;
                break;
            case 2:
                vidComp.SetActive(true);
                if (ScriptsManager.Instance.ModelVideo.Count > 0)
                {
                    ScriptsManager.Instance.AllVideoOff(labelObj.ThisVideo);
                }
                ScriptsManager.Instance.videoPanel = labelObj.ThisVideo;

                //ScriptsManager.Instance.videoPanel.transform.SetParent(ScriptsManager.Instance.objectCollection.transform);
                ScriptsManager.Instance.skyAudVid.linkHolder = labelObj.gameObject;
                labelObj.videoComponent = ScriptsManager.Instance.videoPropertyObj.GetComponent<VideoComponentScene>();
                ScriptsManager.Instance.videoPanel.GetComponent<VideoPlayer>().targetTexture = null;
                ScriptsManager.Instance.videoPanel.GetComponent<MeshRenderer>().sharedMaterial.mainTexture = null;

                if (labelObj.videoComponent.noTransform != null)
                {
                    labelObj.videoComponent.noTransform.isVideoPlaying = false;
                }

                if (labelObj.videoComponent.labelPublish != null)
                {
                    labelObj.videoComponent.labelPublish.isVideoPlaying = false;
                }

                labelObj.videoComponent.noTransform = null;
                labelObj.videoComponent.labelPublish = null;
                labelObj.videoComponent.labelPublish = labelObj;
                labelObj.videoComponent.removeButton.SetActive(false);
                labelObj.videoComponent.hierarchyReference = labelObj.gameObject;
                labelObj.videoComponent.videoType = ComponentType.Label;
                labelObj.ThisVideo.SetActive(!labelObj.ThisVideo.activeSelf);
                labelObj.videoComponent.height.value = labelObj.videoHeight;
                labelObj.videoComponent.dropdown.value = labelObj.aspectRatioValue;
                labelObj.videoComponent.inputVideoClipPath.text = labelObj.serverURL;
                labelObj.videoComponent.localURL = labelObj.serverURL;

                ScriptsManager.Instance.labelPropertyText.text = "Label with video";
                break;
            case 5:
                ScriptsManager.Instance.transistionObject.SetActive(true);
                ScriptsManager.Instance.labelPropertyText.text = "Label with transition";
                break;
            default:
                Debug.Log("No Such Cases.");
                break;
        }

    }
    public void ApplyTextChange(string str)
    {
        if (display_Text != null)
        {
            inputField_textchange.text = str;
            display_Text.text = str;
            inputfield.text = str;
        }
    }
    public void ApplyURLchange(string str)
    {
        labelPublish.serverURL = str;
        print("changes applied");
    }
    public void ApplyVwidthChange(float value)
    {
        //labelPublish.Vwidth = value;
    }
    public void ApplyVheightChange(float value)
    {
        //labelPublish.Vheight = value;
    }
}
