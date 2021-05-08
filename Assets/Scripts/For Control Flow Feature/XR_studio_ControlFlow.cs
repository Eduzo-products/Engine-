using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Video;



/// <summary>
/// Handling scene with apprapoiate butn object in 3d space
/// </summary>
public class AnotherCollection
{
    public int seqNum;
    public GameObject butnObj;
    public UnityAction delegateMethod;
}


/// <summary>
/// Script by saravanan on control flow (sequence play).
/// Make sure you have only one component of this script in any scene(one instance).
/// Pardon the mistakes if any occurs and fix it with proper comments and alter technique
/// 
/// </summary>
public class XR_studio_ControlFlow : MonoBehaviour
{
    public struct ControlFlowDefault
    {
        public XR_studio_ControlFlow Instance;
        public bool currentStateAlive;
    }
    /// <summary>
    ///only instance of control flow operation script in the entire scene 
    /// </summary>
    public static ControlFlowDefault flowDefault;

    public static Action SequenceActionDownloadingEnded;
    public static Action SequenceActionLiveEnded;
    public List<SequenceContentDownloaded> xR_Studio_Sequences_contentDownloaded = new List<SequenceContentDownloaded>();
    public SequenceContentDownloaded currentContentDownloaded;
    public List<SceneSequence> xR_Studio_Sequences = new List<SceneSequence>();

    //  public List<XR_studio_Sequence> xR_Studio_Sequences_dummy = new List<XR_studio_Sequence>();

    public Dictionary<int, AssertBundleData> dowloadedSequenceJsonList = new Dictionary<int, AssertBundleData>();

    [SerializeField]
    short currentSequenceNum;

    public short _currentSequenceNum
    {
        get
        {
            return currentSequenceNum;
        }
        set
        {
            currentSequenceNum = value;
        }
    }


    public GameObject currentSceneButt, closeBtnPreview;

    //coroutine used for delay trigger for current sequence 
    public Coroutine triggerDelayCorountine;
    //coroutine used calculating the current timing of the sequence playing
    public Coroutine TimerCheckForSequenceCorountine;
    //coruntine to handle video download
    // public Co

    public List<XR_Studio_Action> currentLiveActionsDownloading = new List<XR_Studio_Action>();

    //  public List<GameObject> gameobjectToDeleteAtEndOfSequence = new List<GameObject>();



    //time added and maintained for a single sequence (all action timings combined) (i.e)current sequence
    public float timerForCurrentSequenceAction;

    public List<GameObject> objOnControlFlow = new List<GameObject>();

    //  public AssetDetails BundleDetailsDummy1, BundleDetailsDummy2, BundleDetailsDummy3, BundleDetailsDummy4;

    //  public AddAudio addAudioDummy1;


    //total actions downloaded in a sequence 
    [Space(30)]
    public short isAllActionDownloaded;

    [Space(20)]
    //scene objects to disable when the sequence flow starts 
    //public GameObject UiSetMainScenePage,
    public GameObject UiSetPopups;//, videoPlane;

    public GameObject cameraForUI;

    protected Transform sequenceObjsParent;
    [SerializeField]
    protected Transform sceneProperty_obj, objCollection;

    //change later to dictionary
    // public bool videoPanelEnable;

    [SerializeField]
    List<Transform> childrenInObjCollection = new List<Transform>();
    [SerializeField]
    List<Transform> childrenInSceneProperty = new List<Transform>();


    public EventTrigger eventTrigger;
    private EventTrigger.Entry entry;
    //  public Dictionary<EventTrigger, EventTrigger.Entry> keyValuePairsBUttons = new Dictionary<EventTrigger, EventTrigger.Entry>();

    //button for scenes to use
    public HashSet<AnotherCollection> keyValuePairsBUttons = new HashSet<AnotherCollection>();

    public List<VideoPlayer> videoPlayersInCurrentSeq = new List<VideoPlayer>();

    [SerializeField]
    raycasthitting raycastHittingScript;
    private void Awake()
    {
        flowDefault.Instance = this;
        flowDefault.currentStateAlive = false;
        raycastHittingScript = GetComponent<raycasthitting>();
    }

    private void OnDestroy()
    {
        ScriptsManager.OnCloseClicked -= CloseProject;
        SequenceActionDownloadingEnded -= StartDownloadNextSequence;

        SequenceActionLiveEnded -= TriggerNextSequence;

        ControlFlowManagerV2.startControlFlowAction -= ReceiveData;
    }

    private void Start()
    {
        ScriptsManager.OnCloseClicked += CloseProject;
        SequenceActionDownloadingEnded += StartDownloadNextSequence;

        SequenceActionLiveEnded += TriggerNextSequence;

        ControlFlowManagerV2.startControlFlowAction += ReceiveData;

        //StartCoroutine(SetRefTemp("MainScenePage", (obj) =>
        //{
        //    UiSetMainScenePage = obj;
        //}));
        StartCoroutine(SetRefTemp("Pop-Ups", (obj) =>
        {
            UiSetPopups = obj;
        }));

        StartCoroutine(SetRefTemp("UICamera", (obj) =>
        {
            cameraForUI = obj;
        }));

    }



