using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

[Serializable]
public struct IconReferences
{
    public string name;
    public Sprite sprite;
}

public class Label_Publish : MonoBehaviour
{
    public Transform parentTransform;
    public bool isTransitionOn = false;
    public GameObject dispalyLabelObj;
    public GameObject buttonListObj;
    public GameObject descriptionPanelObj;

    List<string> label2Obj;
    public GameObject videoPrefab;

    public GameObject videoPlayerPrefab;
    public bool isVideoPlaying = false;
    public Text textObj;
    public int componentCombo = 0;
    public float videoHeight = 0.0f;
    public int aspectRatioValue = 0;
    public string serverURL = "";

    public Vector3 startPoint, endPoint;
    public string labelToObject = "";

    public PropertyType selectedPropertyType;
    public LabelWithAudio labelWithAudio;
    public LabelWithTextToSpeech labelWithTTS;
    public LabelWithTextContent labelWithTextContent;
    //public TransistionEffect transitionEffect;
    public VideoComponentScene videoComponent;

    public Image labelIcon;
    public List<IconReferences> contentListSprites = new List<IconReferences>();

    public bool isBatchingEnable;
    public string currentObjectName = "";

    public GameObject ThisVideo;
    public AudioClip audioClip;
    public bool isRootObjectDeleted = false;
    public bool isAudioPlaying = false;

    private void Awake()
    {
       // PublishContent.OnSaveContentClick += OnLabelSave;
        ScriptsManager.OnCloseClicked += CloseProject;
    }

    private void CloseProject()
    {
        //OnButtonListClose();

        if (selectedPropertyType != PropertyType.none)
        {
            Destroy(gameObject);
        }
        else
        {
            if (buttonListObj != null)
            {
                Destroy(gameObject);
            }
        }
        isVideoPlaying = false;
    }

    public void OnButtonListClose()
    {
        if (gameObject.activeSelf)
        {
           // StartCoroutine(StopTransition());
        }
    }

   

