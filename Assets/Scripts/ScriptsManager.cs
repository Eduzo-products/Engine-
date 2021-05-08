using CommandUndoRedo;
using Crosstales.RTVoice;
using Crosstales.RTVoice.Tool;
using RuntimeGizmos;
using RuntimeInspectorNamespace;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Video;

public enum AssetTypes
{
    Models = 1,
    SkyBox,
    Video,
    Audio,
    Image,
    ImmersiveVideo,
    ImmersiveImage
}

public enum ProjectType
{
    Simple, Batching, ControlFlow, Immersive
}

public enum LineRendererState { Visible, Dim };

public class ScriptsManager : Singleton<ScriptsManager>
{

    public string APIBaseURL = ""; 

    public float progress;

    public RectTransform myAssetContentRect, inQueueContentRect, libraryContentRect;
    public TMP_Text cloudAssetCount, libraryAssetCount;
    public GameObject myAssetButton;

    public GameObject objectCollection;

    [Header("<------ UI ------>")]
    public TextMeshProUGUI errorMessage;
    public TextMeshProUGUI loginErrorText, projectNameErrorText;
    public TextMeshProUGUI projectTitleText;
    public Text fileExplorerErrorText;
    public GameObject LoginPage, projectSelector, newProject, skyAudVidObject, console, confirmationPanelObject, mainScene,  clearError;//sceneViewNavigationObject;
    public TMP_InputField userNameField, passwordField, projectNameField;
    public Image threeDButton, threeSixtyButton;
    public Sprite defaultSprite, selectedSprite;
    public CameraController cameraController;
    public GameObject descriptionPanelPrefab;
    public int panelCount = 0;

    [Header("Testing Objects")]
    public string mainPath;
    public GameObject mainCamera;

    [Header("<------ Data From Server ------>")]
    public ClientDetails userDetails;
    public DownloadAsset downloadAsset;

    public DownloadCloudAssets cloudAssets;
    public List<AssetDetails> cloudData;

    public DownloadLibraryAssets libraryAssets;
    public List<AssetDetails> libraryData;

    public List<GameObject> downloadedObjects;
    public List<BundleDetails> bundleDetails;


    [Header("<------ UI Theme ------>")]
    public Color darkSkin;
    public Color lightSkin;
    public bool isDark = false;
    public List<Image> panelImages;
    public static ToolContent _toolContent;
    public GameObject transformComponent = null;
    public RuntimeHierarchy runtimeHierarchy;
    public TransformGizmo transformGizmo;
    public AudioSource audioManager;
    public delegate void CloseProjectDelegate();
    public static CloseProjectDelegate OnCloseClicked;
    public EventSystem eventSystem;
    public instantiate instantiateScript;
    public TMP_InputField currentObjectName;
    public SkyAudVid skyAudVid;
    public bool isAssets, isAudio, isVideo, isSkybox, isDeletable, isHotspotDeletable, isExplodable, isGesture, isImage, isImmersiveImage, isImmersiveVideo, saveButton, resetButton, cancelButton, isBrowseFeatureEnabled;
    public ConfirmationPanel confirmationPanel;
    public FeaturesScript featureScript;
    public bool isPopedUp = false;
    public bool move, rotate, scale,
        video, labels, navigator, back;

    public bool showAnimation;
    [HideInInspector] public Transform objecttoBeDeleted;

    public string skuboxURL;
    public EnableDisableComponent enableDisableComponent;
    public int bundleCount = 0;
    public int AssetCount = 0;
    public GameObject transistionObject;
    public GameObject videoPanel;
    public GameObject videoPanel1;

    public bool isDraggingObj;
    public GameObject lastDraggedObj;
    public Transform scenePropertyRect, propertyRect;
    public GameObject skyboxPropertyObj, audioPropertyObj, textPropertyObj, textToSpeechPropertyObj, explodeEffectPropertyObj, manipulationPropertyObj, animationPropertyObj, videoPropertyObj, labelPropertyObj, labelPropertyHandlerObject;
    public GameObject rtEditor, rte, runtimeComponent;
    public GameObject lightPrefab, lightComponent;

    public bool isBatchingEnabled, isBatchingPreviewed;
    public GameObject batchingObj;
    public RuntimeHierarchy mySpaceHierarchy;

    public Color c1 = Color.blue;
    public Color c2 = Color.white;
    public LabelWithAudioProperty labelWithAudioProperty;
    public ManipulationComponent manipulationComponent;
    public ExplodeComponent explodeComponent;
    public LabelComponent labelComponent;

    //added by saravanan (property whole set object which appears on right side for text property)
    public GameObject labelTextPropertyObj;

    [Header("<------ Model List Dependies ------>")]
    public List<ModelObjectLineItem> modelObjectLineItems = new List<ModelObjectLineItem>();
    [SerializeField]
    GameObject modelLineItemPrefab;
    [SerializeField]
    Transform modelListParentObj;
    public static ModelObjectLineItem currentSelectedModelObjectLineItem = null;


    private NewButton previousButton;
    public List<GameObject> ModelVideo = new List<GameObject>();
    public List<GameObject> ModuleVideo = new List<GameObject>();
    public Transform currentSelectedTransform;
    public Dictionary<int, GameObject> videoCollection = new Dictionary<int, GameObject>();
    public List<Animation> allAnimation = new List<Animation>();
    public List<GameObject> dsButtons = new List<GameObject>();
    public GameObject menu, menuBarBlocker, sceneLight;
    public Material defaultSkybox;
    public bool IsExistingProject = false;
    public GameObject CurrentSelectedModel;
    public TextMeshProUGUI verText;

    #region 360_CONTENT
    [Header("360 CONTENT")]
    public GameObject addNewImmersiveContent;
    #endregion

    #region PROJECT_TYPE
    [Header("PROJECT TYPE")]
    public TMP_Dropdown projectTypeDropdown;
    public GameObject featuresPanel;
    public GameObject featuresPropertiesPanel;
    public GameObject cloudSpace;
    public GameObject previewButton;
    public GameObject controlFlowUI;
    public GameObject modelHierarchyPanel;
    #endregion

    public Image showHideImage;
    public GameObject networkPopup;
    public TextMeshProUGUI labelPropertyText;
    public int videoCount = 0;
    public int labelVideoCount = 0;
    public Texture2D cursorhand;
    public float cursorX, cursorY;
    bool assetShow;
    public GraphicRaycaster mainCanvasRaycaster;
    public GameObject guideLinePopup;
    public TextMeshProUGUI guideLinesText;

    #region Unity callbacks
    private new void Awake()
    {
        userNameField.text = PlayerPrefs.GetString("emailAddress");
        passwordField.text = PlayerPrefs.GetString("password");
        verText.text = "Ver: " + Application.version.ToString();
        Screen.fullScreen = false;
        mySpaceHierarchy.mainRootName = objectCollection.name;
        mySpaceHierarchy.isMySpace = true;
        videoPanel1 = videoPanel;
    }

    private void LateUpdate()
    {
        if (raycasthitting.enable_label_flag)
        {
            Cursor.SetCursor(cursorhand, new Vector2(cursorX, cursorY), CursorMode.Auto);
        }
        else if (!raycasthitting.enable_label_flag)
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
    }

    public void GuideLines()
    {
        guideLinePopup.SetActive(true);
        Vector3 localPos = guideLinesText.GetComponent<RectTransform>().localPosition;
        localPos.y = 0.0f;
        guideLinesText.GetComponent<RectTransform>().localPosition = localPos;

        switch (projectTypeDropdown.value)
        {
            case 0:
                guideLinesText.text = "1. Model size should not exceed 50 MB." +
                    "\n\n2. Model should not have polycount of more    than 50K." +
                    "\n\n3. Model should not contain animations." +
                    "\n\n4. Model should not use skinned mesh." +
                    "\n\n5. Model should not be rigged." +
                    "\n\n6. Model should have embedded textures.";
                break;
            case 1:
                guideLinesText.text = "1. TTS on the description panel (scene level) wont play on preview." +
                            "\n\n2. Only one video scene object and one audio scene object can be added to a scene." +
                            "\n\n3. Adding too many models would make the application lag some frames." +
                            "\n\n4. Animation option cannot be added.";
                break;
            case 2:
                guideLinesText.text = $"1. <b>Projection type:</b> All 360 - video uploaded to XR Design Studio should be equirectangular. The image should resemble a 3D sphere flattened onto a 2D rectangular: think of a spherical object (the planet!), mapped onto right - angled coordinates(e.g., a giant poster in geography class)." +
                    "\n\n2. <b>Source type:</b> You can upload monoscopic to XR Design Studio. Monoscopic 360 video is filmed using one camera per field of view from one single point of view, and all videos are later stitched together to form one equirectangular video." +
                    "\n\n3. <b>Resolution:</b> We support upload and playback for videos up to 4K." +
                    "\n\n4. <b>Aspect ratio:</b> We recommend a 2:1 aspect ratio for 4K monoscopic 360 video." +
                    "\n\n5. <b>Audio format:</b> We supports standard mono and 2 - channel stereo audio, and we recommend stereo for the best playback experience.All source files containing spatial audio will be converted to 2 channel and will not adjust as the viewer moves.";
                break;
        }
    }

    public void RevertSkybox()
    {
        RenderSettings.skybox = defaultSkybox;
    }

    //video in scene level
    public void ModuleVideoStop()
    {
        for (int i = 0; i < ModuleVideo.Count; i++)
        {
            if (ModuleVideo[i] != null)
                ModuleVideo[i].GetComponent<VideoPlayer>().Stop();
        }
    }
    public void ModuleVideoDestroy(GameObject temp)
    {
        for (int i = 0; i < ModuleVideo.Count; i++)
        {
            if (ModuleVideo[i] != null)
            {
                if (temp == ModuleVideo[i])
                {
                    ModuleVideo.Remove(temp);
                    //    ModuleVideo[i].GetComponent<VideoPlayer>().Stop();
                }
            }
        }
    }

    public void AddErrorMessage(string message)
    {
        errorMessage.text = message;
    }

    public void ClearErrorMessage()
    {
        errorMessage.text = "";
    }

    public void featureselectFunc()
    {
        ColorPicker.Instance.Close();
    }

    public void AllVideoOff(GameObject videoPanel = null)
    {
        for (int i = 0; i < ModelVideo.Count; i++)
        {
            if (ModelVideo[i] != null)
            {
                if (videoPanel == null)
                {
                    ModelVideo[i].gameObject.SetActive(false);
                }
            }
        }
    }