    public void GetNecessaryObjsForSequence(Action CompletedSearch = null)
    {

        int checkCompleteSearch = 0;
        childrenInSceneProperty.Clear();
        //if (ScriptsManager.Instance.videoPanel != null)
        //    videoPanelEnable = ScriptsManager.Instance.videoPanel.activeSelf;

        sceneProperty_obj = ScriptsManager.Instance.scenePropertyRect;

        foreach (Transform item in sceneProperty_obj.transform)
        {
            childrenInSceneProperty.Add(item);
        }
        checkCompleteSearch++;
        if (checkCompleteSearch == 2)
            CompletedSearch();



        //  sequenceObjsParent = GameObject.Find("sequenceObjParent").transform;

        //if (sequenceObjsParent == null)
        //{
        //    sequenceObjsParent = new GameObject
        //    {
        //        name = "sequenceObjParent"
        //    };
        //}

        childrenInObjCollection.Clear();

        objCollection = ScriptsManager.Instance.objectCollection.transform;
        foreach (Transform item in objCollection.transform)
        {
            childrenInObjCollection.Add(item);
        }
        checkCompleteSearch++;
        if (checkCompleteSearch == 2)
            CompletedSearch();

    }




    public void SetButtonReference(GameObject buttonToTrigger, short seqNumToPlay)
    {
        //  print(buttonToTrigger.name);
        if (buttonToTrigger != null)
        {
            //print(buttonToTrigger.name + "     fffff");

            // eventTrigger = buttonToTrigger.GetComponentInChildren<ControlFlowButton>().button;

            //  entry = new EventTrigger.Entry();
            //  entry.eventID = EventTriggerType.PointerClick;

            UnityAction listenerAction = delegate { SetButtonTriggerRefTemporary(seqNumToPlay); };

            buttonToTrigger.GetComponentInChildren<ControlFlowButton>().button.onClick.AddListener(listenerAction);
            // eventTrigger.triggers.Add(entry);

            keyValuePairsBUttons.Add(new AnotherCollection { seqNum = seqNumToPlay, butnObj = buttonToTrigger, delegateMethod = listenerAction });
            print(buttonToTrigger.name + "   " + seqNumToPlay);
            buttonToTrigger.GetComponentInChildren<ControlFlowButton>().referencedTo = AssignButton.ButtonControlFlowTrigger.ToString();
        }
    }


    public void SetButtonTriggerRefTemporary(short seqNumToPlay)
    {
        FireTriggerTypeButton(seqNumToPlay);
    }


    public IEnumerator CoroutineToCheckCurrentSequenceCompletion()
    {

        //waiting for the videos in the current sequence(scene) to be completed 
        for (int i = 0; i < videoPlayersInCurrentSeq.Count; i++)
        {
            //check for streaming video
            yield return new WaitUntil(() => !videoPlayersInCurrentSeq[i].isPlaying);

        }

        //check whther all audios are done ,including label's
        yield return new WaitUntil(() => !ScriptsManager.Instance.audioManager.isPlaying);

        yield return new WaitUntil(() => !ScriptsManager.Instance.videoPanel.GetComponent<VideoPlayer>().isPlaying);

        //while (timerForCurrentSequenceAction > 0.0f)
        //{
        //    timerForCurrentSequenceAction -= Time.deltaTime;

        //    yield return null;
        //}

        if (_currentSequenceNum == xR_Studio_Sequences.Count - 1)
        {


        }
        else
        {
            SequenceActionLiveEnded();
        }

    }

    /// <summary>
    /// suspend a ongoing sequence flow
    /// </summary>
    public void SuspendSequence()
    {
        if (currentSceneButt != null)
            ChangeStateColor(currentSceneButt.GetComponentInChildren<ControlFlowButton>().normalColor);

        //currently wait time for a trigger to feature isnt used
        if (TimerCheckForSequenceCorountine != null)
        {
            //eliminate corountine handling the sequence timer and reset timer
            StopCoroutine(TimerCheckForSequenceCorountine);

            TimerCheckForSequenceCorountine = null;
        }

        if (xR_Studio_Sequences_contentDownloaded.Count == 0)
        {
            return;
        }
        // if (xR_Studio_Sequences_contentDownloaded[_currentSequenceNum].audioClip != null)
        //  {
        ScriptsManager.Instance.audioManager.Stop();
        ScriptsManager.Instance.audioManager.clip = null;

        //  }


        // ScriptsManager.Instance.videoPanel.GetComponent<VideoPlayer>().Stop();
        //  RenderTexture newText = new RenderTexture()
        //   ScriptsManager.Instance.videoPanel.GetComponent<MeshRenderer>().sharedMaterial.mainTexture = newText;

        timerForCurrentSequenceAction = 0;
        if (xR_Studio_Sequences_contentDownloaded[_currentSequenceNum].videoUrl.Count != 0)
        {
            foreach (var kvp in xR_Studio_Sequences_contentDownloaded[_currentSequenceNum].videoUrl.Keys)
            {
                kvp.GetComponent<VideoPlayer>().Stop();
                if (kvp.GetComponent<VideoPlayer>().targetTexture != null)
                    kvp.GetComponent<VideoPlayer>().targetTexture.Release();
            }
            videoPlayersInCurrentSeq.Clear();
        }

       

        //for (int i = 0; i < tempstorage.Count; i++)
        //{
        //    tempstorage[i].Stop();
        //    tempstorage[i].RemoveClip(tempstorage[i].GetClip("anim"));
        //}

    }