    /*private void OnLabelSave()
    {
        bool flag = false;

        if (componentCombo == 2)
        {
            flag = true;
        }

        List<string> parentNameList = new List<string>();
      //  List<GameObject> local = GetComponent<ObjectNavigator>().parents;
        List<string> childNameList = new List<string>();
       // List<GameObject> temp = GetComponent<ObjectNavigator>().childs;

        for (int i = 0; i < local.Count; i++)
        {
            if (local[i] != null)
                parentNameList.Add(local[i].name);
        }

        for (int i = 0; i < temp.Count; i++)
        {
            if (temp[i] != null)
                childNameList.Add(temp[i].name);
        }

        if (selectedPropertyType == PropertyType.none)
        {
            if (videoComponent != null)
            {
                Debug.Log(isBatchingEnable);

                ScriptsManager._XRStudioContent.addLabel.Add(
                new AddLabel
                {
                    label2Objects = labelToObject,
                    videoEnabled = flag,
                    audioEnabled = false,
                    text2SpeechEnabled = false,
                    descriptionEnabled = false,

                    isBatchingEnabled = isBatchingEnable,
                    labelName = textObj.text,
                    videoDetails = new AddVideoLabel
                    {
                        videoURL = serverURL,
                        attachedObject = currentObjectName,
                        height = videoHeight,
                        aspectRatio = aspectRatioValue,
                        videoType = ComponentType.Label.ToString(),
                        position = ThisVideo.transform.position,
                        rotation = ThisVideo.transform.eulerAngles,
                        scale = ThisVideo.transform.localScale

                    },
                    name = gameObject.name,
                    parentNames = parentNameList,
                    childNames = childNameList
                }
                );
            }
            else
            {

                Debug.Log(isBatchingEnable);

                ScriptsManager._XRStudioContent.addLabel.Add(
                new AddLabel
                {
                    label2Objects = labelToObject,
                    videoEnabled = flag,
                    audioEnabled = false,
                    isBatchingEnabled = isBatchingEnable,
                    text2SpeechEnabled = false,
                    descriptionEnabled = false,
                    labelName = textObj.text,
                    videoDetails = new AddVideoLabel
                    {
                        videoURL = serverURL,

                        //  height = scalePropotion.value
                    },
                    name = gameObject.name,
                    parentNames = parentNameList,
                    childNames = childNameList
                }
                );
            }
            //List<Vector3> lineStartPoint = new List<Vector3>();
            //List<Vector3> lineEndPoint = new List<Vector3>();
            //List<string> label2Objects = new List<string>();

            //lineStartPoint.Add(startPoint);
            //lineEndPoint.Add(endPoint);
            //label2Objects.Add(labelToObject);

            //ScriptsManager._XRStudioContent.addArrowDetails = new LabelArrowDetails();

            //ScriptsManager._XRStudioContent.addArrowDetails.lineEnd = lineEndPoint;
            //ScriptsManager._XRStudioContent.addArrowDetails.lineStart = lineStartPoint;
            //ScriptsManager._XRStudioContent.addArrowDetails.label2Objects = label2Objects;
        }
        else if (selectedPropertyType == PropertyType.LabelWithAudio)
        {
            labelWithAudio.attachedObjectName = gameObject.name;
            labelWithAudio.attachedObjectFullPath = ScriptsManager.Instance.GetObjectFullPath(gameObject);
            labelWithAudio.lineStartPoint = gameObject.GetComponent<LineRenderer>().GetPosition(0);
            labelWithAudio.lineEndPoint = gameObject.GetComponent<LineRenderer>().GetPosition(1);

            ScriptsManager._XRStudioContent.labelWithAudios.Add(labelWithAudio);
        }
        else if (selectedPropertyType == PropertyType.LabelWithTextToSpeech)
        {
            labelWithTTS.attachedObjectName = gameObject.name;
            labelWithTTS.attachedObjectFullPath = ScriptsManager.Instance.GetObjectFullPath(gameObject);
            labelWithTTS.lineStartPoint = gameObject.GetComponent<LineRenderer>().GetPosition(0);
            labelWithTTS.lineEndPoint = gameObject.GetComponent<LineRenderer>().GetPosition(1);

            ScriptsManager._XRStudioContent.labelWithTextToSpeeches.Add(labelWithTTS);
        }
        else if (selectedPropertyType == PropertyType.LabelWithDescription)
        {
            labelWithTextContent.attachedObjectName = gameObject.name;
            labelWithTextContent.attachedObjectFullPath = ScriptsManager.Instance.GetObjectFullPath(gameObject);
            labelWithTextContent.lineStartPoint = gameObject.GetComponent<LineRenderer>().GetPosition(0);
            labelWithTextContent.lineEndPoint = gameObject.GetComponent<LineRenderer>().GetPosition(1);
            labelWithTextContent.contentDetails.panelName = descriptionPanelObj.name;
            if (descriptionPanelObj.GetComponent<ObjectTransformComponent>() != null)
            {
                labelWithTextContent.contentDetails.transform = descriptionPanelObj.GetComponent<ObjectTransformComponent>().currentTransform;
            }
            else
            {
                // labelWithTextContent.contentDetails.transform.position = new ObjectPosition();
                labelWithTextContent.contentDetails.transform.position.x = descriptionPanelObj.transform.position.x;
                labelWithTextContent.contentDetails.transform.position.y = descriptionPanelObj.transform.position.y;
                labelWithTextContent.contentDetails.transform.position.z = descriptionPanelObj.transform.position.z;
                labelWithTextContent.contentDetails.transform.rotation.x = descriptionPanelObj.transform.rotation.x;
                labelWithTextContent.contentDetails.transform.rotation.y = descriptionPanelObj.transform.rotation.y;
                labelWithTextContent.contentDetails.transform.rotation.z = descriptionPanelObj.transform.rotation.z;
                labelWithTextContent.contentDetails.transform.scale.x = descriptionPanelObj.transform.localScale.x;
                labelWithTextContent.contentDetails.transform.scale.y = descriptionPanelObj.transform.localScale.y;
                labelWithTextContent.contentDetails.transform.scale.z = descriptionPanelObj.transform.localScale.z;
                // ( descriptionPanelObj.transform.position.x,descriptionPanelObj.transform.position.y, descriptionPanelObj.transform.position.z);
            }
            ScriptsManager._XRStudioContent.labelWithTextContents.Add(labelWithTextContent);
        }
        else if (selectedPropertyType == PropertyType.Transition)
        {
            //transitionEffect.lineStartPoint = gameObject.GetComponent<LineRenderer>().GetPosition(0);
           // transitionEffect.lineEndPoint = gameObject.GetComponent<LineRenderer>().GetPosition(1);
        }
    }*/


