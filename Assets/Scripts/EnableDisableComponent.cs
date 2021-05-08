using CommandUndoRedo;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

/// <summary>
/// Added on: 22/07/2018
/// Added by: Periyasamy
/// Purpose: To enable disable feaures
/// </summary>

public class EnableDisableComponent : MonoBehaviour
{
    [Header("Transforms")]
    public Transform scenePropertiesContent;

    [Header("Feature Buttons")]
    [SerializeField]
    private GameObject btnChangeSkybox;
    [SerializeField]
    private GameObject btnTextContent;
    [SerializeField]
    private GameObject btnAddAudio;
    [SerializeField]
    private GameObject btnAddVideo;
    [SerializeField]
    private GameObject btnTextToSpeech;
    [SerializeField]
    private GameObject btnAddManipulation;
    [SerializeField]
    private GameObject btnExplodeEffect;
    [SerializeField]
    private GameObject btnCurvedUI;
    [SerializeField]
    private GameObject btnAddLabel;
    [SerializeField]
    private GameObject btnAddAnimation;

   
    [SerializeField]
    public GameObject compChangeSkybox;
    [SerializeField]
    public GameObject compTextContent;

    [SerializeField]
    public GameObject compAddAudio;
    [SerializeField]
    public GameObject compTextToSpeech;
    [SerializeField]
    public GameObject compAddVideo;
    [SerializeField]
    public GameObject compAddManipulation;
    [SerializeField]
    public GameObject compAddExplodeEffect;
    [SerializeField]
    private GameObject compAddCurvedUI;
    [SerializeField]
    private GameObject compAddLabel;
    [SerializeField]
    public GameObject compAddAnimation;
    [SerializeField]
    private GameObject compBrowser;
    [SerializeField]
    public GameObject compLayerEvents;

    // public XR_studio_ControlFlow xR_Studio_ControlFlow;

 

   // public LayerEventsParent layerEventsParent;

   // public actionController currentClickedActionController;

    public Button SaveBtn;

    public static bool videoAdded = false;
   // public static CurvedUI.CurvedUISettings curve;

    public instantiate instantiateScript;

    public GameObject objectCollection = null;

    public GameObject skyboxPrefab = null, textContentPrefab = null, audioPrefab = null,
    textToSpeechPrefab = null, videoPrefab = null, manipulatePrefab = null, explodePrefab = null,
        browserPrefab = null, labelPrefab = null;

    public GameObject labelSceneReference;

    #region COMPONENTCOUNTS
    public short audioCount = 0;
    public short videoCount = 0;
    public short imageCount = 0;
    #endregion

    
    public GameObject animationControllerPrefab;
    public GameObject batchingPrefab = null;
    public GameObject controlFlowUI;
    public GameObject imageComponent, imagePrefab;

    //code by prabu
    public List<GameObject> AllDesccriptionPanelHierarchy = new List<GameObject>();
    public List<GameObject> objListForTTS = new List<GameObject>();

    private void Start()
    {
      

        ScriptsManager.OnCloseClicked += CloseProject;
    }

    private void OnEnable()
    {
        if (ScriptsManager.Instance.projectTypeDropdown.value == 0 && ScriptsManager.Instance.IsExistingProject == false)
        {
            EnableDisableSkyboxFeature(true);
        }
    }

    private void OnDestroy()
    {
        ScriptsManager.OnCloseClicked -= CloseProject;
    }

    private void CloseProject()
    {
        Transform temp = ScriptsManager.Instance.scenePropertyRect.Find("Label");
        if (temp != null)
        {
            Destroy(temp.gameObject);
        }
        explodePrefab.GetComponent<ExplodeComponent>().ResetValue();
       // browserPrefab.GetComponent<ModelReferenceComponent>().ResetValue();
    }

    //Calling Skybox feature button click and component remove button click
    public void EnableDisableSkyboxFeature(bool showPopup)
    {
        if (ScriptsManager.Instance.projectTypeDropdown.value == 0 && ScriptsManager.Instance.IsExistingProject == false)
            showPopup = true;

        //if (skyboxPrefab == null)
        //{
        //    skyboxPrefab = Instantiate(compChangeSkybox, scenePropertiesContent) as GameObject;
        //    //skyboxPrefab.transform.SetAsFirstSibling();

        //    ScriptsManager.Instance.featureScript.skyBox.sprite = ScriptsManager.Instance.featureScript.skyBoxSprites [1];

        //    if (!skyboxPrefab.activeSelf)
        //    {
        //        skyboxPrefab.SetActive(enable);
        //    }

        //    ScriptsManager.Instance.isSkybox = true;
        //    ScriptsManager.Instance.isAudio = ScriptsManager.Instance.isVideo = false;
        //    ScriptsManager.Instance.skyAudVid.titleText.text = "Skybox";
        //    ScriptsManager.Instance.skyAudVid.selectedInputField = skyboxPrefab.GetComponent<ChangeSkybox>().urlInput;
        //    ScriptsManager.Instance.skyAudVid.skyAudVidParent.SetActive(true);
        //    ScriptsManager.Instance.skyAudVid.URLButton();
        //    ScriptsManager.Instance.skyAudVid.CollapsePanel();

        //    GameObject obj = Instantiate(new GameObject(), ScriptsManager.Instance.mySpaceSceneProperty.transform);
        //    obj.name = "Skybox";
        //    obj.AddComponent<NoTransform>();
        //}
        ScriptsManager.Instance.featureselectFunc();

        Reset();
        ScriptsManager.Instance.gameObject.GetComponent<Label_components>().ClearComp();
        ScriptsManager.Instance.transformGizmo.ManipulationControl();
        ScriptsManager.Instance.isSkybox = true;

        if (skyboxPrefab != null && ScriptsManager.Instance.scenePropertyRect.Find("Skybox") == null)
        {


            if (showPopup)
            {
                ScriptsManager.Instance.isAudio = ScriptsManager.Instance.isVideo = false;
                ScriptsManager.Instance.skyAudVid.titleText.text = "Skybox";
                ScriptsManager.Instance.skyAudVid.icon.sprite = Helper.Instance.PopUpSprite("Skybox");
                ScriptsManager.Instance.skyAudVid.noteText.text = "Note: A image source URL can be only from your own hosting server with .jpg file format. For example: https://yourwebsite.com/media/image.jpg";
                ScriptsManager.Instance.skyAudVid.selectedInputField = skyboxPrefab.GetComponent<ChangeSkybox>().urlInput;
                ScriptsManager.Instance.skyAudVid.skyAudVidParent.SetActive(true);
                ScriptsManager.Instance.skyAudVid.CloudButton();
                ScriptsManager.Instance.skyAudVid.RefreshList();
                ScriptsManager.Instance.skyAudVid.ExpandPanel();

                ScriptsManager.Instance.featureScript.skyBox.sprite = ScriptsManager.Instance.featureScript.skyBoxSprites[1];
            }
            if (ScriptsManager.Instance.projectTypeDropdown.value != 0)
            {
                GameObject obj = new GameObject();
                obj.transform.SetParent(ScriptsManager.Instance.scenePropertyRect);
                obj.name = "Skybox";
                obj.tag = "SceneProperty";
                obj.AddComponent<NoTransform>();
                ScriptsManager.Instance.runtimeHierarchy.Refresh();
                ScriptsManager.Instance.runtimeHierarchy.Select(obj.transform);
                skyboxPrefab.SetActive(true);
            }
        }
        else if (ScriptsManager.Instance.scenePropertyRect.Find("Skybox"))
        {
            ScriptsManager.Instance.runtimeHierarchy.Select();
            ScriptsManager.Instance.runtimeHierarchy.Select(ScriptsManager.Instance.scenePropertyRect.Find("Skybox"));
        }
    }

   