    public void SequenceDownloadCompleted()
    {


        xR_Studio_Sequences_contentDownloaded.Add(currentContentDownloaded);
        currentLiveActionsDownloading.Clear();


    }

    /// <summary>
    /// the assets available in upcoming sequence is downloaded, if none sequence after then proceed to preview (play control flow)
    /// </summary>
    public void StartDownloadNextSequence()
    {
        SequenceDownloadCompleted();
        currentContentDownloaded = new SequenceContentDownloaded();
        isAllActionDownloaded = 0;

        if (_currentSequenceNum == xR_Studio_Sequences.Count - 1)
        {
            InitStartSequence();
        }
        else
        {
            _currentSequenceNum++;

            StartSequenceDownload(_currentSequenceNum);
        }

    }

    /// <summary>
    /// download started ====> assets available in the actions of sequence 
    /// </summary>
    public void InitStartDownload()
    {
        GetNecessaryObjsForSequence(() =>
        {
            currentLiveActionsDownloading.Clear();
            currentContentDownloaded = new SequenceContentDownloaded();
            isAllActionDownloaded = 0;
            _currentSequenceNum = 0;
            flowDefault.currentStateAlive = true;
            StartSequenceDownload(_currentSequenceNum);
        });



    }


    /// <summary>
    /// downloading the assets that are available in a sequence (mostly the actions) 
    /// </summary>
    /// <param name="SequenceNum"></param>
    public void StartSequenceDownload(short SequenceNum)
    {
        SetReferenceMethodOnButtonTrigger(SequenceNum);
        StartSequenceActionDownload(SequenceNum);
    }


    public void StartSequenceActionDownload(short SequenceNum)
    {

        if (xR_Studio_Sequences[SequenceNum].actionsSequenceControlFlow.All(o => o.thisAction == ActionStates.None))
        {
            SetMissingMessage("Add scene objects to scene on multi scenes");
            return;
        }
        isAllActionDownloaded = (short)xR_Studio_Sequences[SequenceNum].actionsSequenceControlFlow.Count;

        for (short item = 0; item < xR_Studio_Sequences[SequenceNum].actionsSequenceControlFlow.Count; item++)
        {
            switch (xR_Studio_Sequences[SequenceNum].actionsSequenceControlFlow[item].thisAction)
            {
                case ActionStates.None:
                    //adding the action count .. because none is counted as a action too
                    currentLiveActionsDownloading.Add(xR_Studio_Sequences[SequenceNum].actionsSequenceControlFlow[item]);

                    break;

                case ActionStates.TextToSpeech:

                    break;

                case ActionStates.Model:
                    AssetDetails tempDetails = xR_Studio_Sequences[SequenceNum].actionsSequenceControlFlow[item].assetDetails;
                    if (tempDetails.ObjectTransform != null
                        && !string.IsNullOrEmpty(tempDetails.ObjectTransform.objectFullPath))
                    //using the models already available on the scene... so adding to the count 
                    {

                        ThreeDModelFunctions[] modelTemps = Resources.FindObjectsOfTypeAll<ThreeDModelFunctions>();
                        GameObject tempObj = null;

                        for (int i = 0; i < modelTemps.Length; i++)
                        {

                            if (modelTemps[i].name.Equals(tempDetails.ObjectTransform.objectName))
                            {
                                tempObj = modelTemps[i].gameObject;
                            }
                        }



                        if (!objOnControlFlow.Contains(tempObj))
                        {
                            objOnControlFlow.Add(tempObj);
                            tempObj.GetComponent<ThreeDModelFunctions>().AddChild();
                        }


                        currentContentDownloaded.assetData.Add(item, tempObj);


                        bool modelSafeToUse = true;

                        GetTotalChildrenVisiablity(tempObj.transform, tempDetails, (flowCheck) =>
                        {
                            modelSafeToUse = flowCheck;

                        }
                       );

                        if (!modelSafeToUse)
                            return;


                      

                        currentLiveActionsDownloading.Add(xR_Studio_Sequences[SequenceNum].actionsSequenceControlFlow[item]);

                    }
                    else
                    {
                        SetMissingMessage("Please add content to model scene object on multi scene.");
                        return;
                    }
                    break;
                case ActionStates.Video:
                    AddVideo av = xR_Studio_Sequences[SequenceNum].actionsSequenceControlFlow[item].videoDetails;

                    if (!string.IsNullOrEmpty(av.videoURL) && av.ObjectTransform != null && !string.IsNullOrEmpty(av.ObjectTransform.objectFullPath))
                    {
                        GameObject tempObj = ScriptsManager.Instance.GetVideo(av.videoID);
                        // transform.f
                        DownloadActionVideoClip(xR_Studio_Sequences[SequenceNum].actionsSequenceControlFlow[item], tempObj);

                        currentContentDownloaded.assetData.Add(item, tempObj);
                    }
                    else
                    {
                        SetMissingMessage("Please add content to video scene object on multi scene.");
                        return;
                    }
                    break;
              //  case ActionStates.Image:

                    //AddVideo av = xR_Studio_Sequences[SequenceNum].actionsSequenceControlFlow[item].videoDetails;

                    //if (!string.IsNullOrEmpty(av.videoURL) && av.ObjectTransform != null && !string.IsNullOrEmpty(av.ObjectTransform.objectFullPath))
                    //{
                    //    GameObject tempObj = ScriptsManager.Instance.GetVideo(av.videoID);
           

                    //    currentContentDownloaded.assetData.Add(item, tempObj);
                    //}
                    //else
                    //{
                    //    SetMissingMessage("Please add content to video scene object on multi scene.");
                    //    return;
                    //}

                 //   break;
                case ActionStates.Audio:

                    if (!string.IsNullOrEmpty(xR_Studio_Sequences[SequenceNum].actionsSequenceControlFlow[item].audioDetails.audioURL))
                    {
                        DownloadActionAudioClip(xR_Studio_Sequences[SequenceNum].actionsSequenceControlFlow[item]);

                    }
                    else
                    {
                        SetMissingMessage("Please add content to audio scene object on multi scene.");
                        return;
                    }
                    break;
               
                case ActionStates.TextPanel:

                    if (xR_Studio_Sequences[SequenceNum].actionsSequenceControlFlow[item].textContent.transform != null
                        && !string.IsNullOrEmpty(xR_Studio_Sequences[SequenceNum].actionsSequenceControlFlow[item].textContent.transform.objectFullPath))
                    {

                        TextContentCanvasHandler[] textTemps = Resources.FindObjectsOfTypeAll<TextContentCanvasHandler>();
                        GameObject txtTemp = null;
                        //for (int i = 0; i < ScriptsManager.Instance.enableDisableComponent.AllDesccriptionPanelHierarchy.Count; i++)
                        //{

                        //    if (ScriptsManager.Instance.enableDisableComponent.AllDesccriptionPanelHierarchy[i].name.Equals(xR_Studio_Sequences[SequenceNum].actionsSequenceControlFlow[item].textContent.transform.objectName))
                        //    {
                        //         txtTemp = ScriptsManager.Instance.enableDisableComponent.AllDesccriptionPanelHierarchy[i].gameObject;
                        //    }
                        //}

                        for (int i = 0; i < textTemps.Length; i++)
                        {

                            if (textTemps[i].name.Equals(xR_Studio_Sequences[SequenceNum].actionsSequenceControlFlow[item].textContent.transform.objectName))
                            {
                                txtTemp = textTemps[i].gameObject;
                            }
                        }


                        //using the text content already available on the scene... so adding to the count 
                        currentContentDownloaded.assetData.Add(item, txtTemp);
                        currentLiveActionsDownloading.Add(xR_Studio_Sequences[SequenceNum].actionsSequenceControlFlow[item]);
                    }
                    else
                    {
                        SetMissingMessage("Please link text panel to scene object on multi scene.");
                        return;
                    }

                    break;

                case ActionStates.Button:
                    if (!string.IsNullOrEmpty(xR_Studio_Sequences[SequenceNum].actionsSequenceControlFlow[item].customButton.path))
                    {
                        //using the custom button already available on the scene... so adding to the count 
                        currentContentDownloaded.assetData.Add(item, SearchAndSetObjectOnSequence(xR_Studio_Sequences[SequenceNum].actionsSequenceControlFlow[item].customButton.path));
                        currentLiveActionsDownloading.Add(xR_Studio_Sequences[SequenceNum].actionsSequenceControlFlow[item]);
                    }
                    else
                    {
                        SetMissingMessage("please give reference to button scene object on multi scene");
                        return;
                    }


                    break;

                default:
                    break;
            }
            IsAllActionDownloadedMethod();

        }
    }