    public void CreateVideoPlane(Vector3 pos, bool active = false, bool isReloading = false)
    {
        GameObject obj = Instantiate(ScriptsManager.Instance.videoPanel1);
        obj.transform.parent = parentTransform.GetComponentInParent<ThreeDModelFunctions>().transform;

        obj.gameObject.SetActive(false);

        ScriptsManager.Instance.ModelVideo.Add(obj);

        if (isReloading)
        {
            obj.transform.position = pos;
        }

        obj.name = "label_video " + obj.transform.GetSiblingIndex() + "(" + parentTransform.name + ")";
        obj.AddComponent<NoTransform>();
        obj.AddComponent<SkipThisObject>();
        NoTransform noTransform = obj.GetComponent<NoTransform>();
        ThisVideo = obj;
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
        OnButtonListClose();

       // PublishContent.OnSaveContentClick -= OnLabelSave;
        ScriptsManager.OnCloseClicked -= CloseProject;
        if (ThisVideo)
        {
            Destroy(ThisVideo);
        }

        if (descriptionPanelObj != null)
            Destroy(descriptionPanelObj);
    }

    public void AddImage()
    {
        selectedPropertyType = PropertyType.Image;
        ScriptsManager.Instance.enableDisableComponent.imageCount++;
        GameObject imagePanel = Instantiate(ScriptsManager.Instance.enableDisableComponent.imagePrefab, ScriptsManager.Instance.scenePropertyRect) as GameObject;
        imagePanel.name = $"Image ({ScriptsManager.Instance.enableDisableComponent.imageCount})";

        ScriptsManager.Instance.transformGizmo.ManipulationControl();
        ScriptsManager.Instance.enableDisableComponent.imageComponent.SetActive(true);

        ImageComponent component = ScriptsManager.Instance.enableDisableComponent.imageComponent.GetComponent<ImageComponent>();
        imagePanel.GetComponent<NoTransform>().imageComponent = component;
        component.icon = imagePanel.GetComponent<SpriteRenderer>();
        component.imageURLField.text = "";
        component.imageType = ComponentType.Label;

        ScriptsManager.Instance.runtimeHierarchy.Refresh();
        ScriptsManager.Instance.runtimeHierarchy.Select(transform);

        labelIcon.sprite = contentListSprites[6].sprite;
        dispalyLabelObj.SetActive(true);
        Destroy(buttonListObj);
        ScriptsManager.Instance.GetComponent<Label_components>().ClearComp();

        //componentCombo = 5;
        //ScriptsManager.Instance.GetComponent<Label_components>().SelectComp(componentCombo, textObj, this);
    }

    
    public void EnableLabel()
    {
        ScriptsManager.Instance.textPropertyObj.SetActive(false);

        ScriptsManager.Instance.runtimeHierarchy.Select(transform);
        isBatchingEnable = true;
        labelIcon.sprite = contentListSprites[0].sprite;
        dispalyLabelObj.SetActive(true);
        Destroy(buttonListObj);
        ScriptsManager.Instance.GetComponent<Label_components>().ClearComp();
        componentCombo = 0;
        ScriptsManager.Instance.GetComponent<Label_components>().SelectComp(componentCombo, textObj, this);
    }
    public void EnableVideo()
    {
        ScriptsManager.Instance.textPropertyObj.SetActive(false);

        ScriptsManager.Instance.runtimeHierarchy.Select(transform);
        if (ThisVideo == null)
        {
            CreateVideoPlane(ScriptsManager.Instance.videoPanel1.transform.localPosition, true, false);
        }
        labelIcon.sprite = contentListSprites[1].sprite;
        dispalyLabelObj.SetActive(true);
        Destroy(buttonListObj);
        ScriptsManager.Instance.GetComponent<Label_components>().ClearComp();
        componentCombo = 2;
        ScriptsManager.Instance.GetComponent<Label_components>().SelectComp(componentCombo, textObj, this);
        videoComponent.OnScaleProportionChange();
    }
    public void EnableVideoReload()
    {
        ScriptsManager.Instance.GetComponent<Label_components>().ClearComp();
        componentCombo = 2;
    }
    public void EnableAudio()
    {
        ScriptsManager.Instance.textPropertyObj.SetActive(false);

        ScriptsManager.Instance.runtimeHierarchy.Select(transform);
        labelIcon.sprite = contentListSprites[2].sprite;
        dispalyLabelObj.SetActive(true);
        Destroy(buttonListObj);
        ScriptsManager.Instance.GetComponent<Label_components>().ClearComp();
        //GameObject lbl = Instantiate(labelPropertyPrefab, ScriptsManager.Instance.propertyDisplayContent);
        //GameObject aud = Instantiate(audioPrefab, ScriptsManager.Instance.propertyDisplayContent);
        //lbl.GetComponent<LabelPropertyHandler>().displayLebelText = dispalyLabelObj.GetComponentInChildren<Text>();
        //ScriptsManager.Instance.GetComponent<Label_components>().SelectComp(textObj);
        componentCombo = 1;

        selectedPropertyType = PropertyType.LabelWithAudio;
        if (labelWithAudio == null)
        {
            labelWithAudio = new LabelWithAudio();
            labelWithAudio.audioDetails = new AddAudio();
        }
        textObj.GetComponentInParent<Button>().onClick.RemoveListener(reset);
        textObj.GetComponentInParent<Button>().onClick.AddListener(ShowProperty);
        ShowProperty();
        textObj.text = "Enter label name";
    }

