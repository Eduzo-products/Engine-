using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public enum ComponentType
{
    Scene, Label, AddOn
} 

public class VideoComponentScene : MonoBehaviour
{
    public TMPro.TMP_InputField inputVideoClipPath;
    [SerializeField]
    private TMP_InputField inputStartTime;
    [SerializeField]
    private TMP_InputField inputEndTime;

    public ComponentType videoType;

    public instantiate videoProps;
    public TMP_InputField labelInput;
    public Slider width;
    public Slider height;

    public TMP_Dropdown dropdown;
    public string objName;

    public instantiate instantiateScript;
    public bool isGameObjectProperty = false;
    public GameObject hierarchyReference;

    public Label_Publish labelPublish;
    public NoTransform noTransform;
    public string localURL = "";
    [HideInInspector] public GameObject linkHolder;
    public GameObject removeButton;
    public TextMeshProUGUI playButtonText;
    private Coroutine routine;

    private void Start()
    {
        instantiateScript = ScriptsManager.Instance.instantiateScript;
        ScriptsManager.OnCloseClicked += CloseProject;

        OnScaleProportionChange();
    }

    private void OnDestroy()
    {
        ScriptsManager.OnCloseClicked -= CloseProject;
    }

    private IEnumerator VideoCompleted(NoTransform notransform)
    {
        yield return new WaitUntil(() => notransform.GetComponent<VideoPlayer>().isPlaying);
        while (notransform.GetComponent<VideoPlayer>().isPlaying)
        {
            yield return null;
        }
        playButtonText.text = "play";
        notransform.isVideoPlaying = false;
    }

    private IEnumerator VideoCompletedlabel(Label_Publish labelPublish)
    {
        yield return new WaitUntil(() => labelPublish.ThisVideo.GetComponent<VideoPlayer>().isPlaying);
        while (labelPublish.ThisVideo.GetComponent<VideoPlayer>().isPlaying)
        {
            yield return null;
        }
        playButtonText.text = "play";
        this.labelPublish.isVideoPlaying = false;
    }

    public void PlayPushSample()
    {
        ScriptsManager.Instance.videoPanel.GetComponent<VideoPlayer>().Stop();

        //commented these lines in order .. to avoid the play of video at scene level on multi scene content type
        //  ScriptsManager.Instance.ModuleVideoStop();

        if (localURL.Length > 0)
        {
            if (videoType == ComponentType.Scene)
            {
                if (labelPublish != null)
                {
                    labelPublish.isVideoPlaying = false;
                }
                //  noTransform.isVideoPlaying = !noTransform.isVideoPlaying;

                if (noTransform.isVideoPlaying == false)
                {
                    noTransform.isVideoPlaying = true;
                    PlayVideo();
                    StartCoroutine(VideoCompleted(noTransform));

                    //routine = StartCoroutine(OnVideoStop(noTransform.isVideoPlaying));
                }
                else
                {
                    noTransform.isVideoPlaying = false;
                    StopCoroutine(VideoCompleted(noTransform));
                    ScriptsManager.Instance.videoPanel.GetComponent<VideoPlayer>().Stop();
                    playButtonText.text = "play";
                }
            }
            else if (videoType == ComponentType.Label)
            {
                if (noTransform != null)
                {
                    noTransform.isVideoPlaying = false;
                }

                if (labelPublish)
                {
                    if (!labelPublish.isVideoPlaying)
                    {
                        if (labelPublish.ThisVideo != null)
                        {
                            labelPublish.isVideoPlaying = true;
                            PlayVideo();
                            StartCoroutine(VideoCompletedlabel(labelPublish));
                        }
                    }
                    else
                    {
                        labelPublish.isVideoPlaying = false;
                        StopCoroutine(VideoCompletedlabel(labelPublish));
                        ScriptsManager.Instance.videoPanel.GetComponent<VideoPlayer>().Stop();
                        playButtonText.text = "play";
                    }
                }
            }
        }
    }

    private void PlayVideo()
    {
        //    ScriptsManager.Instance.videoPanel.transform.localScale = new Vector3(height.value * 1.0f, 1.0f, height.value * 0.57f);

        ScriptsManager.Instance.videoPanel.GetComponent<VideoPlayer>().url = localURL;
        Material newMat = new Material(Shader.Find("Standard"));
        ScriptsManager.Instance.videoPanel.GetComponent<Renderer>().material = newMat;
        RenderTexture newText = new RenderTexture(2000, 1540, 24);
        newText.useDynamicScale = true;
        newText.dimension = UnityEngine.Rendering.TextureDimension.Tex2D;
        ScriptsManager.Instance.videoPanel.GetComponent<VideoPlayer>().targetTexture = newText;
        ScriptsManager.Instance.videoPanel.GetComponent<MeshRenderer>().sharedMaterial.mainTexture = newText;

        OnAspectRatioChange();

        //  ScriptsManager.Instance.videoPanel1.GetComponent<VideoPlayer>().Stop();

        ScriptsManager.Instance.videoPanel.GetComponent<VideoPlayer>().Play();
        playButtonText.text = "stop";


        //  ScriptsManager.Instance.videoPanel1.GetComponent<VideoPlayer>().targetTexture = textur;
        // ScriptsManager.Instance.videoPanel1.GetComponent<MeshRenderer>().sharedMaterial.mainTexture = newText1;

    }