    private void Start()
    {
        isAudio = isVideo = isSkybox = isImmersiveImage = isImmersiveVideo = false;

        RuntimeHierarchy.OnSelectionChanged += OnHierarchySelectionChanged;
        // RuntimeHierarchy.onHeirarchyChangedDelegate += hierarchyClicked;
        //  RuntimeHierarchy.onHeirarchyChangedDelegateAlone += hierarchyClickedAlone;

        RuntimeHierarchy.OnObjectDelete += OnObjectDelete;
        RuntimeHierarchy.OnActiveStatusChanged += activeStatusChanged;
        DraggedReferenceItem.Drag += Drag;
        OnCloseClicked += OnCloseContent;

        StartCoroutine(CheckNetwork());
    }

    //private void OnApplicationPause(bool pause)
    //{
    //    if (pause)
    //    {
    //        errorMessage.text = "Application is pause.";
    //    }
    //    else
    //    {
    //        errorMessage.text = "Application is quitting.";
    //    }
    //}

    private void OnApplicationQuit()
    {
        StopCoroutine(CheckNetwork());
    }

    public IEnumerator CheckNetwork()
    {
        yield return new WaitForSeconds(7.0f);
        if (Application.internetReachability.Equals(NetworkReachability.NotReachable))
        {
            networkPopup.SetActive(true);
        }
        else
        {
            networkPopup.SetActive(false);
        }
        StartCoroutine(CheckNetwork());
    }

    public HierarchyItemTransform lastCLickedHierarchyTransform;

    void activeStatusChanged(Transform item)
    {
        //Debug.LogError(item);
        if (item.GetComponent<NoTransform>())
        {

        }
        //lastCLickedHierarchyTransform = item;
        ////if (lastCLickedHierarchyTransformStringText != null)
        ////{
        ////    lastCLickedHierarchyTransformStringText.ShowHidePanel();
        ////}
        //if (lastCLickedHierarchyTransformStringText != null)
        //{
        //    lastCLickedHierarchyTransformStringText.ShowHidePanelNoRec();
        //}
    }

    void hierarchyClickedAlone(HierarchyItemTransform item)
    {
        // Debug.LogError("ss");
        //  lastCLickedHierarchyTransform = item;
    }

    public void DelayOnFunc(GameObject temp)
    {
        //LayoutRebuilder.ForceRebuildLayoutImmediate(temp.GetComponent<RectTransform>());
        //for(int i=0;i<temp.GetComponent<AnimationController>().AllObject.Count;i++)
        //{
        //    LayoutRebuilder.ForceRebuildLayoutImmediate(temp.GetComponent<AnimationController>().AllObject[i].GetComponent<RectTransform>());
        //    if(temp.GetComponent<AnimationController>().AllObject[i].allSequence.Count!=0)
        //    {
        //        LayoutRebuilder.ForceRebuildLayoutImmediate(temp.GetComponent<AnimationController>().AllObject[i].allSequence[0].GetComponent<RectTransform>());

        //    }
        //}

        StartCoroutine(delayOn(temp));
    }
    IEnumerator delayOn(GameObject temp)
    {
        temp.gameObject.SetActive(false);
        yield return null;
        temp.gameObject.SetActive(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate(temp.GetComponent<RectTransform>());


    }
    private void OnEnable()
    {
        StartCoroutine(Helper.Instance.SetRefTemp("AddManipulation", (obj) =>
        {
            manipulationComponent = obj.GetComponent<ManipulationComponent>();
        }));

        StartCoroutine(Helper.Instance.SetRefTemp("AddExplodeEffect (1)", (obj) =>
        {
            Debug.Log($"{obj.name} Drop");
            explodeComponent = obj.GetComponent<ExplodeComponent>();
        }));

        StartCoroutine(Helper.Instance.SetRefTemp("LabelComponent", (obj) =>
        {
            labelComponent = obj.GetComponent<LabelComponent>();
        }));
    }
    public void AllAnimationOff()
    {
        for (int i = 0; i < allAnimation.Count; i++)
        {
            if (allAnimation[i] != null)
                allAnimation[i].Stop();
        }
    }

    public void OnCloseContent()
    {
        isPopedUp = false;
        IsExistingProject = false;
        for (int i = 0; i < enableDisableComponent.scenePropertiesContent.transform.childCount; i++)
        {
            String s = enableDisableComponent.scenePropertiesContent.transform.GetChild(i).name;
            if (s.Contains("Description"))
            {
                Destroy(enableDisableComponent.scenePropertiesContent.transform.GetChild(i).gameObject);
            }
        }
        if (videoPanel)
        {
            videoPanel.SetActive(false);
        }

       
       
        errorMessage.text = "";
        enableDisableComponent.browserPrefab.gameObject.SetActive(false);
        enableDisableComponent.videoCount = 0;
        enableDisableComponent.audioCount = 0;
        enableDisableComponent.textToSpeechPrefab.GetComponent<SpeechText>().Silence();
        enableDisableComponent.textToSpeechPrefab.GetComponent<TextToSpeechComponent>().speakTextField.text = "";
        GameObject temp = null;
        if (scenePropertyRect != null)
        {
            temp = FindObject(scenePropertyRect.gameObject, "Browser");
            if (temp != null)
            {
                Destroy(temp);
            }
            temp = FindObject(scenePropertyRect.gameObject, "Animation");
            if (temp != null)
            {
                Destroy(temp);
            }
            temp = FindObject(scenePropertyRect.gameObject, "WalkThrough");
            if (temp != null)
            {
                Destroy(temp);
            }
        }
       
        featureScript.Animation.sprite = featureScript.AnimationSprites[0];
        enableDisableComponent.controlFlowUI.gameObject.SetActive(false);
        featureScript.WalkThrough.sprite = featureScript.WalkThroughSprites[0];

        skyboxPropertyObj.SetActive(false); audioPropertyObj.SetActive(false); textPropertyObj.SetActive(false);
        textToSpeechPropertyObj.SetActive(false); explodeEffectPropertyObj.SetActive(false); manipulationPropertyObj.SetActive(false);
        animationPropertyObj.SetActive(false); videoPropertyObj.SetActive(false); labelPropertyObj.SetActive(false);
        videoPanel = videoPanel1;
        modelvideoDestroy();
        ButtonCustomizationManager.Instance.dict.Clear();
        //  for (int i = 0; i < ScriptsManager.Instance.enableDisableComponent.AllDesccriptionPanel.Count; i++)
        //  {
        //     if (ScriptsManager.Instance.enableDisableComponent.AllDesccriptionPanel != null)
        //     {
        //         ScriptsManager.Instance.enableDisableComponent.AllDesccriptionPanel[i].RemoveComponent();
        //  Destroy(ScriptsManager.Instance.enableDisableComponent.AllDesccriptionPanel[i]);
        //    }
        // }
        for (int i = 0; i < enableDisableComponent.AllDesccriptionPanelHierarchy.Count; i++)
        {
            if (enableDisableComponent.AllDesccriptionPanelHierarchy != null)
            {

                Destroy(enableDisableComponent.AllDesccriptionPanelHierarchy[i]);
            }
        }
        // ScriptsManager.Instance.enableDisableComponent.AllDesccriptionPanel.Clear();

        enableDisableComponent.AllDesccriptionPanelHierarchy.Clear();
        videoCount = labelVideoCount = 0;
        videoCollection.Clear();
        transformComponent.SetActive(false);
       
        skuboxURL = "";
    }


    public GameObject GetVideo(int id)
    {
        GameObject temp = null;
        if (videoCollection.TryGetValue(id, out temp))
        {
            return temp;
        }
        else
        {
            return null;
        }
    }
    void modelvideoDestroy()
    {

        for (int i = 0; i < ModelVideo.Count; i++)
        {
            Destroy(ModelVideo[i].gameObject);
        }
        ModelVideo.Clear();
    }

    public static GameObject FindObject(GameObject parent, string name)
    {
        NoTransform[] trs = parent.GetComponentsInChildren<NoTransform>(true);
        NoTransform[] trn = parent.GetComponentsInChildren<NoTransform>(false);

        foreach (NoTransform t in trs)
        {
            if (t.name == name)
            {
                return t.gameObject;
            }
        }
        foreach (NoTransform t in trn)
        {
            if (t.name == name)
            {
                return t.gameObject;
            }
        }
        return null;
    }


    public GameObject lastDraggedItem;

    private void OnDestroy()
    {
        RuntimeHierarchy.OnSelectionChanged -= OnHierarchySelectionChanged;
        OnCloseClicked -= OnCloseContent;
    }

    private void Drag(Transform temp)
    {
        //if (Instance.enableDisableComponent.compLayerEvents)
        //{
        //    Instance.enableDisableComponent.compLayerEvents.GetComponent<ObjectCollect>().HittedObj = temp.GetComponent<DraggedReferenceItem>().CurrentDraggedGameObject;
        //}
        // Debug.LogError(temp.GetComponent<DraggedReferenceItem>().CurrentDraggedGameObject);
        lastDraggedItem = temp.GetComponent<DraggedReferenceItem>().CurrentDraggedGameObject;
    }

    public void OnObjectDelete(Transform selection)
    {
       
            isDeletable = true;
            confirmationPanel.confirmationPanel.SetActive(true);
            confirmationPanel.confirmationTitleText.text = "Delete GameObject?";
            confirmationPanel.submitButtonText.text = "Delete";
            confirmationPanel.confirmationTextFieldText.text = "Do you want to delete the object?";
            confirmationPanel.cancelButtonText.text = "Cancel";
            confirmationPanel.confirmationIcon.sprite = Helper.Instance.PopUpSprite("Delete");

            objecttoBeDeleted = selection;
        
        
    }
    public IEnumerator TextureDownload(string imageURL, Image img)
    {
        Texture2D tex = new Texture2D(4, 4, TextureFormat.DXT1, false);

        UnityWebRequest www = UnityWebRequestTexture.GetTexture(ScriptsManager.Instance.APIBaseURL + imageURL);
        yield return www.SendWebRequest();

        if (!www.isNetworkError || !www.isHttpError)
        {
            if (!String.IsNullOrEmpty(imageURL))
            {
                DownloadHandlerTexture handler = (DownloadHandlerTexture)www.downloadHandler;
                yield return handler.isDone;

                if (handler.isDone)
                {
                    tex = ((DownloadHandlerTexture)www.downloadHandler).texture as Texture2D;

                    Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.one / 2);
                    // if(myAssets.gameObject.activeInHierarchy)
                    returnDownloadTexture(sprite, img);
                    //    fd.buttonImage.sprite = sprite;
                }
            }
        }
        else
        {
            Debug.Log($"Error - {www.error}.");
        }
    }
    public Sprite returnDownloadTexture(Sprite temp, Image img)
    {
        if (img != null)
        {
            img.sprite = temp;
        }
        return temp;
    }