    public void ShowProperty()
    {
        Debug.Log("Show property called!!!!!");
        if (!ScriptsManager.Instance.isBatchingEnabled)
        {
            ScriptsManager.Instance.GetComponent<Label_components>().labelWithAudioProperty.SetActive(false);
            ScriptsManager.Instance.GetComponent<Label_components>().labelWithTextToSpeechProperty.SetActive(false);
            ScriptsManager.Instance.GetComponent<Label_components>().labelWithTextContentProperty.SetActive(false);
            ScriptsManager.Instance.currentObjectName.gameObject.SetActive(true);

            if (selectedPropertyType == PropertyType.LabelWithAudio)
            {
                ScriptsManager.Instance.GetComponent<Label_components>().labelWithAudioProperty.SetActive(true);
                LabelWithAudioProperty labelWithAudioProperty = ScriptsManager.Instance.GetComponent<Label_components>().labelWithAudioProperty.GetComponent<LabelWithAudioProperty>();
                labelWithAudioProperty.label_Publish = this;
                labelWithAudioProperty.audioClip = audioClip;
                labelWithAudioProperty.SetPropertyValue();
            }
            else if (selectedPropertyType == PropertyType.LabelWithTextToSpeech)
            {
                ScriptsManager.Instance.GetComponent<Label_components>().labelWithTextToSpeechProperty.SetActive(true);
                LabelWithTextToSpeechProperty tts = ScriptsManager.Instance.GetComponent<Label_components>().labelWithTextToSpeechProperty.GetComponent<LabelWithTextToSpeechProperty>();
                tts.label_Publish = this;
                tts.SetPropertyValue();
            }
            else if (selectedPropertyType == PropertyType.LabelWithDescription)
            {
                SetTextPanelPropertyValues();
                DisableTextPanels();
            }
            
        }
    }

    private void DisableTextPanels()
    {
        raycasthitting raycasthitting = ScriptsManager.Instance.GetComponent<raycasthitting>();
        if (raycasthitting.labels.Count > 0)
        {
            for (int p = 0; p < raycasthitting.labels.Count; p++)
            {
                Label_Publish label_Publish = raycasthitting.labels[p].GetComponent<Label_Publish>();

                if (!label_Publish.Equals(this) && label_Publish.selectedPropertyType.Equals(PropertyType.LabelWithDescription))
                {
                    label_Publish.descriptionPanelObj.SetActive(false);
                }
            }
        }
    }

    private void SetTextPanelPropertyValues()
    {
        ScriptsManager.Instance.GetComponent<Label_components>().labelWithTextContentProperty.SetActive(true);
        LabelWithTextContentProperty txt = ScriptsManager.Instance.GetComponent<Label_components>().labelWithTextContentProperty.GetComponent<LabelWithTextContentProperty>();
        txt.label_Publish = this;
        txt.SetPropertyValue();
    }