    public void Reset()
    {
        ScriptsManager.Instance.featureScript.skyBox.sprite = ScriptsManager.Instance.featureScript.skyBoxSprites[0];
        ScriptsManager.Instance.featureScript.lightImg.sprite = ScriptsManager.Instance.featureScript.lightSprites[0];
        ScriptsManager.Instance.featureScript.browse.sprite = ScriptsManager.Instance.featureScript.browseSprites[0];
        ScriptsManager.Instance.featureScript.explosion.sprite = ScriptsManager.Instance.featureScript.explosionSprites[0];
        ScriptsManager.Instance.featureScript.manipulation.sprite = ScriptsManager.Instance.featureScript.manipulationSprites[0];
        ScriptsManager.Instance.lightComponent.SetActive(false);

        ScriptsManager.Instance.featureselectFunc();
        ScriptsManager.Instance.ModuleVideoStop();

        raycasthitting.enable_label_flag = false;
        ScriptsManager.Instance.featureScript.labels.sprite = ScriptsManager.Instance.featureScript.labelSprites[0];

        skyboxPrefab.SetActive(false);
        textContentPrefab.SetActive(false);
        audioPrefab.SetActive(false);
        videoPrefab.SetActive(false);
        textToSpeechPrefab.SetActive(false);
        textToSpeechPrefab.GetComponent<TextToSpeechComponent>().speechManager.Silence();
        manipulatePrefab.SetActive(false);
        explodePrefab.SetActive(false);
        browserPrefab.SetActive(false);
        labelPrefab.SetActive(false);
        batchingPrefab.SetActive(false);
        imageComponent.SetActive(false);

        ScriptsManager.Instance.transformComponent.SetActive(false);

      //  ObjectNav_Manager.Instance.DisableBatching();

        VerticalLayoutGroup verticalLayoutGroup = scenePropertiesContent.GetComponent<VerticalLayoutGroup>();
        verticalLayoutGroup.SetLayoutVertical();
        ControlFlowManagerV2.Instance.getSceneLineItemPropertyObjects.propertyPanel.SetActive(false);

        ScriptsManager.Instance.runtimeHierarchy.Deselect();
        ScriptsManager.Instance.currentObjectName.gameObject.SetActive(false);
        if (ScriptsManager.currentSelectedModelObjectLineItem != null)
            ScriptsManager.currentSelectedModelObjectLineItem.DeSelect();
    }

    public void EnableLabelComponent()
    {
        //Reset();
        labelPrefab.SetActive(false);

        if (labelSceneReference == null)
        {
            if (raycasthitting.enable_label_flag)
            {
                LabelSceneProperty();
                ScriptsManager.Instance.runtimeHierarchy.Refresh();
                ScriptsManager.Instance.runtimeHierarchy.Select(labelSceneReference.transform);
            }
        }
        else
        {
            List<GameObject> labelGobject = ScriptsManager.Instance.GetComponent<raycasthitting>().labels;

            if (labelGobject.Count > 0)
            {
                labelPrefab.SetActive(true);
                ScriptsManager.Instance.runtimeHierarchy.Refresh();
                ScriptsManager.Instance.runtimeHierarchy.Select(labelSceneReference.transform);
            }
            else
            {
                labelPrefab.GetComponent<LabelComponent>().RemoveComponent();
                Destroy(labelSceneReference);
            }
        }

        ScriptsManager.Instance.transformComponent.SetActive(false);
    }

    public void LabelSceneProperty()
    {
        GameObject labelComponent = new GameObject();
        labelComponent.name = "Label";
        labelComponent.tag = "SceneProperty";
        labelComponent.transform.SetParent(ScriptsManager.Instance.scenePropertyRect);
        labelComponent.AddComponent<NoTransform>().propertyType = TypesOfProperty.Labels;
        labelSceneReference = labelComponent;
        labelPrefab.SetActive(true);
        labelPrefab.GetComponent<LabelComponent>().labelSceneReference = labelSceneReference;
    }