    public void ButtonAttributeChanged(NewButton button)
    {
        print(button.customButton.ID);

        try
        {
           
           
            //      print(objecttoBeDeleted.GetComponentInChildren<NewButton>().customButton.ID);
            //    print(ScriptsManager._XRStudioContent.Browse.Id);
           
        }
        catch (Exception e)
        {
            Debug.Log("Exception Test" + e);

        }



    }
    public void ButtonAttributeChangedCM(CustomButton button)
    {
        //  print(button.customButton.ID);


    }

    //public void ControlFLowRefDelete(Transform todel)
    //{
    //    for (int i = 0; i < enableDisableComponent.layerEventsParent.AllSequenceGrid.Count; i++)
    //    {
    //        for (int j = 0; j < enableDisableComponent.layerEventsParent.AllSequenceGrid[i].actions.Count; j++)
    //        {
    //            actionController ac = enableDisableComponent.layerEventsParent.AllSequenceGrid[i].actions[j].transform.GetChild(0).GetComponent<actionController>();
    //            if (ac.resultDetails != null)
    //            {
    //                if (ac.resultDetails.ObjectTransform != null)
    //                {
    //                    if (todel.name == ac.resultDetails.ObjectTransform.objectName)
    //                    {
    //                        ac.resultDetails = new AssetDetails();
    //                        ac.DeleteObjectClearAction();

    //                    }
    //                }
    //                if (todel.GetComponent<ThreeDModelFunctions>())
    //                {
    //                    if (ac.animationControlFlow.model != null)
    //                    {
    //                        if (todel.GetComponent<ThreeDModelFunctions>().objDetails.FileName == ac.animationControlFlow.model.FileName)
    //                        {
    //                            ac.animationControlFlow = new AnimationControlFlow();
    //                            ac.animationControlFlow.model = new AssetDetails();
    //                            ac.DeleteObjectClearAction();

    //                        }
    //                    }
    //                }
    //                if (ac.textContent != null)
    //                {
    //                    if (ac.textContent.transform != null)
    //                    {
    //                        if (todel.name == ac.textContent.transform.objectName)
    //                        {
    //                            ac.textContent = new TextContent();
    //                            ac.textContent.transform = new ObjectTransform();
    //                            ac.DeleteObjectClearAction();

    //                        }
    //                    }
    //                }
    //                if (ac.button != null)
    //                {
    //                    if (todel.name == ac.button.customButtonName)
    //                    {
    //                        ac.button = new CustomButtons();
    //                        ac.DeleteObjectClearAction();
    //                        //    ac.textContent.transform = new ObjectTransform();

    //                    }
    //                }
    //                if (ac.triggerbutton != null)
    //                {
    //                    if (todel.name == ac.triggerbutton.customButtonName)
    //                    {
    //                        ac.triggerbutton = new CustomButtons();
    //                        //    ac.textContent.transform = new ObjectTransform();
    //                        ac.DeleteObjectClearTrigger();

    //                    }
    //                }
    //                if (todel.GetComponent<NoTransform>())
    //                {
    //                    if (ac.video != null)
    //                    {
    //                        if (todel.GetComponent<NoTransform>().videoID == ac.video.videoID)
    //                        {
    //                            ac.video = new AddVideo();
    //                            ac.DeleteObjectClearAction();

    //                            //    ac.textContent.transform = new ObjectTransform();

    //                        }
    //                    }
    //                }


    //            }
    //        }
    //    }
    //    enableDisableComponent.modelReference.offAllPrefab();
    //}

    public void DeleteObject()
    {
        string parentName = objecttoBeDeleted.transform.root.name;
        //  ControlFLowRefDelete(objecttoBeDeleted);
        try
        {
            if (objecttoBeDeleted.GetComponentInChildren<NewButton>())
            {

               
                

            }
        }
        catch (Exception exception)
        {
            Debug.Log($"Exception: {exception}.");
        }

        if (parentName.Equals("SceneProperty") || parentName.Equals("ObjectCollection")) // Condition Added Karthick
        {
            if (objecttoBeDeleted.childCount > 0)
            {
                Transform[] childTransforms = objecttoBeDeleted.GetComponentsInChildren<Transform>();

                foreach (var child in childTransforms)
                {
                    if (currentSelectedTransform != null)
                    {
                        if (currentSelectedTransform == child)
                        {
                            transformComponent.SetActive(false);
                            transformGizmo.ManipulationControl();
                            currentObjectName.text = "";
                            break;
                        }
                        else if (currentSelectedTransform != child)
                        {
                            currentObjectName.text = currentSelectedTransform.name.ToString();
                        }
                    }
                }
            }

            if (objecttoBeDeleted.name.Equals("Add On Text Field"))
            {
               // transistionObject.GetComponent<TransistionScript>().textContentComponent.GetComponent<AddOnComponents>().OnDeactivate();
            }

            foreach (var item in bundleDetails)
            {
                if (item.gameObjectName == objecttoBeDeleted.gameObject.name)
                {
                    //objecttoBeDeleted.gameObject.GetComponent<ThreeDModelFunctions>().BroadCast_DeleteLabels(); //This line was commented by Jeffri
                    bundleDetails.Remove(item);
                    break;
                }
            }

            foreach (var item in downloadedObjects)
            {
                if (item.name == objecttoBeDeleted.gameObject.name)
                {
                    raycasthitting rayCastHitting = GetComponent<raycasthitting>();
                    Transform[] getLabels = item.GetComponentsInChildren<Transform>();
                    GameObject[] raycastHittingLabels = rayCastHitting.labels.ToArray();

                    rayCastHitting.firsthit = false;
                    int labelCount = rayCastHitting.labels.Count;

                    if (labelCount > 0) // Using this condition to skip this block of code when no labels are created. By Jeffri
                    {
                        foreach (var labels in raycastHittingLabels.Reverse<GameObject>())
                        {
                            foreach (var childLabels in getLabels)
                            {
                                if (labelCount > 0 && childLabels.name == labels.name) // Using the same condition to skip this block of code when the labels are removed completely. By Jeffri.
                                {
                                    labels.GetComponent<Label_Publish>().isRootObjectDeleted = true;
                                    labels.GetComponent<Label_Publish>().OnButtonListClose();
                                }
                            }
                        }
                    }


                    if (rayCastHitting.LineStart.Count > rayCastHitting.labels.Count)
                    {
                        Transform[] rayCastHittingLineStart = rayCastHitting.LineStart.ToArray();

                        foreach (var lineStart in rayCastHittingLineStart.Reverse<Transform>())
                        {
                            rayCastHitting.LineStart.Remove(lineStart);
                        }
                    }

                    //objecttoBeDeleted.gameObject.GetComponent<ThreeDModelFunctions>().CleanLabels(); //This line was commented by Jeffri
                    downloadedObjects.Remove(item);
                    break;
                }
            }

            if (ButtonCustomizationManager.Instance.prefabsList.Count > 0)
            {
                foreach (var item in ButtonCustomizationManager.Instance.prefabsList)
                {
                    if (objecttoBeDeleted.gameObject == item.buttonObj)
                    {
                        item.OnDeleteButtons();
                        break;
                    }
                }
            }

            if (enableDisableComponent.explodePrefab != null)
            {
                //List<ObjectExplode> tempList = new List<ObjectExplode>();
                //tempList = enableDisableComponent.explodePrefab.GetComponent<ExplodeComponent>().explodeList;
                //int count = tempList.Count;

                //for (int i = 0; i < count; i++)
                //{
                //    if (objecttoBeDeleted.gameObject.name == tempList[i].gameObject.name)
                //    {
                //        Destroy(tempList[i].gameObject);
                //        tempList.RemoveAt(i);
                //        break;
                //    }
                //}
            }

            isDeletable = false;

            if (objecttoBeDeleted != null)
            {
                if (projectTypeDropdown.value == 1)
                {
                    if (ControlFlowManagerV2.currentSelectedSceneObjLineItem != null)
                    {
                        ControlFlowManagerV2.Instance.DeleteMethodForSceneObjLineItem(objecttoBeDeleted.gameObject);
                    }
                }
                else if (projectTypeDropdown.value == 0)
                {
                    DeleteMethodForModelObjLineItem(objecttoBeDeleted.gameObject);
                }
                Destroy(objecttoBeDeleted.gameObject);
            }

        }
        else
        {

            string currentObjName = objecttoBeDeleted.gameObject.GetComponent<HierarchyItemTransform>().myName;

            List<string> deletedList = new List<string>();
            bool isParentDeleted = false;
            GameObject rootObj = objecttoBeDeleted.gameObject.GetComponent<HierarchyItemTransform>().rootTarget.gameObject;

            if (rootObj.transform.childCount == 0) // && (rootObj.transform.parent.name.Length >= 10) ? !rootObj.transform.parent.name.Remove(8, 2).Equals("Batching") : true
            {
                deletedList.Add(rootObj.transform.parent.name);
                deletedList.Add(currentObjName);
                isParentDeleted = false;
                Destroy(objecttoBeDeleted.gameObject.GetComponent<HierarchyItemTransform>().rootTarget.gameObject);
            }
            else
            {
                deletedList.Add(currentObjName);
                isParentDeleted = true;
                for (int i = 0; i < rootObj.transform.childCount; i++)
                {
                    deletedList.Add(rootObj.transform.transform.GetChild(i).gameObject.name);
                }
                int leng = objecttoBeDeleted.gameObject.GetComponent<HierarchyItemTransform>().rootTarget.parent.name.Length;
                Debug.Log(objecttoBeDeleted.gameObject.GetComponent<HierarchyItemTransform>().rootTarget.parent.name);

                Destroy(objecttoBeDeleted.gameObject.GetComponent<HierarchyItemTransform>().rootTarget.parent.gameObject);


            }
        }

    }