    public void EnableDescription()
    {
        ScriptsManager.Instance.textPropertyObj.SetActive(false);

        ScriptsManager.Instance.runtimeHierarchy.Select(transform);
        labelIcon.sprite = contentListSprites[4].sprite;
        dispalyLabelObj.SetActive(true);
        Destroy(buttonListObj);
        ScriptsManager.Instance.GetComponent<Label_components>().ClearComp();

        GameObject panel = Instantiate(ScriptsManager.Instance.descriptionPanelPrefab);
        panel.AddComponent<SkipThisObject>();
        descriptionPanelObj = panel;
        panel.name = "Description panel label";
        if (parentTransform != null)
        {
            panel.name += " (" + parentTransform.name + ")";
        }
        // (" + ScriptsManager.Instance.panelCount.ToString() + ")";
        //  ScriptsManager.Instance.panelCount++;

        // panel.transform.parent = ScriptsManager.Instance.objectCollection.transform;

        panel.transform.SetParent(parentTransform.GetComponentInParent<ThreeDModelFunctions>().transform);

        componentCombo = 3;
        selectedPropertyType = PropertyType.LabelWithDescription;
        if (labelWithTextContent == null)
        {
            labelWithTextContent = new LabelWithTextContent();
            labelWithTextContent.contentDetails = new TextContent();
        }
        labelWithTextContent.contentDetails.fontSize = 36;
        labelWithTextContent.contentDetails.titleFontSize = 44;
        labelWithTextContent.contentDetails.headingColor = labelWithTextContent.contentDetails.contentColor = Color.white;
        labelWithTextContent.contentDetails.bgColor = Color.black;
        //labelWithTextContent.contentDetails.showScrollbar = true;
        labelWithTextContent.contentDetails.contentWidth = 800;
        //labelWithTextContent.contentDetails.contentHeight = 500;
        //labelWithTextContent.contentDetails.viewWidth = 783;
        //labelWithTextContent.contentDetails.viewHeight = 500;

        textObj.GetComponentInParent<Button>().onClick.RemoveListener(reset);
        textObj.GetComponentInParent<Button>().onClick.AddListener(ShowProperty);
        ShowProperty();
        textObj.text = "Enter label name";
        panel.SetActive(true);
    }

    public void DescriptionPanelReload(ObjectTransform transformData)
    {
        componentCombo = 3;
        GameObject panel = Instantiate(ScriptsManager.Instance.descriptionPanelPrefab);
        panel.GetComponent<SkipThisObject>();
        descriptionPanelObj = panel;
        panel.name = "Description panel label";
        if (parentTransform != null)
        {
            panel.name += " (" + parentTransform.name + ")";
        }

        // (" + ScriptsManager.Instance.panelCount.ToString() + ")";
        // ScriptsManager.Instance.panelCount++;
        panel.transform.parent = parentTransform.GetComponentInParent<ThreeDModelFunctions>().transform;
        // panel.AddComponent<ObjecstTransformComponent>();
        panel.transform.position = transformData.position.getVec();
        panel.transform.eulerAngles = transformData.rotation.getVec();

        descriptionPanelObj.SetActive(false);
    }

    public void EnableText2Speech()
    {
        ScriptsManager.Instance.textPropertyObj.SetActive(false);

        ScriptsManager.Instance.runtimeHierarchy.Select(transform);
        labelIcon.sprite = contentListSprites[3].sprite;
        dispalyLabelObj.SetActive(true);
        Destroy(buttonListObj);
        ScriptsManager.Instance.GetComponent<Label_components>().ClearComp();
        //GameObject lbl = Instantiate(labelPropertyPrefab, ScriptsManager.Instance.propertyDisplayContent);
        //GameObject tex2Speech = Instantiate(text2SpeechPrefab, ScriptsManager.Instance.propertyDisplayContent);
        //lbl.GetComponent<LabelPropertyHandler>().displayLebelText = dispalyLabelObj.GetComponentInChildren<Text>();
        // ScriptsManager.Instance.GetComponent<Label_components>().SelectComp(4, textObj, this);
        componentCombo = 4;
        selectedPropertyType = PropertyType.LabelWithTextToSpeech;
        if (labelWithTTS == null)
        {
            labelWithTTS = new LabelWithTextToSpeech();
            labelWithTTS.textToSpeechDetails = new XRTextToSpeech();
        }
        textObj.GetComponentInParent<Button>().onClick.RemoveListener(reset);
        textObj.GetComponentInParent<Button>().onClick.AddListener(ShowProperty);
        ShowProperty();
        textObj.text = "Enter label name";
    }