    /// <summary>
    ///disable/reenable raycast on 3d space (interaction) and it contains loader animation object control too
    /// </summary>
    /// <param name="v"></param>
    void ChangeAxisFunctionality(bool v)
    {
        //SetUpLoadingForPreview(!v);

        ScriptsManager.Instance.rtEditor.SetActive(v);
        ScriptsManager.Instance.rte.SetActive(v);
        ScriptsManager.Instance.runtimeComponent.SetActive(v);
        ScriptsManager.Instance.transformGizmo.enabled = v;

    }


    public void SetMissingMessage(string msg)
    {

        ChangeAxisFunctionality(true);
        SetUpLoadingForPreview(false);
        RemoveButtonReferenceMethods();

        ScriptsManager.Instance.errorMessage.text = msg;

        if (ScriptsManager.Instance.errorMessage.text.Length > 0)
        {
            ScriptsManager.Instance.clearError.SetActive(true);
        }
    }

    

    public void SetReferenceMethodOnButtonTrigger(short seqNum)
    {
        for (short i = 0; i < xR_Studio_Sequences[seqNum].triggersSequenceControlFlow.Count; i++)
        {
            if (xR_Studio_Sequences[seqNum].triggersSequenceControlFlow[i].thisTrigger == TypeOfTriggers.Button)
            {
                //enable the btn object before null check
                //  xR_Studio_Sequences[seqNum].triggersSequenceControlFlow[i].buttObj.SetActive(true);
                if (xR_Studio_Sequences[seqNum].triggersSequenceControlFlow[i].buttonObj != null)
                {
                    if (string.Empty != xR_Studio_Sequences[seqNum].triggersSequenceControlFlow[i].buttonObj.givenName)
                    {
                        // Debug.Log($"{xR_Studio_Sequences[seqNum].triggersSequenceControlFlow[i].buttonObj.path}. PAth");
                        //StartCoroutine(setRefTemp(xR_Studio_Sequences[seqNum].triggersSequenceControlFlow[i].buttonObj.path, (obj) =>
                        //{
                        //    SetButtonReference(obj, seqNum);
                        //}));
                        for (int j = 0; j < ScriptsManager.Instance.dsButtons.Count; j++)
                        {
                            if (ScriptsManager.Instance.dsButtons[j].name == xR_Studio_Sequences[seqNum].triggersSequenceControlFlow[i].buttonObj.givenName)
                            {
                                SetButtonReference(ScriptsManager.Instance.dsButtons[j], seqNum);
                            }
                        }
                    }
                    else
                    {
                        //SetMissingMessage("please give reference to button scene object on multi scene");
                        return;
                    }
                }
            }
        }
    }