    //delete line item in model hierarchy panel with the gameobject model in space
    private void DeleteMethodForModelObjLineItem(GameObject ObjToDeleted)
    {
        foreach (var item in modelObjectLineItems)
        {
            if (item.ModelGameObj.Equals(ObjToDeleted))
            {
                modelObjectLineItems.Remove(item);
                item.Delete();
                break;
            }
        }
    }

  
    [SerializeField]
    List<GameObject> allScenePropertyTab;

    public void DeactivateAllComponent()
    {
        transformComponent.SetActive(false);
        lightComponent.SetActive(false);
        skyboxPropertyObj.SetActive(false);
        audioPropertyObj.SetActive(false);
        audioPropertyObj.GetComponent<AudioComponent>().playButtonText.text = "play";
        textPropertyObj.SetActive(false);
        textPropertyObj.GetComponent<TextContentComponent>().playTTSButtonText.text = "play";
        textToSpeechPropertyObj.SetActive(false);
        explodeEffectPropertyObj.SetActive(false);

        enableDisableComponent.imageComponent.SetActive(false);

        manipulationPropertyObj.SetActive(false);
        featureScript.manipulation.sprite = featureScript.manipulationSprites[0];
        featureScript.lightImg.sprite = featureScript.lightSprites[0];

        animationPropertyObj.SetActive(false);
        videoPropertyObj.SetActive(false);
        videoPropertyObj.GetComponent<VideoComponentScene>().playButtonText.text = "play";
        if (videoPropertyObj.GetComponent<VideoComponentScene>().labelPublish != null)
        {
            videoPropertyObj.GetComponent<VideoComponentScene>().labelPublish.isVideoPlaying = false;
        }
        if (videoPropertyObj.GetComponent<VideoComponentScene>().noTransform != null)
        {
            videoPropertyObj.GetComponent<VideoComponentScene>().noTransform.isVideoPlaying = false;
        }
        labelWithAudioProperty.playButtonText.text = "play";
        labelPropertyObj.SetActive(false);
        labelTextPropertyObj.SetActive(false);
        GetComponent<Label_components>().labelWithTextToSpeechProperty.GetComponent<LabelWithTextToSpeechProperty>().RemoveComponent();
        GetComponent<Label_components>().labelWithTextContentProperty.GetComponent<LabelWithTextContentProperty>().RemoveComponent();
        ControlFlowManagerV2.Instance.getSceneLineItemPropertyObjects.propertyPanel.SetActive(false);
        ControlFlowManagerV2.Instance.insForAddSceneObjs.SetActive(false);
        ColorPicker.Instance.Cancel();
        currentObjectName.text = "";
        currentObjectName.gameObject.SetActive(false);
    }

    public void OnHierarchySelectionChanged(Transform objTransform)
    {
        if (!objTransform.IsNull())
        {
            currentSelectedTransform = objTransform;

            string parentName = objTransform.transform.root.name;

            DeactivateAllComponent();
            //  enableDisableComponent.AllDescriptionPanelOff();

            // if(videoPanel)
            //  videoPanel.SetActive(false);
            if (objTransform.name.Contains("label_video"))
            {

            }
            else
            {
                if (projectTypeDropdown.value != 1)
                    AllVideoOff();

            }
            transformGizmo.target = objTransform;

            if (parentName.Equals("SceneProperty") || parentName.Equals("ObjectCollection"))
            {
                if (objTransform.name.Equals("SceneProperty") || objTransform.name.Equals("ObjectCollection") || objTransform.tag.Equals("SceneProperty"))
                {
                    transformComponent.SetActive(false);
                    transformGizmo.ManipulationControl();
                }
                else
                {
                    transformComponent.SetActive(true);

                    CurrentObjectTransform.currentObject = objTransform;
                    LocalLogs(objTransform.ToString());
                    CurrentObjectTransform currentObj = GetComponent<CurrentObjectTransform>();
                    currentObj.SetCurrentTransform();
                }

                if (transformGizmo.isMovable || transformGizmo.isRotateble || transformGizmo.isScaleable)
                {
                    transformGizmo.ClearAndAddTarget(objTransform);
                    transformGizmo.AddTargetHighlightedRenderers(objTransform);
                }
                else
                {
                    transformGizmo.ClearTargets(objTransform);
                    transformGizmo.AddTargetHighlightedRenderers(objTransform);
                }

                #region COMMENTEDPART
                //Remove label properties..
                //LabelPropertyHandler[] labelPropertyHandlers = propertyDisplayContent.GetComponentsInChildren<LabelPropertyHandler>();
                //foreach (var item in labelPropertyHandlers)
                //{
                //    DestroyImmediate(item.gameObject);
                //}

                // LabelPropertyHandler labelPropertyHandlers = propertyDisplayContent.GetComponentInChildren<LabelPropertyHandler>();
                //// foreach (var item in labelPropertyHandlers)
                //// {
                //if(labelPropertyHandlers!=null)
                //     DestroyImmediate(labelPropertyHandlers.gameObject);
                //// }

                // //Add any label properties added previouslly..

                // //List<Label_Publish> label_Publishes = GetComponentsInFirstLevelChildren<Label_Publish>(objTransform);
                // //foreach (var item in label_Publishes)
                // //{
                // //   item.InstantiateLabelProperty();
                // //}
                // Label_Publish label_Publishe = objTransform.GetComponent<Label_Publish>();
                // //if (label_Publishe != null)
                // //    label_Publishe.InstantiateLabelProperty();
                #endregion
                //for(int i=0;i<allScenePropertyTab.Count;i++)
                //{
                //    allScenePropertyTab[i].gameObject.SetActive(false);
                //}

                GetComponent<Label_components>().ClearComp();
                // st = objTransform;
                if (objTransform.name == "Skybox")
                {
                    //skyboxPropertyObj.SetActive(true);
                    skyboxPropertyObj.SetActive(false);
                    currentObjectName.gameObject.SetActive(false);
                }
                else
                {
                    featureScript.lightImg.sprite = featureScript.lightSprites[0];
                    skyboxPropertyObj.SetActive(false);
                }

                //if (objTransform.name == "Audio")
                //{
                //    audioPropertyObj.SetActive(true);
                //}
                //else
                //{
                //    audioPropertyObj.SetActive(false);
                //}

                //change code
                if (objTransform.name.Contains("Description panel("))
                {
                    #region Code by prabu
                    // if (propertyRect.Find("Property_" + currentObjectName.text))
                    //     propertyRect.Find("Property_" + currentObjectName.text).gameObject.SetActive(false);

                    //  propertyRect.Find("Property_" + objTransform.name).gameObject.SetActive(true);

                    //Transform temp = propertyRect.Find("Property_" + objTransform.name);
                    //if (temp != null)
                    //{
                    //    if (temp.GetComponent<TextContentComponent>())
                    //    {
                    //        lastCLickedHierarchyTransformStringText = temp.GetComponent<TextContentComponent>();
                    //    }
                    //} 
                    #endregion
                    textPropertyObj.SetActive(true);
                    textPropertyObj.GetComponent<TextContentComponent>().panelProperty = objTransform.GetComponent<TextContentCanvasHandler>();
                    textPropertyObj.GetComponent<TextContentComponent>().GetContentValue(objTransform.GetComponent<TextContentCanvasHandler>().textContentCurrentObjDetails);
                }
                else if (objTransform.name.Contains("Description panel label"))
                {
                    //Using this else if to skip the else condition.
                }
                else
                {
                    #region Code by prabu
                    //if (currentObjectName.text.Contains("Description panel(") && propertyRect.Find("Property_" + currentObjectName.text))
                    //    propertyRect.Find("Property_" + currentObjectName.text).gameObject.SetActive(false);
                    //  lastCLickedHierarchyTransformStringText = null;
                    #endregion
                    textPropertyObj.SetActive(false);
                    GetComponent<raycasthitting>().DisableTextPanels();
                }

                if (objTransform.name == "TextToSpeech")
                {
                    textToSpeechPropertyObj.SetActive(true);
                }
                else
                {
                    textToSpeechPropertyObj.SetActive(false);
                    textToSpeechPropertyObj.GetComponent<TextToSpeechComponent>().speechManager.Silence();
                }

                if (objTransform.name == "Explode Effect")
                {
                    explodeEffectPropertyObj.SetActive(true);
                    featureScript.explosion.sprite = featureScript.explosionSprites[1];
                }
                else
                {
                    featureScript.explosion.sprite = featureScript.explosionSprites[0];
                    explodeEffectPropertyObj.SetActive(false);
                }

                if (objTransform.name == "Gestures")
                {
                    manipulationPropertyObj.SetActive(true);
                    featureScript.manipulation.sprite = featureScript.manipulationSprites[1];
                }
                else
                {
                    manipulationPropertyObj.SetActive(false);
                }

              


              

                if (objTransform.name.Contains("Light ("))
                {
                    // ScriptsManager.Instance.featureScript.lightImg.sprite = ScriptsManager.Instance.featureScript.lightSprites[1];
                }
                else
                {
                    featureScript.lightImg.sprite = featureScript.lightSprites[0];
                }

                if (objTransform.GetComponent<ThreeDModelFunctions>())
                {
                    CurrentSelectedModel = objTransform.gameObject;
                }

                if (objTransform.GetComponent<NoTransform>())
                {
                    transformComponent.SetActive(false);
                    NoTransform noTransform = objTransform.GetComponent<NoTransform>();

                    if (noTransform.componentObject != null)
                    {
                        noTransform.componentObject.SetActive(false);
                    }

                    skyAudVid.linkHolder = noTransform.gameObject;

                    switch (noTransform.propertyType)
                    {
                        case TypesOfProperty.None:
                            break;
                        case TypesOfProperty.Audio:
                            noTransform.componentObject.SetActive(true);
                            noTransform.audioComponent.hierarchyReference = objTransform.gameObject;
                            noTransform.audioComponent.linkHolder = noTransform.linkHolder;
                            noTransform.ApplyAudioValues();
                            break;
                        case TypesOfProperty.Video:
                            videoPanel.GetComponent<VideoPlayer>().Stop();
                            videoPanel.GetComponent<VideoPlayer>().targetTexture = null;
                            videoPanel.GetComponent<MeshRenderer>().sharedMaterial.mainTexture = null;
                            // VideoPlane
                            //   videoPanel.SetActive(true);

                            // GameObject temp = GameObject.Find("VideoPlane");
                            // CurrentObjectTransform.currentObject = temp.transform;
                            //  CurrentObjectTransform currentObj1 = ScriptsManager.Instance.mainScene.GetComponent<CurrentObjectTransform>();
                            // currentObj1.SetCurrentTransform();
                            // Debug.LogError(CurrentObjectTransform.currentObject);
                            videoPanel = CurrentObjectTransform.currentObject.gameObject;
                            transformComponent.gameObject.SetActive(true);
                            //  CurrentObjectTransform.currentObject.gameObject.SetActive(true);
                            noTransform.componentObject.SetActive(true);
                            LocalLogs(CurrentObjectTransform.currentObject.ToString());
                            if (noTransform.videoComponent.noTransform != null)
                            {
                                noTransform.videoComponent.noTransform.isVideoPlaying = false;
                            }

                            if (noTransform.videoComponent.labelPublish != null)
                            {
                                noTransform.videoComponent.labelPublish.isVideoPlaying = false;
                            }

                            noTransform.videoComponent.noTransform = null;
                            noTransform.videoComponent.labelPublish = null;
                            noTransform.videoComponent.noTransform = noTransform;
                            //  noTransform.videoComponent.removeButton.SetActive(true);
                            noTransform.videoComponent.height.value = noTransform.videoHeight;
                            noTransform.videoComponent.dropdown.value = noTransform.aspectRatioValue;
                            noTransform.videoComponent.hierarchyReference = objTransform.gameObject;
                            noTransform.videoComponent.videoType = ComponentType.Scene;
                            noTransform.videoComponent.linkHolder = noTransform.linkHolder;
                            noTransform.videoComponent.localURL = noTransform.serverURL;
                            noTransform.videoComponent.inputVideoClipPath.text = noTransform.serverURL;
                            //  videoPanel.SetActive(true);
                            noTransform.ApplyVideoValues();
                            break;
                        case TypesOfProperty.Text:
                            break;
                        case TypesOfProperty.TTS:
                            break;
                        case TypesOfProperty.Skybox:
                            break;
                        case TypesOfProperty.Explode:
                            break;
                        case TypesOfProperty.Manipulation:
                            break;
                        case TypesOfProperty.Light:
                            transformComponent.SetActive(true);
                            break;
                        case TypesOfProperty.Labels:
                            labelPropertyObj.SetActive(true);
                            break;
                        case TypesOfProperty.Image:
                            noTransform.imageComponent.gameObject.SetActive(true);
                            break;
                    }

                    if (projectTypeDropdown.value.Equals(0))
                    {
                        objTransform.gameObject.SetActive(true);
                    }
                }

                

                if (objTransform.CompareTag("videoPlane"))
                {
                    transformComponent.SetActive(true);
                }

                //from and for control flow  (by saravanan)
                #region multi scenes operation functionality 
                if (projectTypeDropdown.value == 1)
                {
                    //button scene line item
                    if (objTransform.GetComponentInChildren<ControlFlowButton>())
                    {
                        for (int i = 0; i < ControlFlowManagerV2.Instance.sceneLineItems.Count; i++)
                        {
                            if (ControlFlowManagerV2.Instance.sceneLineItems[i].GameObjInSpace.Equals(objTransform.gameObject))
                            {
                                ControlFlowManagerV2.Instance.sceneLineItems[i].Select();
                            }

                        }
                    }
                    //text panel scene object line item
                    if (objTransform.GetComponent<TextContentCanvasHandler>())
                    {
                        if (objTransform.GetComponent<TextContentCanvasHandler>().isSceneTextpanel)
                        {
                            if (ControlFlowManagerV2.currentSelectedSceneObjLineItem != null)
                                ControlFlowManagerV2.currentSelectedSceneObjLineItem.DeSelect();

                            ControlFlowManagerV2.currentSelectedSceneObjLineItem = objTransform.GetComponent<TextContentCanvasHandler>().thisSceneObjLineItem;
                            //print(objTransform.GetComponent<TextContentCanvasHandler>().thisSceneObjLineItem);
                            ControlFlowManagerV2.currentSelectedSceneObjLineItem.Select();
                        }

                      
                    }
                    //video panel scene object line item
                    if (objTransform.GetComponent<NoTransform>())
                    {
                        if (ControlFlowManagerV2.currentSelectedSceneObjLineItem != null)
                            ControlFlowManagerV2.currentSelectedSceneObjLineItem.DeSelect();

                        ControlFlowManagerV2.currentSelectedSceneObjLineItem = objTransform.GetComponent<NoTransform>().thisSceneObjLineItem;
                        //print(objTransform.GetComponent<TextContentCanvasHandler>().thisSceneObjLineItem);
                        if (ControlFlowManagerV2.currentSelectedSceneObjLineItem != null)
                            ControlFlowManagerV2.currentSelectedSceneObjLineItem.Select();
                    }
                  
                }
                #endregion

                currentObjectName.gameObject.SetActive(false);
                if (!objTransform.name.Equals("Gestures"))
                {
                    currentObjectName.gameObject.SetActive(true);
                    currentObjectName.text = objTransform.name;
                }
            }

            else
            {
                // Debug.Log("SD");
                if (!objTransform.name.Equals("BatchingCollection"))
                {
                    //  Debug.Log("SD");

                    GameObject rootBatching = null;
                    GameObject parentBatching = null;
               
                    List<string> childList = new List<string>();

                   // List<Patch> patchList = new List<Patch>();


                    if (objTransform.name.Length > 9 && objTransform.name.Remove(8, 2).Equals("Batching"))
                    {
                        //Selected BatchingGroup List

                        if (objTransform.childCount > 0)
                        {
                           
                        }
                        else
                        {
                           
                        }


                    }
                   
                    //Convert As GameObjects
                    GameObject parent = null;
                    List<GameObject> childObList = new List<GameObject>();

                  

                    objectCollection.BroadcastMessage("BroadCastMessages");

                  

                }
            }
        }
        else
        {
            runtimeHierarchy.Deselect();
            currentObjectName.text = "";
            currentObjectName.gameObject.SetActive(false);
            AllVideoOff();
        }
    }