    void PlayStopAudioOnLabel()
    {
        isAudioPlaying = !isAudioPlaying;
        //  ScriptsManager.Instance.VideoRaycastRef.allVideopause();

        switch (isAudioPlaying)
        {
            case true:
                ScriptsManager.Instance.audioManager.clip = audioClip;
                ScriptsManager.Instance.audioManager.Play();
                break;
            case false:
                ScriptsManager.Instance.audioManager.Stop();
                break;
        }

    }

    public void reset()
    {
        Debug.Log("Reset called!!!!!");
        //for label functionality inside multi scenes and when the preview is active
        if (XR_studio_ControlFlow.flowDefault.currentStateAlive)
        {
            foreach (var item in ScriptsManager.Instance.GetComponent<raycasthitting>().labels)
            {
                if (item.name != gameObject.name)
                {
                    Label_Publish label_Publish = item.GetComponent<Label_Publish>();
                    label_Publish.isAudioPlaying = false;


                }
            }
            ScriptsManager.Instance.audioManager.Stop();
            ScriptsManager.Instance.videoPanel.SetActive(false);
            switch (selectedPropertyType)
            {
                case PropertyType.none:

                    if (videoComponent != null)
                    {

                        ScriptsManager.Instance.GetComponent<Label_components>().SelectComp(2, textObj, this);
                        videoComponent.PlayPushSample();
                        //in order to merge label video play time with scene time in multi scene content type
                        XR_studio_ControlFlow.flowDefault.Instance.videoPlayersInCurrentSeq.Add(ScriptsManager.Instance.videoPanel.GetComponent<VideoPlayer>());
                    }
                    break;
                case PropertyType.LabelWithAudio:
                    PlayStopAudioOnLabel();
                    break;
                case PropertyType.LabelWithDescription:
                    //  ScriptsManager.Instance.audioManager.Stop();

                    // SetTextPanelPropertyValues();
                    break;
                case PropertyType.LabelWithTextToSpeech:
                    //  ScriptsManager.Instance.audioManager.Stop();
                    LabelWithTextToSpeechProperty tts = ScriptsManager.Instance.GetComponent<Label_components>().labelWithTextToSpeechProperty.GetComponent<LabelWithTextToSpeechProperty>();
                    tts.speechManager.CurrentText = tts.speakTextField.text;
                    tts.speechManager.Speak();

                    break;
                
                default:
                    break;
            }
        }
        else
        {
            //   ScriptsManager.Instance.textPropertyObj.SetActive(false);
            ScriptsManager.Instance.DeactivateAllComponent();

            if (videoComponent != null)
            {
                videoComponent.labelPublish = null;
            }

            ScriptsManager.Instance.GetComponent<Label_components>().ClearComp();

            if (ScriptsManager.Instance.isBatchingEnabled)
            {
                ScriptsManager.Instance.currentObjectName.text = "Batching";
                ScriptsManager.Instance.transformComponent.SetActive(false);

               

            }
            else
            {
                ScriptsManager.Instance.runtimeHierarchy.Select(transform);
                ScriptsManager.Instance.currentObjectName.text = gameObject.name;
                ScriptsManager.Instance.transformComponent.SetActive(true);

                switch (componentCombo)
                {
                    case 0:
                        ScriptsManager.Instance.GetComponent<Label_components>().SelectComp(0, textObj, this);
                        break;
                    case 2:
                        ScriptsManager.Instance.GetComponent<Label_components>().SelectComp(2, textObj, this);
                        break;
                    case 5:
                        ScriptsManager.Instance.GetComponent<Label_components>().SelectComp(5, textObj, this);
                        break;
                    default:
                        Debug.Log("No Such Cases.");
                        break;
                }
            }
        }
    }
    public void applyIcon(int index)
    {
        labelIcon.sprite = contentListSprites[index].sprite;
    }
}