    public IEnumerator SetRefTemp(string path, Action<GameObject> objFetched = null)
    {
        GameObject obj = null;
        yield return new WaitUntil(() => obj = SearchAndSetObjectOnSequence(path));

        objFetched(obj);
    }

    public GameObject SearchAndSetObjectOnSequence(string path)
    {
        return GameObject.Find(path);
    }



    /// <summary>
    /// Disable and reverting the assets of the sequence after it is completed
    /// 1. in order to show seamless flow in the control flow
    /// 2. in order to start properly if the sequence is linked to a button
    /// </summary>
    public void DeactivePreviousSequenceAssets()
    {
        if (_currentSequenceNum > 0)
        {
            foreach (var r in xR_Studio_Sequences_contentDownloaded[_currentSequenceNum - 1].assetData.Keys)
            {
                xR_Studio_Sequences_contentDownloaded[_currentSequenceNum - 1].assetData[r].SetActive(false);
            }

            if (xR_Studio_Sequences_contentDownloaded[_currentSequenceNum - 1].audioClip != null)
            {
                ScriptsManager.Instance.audioManager.clip = null;

            }
        }
    }

    /// <summary>
    /// starts the next sequence seamless 
    /// </summary>
    public void TriggerNextSequence()
    {

        _currentSequenceNum++;

        FireSequence(_currentSequenceNum);
    }

    void ChangeStateColor(Color tempC)
    {
        ColorBlock cb = currentSceneButt.GetComponentInChildren<Button>().colors;

        cb.normalColor = tempC;
        currentSceneButt.GetComponentInChildren<Button>().colors = cb;


    }

    void ChangeSceneButtonHighlight()
    {

        //in here abt the auto sequence chnge rectification
        // if (keyValuePairsBUttons.Any(i => (i.butnObj == EventSystem.current.currentSelectedGameObject.transform.parent.parent.gameObject)))
        //Contains((item) => item. EventSystem.current.currentSelectedGameObject.transform.parent.parent.gameObject))
        //above is good line stuff and so keeping it
        if (GetCurrentAnotherCollection() != null)
        {
            currentSceneButt = GetCurrentAnotherCollection().butnObj;

            ChangeStateColor(currentSceneButt.GetComponentInChildren<ControlFlowButton>().highlightColor);
        }
    }




    AnotherCollection GetCurrentAnotherCollection()
    {
        // return keyValuePairsBUttons.Select(i => i.seqNum == currentSequenceNum) as AnotherCollection;
        return keyValuePairsBUttons.Single(i => i.seqNum == currentSequenceNum);
    }



    /// <summary>
    /// start sequence on the button press
    /// use this method on Trigger button panel to map the sequence number with respective button
    /// </summary>
    /// <param name="seqNum"></param>
    public void FireTriggerTypeButton(short seqNum = 0)
    {
        SuspendSequence();
        DeactivePreviousSequenceAssets();

        //temporary code used change in future
        DisableEnableSceneObjs(false);

        SetCurrentSequenceNum(seqNum);
        FireAllSequenceAction(seqNum);
    }


    /// <summary>
    /// use this to trigger sequence inbetween by setting the sequence 
    /// </summary>
    /// <param name="num"></param>
    public void SetCurrentSequenceNum(short num)
    {
        _currentSequenceNum = num;
    }

    /// <summary>
    /// receiving data from the screen input
    /// </summary>
    /// <param name="_Sequence"></param>
    public void ReceiveData(List<SceneSequence> _Sequence)
    {
        SetUpLoadingForPreview(true);
        ChangeAxisFunctionality(false);

        if (_Sequence.Count == 0)
        {
            SetMissingMessage("Add scene on multi scenes"); return;
        }
        objOnControlFlow.Clear();
        xR_Studio_Sequences.Clear();
        xR_Studio_Sequences_contentDownloaded.Clear();
        xR_Studio_Sequences = _Sequence;
        xR_Studio_Sequences_contentDownloaded = new List<SequenceContentDownloaded>(_Sequence.Count);

        InitStartDownload();

    }

    /// <summary>
    /// disable loading animation
    /// </summary>
    /// <param name="c"></param>
    private void SetUpLoadingForPreview(bool c)
    {
        var temploadingObj = ExistingProjectManager.instance.projectLoaderPanel;
        temploadingObj.SetActive(c);
        temploadingObj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = c ? "Please wait. Preview is loading..." : "Please wait. Project is loading...";
    }

