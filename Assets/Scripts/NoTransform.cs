using UnityEngine;

public enum TypesOfProperty
{
    None, Audio, Video, Text, TTS, Skybox, Explode, Manipulation, Labels, Light, Image
}

public class NoTransform : MonoBehaviour
{
    public TypesOfProperty propertyType;
    public string webLink = "";
    public GameObject linkHolder;

    #region AUDIO_COMPONENT
    public GameObject componentObject;
    public AudioComponent audioComponent;
    public AudioClip audioClip;
#endregion

    #region VIDEO_COMPONENT
    public VideoComponentScene videoComponent;
    public float videoHeight = 0.0f;
    public int aspectRatioValue = 0;
    public string serverURL = "";
    public bool isVideoPlaying = false;
    public int videoID;
    #endregion

    #region LIGHT_COMPONENT
    public LightComponent lightComponent;
    public new Light light = null;
    public int lightValue = 0;
    public Color colorValue;
    public float intensity = 0.0f, indirectMultiplier = 0.0f, angleValue = 0.0f, rangeValue = 0.0f;
    private string lightPath;
    #endregion

    #region IMAGE_COMPONENT
    public ImageComponent imageComponent;
    #endregion

    public string currentObjectName;
    public bool videoVisibility;

    //for control flow (saravanan)
    public SceneObjectLineItem thisSceneObjLineItem;

    private void Awake()
    {
        InitializeSubscriptionToDelegate();
    }

    public void InitializeSubscriptionToDelegate()
    {
        PublishContent.OnSaveContentClick += OnSaveNoTransform;
        ScriptsManager.OnCloseClicked += RemoveFromMemory;

        currentObjectName = gameObject.name;
    }

    private void OnDestroy()
    {
        PublishContent.OnSaveContentClick -= OnSaveNoTransform;
        ScriptsManager.OnCloseClicked -= RemoveFromMemory;

    }

    private void RemoveFromMemory()
    {
        isVideoPlaying = false;

        if (propertyType == TypesOfProperty.Light)
        {
            Destroy(gameObject);
            lightComponent.addLights.lightsCount = 0;
        }
    }

    public void ApplyAudioValues()
    {
        audioComponent.isPlaying = false;
        audioComponent.inputAudioClipPath.text = webLink;
        audioComponent.audioClip = audioClip;
    }

    public void ApplyVideoValues()
    {
        videoComponent.inputVideoClipPath.text = webLink;
        videoComponent.OnAspectRatioChange();
        videoComponent.OnScaleProportionChange();
    }

    public void ApplyImageValues()
    {

    }

    public AddAudio ThisObjAudioProperties()
    {
        AddAudio tempAudioStuff = new AddAudio
        {
            audioURL = webLink,
            attachedObject = currentObjectName
        };

        return tempAudioStuff;
    }

    public AddVideo ThisObjVideoProperties()
    {
       AddVideo tempVideoStuff = new AddVideo
        {
            videoURL = serverURL,
            attachedObject = currentObjectName,
            aspectRatio = aspectRatioValue,
            height = videoHeight,
            videoType = ComponentType.Scene.ToString(),
            position = this.transform.position,
            rotation = this.transform.eulerAngles,
            scale = this.transform.localScale,

            totalcount = ScriptsManager.Instance.enableDisableComponent.videoCount,
            videoID = videoID,
            visible = this.gameObject.activeSelf

        };

        return tempVideoStuff;
    }

    private void OnSaveNoTransform()
    {
        switch (propertyType)
        {
            case TypesOfProperty.Audio:
                ScriptsManager._toolContent.addAudio.Add(
                    ThisObjAudioProperties()
                    );
                break;
            case TypesOfProperty.Video:
                ScriptsManager._toolContent.addVideo.Add(
                  ThisObjVideoProperties()
                    );
                break;
            case TypesOfProperty.Light:
                ScriptsManager._toolContent.lightComponents.Add(new LightComponents
                {
                    lightDropDownValue = lightValue,
                    lightColor = colorValue,
                    intensity = intensity,
                    indirectMultiplier = indirectMultiplier,
                    angle = angleValue,
                    range = rangeValue,
                    sliderValue = angleValue,
                    path = lightPath,
                    isActive = gameObject.activeSelf
                });
                break;
            default:
                break;
        }
    }

    public void GetLightPath(GameObject lightObject)
    {
        lightPath = ScriptsManager.Instance.GetObjectFullPath(lightObject.gameObject);
    }
}