    public void AddNewAsset()
    {
        Application.OpenURL(APIBaseURL + "/asset/upload/" + userDetails.clientId);
    }

    #region helper
    public List<T> GetComponentsInFirstLevelChildren<T>(Transform trans)
    {
        List<T> n = new List<T>();
        for (int i = 0; i < trans.childCount; i++)
        {
            if (trans.GetChild(i).GetComponent<T>() != null)
                n.Add(trans.GetChild(i).GetComponent<T>());
        }
        return n;
    }
    //public static string GetGameObjectPath(GameObject obj)
    //{
    //    string path = "/" + obj.name;
    //    while (obj.transform.parent != null)
    //    {
    //        obj = obj.transform.parent.gameObject;
    //        path = "/" + obj.name + path;
    //    }
    //    return path;
    //}

    public string GetObjectFullPath(GameObject obj, string path = "")
    {
        if (path == "")
            path = obj.name;
        if (obj.transform.parent != null)
        {
            path = obj.transform.parent.name + "/" + path;
            path = GetObjectFullPath(obj.transform.parent.gameObject, path);
        }
        return path;
    }

    public string GetObjectFullPathWithoutLabelname(GameObject obj, string path = "")
    {
        if (path == "")
            path = obj.name;
        if (obj.transform.parent != null)
        {
            path = obj.transform.parent.name;// + "/" + path;
            path = GetObjectFullPath(obj.transform.parent.gameObject, path);
        }
        return path;
    }
    #endregion

    public void RegisterSelectControls()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var clickLocation = Input.mousePosition;

            Transform closestBone = null;

            this.MinimumRecursiveBoneDistance(clickLocation, objectCollection.transform, out closestBone);