    /// <summary>
    /// disable other objects in the scene
    /// </summary>
    /// <param name="bol"></param>
    public void DisableEnableSceneObjs(bool bol)
    {
        UiSetPopups.SetActive(bol);
        //UiSetMainScenePage.SetActive(bol);
        cameraForUI.SetActive(bol);
        //     ScriptsManager.Instance.videoPanel.SetActive(bol);

        for (int i = 0; i < childrenInObjCollection.Count; i++)
        {
            childrenInObjCollection[i].gameObject.SetActive(bol);
        }


        for (int i = 0; i < childrenInSceneProperty.Count; i++)
        {
            childrenInSceneProperty[i].gameObject.SetActive(bol);
        }
    }

    //public void EnableDisableVIdeoPanel()
    //{

    //    ScriptsManager.Instance.videoPanel.SetActive(videoPanelEnable);

    //}

    /// <summary>
    /// starts the play of sequence in an order
    /// </summary>
    /// <param name="_Sequence"></param>
    public void InitStartSequence(List<SceneSequence> _Sequence = null)
    {
        ScriptsManager.Instance.mainCanvasRaycaster.enabled = false;

        closeBtnPreview.SetActive(true);

        //place camera in default position and rotation
        ScriptsManager.Instance.mainCamera.transform.SetPositionAndRotation(new Vector3(0, 0, -3.13f), new Quaternion(0, 0, 0, 0));

        //find alternative to place this below line of code 
        SetUpLoadingForPreview(false);

        SuspendSequence();
        DisableEnableSceneObjs(false);

        _currentSequenceNum = 0;

        //start sequence play.. so ending the download processes 
        isAllActionDownloaded = 0;
        FireSequence(_currentSequenceNum);
    }


    /// <summary>
    /// rewrite this method
    /// </summary>
    /// <param name="trans"></param>
    /// <param name="ad"></param>
    void GetTotalChildrenVisiablity(Transform trans, AssetDetails ad, Action<bool> continueFlow)
    {
        Transform[] transforms = trans.GetComponentsInChildren<Transform>(true);
        List<Transform> childTransforms = new List<Transform>();

        for (int p = 0; p < transforms.Length; p++)
        {
            if (!transforms[p].GetComponent<SkipThisObject>())
            {
                childTransforms.Add(transforms[p]);
            }
        }

        // print(ad.childName.Count + "   " + trans.name + "    " + childTransforms.Length);

        //validate the length of the child count
        if (ad.childName.Count != childTransforms.Count)
        {
            currentSequenceNum -= 1;
            InitCloseControlFlow();


            SetMissingMessage("Object has been modified on model content. Delete model scene object and add to scene objects. ");
            continueFlow(false);
            return;
        }
        continueFlow(true);
        //   GetTotalChildrenVisiablity(trans, ad.childName, ad.childActiveState);
    }


    void GetTotalChildrenVisiablity(Transform trans, List<string> cn, List<bool> cas)
    {
        Transform[] transforms = trans.GetComponentsInChildren<Transform>(true);
        List<Transform> childTransforms = new List<Transform>();

        for (int p = 0; p < transforms.Length; p++)
        {
            if (!transforms[p].GetComponent<SkipThisObject>())
            {
                childTransforms.Add(transforms[p]);
            }
        }

        for (int p = 0; p < childTransforms.Count; p++)
        {
            if (childTransforms[p].name == cn[p])
            {
                childTransforms[p].gameObject.SetActive(cas[p]);
            }
        }
        //enable the parent at last
        trans.gameObject.SetActive(true);

    }
    private void Update()
    {
        if (Input.GetMouseButton(1))
            return;

        if (Input.GetKey(KeyCode.Escape) && !ExistingProjectManager.instance.projectLoaderPanel.activeSelf && ScriptsManager.Instance.projectTypeDropdown.value == 1)
        {
            InitCloseControlFlow();
        }
    }

    public void InitCloseControlFlow()
    {

        SuspendSequence();

        RemoveButtonReferenceMethods();

        DisableEnableSceneObjs(true);

        //SceneLineItem sceneLineItem = ControlFlowManagerV2.currentSelectedSceneLineItem;
        //print(sceneLineItem.Name());
        // ControlFlowManagerV2.currentSelectedSceneLineItem.DeSelect();

        foreach (var item in ControlFlowManagerV2.Instance.sceneLineItems)
        {
            item.DeSelect();

        }


        flowDefault.currentStateAlive = false;

        //for (int i = 0; i < objOnControlFlow.Count; i++)
        //{
        //    GetTotalChildrenVisiablity(objOnControlFlow[i].transform, objOnControlFlow[i].GetComponent<ThreeDModelFunctions>().childsName,
        //        objOnControlFlow[i].GetComponent<ThreeDModelFunctions>().childsActivateStatus);
        //}

        ScriptsManager.Instance.DeactivateAllComponent();
        ScriptsManager.Instance.GetComponent<Label_components>().ClearComp();

        //Revert the enabled functionality for preview after exiting it   
        ChangeAxisFunctionality(true);

        //reEnable label delete button 
        ControlFlowManagerV2.Instance.EnableFunctionalityForPreview(true);
        //disable the functionality on Label Click(tis os done automatically)


        StopAllCoroutines();
        currentSceneButt = null;
        closeBtnPreview.SetActive(false);
        ScriptsManager.Instance.mainCanvasRaycaster.enabled = true;
    }