    //public void AllDescriptionPanelOff()
    //{
    //    for (int i = 0; i < AllDesccriptionPanel.Count; i++)
    //    {
    //        if (AllDesccriptionPanel[i] != null)
    //            AllDesccriptionPanel[i].gameObject.SetActive(false);
    //    }
    //}

    //Calling Text content feature button click and component remove button click
    //  public List<TextContentComponent> AllDesccriptionPanel = new List<TextContentComponent>();



    public void EnableDisableTextContentFeature(bool enable)
    {
        Reset();
        //if (textContentPrefab == null)
        //{
        //    textContentPrefab = Instantiate(compTextContent, scenePropertiesContent) as GameObject;

        //    if (!textContentPrefab.activeSelf)
        //    {
        //        textContentPrefab.SetActive(enable);
        //    }
        //}
        //   ScriptsManager.Instance.textPropertyObj.text = "";

        ScriptsManager.Instance.textPropertyObj.SetActive(enable);

        //GameObject textProperty = Instantiate(compTextContent, scenePropertiesContent);
        //GameObject textProperty = Instantiate(compTextContent, ScriptsManager.Instance.propertyRect.transform);


        //done by prabhu
        //GameObject textProperty = Instantiate(textContentPrefab, ScriptsManager.Instance.propertyRect.transform);


        //newTextProperty = textProperty;
        //textProperty.transform.SetAsFirstSibling();

        //GameObject panel = Instantiate(ScriptsManager.Instance.descriptionPanelPrefab, ScriptsManager.Instance.mySpaceSceneProperty.transform);

        GameObject panel = Instantiate(ScriptsManager.Instance.descriptionPanelPrefab, ScriptsManager.Instance.scenePropertyRect);

        TextContentComponent propScript = ScriptsManager.Instance.textPropertyObj.GetComponent<TextContentComponent>();

        //  AllDesccriptionPanel.Add(propScript);

        AllDesccriptionPanelHierarchy.Add(panel);

        propScript.SubscribeToDelegate();

        propScript.panelProperty = panel.GetComponent<TextContentCanvasHandler>();

        panel.AddComponent<ObjectTransformComponent>();




        #region prabu code

        ScriptsManager.Instance.panelCount++;

        panel.name = "Description panel(" + ScriptsManager.Instance.panelCount.ToString() + ")";

        // ScriptsManager.Instance.textPropertyObj.name = "Property_Description panel(" + ScriptsManager.Instance.panelCount.ToString() + ")"; 

        // ScriptsManager.Instance.textPropertyObj.tag = "SceneProperty";

        propScript.panelProperty.gameObject.GetComponent<ObjectTransformComponent>().currentTransform.objectName = panel.name;
        propScript.panelProperty.gameObject.GetComponent<ObjectTransformComponent>().SetTransform(panel.transform.position, panel.transform.rotation.eulerAngles, panel.transform.localScale);

        propScript.panelProperty.isSceneTextpanel = true;
        // propScript.panelProperty.textContentCurrentObjDetails= propScript.SetContentValue(propScript.panelProperty.textContentCurrentObjDetails);

        #endregion

        //propScript.panelProperty.gameObject.GetComponent<ObjectTransformComponent>().SetPosition(new Vector3(panel.transform.position.x, panel.transform.position.y, panel.transform.position.z));
        //propScript.panelProperty.gameObject.GetComponent<ObjectTransformComponent>().SetRotation(new Vector3(panel.transform.rotation.x, panel.transform.rotation.y, panel.transform.rotation.z));
        //propScript.panelProperty.gameObject.GetComponent<ObjectTransformComponent>().SetScale(new Vector3(panel.transform.localScale.x, panel.transform.localScale.y, panel.transform.localScale.z));

        // propScript.panelProperty.transform.eulerAngles = new Vector3(panel.transform.rotation.x, panel.transform.rotation.y, panel.transform.rotation.z);
        //  propScript.panelProperty.transform.localScale = new Vector3(panel.transform.localScale.x, panel.transform.localScale.y, panel.transform.localScale.z);

        if (ScriptsManager.Instance.projectTypeDropdown.value == 1)
        {
            //pushing the content type obj to control flow scene objects list
            ControlFlowManagerV2.Instance.CreateSeneObjectLineItem(panel, ActionStates.TextPanel);
        }
        else
        {
            ScriptsManager.Instance.runtimeHierarchy.Refresh();
            ScriptsManager.Instance.runtimeHierarchy.Select(panel.transform);

            SelectTransform(panel.transform);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(propScript.GetComponent<RectTransform>());

    }

    private void SelectTransform(Transform transform)
    {
        if (ScriptsManager.Instance.projectTypeDropdown.value.Equals(1))
        {
            ScriptsManager.Instance.OnHierarchySelectionChanged(transform);
        }
    }

    public void EnableDisableAudioFeature(bool enable)
    {
        if (ScriptsManager.Instance.projectTypeDropdown.value == 1)
        {
            if (ControlFlowManagerV2.currentSelectedSceneLineItem.objList.Any(s => s.ThisClassAction == ActionStates.Audio))
            {
                ScriptsManager.Instance.errorMessage.text = "A audio scene object has been already added to the scene.";

                return;
            }
        }
        Reset();
        audioCount++;

        ScriptsManager.Instance.isAudio = true;
        ScriptsManager.Instance.isSkybox = ScriptsManager.Instance.isVideo = false;
        ScriptsManager.Instance.transformGizmo.ManipulationControl();

        if (enable)
        {
            ScriptsManager.Instance.skyAudVid.titleText.text = "Audio";
            ScriptsManager.Instance.skyAudVid.icon.sprite = Helper.Instance.PopUpSprite("Audio");
            ScriptsManager.Instance.skyAudVid.noteText.text = "Note: A audio source URL can be only from your own hosting server with .mp3 file format. For example: https://yourwebsite.com/media/audio.mp3";
            audioPrefab.SetActive(true);
            GameObject audioReference = CreateAudio_HierarchyObject($"Audio ({audioCount})");

            ExistingProjectManager.instance.scenePropertyObjects.Add(audioReference);
            ScriptsManager.Instance.skyAudVid.skyAudVidParent.SetActive(true);
            ScriptsManager.Instance.skyAudVid.CloudButton();
            ScriptsManager.Instance.skyAudVid.RefreshList();
            ScriptsManager.Instance.skyAudVid.ExpandPanel();

            if (ScriptsManager.Instance.projectTypeDropdown.value == 1)
            {
                //pushing the content type obj to control flow scene objects list
                //  ControlFlowManagerV2.Instance.CreateSeneObjectLineItem(audioReference, ActionStates.Audio);

            }
            else
            {
                ScriptsManager.Instance.runtimeHierarchy.Refresh();
                ScriptsManager.Instance.runtimeHierarchy.Select(audioReference.transform);

                SelectTransform(audioReference.transform);
            }

            UndoRedoManager.Clear();

        }
    }


    public GameObject CreateAudio_HierarchyObject(string name, string url = null)
    {

        GameObject obj = new GameObject();
        obj.transform.SetParent(ScriptsManager.Instance.scenePropertyRect);
        obj.name = name;
        obj.tag = "SceneProperty";
        obj.AddComponent<NoTransform>();
        NoTransform noTransform = obj.GetComponent<NoTransform>();

        noTransform.propertyType = TypesOfProperty.Audio;
        noTransform.componentObject = audioPrefab;
        noTransform.audioComponent = noTransform.componentObject.GetComponent<AudioComponent>();
        noTransform.webLink = url;
        noTransform.audioComponent.inputAudioClipPath.text = url;
        noTransform.audioComponent.hierarchyReference = obj;

        ScriptsManager.Instance.skyAudVid.linkHolder = obj;
        ScriptsManager.Instance.skyAudVid.selectedInputField = audioPrefab.GetComponent<AudioComponent>().inputAudioClipPath;
        if (ScriptsManager.Instance.projectTypeDropdown.value == 1)
        {
            //pushing the content type obj to control flow scene objects list
            ControlFlowManagerV2.Instance.CreateSeneObjectLineItem(obj, ActionStates.Audio);
        }
        return obj;
    }

    Transform FindedWalkThroughObj;
    public void EnableDisableWalkThroughheirarchy()
    {
        //  ScriptsManager.
        // if (compWalkThrough != null && ScriptsManager.Instance.scenePropertyRect.Find("WalkThrough") == null)
        Reset();
        ScriptsManager.Instance.transformGizmo.ManipulationControl();
        ScriptsManager.Instance.featureScript.WalkThrough.sprite = ScriptsManager.Instance.featureScript.WalkThroughSprites[1];
        if (ScriptsManager.Instance.scenePropertyRect.Find("WalkThrough") == null)
        {

            GameObject obj = new GameObject();
            obj.transform.SetParent(ScriptsManager.Instance.scenePropertyRect);
            obj.name = "WalkThrough";
            obj.tag = "SceneProperty";
            obj.AddComponent<NoTransform>();
            ScriptsManager.Instance.runtimeHierarchy.Refresh();
            // ScriptsManager.Instance.runtimeHierarchy.Select();
            ScriptsManager.Instance.runtimeHierarchy.Select(obj.transform);
            return;
        }
        else if (ScriptsManager.Instance.scenePropertyRect.Find("WalkThrough"))
        {
            //  ScriptsManager.Instance.runtimeHierarchy.Select();
            ScriptsManager.Instance.runtimeHierarchy.Refresh();

            ScriptsManager.Instance.runtimeHierarchy.Select(ScriptsManager.Instance.scenePropertyRect.Find("WalkThrough"));
        }
        if (FindedWalkThroughObj == null)
        {
            FindedWalkThroughObj = ScriptsManager.Instance.scenePropertyRect.Find("WalkThrough");
        }


    }

    public void EnableDisableWalkThroughFeature(bool enable)
    {
        //  ScriptsManager.
        // if (compWalkThrough != null && ScriptsManager.Instance.scenePropertyRect.Find("WalkThrough") == null)
        Reset();
        ScriptsManager.Instance.transformGizmo.ManipulationControl();
        if (ScriptsManager.Instance.scenePropertyRect.Find("WalkThrough") == null)
        {

            GameObject obj = new GameObject();
            obj.transform.SetParent(ScriptsManager.Instance.scenePropertyRect);
            obj.name = "WalkThrough";
            obj.tag = "SceneProperty";
            obj.AddComponent<NoTransform>();
            ScriptsManager.Instance.runtimeHierarchy.Refresh();
            ScriptsManager.Instance.runtimeHierarchy.Select(obj.transform);
        }
        else if (ScriptsManager.Instance.scenePropertyRect.Find("WalkThrough"))
        {

            ScriptsManager.Instance.runtimeHierarchy.Select();
            ScriptsManager.Instance.runtimeHierarchy.Select(ScriptsManager.Instance.scenePropertyRect.Find("WalkThrough"));
        }
        if (FindedWalkThroughObj == null)
        {
            FindedWalkThroughObj = ScriptsManager.Instance.scenePropertyRect.Find("WalkThrough");
            //  ScriptsManager.Instance.runtimeHierarchy.Refresh();
            // ScriptsManager.Instance.runtimeHierarchy.Select(FindedWalkThroughObj.transform);
        }
       

    }

    

    public void AddImage()
    {
        Reset();
        imageCount++;
        GameObject imagePanel = Instantiate(imagePrefab, ScriptsManager.Instance.scenePropertyRect);
        imagePanel.name = $"Image ({imageCount})";

        ScriptsManager.Instance.transformGizmo.ManipulationControl();
        ScriptsManager.Instance.isImage = true;
        ScriptsManager.Instance.isSkybox = ScriptsManager.Instance.isAudio = ScriptsManager.Instance.isVideo = false;
        ScriptsManager.Instance.skyAudVid.titleText.text = "Image";
        ScriptsManager.Instance.skyAudVid.icon.sprite = Helper.Instance.PopUpSprite("Image");
        imageComponent.SetActive(true);

        ImageComponent component = imageComponent.GetComponent<ImageComponent>();
        imagePanel.GetComponent<NoTransform>().imageComponent = component;
        component.icon = imagePanel.GetComponent<SpriteRenderer>();
        component.imageURLField.text = "";
        component.imageType = ComponentType.Scene;

        ScriptsManager.Instance.skyAudVid.selectedInputField = component.imageURLField;
        ScriptsManager.Instance.skyAudVid.skyAudVidParent.SetActive(true);    //video select ,audio select ,skybox select tab
        ScriptsManager.Instance.skyAudVid.CloudButton();
        ScriptsManager.Instance.skyAudVid.RefreshList();
        ScriptsManager.Instance.skyAudVid.ExpandPanel();
    }

    public void EnableDisableVideoFeature(bool enable)
    {
        if (ScriptsManager.Instance.projectTypeDropdown.value == 1)
        {
            if (ControlFlowManagerV2.currentSelectedSceneLineItem.objList.Any(s => s.ThisClassAction == ActionStates.Video))
            {
                ScriptsManager.Instance.errorMessage.text = "A video scene object has been already added to the scene.";
                return;
            }
        }

        Reset();

        videoCount++;
        ScriptsManager.Instance.transformGizmo.ManipulationControl();
        if (enable)
        {
            videoAdded = enable;
            ButtonCustomizationManager.Instance.videoAdded = true;
            ScriptsManager.Instance.isVideo = true;
            ScriptsManager.Instance.isSkybox = ScriptsManager.Instance.isAudio = ScriptsManager.Instance.isImage = false;
            ScriptsManager.Instance.skyAudVid.titleText.text = "Video";
            ScriptsManager.Instance.skyAudVid.icon.sprite = Helper.Instance.PopUpSprite("Video");
            ScriptsManager.Instance.skyAudVid.noteText.text = "Note: A video source URL can be only from your own hosting server with .mp4 file format. For example: https://yourwebsite.com/media/video.mp4";
            videoPrefab.GetComponent<VideoComponentScene>().inputVideoClipPath.text = "";
            ScriptsManager.Instance.skyAudVid.selectedInputField = videoPrefab.GetComponent<VideoComponentScene>().inputVideoClipPath;
            ScriptsManager.Instance.skyAudVid.skyAudVidParent.SetActive(true);    //video select ,audio select ,skybox select tab
            ScriptsManager.Instance.skyAudVid.CloudButton();
            ScriptsManager.Instance.skyAudVid.RefreshList();
            ScriptsManager.Instance.skyAudVid.ExpandPanel();

            GameObject videoObject = CreateVideo_HierarchyObject($"Video ({videoCount})");

            if (ScriptsManager.Instance.projectTypeDropdown.value == 1)
            {
                //pushing the content type obj to control flow scene objects list
                ControlFlowManagerV2.Instance.CreateSeneObjectLineItem(videoObject, ActionStates.Video);
            }
            else
            {
                ScriptsManager.Instance.runtimeHierarchy.Select(videoObject.transform);

                SelectTransform(videoObject.transform);
            }

            // videoObject.AddComponent<ObjectTransformComponent>();
            CurrentObjectTransform.currentObject = videoObject.transform;
            ExistingProjectManager.instance.scenePropertyObjects.Add(videoObject);
            //ScriptsManager.Instance.videoPanel.transform.SetParent(ScriptsManager.Instance.objectCollection.transform);
            // ScriptsManager.Instance.videoPanel.SetActive(true);
            //  ScriptsManager.Instance.runtimeHierarchy.Refresh();
        }

        //videoPrefab = Instantiate(compAddVideo, scenePropertiesContent) as GameObject;
        //videoPrefab.transform.SetAsFirstSibling();
        //Canvas.ForceUpdateCanvases();
        //ScriptsManager.Instance.featureScript.video.sprite = ScriptsManager.Instance.featureScript.videoSprites[1];

        //if (!videoPrefab.activeSelf)
        //{
        //    videoPrefab.SetActive(enable);
        //}

        //instantiateScript.enableVPanel();

        //videoPrefab.GetComponent<VideoComponentScene>().instantiateScript = instantiateScript;
        //instantiateScript.url = videoPrefab.GetComponent<VideoComponentScene>().inputVideoClipPath;
        //instantiateScript.widthSlider = videoPrefab.GetComponent<VideoComponentScene>().width;
        //instantiateScript.heightSlider = videoPrefab.GetComponent<VideoComponentScene>().height;
        //instantiateScript.dropDown = videoPrefab.GetComponent<VideoComponentScene>().dropdown;
    }


    /// <summary>
    /// this method is called when trying to create the object as well as loading the object from existing content 
    /// </summary>
    /// <param name="objectName"></param>
    /// <param name="url"></param>
    /// <param name="addVideo"></param>
    /// <returns></returns>
    public GameObject CreateVideo_HierarchyObject(string objectName, string url = null, AddVideo addVideo = null)
    {
        // GameObject obj = new GameObject();
        //  GameObject t = GameObject.Find("VideoPlane");
        GameObject obj = Instantiate(ScriptsManager.Instance.videoPanel1);
        ScriptsManager.Instance.ModuleVideo.Add(obj);
        obj.transform.gameObject.SetActive(true);
        ScriptsManager.Instance.transformComponent.gameObject.SetActive(true);
        obj.transform.SetParent(ScriptsManager.Instance.scenePropertyRect);
        obj.name = objectName;
        obj.tag = "videoPlane";
        obj.AddComponent<NoTransform>();

        NoTransform noTransform = obj.GetComponent<NoTransform>();
        ScriptsManager.Instance.videoCount++;
        noTransform.videoID = ScriptsManager.Instance.videoCount;
        //need to remove later (due to improper code flow)
        ScriptsManager.Instance.videoCollection.Add(noTransform.videoID, obj);
        noTransform.propertyType = TypesOfProperty.Video;
        noTransform.currentObjectName = obj.name;
        noTransform.componentObject = videoPrefab;
        noTransform.componentObject.SetActive(true);
        noTransform.videoComponent = noTransform.componentObject.GetComponent<VideoComponentScene>();
        noTransform.videoComponent.noTransform = noTransform;
        // noTransform.videoComponent.removeButton.SetActive(true);
        noTransform.webLink = url;
        noTransform.videoComponent.inputVideoClipPath.text = noTransform.webLink;
        noTransform.videoComponent.hierarchyReference = obj;

        if (addVideo != null)
        {
            noTransform.videoHeight = addVideo.height;
            obj.GetComponent<VideoPlayer>().aspectRatio = (VideoAspectRatio)addVideo.aspectRatio;
            noTransform.aspectRatioValue = addVideo.aspectRatio;
            noTransform.videoComponent.videoType = ComponentType.Scene;
        }

        ScriptsManager.Instance.skyAudVid.linkHolder = obj;
        ScriptsManager.Instance.runtimeHierarchy.Refresh();
        //ScriptsManager.Instance.runtimeHierarchy.Select(obj.transform);
        // if(SelectHierarchy==false)
        return obj;
    }
    public NoTransform CreateVideo_HierarchyObjectret(string objectName, string url = null, AddVideo addVideo = null)
    {
        GameObject obj = new GameObject();
        obj.transform.SetParent(ScriptsManager.Instance.scenePropertyRect);
        ScriptsManager.Instance.videoPanel = obj;
        obj.name = objectName;
        obj.tag = "SceneProperty";
        obj.AddComponent<NoTransform>();
        NoTransform noTransform = obj.GetComponent<NoTransform>();

        noTransform.propertyType = TypesOfProperty.Video;
        noTransform.componentObject = videoPrefab;
        noTransform.componentObject.SetActive(true);

        if (addVideo != null)
        {
            noTransform.videoHeight = addVideo.height;
            noTransform.aspectRatioValue = addVideo.aspectRatio;
        }

        noTransform.videoComponent = noTransform.componentObject.GetComponent<VideoComponentScene>();
        noTransform.videoComponent.noTransform = noTransform;
        noTransform.webLink = url;
        noTransform.videoComponent.inputVideoClipPath.text = noTransform.webLink;
        noTransform.videoComponent.hierarchyReference = obj;

        ScriptsManager.Instance.skyAudVid.linkHolder = obj;

        return noTransform;
    }

    public void EnableDisabelControlFlow(bool enable)
    {
        if (enable)
        {
            ScriptsManager.Instance.rtEditor.SetActive(false);
            ScriptsManager.Instance.transformComponent.SetActive(false);
            controlFlowUI.SetActive(true);
        }
    }


    //method changes altered by saravanan
    public void EnableDisableTextToSpeech(bool enable)
    {
        Reset();
        ScriptsManager.Instance.transformGizmo.ManipulationControl();
        //   if (textToSpeechPrefab != null && ScriptsManager.Instance.scenePropertyRect.Find("TextToSpeech") == null)
        // {
        ScriptsManager.Instance.featureScript.textToSpeech.sprite = ScriptsManager.Instance.featureScript.textToSpeechSprites[1];

        GameObject obj = new GameObject();
        obj.transform.SetParent(ScriptsManager.Instance.scenePropertyRect);
        obj.name = "TextToSpeech" + objListForTTS.Count;
        obj.tag = "SceneProperty";
        obj.AddComponent<NoTransform>();
        ScriptsManager.Instance.textToSpeechPropertyObj.GetComponent<TextToSpeechComponent>().SubscribeDelegates();
        ScriptsManager.Instance.runtimeHierarchy.Refresh();
        ScriptsManager.Instance.runtimeHierarchy.Select(obj.transform);
        SelectTransform(obj.transform);

        objListForTTS.Add(obj);

        //pushing the content type obj to control flow scene objects list
        ControlFlowManagerV2.Instance.CreateSeneObjectLineItem(obj, ActionStates.TextToSpeech);

        //}
        //else if (ScriptsManager.Instance.scenePropertyRect.Find("TextToSpeech"))
        //{
        //    ScriptsManager.Instance.runtimeHierarchy.Select(ScriptsManager.Instance.scenePropertyRect.Find("TextToSpeech"));
        //    SelectTransform(ScriptsManager.Instance.scenePropertyRect.Find("TextToSpeech").transform);
        //}
    }

    public void EnableDisableManinpulation(bool enable)
    {
        Reset();
        ScriptsManager.Instance.transformGizmo.ManipulationControl();
        if (manipulatePrefab != null && ScriptsManager.Instance.scenePropertyRect.Find("Gestures") == null)
        {
            ScriptsManager.Instance.isGesture = true;
            ScriptsManager.Instance.featureScript.manipulation.sprite = ScriptsManager.Instance.featureScript.manipulationSprites[1];

            if (ScriptsManager._toolContent != null)
            {
                manipulatePrefab.GetComponent<ManipulationComponent>().move.isOn = ScriptsManager._toolContent.moveScript;
                manipulatePrefab.GetComponent<ManipulationComponent>().rotate.isOn = ScriptsManager._toolContent.rotScript;
                manipulatePrefab.GetComponent<ManipulationComponent>().scale.isOn = ScriptsManager._toolContent.scaleScript;
            }

            instantiateScript.move = manipulatePrefab.GetComponent<ManipulationComponent>().move;
            instantiateScript.rotate = manipulatePrefab.GetComponent<ManipulationComponent>().rotate;
            instantiateScript.scaling = manipulatePrefab.GetComponent<ManipulationComponent>().scale;

            manipulatePrefab.GetComponent<ManipulationComponent>().SubscribeToDelegate();

            GameObject obj = new GameObject("Gestures");
            obj.transform.SetParent(ScriptsManager.Instance.scenePropertyRect);
            obj.tag = "SceneProperty";
            obj.AddComponent<NoTransform>();
            ScriptsManager.Instance.runtimeHierarchy.Refresh();
            ScriptsManager.Instance.runtimeHierarchy.Select(obj.transform);

            LayoutRebuilder.ForceRebuildLayoutImmediate(manipulatePrefab.GetComponent<RectTransform>());
        }
        else if (ScriptsManager.Instance.scenePropertyRect.Find("Gestures"))
        {
            ScriptsManager.Instance.runtimeHierarchy.Select();
            ScriptsManager.Instance.runtimeHierarchy.Select(ScriptsManager.Instance.scenePropertyRect.Find("Gestures"));
        }

        //if (manipulatePrefab == null)
        //{
        //    manipulatePrefab = Instantiate(compAddManipulation, scenePropertiesContent) as GameObject;
        //    manipulatePrefab.transform.SetAsFirstSibling();

        //    ScriptsManager.Instance.featureScript.manipulation.sprite = ScriptsManager.Instance.featureScript.manipulationSprites [1];

        //    if (!manipulatePrefab.activeSelf)
        //    {
        //        manipulatePrefab.SetActive(enable);
        //    }

        //    if (ScriptsManager._XRStudioContent != null)
        //    {
        //        Debug.LogError(ScriptsManager._XRStudioContent);
        //        manipulatePrefab.GetComponent<ManipulationComponent>().move.isOn = ScriptsManager._XRStudioContent.moveScript;
        //        manipulatePrefab.GetComponent<ManipulationComponent>().rotate.isOn = ScriptsManager._XRStudioContent.rotScript;
        //        manipulatePrefab.GetComponent<ManipulationComponent>().scale.isOn = ScriptsManager._XRStudioContent.scaleScript;
        //    }

        //    instantiateScript.move = manipulatePrefab.GetComponent<ManipulationComponent>().move;
        //    instantiateScript.rotate = manipulatePrefab.GetComponent<ManipulationComponent>().rotate;
        //    instantiateScript.scaling = manipulatePrefab.GetComponent<ManipulationComponent>().scale;
        //}
    }

    public List<GameObject> objHasAnimation = new List<GameObject>();

   

   
    public void AddExplodeEffect(bool enable)
    {
        Reset();
        ScriptsManager.Instance.transformGizmo.ManipulationControl();
        //ScriptsManager.Instance.isExplodable = true;

        if (explodePrefab != null && ScriptsManager.Instance.scenePropertyRect.Find("Explode Effect") == null)
        {
            ScriptsManager.Instance.featureScript.explosion.sprite = ScriptsManager.Instance.featureScript.explosionSprites[1];
            instantiateScript.explode = explodePrefab.GetComponentInChildren<UnityEngine.UI.Toggle>();

            if (objectCollection != null && objectCollection.transform.childCount > 0)
            {
                for (int i = 0; i < objectCollection.transform.childCount; i++)
                {
                    Transform local = objectCollection.transform.GetChild(i);

                    if (local != null && local.tag != "Label")
                    {
                        if (!local.gameObject.GetComponent<TextContentCanvasHandler>() && !local.CompareTag("videoPlane"))
                        {
                            if (local.gameObject.GetComponent<ThreeDModelFunctions>() && local.gameObject.GetComponentInChildren<Animation>())
                            {
                                if (local.gameObject.GetComponentInChildren<Animation>().GetClipCount() == 0)
                                {
                                    PopulateCollectionChildren(local);
                                }
                            }
                            else
                            {
                                PopulateCollectionChildren(local);
                            }
                        }
                    }
                }
            }
            GameObject obj = new GameObject();
            obj.transform.SetParent(ScriptsManager.Instance.scenePropertyRect);
            obj.name = "Explode Effect";
            obj.tag = "SceneProperty";
            obj.AddComponent<NoTransform>();
            ScriptsManager.Instance.runtimeHierarchy.Refresh();
            ScriptsManager.Instance.runtimeHierarchy.Select(obj.transform);
        }
        else if (ScriptsManager.Instance.scenePropertyRect.Find("Explode Effect"))
        {
            ScriptsManager.Instance.runtimeHierarchy.Select();
            ScriptsManager.Instance.runtimeHierarchy.Select(ScriptsManager.Instance.scenePropertyRect.Find("Explode Effect"));
        }


        //if (explodePrefab == null)
        //{
        //    ScriptsManager.Instance.isExplodable = true;

        //    explodePrefab = Instantiate(compAddExplodeEffect, scenePropertiesContent) as GameObject;
        //    explodePrefab.transform.SetAsFirstSibling();

        //    ScriptsManager.Instance.featureScript.explosion.sprite = ScriptsManager.Instance.featureScript.explosionSprites[1];

        //    if (!explodePrefab.activeSelf)
        //    {
        //        explodePrefab.SetActive(enable);
        //    }

        //    instantiateScript.explode = explodePrefab.GetComponentInChildren<UnityEngine.UI.Toggle>();
        //    explodePrefab.GetComponent<ExplodeComponent>().instatiateScript = instantiateScript;

        //    if (objectCollection != null && objectCollection.transform.childCount > 0)
        //    {
        //        for (int i = 0; i < objectCollection.transform.childCount; i++)
        //        {
        //            Transform local = objectCollection.transform.GetChild(i);

        //            if (local != null)
        //            {
        //                if (!local.gameObject.GetComponent<TextContentCanvasHandler>())
        //                {
        //                    local.gameObject.AddComponent<ThreeDModelFunctions>();

        //                    PopulateCollectionChildren(local);
        //                }
        //            }
        //        }
        //    }
        //    else if (objectCollection.transform.childCount == 0)
        //    {
        //        Debug.Log("There's no children!");
        //    }
        //}

    }

    public void PopulateCollectionChildren(Transform child)
    {
        ExplodeComponent explodeComponent = explodePrefab.GetComponent<ExplodeComponent>();

        //GameObject explodeObject = Instantiate(explodeComponent.objectExplodePrefab.gameObject, explodeComponent.parent);
        //ObjectExplode objectExplode = explodeObject.GetComponent<ObjectExplode>();
        //explodeComponent.explodeList.Add(objectExplode);

        //objectExplode.model = child;
        //objectExplode.explodeComponent = explodeComponent;
        //objectExplode.name = objectExplode.nameField.text = child.name;
        //objectExplode.modelFunction = child.GetComponent<ThreeDModelFunctions>();
        //objectExplode.toggle.isOn = objectExplode.modelFunction.canExplode;
    }

    public void enableAddLabel()
    {


        if (ScriptsManager.Instance.isBatchingEnabled) //Need to find a better way.
        {
            Reset();
        }




        ScriptsManager.Instance.transformGizmo.ManipulationControl();

        raycasthitting.enable_label_flag = !raycasthitting.enable_label_flag;

        //if (ControlFlowManagerV2.currentSelectedSceneObjLineItem != null)
        //{
        //    ControlFlowManagerV2.Instance.labelButtObj.GetComponent<Image>().color = raycasthitting.enable_label_flag ? Color.blue : Color.white;

        //}

        //for simple project
        ScriptsManager.Instance.featureScript.labels.sprite = raycasthitting.enable_label_flag ? ScriptsManager.Instance.featureScript.labelSprites[1] : ScriptsManager.Instance.featureScript.labelSprites[0];

        //ScriptsManager.Instance.transformComponent.SetActive(false);
        //EnableLabelComponent();
    }


    public void AddBrowserHeirarchy()
    {
        Reset();
        if (ScriptsManager.Instance.scenePropertyRect.Find("Browser") == null)
        {

            GameObject obj = new GameObject();
            obj.transform.SetParent(ScriptsManager.Instance.scenePropertyRect);
            obj.name = "Browser";
            obj.tag = "SceneProperty";
            obj.AddComponent<NoTransform>();
            ScriptsManager.Instance.runtimeHierarchy.Refresh();
            ScriptsManager.Instance.runtimeHierarchy.Select(obj.transform);
        }
        else if (ScriptsManager.Instance.scenePropertyRect.Find("Browser"))
        {
            ScriptsManager.Instance.runtimeHierarchy.Select();
            ScriptsManager.Instance.runtimeHierarchy.Select(ScriptsManager.Instance.scenePropertyRect.Find("Browser"));
        }
        var temp = ScriptsManager._toolContent;

        foreach (var button in temp.dsButtons)
        {
            ControlFlowButton controlFlowButton = ScriptsManager.Instance.dsButtons[temp.dsButtons.IndexOf(button)].GetComponentInChildren<ControlFlowButton>();
            if (controlFlowButton.referencedTo.Equals(button.referencedTo))
            {
                if (button.referencedTo.Equals(AssignButton.Browser.ToString()))
                {
                   // ScriptsManager.Instance.enableDisableComponent.modelReference.BrowserPrefab.browseButton = ScriptsManager.Instance.dsButtons[temp.dsButtons.IndexOf(button)];
                   // ScriptsManager.Instance.enableDisableComponent.modelReference.BrowserPrefab.browse.isOn = true;
                    //ScriptsManager.Instance.enableDisableComponent.modelReference.BrowserPrefab.BrowseFunc(ScriptsManager.Instance.enableDisableComponent.modelReference.BrowserPrefab.browse);
                   // ScriptsManager.Instance.enableDisableComponent.modelReference.BrowserPrefab.editBrowse.interactable = true;
                    break;
                }

            }
        }
        //ScriptsManager.Instance.featureScript.browse.sprite = ScriptsManager.Instance.featureScript.browseSprites[1];

    }
    public void AddBrowser()
    {
        Reset();
        if (ScriptsManager.Instance.scenePropertyRect.Find("Browser") == null)
        {

            GameObject obj = new GameObject();
            obj.transform.SetParent(ScriptsManager.Instance.scenePropertyRect);
            obj.name = "Browser";
            //   obj.tag = "SceneProperty";
            obj.AddComponent<NoTransform>();
            ScriptsManager.Instance.runtimeHierarchy.Refresh();
            ScriptsManager.Instance.runtimeHierarchy.Select(obj.transform);
        }
        else if (ScriptsManager.Instance.scenePropertyRect.Find("Browser"))
        {
            ScriptsManager.Instance.runtimeHierarchy.Select();
            ScriptsManager.Instance.runtimeHierarchy.Select(ScriptsManager.Instance.scenePropertyRect.Find("Browser"));
        }
    }
}