            if (null != closestBone)
            {
                Debug.Log(closestBone.gameObject.name);
            }
        }
    }

    public float MinimumRecursiveBoneDistance(Vector3 clickLocation, Transform root, out Transform closestBone)
    {
        float minimumDistance = float.MaxValue;
        closestBone = null;
        foreach (Transform bone in root)
        {
            var boneScreenPoint = Camera.main.WorldToScreenPoint(bone.position);
            var distance = Math.Abs(Vector3.Distance(clickLocation, boneScreenPoint));

            if (distance < minimumDistance)
            {
                minimumDistance = distance;
                closestBone = bone;
            }

            if (bone.childCount > 0)
            {
                Transform closestChildBone;
                var minimumChildDistance = this.MinimumRecursiveBoneDistance(clickLocation, bone, out closestChildBone);
                if (minimumChildDistance < minimumDistance)
                {
                    minimumDistance = minimumChildDistance;
                    closestBone = closestChildBone;
                }
            }

        }
        return minimumDistance;
    }
   
    public ThreeDModelFunctions modelObj(GameObject temp)
    {
        if (temp.GetComponent<ThreeDModelFunctions>())
        {
            return temp.GetComponent<ThreeDModelFunctions>();
        }
        ThreeDModelFunctions s = temp.GetComponentInChildren<ThreeDModelFunctions>();
        return s;
    }
    public void animationPlayButton(string temp, int start, int end, NewButton but)
    {
        ScriptsManager.Instance.LocalLogs(temp);
        ScriptsManager.Instance.LocalLogs("ssss");

        if (start > end)
        {
            return;
        }

        GameObject animationObj = GameObject.Find(temp);

        if (animationObj != null)
        {

            Animation animationTest = animationObj.transform.GetComponent<ThreeDModelFunctions>().AnimationComp;
            //Debug.LogError(animationTest.IsPlaying("a"));
            if (previousButton == but)
            {
                if (animationTest.IsPlaying("a"))
                {
                    if (but != null)
                    {
                        //   but.buttonText.text = "Play";
                        but.buttonText.text = but.normalName;
                        but.buttonImage.color = but.normalColor;

                    }
                    //  animationTest["a"].time = 0;
                    //   Debug.LogError("stoped");
                    animationTest.Stop();

                    AnimationClip s = animationTest.clip;
                    animationTest.AddClip(s, "a", (int)0, (int)0);
                    AnimationClip sa = animationTest.GetClip("a");
                    sa.frameRate = 30;

                    //   animationTest.clip = animClip;
                    ///  animationTest["a"].time = 0;
                    animationTest["a"].speed = 1.0f;
                    StartCoroutine(delayPlay(animationTest, sa.length, but));

                    //  animationTest.Play
                    return;
                }
            }
            animationTest.Stop(start.ToString());
            AnimationClip originalClip = animationTest.clip;

            animationTest.AddClip(originalClip, "a", (int)start, (int)end);

            AnimationClip animClip = animationTest.GetClip("a");

            animClip.frameRate = 30;

            //   animationTest.clip = animClip;

            animationTest["a"].speed = 1.0f;
            //  animationTest.Play();
            //  animationTest["a"].time = 1.0f;
            //animationTest.Play("a");
            StartCoroutine(delayPlay(animationTest, animClip.length, but));
            previousButton = but;
            //animationObj.GetComponent<Animation>().Play();

        }
    }
    IEnumerator delayPlay(Animation temp, float temp1, NewButton but)
    {
        temp.Play();
        yield return null;
        temp.Play("a");
        if (but != null)
        {
            // but.buttonText.text = "Stop";
            but.buttonText.text = but.alternateName;
            but.buttonImage.color = but.highlightColor;


        }
        yield return new WaitForSeconds(temp1);
        if (temp)
        {
            temp.Stop();
            yield return null;
            temp["a"].time = 0;
            if (but != null)
            {
                // but.buttonText.text = "Play";
                but.buttonText.text = but.normalName;
                but.buttonImage.color = but.normalColor;


            }
        }
    }
    public MyAssets myAssets;
    public void showAssetFunc(Image temp)
    {
        if (temp != null)
            temp.sprite = ResourceManager.Instance.radialSelected;
        if (assetShow == false)
        {

            myAssets.specialTab.gameObject.SetActive(true);
            myAssets.LocalRefresh.gameObject.SetActive(false);

            StartCoroutine(showAsset());
        }
    }


    public void TextureDownloadInit(string imageURL, Image img)
    {
        StartCoroutine(TextureDownload(imageURL, img));
    }
    public void RoutineStop()
    {
        //this.StopAllCoroutines();
        isAssets = false;
        myAssets.gameObject.SetActive(false);
    }
    public void showLibraryAsset(Image temp)
    {
        if (assetShow == false)
        {
            myAssets.specialTab.gameObject.SetActive(false);
            myAssets.LocalRefresh.gameObject.SetActive(true);
            libraryData.Clear();


            StartCoroutine(ShowLocalAsset());

            temp.sprite = ResourceManager.Instance.radialSelected;
        }
        temp.sprite = ResourceManager.Instance.radialSelected;

        //    Debug.LogError(libraryData.Count);

    }


    IEnumerator ShowLocalAsset()
    {
        assetShow = true;
        myAssets.refresh();
        yield return StartCoroutine(LibraryAssetURLs((int)AssetTypes.Models));
        if (libraryData.Count > 0)
        {
            int j = libraryData.Count;
            //int count = 0;

            for (int p = 0; p < j; p++)
            {
                if (libraryData[p].AssetTypeId == (int)AssetTypes.Models && libraryData[p].InQueue == false)
                {
                    //  count++;
                    string fileName = libraryData[p].FileName.Split('.')[0];

                 
                    AssetDetails assetDetails = libraryData[p];

                    myAssets.myAssetPrefabInst(assetDetails, fileName);

                }
            }
        }
        assetShow = false;


    }
    IEnumerator showAsset()
    {
        assetShow = true;

        myAssets.refresh();

        WWWForm form = new WWWForm();
        form.AddField("clientId", userDetails.clientId);

        UnityWebRequest www = UnityWebRequest.Post(APIBaseURL + "api/GetMyAssets?clientid=" + userDetails.clientId, form);
        Debug.Log($"Show Asset : {www.url}");
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            string dataAsJson = www.downloadHandler.text;
            DownloadAsset assetFromJson = JsonUtility.FromJson<DownloadAsset>(dataAsJson);
            downloadAsset = assetFromJson;
            for (int p = 0; p < downloadAsset.Data.Count; p++)
            {
                int j = p;
                string fileName = downloadAsset.Data[p].FileName.Split('.')[0];

                myAssets.myAssetPrefabInst(downloadAsset.Data[p], fileName);

            }

            Debug.Log("Form upload complete! " + dataAsJson);
        }
        assetShow = false;

    }
    private IEnumerator PostRequest()
    {
        WWWForm form = new WWWForm();
        form.AddField("clientId", userDetails.clientId);

        UnityWebRequest www = UnityWebRequest.Post(APIBaseURL + "api/GetMyAssets?clientid=" + userDetails.clientId, form);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            string dataAsJson = www.downloadHandler.text;
            DownloadAsset assetFromJson = JsonUtility.FromJson<DownloadAsset>(dataAsJson);

            downloadAsset = assetFromJson;

            if (downloadAsset.Data.Count <= 999)
            {
                cloudAssetCount.text = "(" + downloadAsset.Data.Count.ToString() + ")";
            }
            else
            {
                cloudAssetCount.text = "(999+)";
            }

            for (int p = 0; p < downloadAsset.Data.Count; p++)
            {
                int j = p;
                string fileName = downloadAsset.Data[p].FileName.Split('.')[0];

                if (!downloadAsset.Data[p].InQueue)
                {
                    GameObject assetButton = Instantiate(myAssetButton, inQueueContentRect);
                    assetButton.name = assetButton.GetComponentInChildren<TextMeshProUGUI>().text = fileName;

                    assetButton.GetComponentInChildren<Button>().onClick.AddListener(() =>
                    {
                        DownloadFiles(downloadAsset.Data[j], fileName, assetButton);
                    });
                }
                else
                {
                    GameObject assetButton = Instantiate(myAssetButton, inQueueContentRect);
                    assetButton.name = assetButton.GetComponentInChildren<TextMeshProUGUI>().text = fileName;

                    assetButton.GetComponent<Image>().color = assetButton.GetComponent<AssetButton>().disabledColor;
                    assetButton.GetComponent<AssetButton>().button.interactable = false;
                    assetButton.GetComponent<AssetButton>().syncObject.SetActive(true);
                    assetButton.GetComponent<AssetButton>().arrowCircle.Rotate(0f, 0f, 90f);
                }
            }

            Debug.Log("Form upload complete! " + dataAsJson);
        }
    }

    public IEnumerator CloudAssetURLs(int assetID)
    {
        WWWForm form = new WWWForm();
        form.AddField("clientId", userDetails.clientId);
        form.AddField("AssetTypeId", assetID);

        UnityWebRequest www = UnityWebRequest.Post(APIBaseURL + "api/GetMyAssets?clientid=" + userDetails.clientId + "&AssetTypeId=" + assetID, form);
       
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            string dataAsJson = www.downloadHandler.text;

            DownloadCloudAssets assetFromJson = JsonUtility.FromJson<DownloadCloudAssets>(dataAsJson);
            if (assetFromJson.Data.Count != 0)
            {
                for (int j = 0; j < assetFromJson.Data.Count; j++)
                {
                    cloudData.Add(assetFromJson.Data[j]);
                }
            }
        }
    }

    public IEnumerator LibraryAssetURLs(int assetID)
    {
        WWWForm form = new WWWForm();
        form.AddField("AssetTypeId", assetID);

        UnityWebRequest www = UnityWebRequest.Post(APIBaseURL + "api/GetPublicAssets?AssetTypeId=" + assetID, form);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            string dataAsJson = www.downloadHandler.text;

            DownloadLibraryAssets assetFromJson = JsonUtility.FromJson<DownloadLibraryAssets>(dataAsJson);

            if (assetFromJson.Data.Count != 0)
            {
                for (int j = 0; j < assetFromJson.Data.Count; j++)
                {
                    libraryData.Add(assetFromJson.Data[j]);
                }
            }

            if (assetID == (int)AssetTypes.Models)
            {
                RefreshLibraryData();
            }
        }
    }

   

    private void RefreshLibraryData()
    {
        if (libraryData.Count > 0)
        {
            int j = libraryData.Count;
            int count = 0;

            for (int p = 0; p < j; p++)
            {
                if (libraryData[p].AssetTypeId == (int)AssetTypes.Models)
                {
                    count++;
                    string fileName = libraryData[p].FileName.Split('.')[0];

                    GameObject libraryButton = Instantiate(myAssetButton, libraryContentRect);
                    libraryButton.name = libraryButton.GetComponentInChildren<TextMeshProUGUI>().text = fileName;

                    AssetDetails assetDetails = libraryData[p];

                    libraryButton.GetComponentInChildren<Button>().onClick.AddListener(() =>
                     {
                         DownloadFiles(assetDetails, fileName, libraryButton);
                     });

                    libraryAssetCount.text = "(" + count.ToString() + ")";
                }
            }
        }
    }

    public string ExtensionRemover(string link)
    {
        string contentName = link.Split('/')[link.Split('/').Length - 1].Split('.')[0];
        return contentName;
    }

    public void LocalLogs(string s)
    {
        Debug.Log($"<color=green>{s}</color>");
        //  Debug.Log("<color=blue>" + s + "</color>");
    }

    public IEnumerator GetAssetBundle(AssetDetails assetDetails, string assetName = null, GameObject assetButton = null, Action<GameObject> objDownloaded = null)
    {
        assetName = ExtensionRemover(assetName);
        Debug.Log($"Asset name: <color=green>{assetName}</color>");
        string url = APIBaseURL + assetDetails.WindowsURL;
        UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(url);
        DownloadFromServer.Instance.DownloadingProgress(www);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(www);

            string[] names = bundle.GetAllAssetNames();

            foreach (var name in names)
            {
                Debug.Log($"Bundle: {name}");
            }

            if (bundle.LoadAsset(assetName) != null)
            {
                errorMessage.text = "";

                GameObject emptyObject = new GameObject();
                emptyObject.transform.SetParent(objectCollection.transform);

                GameObject newObject = Instantiate(bundle.LoadAsset(assetName)) as GameObject;
                emptyObject.name = assetName + (AssetCount + 1).ToString();
                newObject.name = assetName;
                if (newObject.GetComponent<Animation>())
                {
                    Destroy(newObject.GetComponent<Animation>());
                }
                else if (newObject.GetComponent<Animator>())
                {
                    Destroy(newObject.GetComponent<Animator>());
                }

                Renderer[] rends = newObject.GetComponentsInChildren<Renderer>();
                Bounds bounds = rends[0].bounds;

                foreach (Renderer rend in rends)
                {
                    if (rend.enabled)
                    {
                        bounds = bounds.GrowBounds(rend.bounds);
                    }
                }

                Vector3 center = bounds.center;

                emptyObject.transform.position = newObject.transform.position = center;
                newObject.transform.parent = emptyObject.transform;
                emptyObject.transform.position = Vector3.zero;

                CurrentObjectTransform.currentObject = emptyObject.transform;
                CurrentObjectTransform currentObj = GetComponent<CurrentObjectTransform>();
                currentObj.SetCurrentTransform();

                runtimeHierarchy.Refresh();
                runtimeHierarchy.Select(emptyObject.transform);

                bundle.Unload(false);
                AssetCount++;

                if (newObject.GetComponentInChildren<Camera>())
                {
                    newObject.GetComponentInChildren<Camera>().enabled = false;
                }

                if (!emptyObject.GetComponent<ThreeDModelFunctions>())
                {
                    emptyObject.AddComponent<ThreeDModelFunctions>();
                }

                ThreeDModelFunctions threeDFunctions = emptyObject.GetComponent<ThreeDModelFunctions>();
                threeDFunctions.objDetails = assetDetails;

                if (isExplodable)
                {
                    if (emptyObject.GetComponentInChildren<Animation>().GetClipCount() == 0)
                    {
                        enableDisableComponent.PopulateCollectionChildren(emptyObject.transform);
                    }
                }

                if (emptyObject.GetComponent<ThreeDModelFunctions>().HasAnimation)
                {
                    if (emptyObject.GetComponent<ThreeDModelFunctions>().AnimationComp.GetClipCount() > 0)
                    {
                        errorMessage.text = "Labels cannot be added to some animated models.";
                        enableDisableComponent.objHasAnimation.Add(emptyObject);
                    }
                    else if (emptyObject.GetComponent<Animation>().GetClipCount() == 0) // Using this to remove unnecessary Animation component in the model with no animations. Jeffri.
                    {
                        Destroy(emptyObject.GetComponent<Animation>());
                    }
                }
               

                bundleCount++;

                yield return new WaitForSeconds(0.5f);

                string path = GetObjectFullPath(emptyObject.gameObject);

                explodeEffect.addcolliderComp(emptyObject);

                bundleDetails.Add(new BundleDetails { assetId = assetDetails.AssetId, bundleWindowsURL = assetDetails.WindowsURL, bundleAndroidURL = assetDetails.AndroidURL, bundleIOSURL = assetDetails.IOSURL, clientId = userDetails.clientId, fileName = assetDetails.FileName, gameObjectName = emptyObject.name, gameObjectFullPath = path, assetDetails = assetDetails });

                downloadedObjects.Add(emptyObject);

               

                if (projectTypeDropdown.value.Equals(1))
                {
                    //pushing the content type obj to control flow scene objects list
                    ControlFlowManagerV2.Instance.CreateSeneObjectLineItem(emptyObject, ActionStates.Model);
                }
                else if (projectTypeDropdown.value.Equals(0))
                {
                    CreateModelObjectLineItem(emptyObject);
                }

                if (objDownloaded != null)
                {
                    objDownloaded(emptyObject);
                }
            }
            else
            {
                errorMessage.text = "Error parsing the model.";
            }
        }
        yield return null;
    }



    public void CreateModelObjectLineItem(GameObject emptyObject)
    {
        ModelObjectLineItem item = new ModelObjectLineItem(emptyObject, Instantiate(modelLineItemPrefab, modelListParentObj));
        modelObjectLineItems.Add(item);

    }



    public static string GetGameObjectPath(GameObject obj)
    {
        string path = "/" + obj.name;
        while (obj.transform.parent != null)
        {
            obj = obj.transform.parent.gameObject;
            path = "/" + obj.name + path;
        }
        return path;
    }

    public void DownloadFiles(AssetDetails assetDetails, string assetName, GameObject assetButton = null)
    {
        StartCoroutine(GetAssetBundle(assetDetails, assetName, assetButton));
    }
    #endregion

    #region User Interface
    public void LoginButton()
    {
        if (LoginPage.activeSelf)
        {
            if (userNameField.text == "" && passwordField.text == "")
            {
                loginErrorText.text = "Enter email and password";
            }
            else if (userNameField.text == "")
            {
                loginErrorText.text = "Enter email";
            }
            else if (passwordField.text == "")
            {
                loginErrorText.text = "Enter password";
            }
            else
            {
                loginErrorText.text = "";

                StartCoroutine(CheckLogIn());
            }
        }
    }

    private IEnumerator CheckLogIn()
    {
        WWWForm form = new WWWForm();
        form.AddField("EmailId", userNameField.text);
        form.AddField("Password", passwordField.text);

        UnityWebRequest www = UnityWebRequest.Post(APIBaseURL + "api/Client/Login", form);

        byte[] formData = form.data;
        string data = "";
        foreach (var item in formData)
        {
            data = data + item.ToString();
            //Debug.Log($"Login: {APIBaseURL}api/Client/Login{data}");
        }

        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
            yield break;
        }
        yield return www.isDone;
        string dataAsJson = www.downloadHandler.text;
        LoginResponseData content = JsonUtility.FromJson<LoginResponseData>(dataAsJson);

        if (content.ResponseCode == 1)
        {
            PlayerPrefs.SetString("emailAddress", userNameField.text);
            PlayerPrefs.SetString("password", passwordField.text);

            userDetails = content.Data;
            LoginPage.SetActive(false);
            projectSelector.SetActive(true);

            libraryData.Clear();
            cloudData.Clear();

            StartCoroutine(PostRequest());

            StartCoroutine(CloudAssetURLs((int)AssetTypes.Models));
            StartCoroutine(LibraryAssetURLs((int)AssetTypes.Models));

            StartCoroutine(CloudAssetURLs((int)AssetTypes.SkyBox));
            StartCoroutine(LibraryAssetURLs((int)AssetTypes.SkyBox));

            StartCoroutine(CloudAssetURLs((int)AssetTypes.Video));
            StartCoroutine(LibraryAssetURLs((int)AssetTypes.Video));

            StartCoroutine(CloudAssetURLs((int)AssetTypes.Audio));
            StartCoroutine(LibraryAssetURLs((int)AssetTypes.Audio));

            StartCoroutine(CloudAssetURLs((int)AssetTypes.ImmersiveVideo));
            StartCoroutine(LibraryAssetURLs((int)AssetTypes.ImmersiveVideo));

            StartCoroutine(CloudAssetURLs((int)AssetTypes.ImmersiveImage));
            StartCoroutine(LibraryAssetURLs((int)AssetTypes.ImmersiveImage));

            DownloadFromServer.Instance.GetCloudImages();
            DownloadFromServer.Instance.GetLibraryImages();
        }
        else
        {
            loginErrorText.text = content.Message;
        }
    }

    public void RefreshAssetList()
    {
        for (var i = myAssetContentRect.childCount; i-- > 0;)
        {
            DestroyImmediate(myAssetContentRect.GetChild(0).gameObject);
        }
        for (var i = inQueueContentRect.childCount; i-- > 0;)
        {
            DestroyImmediate(inQueueContentRect.GetChild(0).gameObject);
        }

        StartCoroutine(PostRequest());
    }

    public void CloseProject()
    {
        projectTypeDropdown.value = 0;
        RevertSkybox();
        skyboxPropertyObj.SetActive(false);
        skuboxURL = "";
        IsExistingProject = false;
        cameraController.enabled = false;
        clearError.SetActive(false);
        objectCollection.SetActive(true);
        //cameraController.ResetView();
       
        Speaker.Silence();
        audioManager.Stop();
        AssetCount = 0;
        UndoRedoManager.Clear();
        if (videoPanel)
            videoPanel.transform.SetParent(null);
        skyAudVid.linkHolder = null;
        ColorPicker.Instance.Close();
       

        showHideImage.sprite = ResourceManager.Instance.hidePass;
        passwordField.contentType = TMP_InputField.ContentType.Password;
        passwordField.ForceLabelUpdate();

        #region Contrrl flow Version 1
        //if (enableDisableComponent.compLayerEvents != null)
        //{
        //    enableDisableComponent.layerEventsParent.layerParent.LayerNumber = 1;
        //    enableDisableComponent.layerEventsParent.actionNumber = 1;

        //    for (int i = 0; i < enableDisableComponent.compLayerEvents.GetComponent<LayerEventsParent>().AllSequenceGrid.Count; i++)
        //    {
        //        Destroy(enableDisableComponent.compLayerEvents.GetComponent<LayerEventsParent>().AllSequenceGrid[i].gameObject);
        //    }
        //    for (int i = 0; i < enableDisableComponent.compLayerEvents.GetComponent<LayerEventsParent>().layerParent.totalLayer.Count; i++)
        //    {
        //        Destroy(enableDisableComponent.compLayerEvents.GetComponent<LayerEventsParent>().layerParent.totalLayer[i].gameObject);
        //    }
        //    enableDisableComponent.compLayerEvents.GetComponent<LayerEventsParent>().layerParent.totalLayer.Clear();
        //    enableDisableComponent.compLayerEvents.GetComponent<LayerEventsParent>().AllSequenceGrid.Clear();
        //    RectTransform tempRect = enableDisableComponent.layerEventsParent.AllVerticalGrid[0];
        //    enableDisableComponent.layerEventsParent.AllVerticalGrid.Clear();
        //    enableDisableComponent.layerEventsParent.AllVerticalGrid.Add(tempRect);
        //} 
        #endregion

        projectSelector.SetActive(true);
        rtEditor.SetActive(false);
        if (rte != null)
        {
            rte.SetActive(false);
            runtimeComponent.SetActive(false);
        }

        if (OnCloseClicked != null)
            OnCloseClicked();

        bundleDetails.Clear();


        for (int o = modelObjectLineItems.Count - 1; o > -1; o--)
        {
            modelObjectLineItems[o].Delete();
        }

        modelObjectLineItems.Clear();

        currentSelectedModelObjectLineItem = null;

        foreach (var obj in downloadedObjects)
        {
            if (obj != null)
                Destroy(obj);
        }

        downloadedObjects.Clear();
        RemoveDSButtons();
        newProject.SetActive(false);
        transformComponent.SetActive(false);
        panelCount = 0;
        bundleCount = 0;
        raycasthitting.enable_label_flag = false;
        featureScript.labels.sprite = featureScript.labelSprites[0];
        gameObject.GetComponent<raycasthitting>().firsthit = false;
        PopupManager.uniqueButtonID = 0;
        AssetCount = 0;

        ControlFlowManagerV2.Instance.ObjListOutOfObjectCollection.Clear();
        mainScene.SetActive(false);
        manipulationComponent.UnSubscribeToDelegate();
        sceneLight.SetActive(true);
        cameraController.resetView.transform.parent.gameObject.SetActive(false);

        GC.Collect();
        Caching.ClearCache();
    }

    public void SignOut()
    {
        for (var i = myAssetContentRect.childCount; i-- > 0;)
        {
            DestroyImmediate(myAssetContentRect.GetChild(0).gameObject);
        }
        for (var i = inQueueContentRect.childCount; i-- > 0;)
        {
            DestroyImmediate(inQueueContentRect.GetChild(0).gameObject);
        }

        CloseProject();
        downloadAsset = null;
        userDetails = new ClientDetails();
        projectSelector.SetActive(false);
        newProject.SetActive(false);
        mainScene.SetActive(false);
        //sceneViewNavigationObject.SetActive(false);
        LoginPage.SetActive(true);
    }

    public void NewProject()
    {
        projectSelector.SetActive(false);
        newProject.SetActive(true);
        errorMessage.text = "";
        errorMessage.color = Color.white;
        projectNameField.text = "";
        projectNameField.Select();
        projectNameErrorText.text = "";
        skyboxPropertyObj.SetActive(false);
        ScriptsManager.Instance.skuboxURL = "";
        IsExistingProject = false;
    }

    public void NewProjectCancel()
    {
        projectSelector.SetActive(true);
        newProject.SetActive(false);
        errorMessage.text = "";
        errorMessage.color = Color.white;
    }

    public void SaveNewProject()
    {
        if (newProject.activeSelf)
        {
            if (projectNameField.text.Length == 0)
            {
                projectNameErrorText.text = "Project name should not be empty";
            }
            else
            {
                StartCoroutine(APICallToCheckProjectName());
            }
        }
    }

    private IEnumerator APICallToCheckProjectName()
    {
        WWWForm form = new WWWForm();

        UnityWebRequest www = UnityWebRequest.Post(APIBaseURL + "api/CheckProjectNameIsExists?clientId=" + userDetails.clientId + "&newProjectName=" + projectNameField.text, form);
        Debug.Log(www.url);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
            yield break;
        }

        yield return www.isDone;
        string dataAsJson = www.downloadHandler.text;

        ExistingProject content = JsonUtility.FromJson<ExistingProject>(dataAsJson);

        Debug.Log($"New Project {Convert.ToBoolean(content.Data)} - {content.ToString()} - {dataAsJson}.");

        if (content.ResponseCode == 1)
        {
            if (Convert.ToBoolean(content.Data))
            {
                confirmationPanel.mainSceneBool = true;
                confirmationPanel.confirmationTitleText.text = "Existing Project";
                confirmationPanel.confirmationTextFieldText.text = "Would you like to over-write your existing project?";
                confirmationPanel.submitButtonText.text = "Okay";
                confirmationPanel.cancelButtonText.text = "Cancel";
                confirmationPanel.confirmationIcon.sprite = Helper.Instance.PopUpSprite("ExistingProject");

                confirmationPanel.OpenConfirmationPanel();
            }
            else
            {
                OpenScene();
            }
        }
        else
        {
            errorMessage.text = content.Message;
            errorMessage.fontStyle = FontStyles.UpperCase;
            errorMessage.color = Color.red;
        }
    }

    public void OpenCloseFileMenu()
    {
        if (!menu.activeSelf)
        {
            menu.SetActive(true);
            menuBarBlocker.SetActive(true);
        }
        else { menu.SetActive(false); menuBarBlocker.SetActive(false); }
    }

    private void OpenMainScene()
    {

        cloudSpace.SetActive(true);
        mainScene.SetActive(true);
        featuresPanel.SetActive(true);
        featuresPropertiesPanel.SetActive(true);

        modelHierarchyPanel.SetActive(true);

        rtEditor.SetActive(true);
        if (rte != null)
        {
            rte.SetActive(true);
            runtimeComponent.SetActive(true);
        }
        mainCamera.GetComponent<TransformGizmo>().enabled = true;
        objectCollection.SetActive(true);
        Debug.Log("objectCollectionobjectCollectionobjectCollection " + objectCollection.activeSelf);
    }
    /// <summary>
    /// 
    /// </summary>
    private void OpenControlFlowScene()
    {
        ControlFlowManagerV2.Instance.EnableCameraFrustum(true);
        cloudSpace.SetActive(true);
        mainScene.SetActive(true);
        rtEditor.SetActive(true);
        if (rte != null)
        {
            rte.SetActive(true);
            runtimeComponent.SetActive(true);
        }
        featuresPropertiesPanel.SetActive(true);
        previewButton.SetActive(true);
        controlFlowUI.SetActive(true);
        mainCamera.GetComponent<TransformGizmo>().enabled = true;
        objectCollection.SetActive(true);
        ControlFlowManagerV2.Instance.SubscribeToDelegate();
    }

   
    /// <summary>
    /// opens up the working scene editor for the project right after creating project or chossing the existing project
    /// </summary>
    public void OpenScene()
    {
        UndoRedoManager.Clear();
        RevertSkybox();
        isPopedUp = false;
        clearError.SetActive(true);
        addNewImmersiveContent.SetActive(false);
        newProject.SetActive(false);
        errorMessage.text = "";
        controlFlowUI.SetActive(false);
        modelHierarchyPanel.SetActive(false);

        errorMessage.color = Color.white;
        cameraController.enabled = false;
     //   cameraController.ResetView();
        mainScene.SetActive(false);
        featuresPanel.SetActive(false);
        featuresPropertiesPanel.SetActive(false);
        rtEditor.SetActive(false);
        if (rte != null)
        {
            rte.SetActive(false);
            runtimeComponent.SetActive(false);
        }
        cloudSpace.SetActive(false);
        mainCamera.GetComponent<TransformGizmo>().enabled = false;
        ControlFlowManagerV2.Instance.EnableCameraFrustum(false);
        projectTitleText.text = projectNameField.text;
        objectCollection.SetActive(false);
        previewButton.SetActive(false);

        switch (projectTypeDropdown.value)
        {

            case 1:
                OpenControlFlowScene();
                break;
            
        }
    }
    #endregion

    public void LabelData(int LabelIndex)
    {
        ExistingProjectManager.instance.ProcessLabelEvent(LabelIndex);
    }

    public void RemoveUnnecessaryStartPoint()
    {
        raycasthitting raycastHitting = GetComponent<raycasthitting>();
        int indexOfLastElement = raycastHitting.LineStart.Count - 1;

        if (raycastHitting.LineStart.Count > raycastHitting.labels.Count)
        {
            for (int p = 0; p < raycastHitting.LineStart.Count; p++)
            {
                if (p == indexOfLastElement)
                {
                    Destroy(raycastHitting.LineStart[p].gameObject);
                    raycastHitting.LineStart.RemoveAt(p);
                }
            }
        }
    }

    public bool CheckForEmptyLabelName()
    {
        raycasthitting raycastHitting = GetComponent<raycasthitting>();

        if (raycastHitting.labels.Count > 0)
        {
            foreach (var label in raycastHitting.labels)
            {
                Label_Publish label_Publish = label.GetComponent<Label_Publish>();

                if (label_Publish.textObj.text.Length.Equals(0) || label_Publish.textObj.text.Equals("Enter label name"))
                {
                    return true;
                }
            }
        }

        return false;
    }

    public VideoAspectRatio AspectRatio(int value)
    {
        switch (value)
        {
            case 0:
                return VideoAspectRatio.NoScaling;
            case 1:
                return VideoAspectRatio.FitVertically;
            case 2:
                return VideoAspectRatio.FitHorizontally;
            case 3:
                return VideoAspectRatio.FitInside;
            case 4:
                return VideoAspectRatio.FitOutside;
            case 5:
                return VideoAspectRatio.Stretch;
        }
        return VideoAspectRatio.NoScaling;
    }

    #region Batching
    public void ControlObjectCollection_LineRendered_Alpha(LineRendererState rendererState)
    {

        LineRenderer[] lineRenderers = objectCollection.GetComponentsInChildren<LineRenderer>();

        Debug.Log(lineRenderers.Length);

        foreach (var item in lineRenderers)
        {

            float alpha = (rendererState == LineRendererState.Visible) ? 1 : 0.1f;
            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(c1, 0.0f), new GradientColorKey(c2, 1.0f) },
                new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
            );
            item.colorGradient = gradient;
        }
    }
    #endregion

    public void SaveDSButtons()
    {
        if (dsButtons.Count > 0)
        {
            foreach (var button in dsButtons)
            {
                button.GetComponentInChildren<ControlFlowButton>().OnSave();
            }
        }
    }

    private void RemoveDSButtons()
    {
        if (dsButtons.Count > 0)
        {
            foreach (var button in dsButtons)
            {
                Destroy(button);
            }
            dsButtons.Clear();
        }
    }

    public void ShowHidePassword()
    {
        if (passwordField.contentType.Equals(TMP_InputField.ContentType.Password))
        {
            passwordField.contentType = TMP_InputField.ContentType.Standard;
            showHideImage.sprite = ResourceManager.Instance.showPass;
        }
        else if (passwordField.contentType.Equals(TMP_InputField.ContentType.Standard))
        {
            passwordField.contentType = TMP_InputField.ContentType.Password;
            showHideImage.sprite = ResourceManager.Instance.hidePass;
        }
        passwordField.ForceLabelUpdate();
    }

    public void ChangeSkybox(string url)
    {
        if (!string.IsNullOrEmpty(url.Trim()))
            StartCoroutine(DownloadSkybox(url.Trim()));

    }

    IEnumerator DownloadSkybox(string url)
    {
        UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(url);
        ScriptsManager.Instance.skuboxURL = url;
        DownloadFromServer.Instance.DownloadingProgress(uwr);
        yield return uwr.SendWebRequest();

        if (uwr.isNetworkError || uwr.isHttpError)
        {
            Debug.Log(uwr.error);
        }
        else
        {
            var texture = DownloadHandlerTexture.GetContent(uwr);

            skyboxPropertyObj.GetComponent<ChangeSkybox>().skyboxMaterial.SetTexture("_Tex", texture);
            RenderSettings.skybox = skyboxPropertyObj.GetComponent<ChangeSkybox>().skyboxMaterial;
        }
    }
}