    void RemoveButtonReferenceMethods()
    {
        foreach (var item in keyValuePairsBUttons)
        {
            //    item.GetComponentInChildren<ControlFlowButton>().button.onClick.RemoveAllListeners();
            item.butnObj.GetComponentInChildren<ControlFlowButton>().button.onClick.RemoveListener(item.delegateMethod);
        }

        keyValuePairsBUttons.Clear();
    }
    void CloseProject()
    {
        currentSceneButt = null;
        // InitCloseControlFlow();
        StopAllCoroutines();
    }


    /// <summary>
    /// Start sequence with favoured trigger and sequence number
    /// </summary>
    /// <param name="num"></param>
    void FireSequence(short num)
    {

        // ChangeSceneButtonHighlight();

        FireTrigger(num);

    }


    /// <summary>
    /// starts the sequence actions, onload not included here because it runs separately.. 
    /// this method is used to run sequence consecutively
    /// </summary>
    void FireTrigger(short SequenceNumToFire)
    {

        if (xR_Studio_Sequences[SequenceNumToFire].triggersSequenceControlFlow.All(o => o.thisTrigger == TypeOfTriggers.None))
        {
            InitCloseControlFlow();
            SetMissingMessage("Assign scene trigger to scene on multi scene.");
            return;
        }

        for (int i = 0; i < xR_Studio_Sequences[SequenceNumToFire].triggersSequenceControlFlow.Count; i++)
        {
            //print(xR_Studio_Sequences[SequenceNumToFire].triggersSequenceControlFlow[i].thisTrigger);
            if (xR_Studio_Sequences[SequenceNumToFire].triggersSequenceControlFlow[i].thisTrigger == TypeOfTriggers.Continue)
            {
                FireTriggerType(xR_Studio_Sequences[SequenceNumToFire].triggersSequenceControlFlow[i].thisTrigger, SequenceNumToFire);
                // break;
                return;
            }

        }
    }



    void FireAllSequenceAction(int seqToFire)
    {
        ChangeSceneButtonHighlight();

        SpawnSeqObjs(seqToFire, (boolean) =>
         {

             //fired sequence completed 
             //jus in case for future use
             // xR_Studio_Sequences[seqToFire].triggersSequenceControlFlow


         });
    }





    void SpawnSeqObjs(int seqToFire, Action<bool> completed)
    {


        //3d asset content or the object to enable for sequence
        foreach (var r in xR_Studio_Sequences_contentDownloaded[seqToFire].assetData.Keys)
        {
            xR_Studio_Sequences_contentDownloaded[seqToFire].assetData[r].SetActive(true);


            if (xR_Studio_Sequences[seqToFire].actionsSequenceControlFlow[r].thisAction == ActionStates.Model)
            {
                GetTotalChildrenVisiablity(xR_Studio_Sequences_contentDownloaded[seqToFire].assetData[r].transform,
                    xR_Studio_Sequences[seqToFire].actionsSequenceControlFlow[r].assetDetails.childName,
                    xR_Studio_Sequences[seqToFire].actionsSequenceControlFlow[r].assetDetails.childActiveState);
            }
        }
        //disable label delete button 
        ControlFlowManagerV2.Instance.EnableFunctionalityForPreview(false);


        //audio content for sequence
        if (xR_Studio_Sequences_contentDownloaded[seqToFire].audioClip != null)
        {
            ScriptsManager.Instance.audioManager.clip = xR_Studio_Sequences_contentDownloaded[seqToFire].audioClip;
            ScriptsManager.Instance.audioManager.Play();
        }

        //video content for sequence
        foreach (var item in xR_Studio_Sequences_contentDownloaded[seqToFire].videoUrl.Keys)
        {

            //video game obj = enable
            item.SetActive(true);
            var videoPlayer = item.GetComponent<VideoPlayer>();
            videoPlayer.source = VideoSource.Url;
            videoPlayer.url = xR_Studio_Sequences_contentDownloaded[seqToFire].videoUrl[item];

            videoPlayer.playOnAwake = videoPlayer.skipOnDrop = videoPlayer.waitForFirstFrame = false;

            RenderTexture newText = new RenderTexture(2000, 1540, 24);
            newText.useDynamicScale = true;
            newText.dimension = UnityEngine.Rendering.TextureDimension.Tex2D;
            videoPlayer.targetTexture = newText;
            item.GetComponent<MeshRenderer>().sharedMaterial.mainTexture = newText;


            //  videoPlayer.loopPointReached += VideoOver;

            videoPlayersInCurrentSeq.Add(videoPlayer);
            //start url stream
            videoPlayer.Play();

        }





        foreach (var item in keyValuePairsBUttons)
        {
            item.butnObj.SetActive(true);

        }


        timerForCurrentSequenceAction += xR_Studio_Sequences_contentDownloaded[seqToFire].sequenceLengthInSeconds;

        if (TimerCheckForSequenceCorountine == null)
        {
            TimerCheckForSequenceCorountine = StartCoroutine(CoroutineToCheckCurrentSequenceCompletion());
        }

        //any thing which needs to hapn at the end of sequence 
        completed(true);
    }




    public void VideoOver(VideoPlayer videoPlayer)
    {

        videoPlayer.Stop();
    }