    //private IEnumerator OnVideoStop(bool isStopped)
    //{
    //    yield return isStopped;

    //    while (ScriptsManager.Instance.videoPanel.GetComponent<VideoPlayer>().isPlaying)
    //    {
    //        yield return null;
    //    }

    //    if (videoType == VideoType.Scene)
    //    {
    //        if (noTransform != null)
    //        {
    //            noTransform.isVideoPlaying = false;
    //        }
    //    }
    //    else if (videoType == VideoType.Label)
    //    {
    //        if (labelPublish != null)
    //        {
    //            labelPublish.isVideoPlaying = false;
    //        }
    //    }
    //}

    public void OnAspectRatioChange()
    {
        if (videoType == ComponentType.Scene)
        {
            if (noTransform != null)
            {
                noTransform.aspectRatioValue = dropdown.value;
            }
        }
        else if (videoType == ComponentType.Label)
        {
            if (labelPublish != null)
            {
                labelPublish.aspectRatioValue = dropdown.value;
            }
        }

        ScriptsManager.Instance.videoPanel.GetComponent<VideoPlayer>().aspectRatio = ScriptsManager.Instance.AspectRatio(dropdown.value);
    }

    private void CloseProject()
    {
        RemoveComponent();
        closeButton();

        if (routine != null)
        {
            StopCoroutine(routine);
        }
    }

    public void RemoveComponent()
    {
        //  if (noTransform != null)
        // {
        //      ScriptsManager.Instance.ControlFLowRefDelete(noTransform.transform);
        //  }

        height.value = 1;
        dropdown.value = 0;
        ButtonCustomizationManager.Instance.videoAdded = false;

        ScriptsManager.Instance.transformComponent.SetActive(false);
        ScriptsManager.Instance.videoPanel.GetComponent<VideoPlayer>().Stop();
        ScriptsManager.Instance.ModuleVideoDestroy(ScriptsManager.Instance.videoPanel);
        ScriptsManager.Instance.videoPanel.SetActive(false);
        inputVideoClipPath.text = string.Empty;
        RenderTexture renderTexture = new RenderTexture(2000, 1540, 24);
        ScriptsManager.Instance.videoPanel.GetComponent<MeshRenderer>().sharedMaterial.mainTexture = renderTexture;
        ScriptsManager.Instance.featureScript.video.sprite = ScriptsManager.Instance.featureScript.videoSprites[0];
        ScriptsManager.Instance.currentObjectName.text = "";

        gameObject.SetActive(false);
        playButtonText.text = "play";
        DestroyImmediate(hierarchyReference, true);
    }

    public void UrlChange()
    {
        ScriptsManager.Instance.GetComponent<Label_components>().ApplyURLchange(labelInput.text);
    }

    public void widthChange()
    {
        ScriptsManager.Instance.GetComponent<Label_components>().ApplyVwidthChange(width.value);
    }

    public void heightChange()
    {
        ScriptsManager.Instance.GetComponent<Label_components>().ApplyVheightChange(height.value);
    }

    public void OnScaleProportionChange()
    {

        //if (videoType == VideoType.Scene)
        //{
        //    if (noTransform != null)
        //    {
        //        noTransform.videoHeight = height.value;
        //    }
        //}
        //else if (videoType == VideoType.Label)
        //{
        //    if (labelPublish != null)
        //    {
        //        labelPublish.videoHeight = height.value;
        //    }
        //}

        //Vector3 scale = ScriptsManager.Instance.videoPanel1.transform.localScale;
        //scale.x = height.value * 1.0f / 2;
        //scale.z = height.value * 0.57f / 2;
        //ScriptsManager.Instance.videoPanel.transform.localScale = scale;
        //if (ScriptsManager.Instance.videoPanel.GetComponent<ObjectTransformComponent>())
        //{
        //    ScriptsManager.Instance.videoPanel.GetComponent<ObjectTransformComponent>().SetScale(scale);
        //}
    }

    public void closeButton()
    {
        List<GameObject> labelGobject = ScriptsManager.Instance.GetComponent<raycasthitting>().labels;
        for (int i = 0; i < labelGobject.Count; i++)
        {
            if (labelGobject[i].name == objName)
            {

                Destroy(labelGobject[i]);
                ScriptsManager.Instance.GetComponent<raycasthitting>().remove(i);
                ScriptsManager.Instance.GetComponent<raycasthitting>().removeLableObject(i);

                break;
            }
        }
        objName = "";
        ScriptsManager.Instance.GetComponent<Label_components>().labelComp.SetActive(false);
        gameObject.SetActive(false);
    }
}