    /// <summary>
    /// starts the sequence with given type of trigger
    /// </summary>
    /// <param name="typeOfTrigger"></param>
    /// <param name="seqNum"></param>
    /// <param name="timeToDelay"></param>
    public void FireTriggerType(TypeOfTriggers typeOfTrigger, int seqNum, float timeToDelay = 0)
    {
        isAllActionDownloaded = 0;
        // print(typeOfTrigger);
        switch (typeOfTrigger)
        {

            //need to specific the time for delay
            case TypeOfTriggers.Continue:
                SuspendSequence();

                DeactivePreviousSequenceAssets();
                //temporary code used change in future
                DisableEnableSceneObjs(false);

                //setting maunal value for timeToDelay to 0, as the no time interval                
                triggerDelayCorountine = StartCoroutine(FireTriggerTypeDelay(seqNum, timeToDelay));
                break;

            case TypeOfTriggers.None:

                break;

            default:
                break;
        }
    }




    public IEnumerator FireTriggerTypeDelay(int seqNum, float timeDelay = 0)
    {

        yield return StartCoroutine(DelayTimer(timeDelay));

        FireAllSequenceAction(seqNum);


    }

    public IEnumerator DelayTimer(float timeDelay = 0)
    {

        yield return new WaitForSeconds(timeDelay);

    }



    public void IsAllActionDownloadedMethod()
    {
        //check if the action is the last to download
        if (isAllActionDownloaded == currentLiveActionsDownloading.Count)
        {
            print("all actions in current sequence downloaded");

            SequenceActionDownloadingEnded();
        }

    }


    void DownloadActionVideoClip(XR_Studio_Action typeOfAction, GameObject obj)
    {
        print(obj.name);
        currentContentDownloaded.videoUrl.Add(obj, typeOfAction.videoDetails.videoURL);

        currentLiveActionsDownloading.Add(typeOfAction);

        ScriptsManager.Instance.videoPanel.SetActive(false);

        //  IsAllActionDownloadedMethod();

        //StartCoroutine(DownloadVideo(typeOfAction.videoDetails.videoURL, (videoObj_url, videoLength) =>
        // {


        //     //video play time in seconds with sequence time
        //     if (currentContentDownloaded.sequenceLengthInSeconds < videoLength / 2)
        //     {
        //         currentContentDownloaded.sequenceLengthInSeconds += videoLength;
        //     }
        //     else
        //     {
        //         currentContentDownloaded.sequenceLengthInSeconds += videoLength / 2;
        //     }


        // }));




    }



    void DownloadActionAudioClip(XR_Studio_Action typeOfAction)
    {
        // AudioClip actionaudio = ;
        // ScriptsManager.Instance.audioManager.clip = actionaudio;

        //start a coroutine and wait for the audio to get downloaded completely
        StartCoroutine(DownloadAudio(typeOfAction.audioDetails.audioURL, typeOfAction.audioClip, (actionaudio) =>
         {

             currentLiveActionsDownloading.Add(typeOfAction);


             currentContentDownloaded.audioClip = actionaudio;
             //audio play time in seconds with sequence time
             if (currentContentDownloaded.sequenceLengthInSeconds < actionaudio.length / 2)
             {
                 currentContentDownloaded.sequenceLengthInSeconds += actionaudio.length;
             }
             else
             {
                 currentContentDownloaded.sequenceLengthInSeconds += actionaudio.length / 2;
             }


             //  print(currentLiveActionsDownloading.Count + " actions downloaded count " + isAllActionDownloaded + "   isAllActionDownloaded");

             IsAllActionDownloadedMethod();

         }
        ));
    }


    IEnumerator DownloadAudio(string path, AudioClip audioClip, Action<AudioClip> actionAudio)
    {

        // var uwr = UnityWebRequestMultimedia.GetAudioClip(path, AudioType.WAV);
        //  DownloadFromServer.Instance.DownloadingProgress(uwr);
        yield return new WaitForSeconds(.4f);

        //if (uwr.isNetworkError || uwr.isHttpError)
        //{
        //    //  Debug.LogError(uwr.error);
        //    SetMissingMessage("Audio url problem");
        //    yield break;
        //}

        //  AudioClip audioClip = DownloadHandlerAudioClip.GetContent(uwr);

        ScriptsManager.Instance.audioManager.clip = audioClip;

        // ScriptsManager.Instance.ProgressBar(null, ScriptsManager.Instance.isDownloaded);



        // yield return new WaitUntil(() => isAllActionDownloaded == currentLiveActionsDownloading.Count);

        //stopped from playing, inorder to remove implementation of predownload technique (download flow ----- 1)
        //  ScriptsManager.Instance.audioManager.Play();


        //wait time for other action to get downloaded
        // yield return new WaitForSeconds(2f);
        actionAudio(audioClip);
    }







    void CreateAnimation(Animation animationTest, AnimationClip originalClip, int start, int end)
    {

        animationTest.AddClip(originalClip, "anim", start, end);

        animationTest.cullingType = AnimationCullingType.BasedOnRenderers;


        AnimationClip animClip = animationTest.GetClip("anim");

        animClip.frameRate = 30;
        animationTest["anim"].speed = 1.0f;

        animationTest.Play("anim");


    }

    public void ShowMessage()
    {
        throw new NotImplementedException();
    }

    public void MessageColor()
    {
        throw new NotImplementedException();
    }
}
