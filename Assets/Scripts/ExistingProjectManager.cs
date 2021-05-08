using CommandUndoRedo;
using RuntimeGizmos;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Video;

public class ExistingProjectManager : MonoBehaviour
{
    [Header("<------- Manual References ------->")]
    public GameObject existingProject;
    public GameObject buttonPrefab;
    public Transform buttonParent;
    public Color normalColor, selectedColor;
    public Button submitButton;
    public static ExistingProjectManager instance = null;
    [Header("<------- Runtime References ------->")]
    public List<GameObject> buttons = new List<GameObject>();
    public string selectedProjectName;
    public Sprite newSprite;
    public int totalAsset = 0;
    public int DownloadedAsset = 0;
    public List<Vector3> lineStart;
    public List<Vector3> lineEnd;
    public List<string> label2Objects;

    public GameObject videoPrefab;
    public GameObject audioPrefab;
    public GameObject textPrefab;
    public GameObject text2SpeechPrefab;

    private VideoPlayer vidPlayerObj;
    private enum enumData { label = 0, video, audio, text, text2speech };

    [Header("<--------------- LabelName --------------->")]
    public List<string> labelName = new List<string>();
    public List<GameObject> spawnedLabels = new List<GameObject>();
    public List<string> name = new List<string>();
    public List<bool> videoEnabled = new List<bool>();
    List<bool> isBatchingEnabled = new List<bool>();
    List<bool> audioEnabled = new List<bool>();
    List<bool> descriptionEnabled = new List<bool>();
    List<bool> text2SpeechEnabled = new List<bool>();
    List<AddAudio> audioDetails = new List<AddAudio>();
    public List<AddVideoLabel> videoDetails = new List<AddVideoLabel>();
    List<TextContent> textDetails = new List<TextContent>();
    List<XRTextToSpeech> textToSpeechDetails = new List<XRTextToSpeech>();
    List<string> labelgrp = new List<string>();

    public Color c1 = Color.blue;
    public Color c2 = Color.white;
    public GameObject prefab;

    [SerializeField]
    private Material skyboxMaterial = null;
    public List<GameObject> listLabel = new List<GameObject>();
    public List<AddLabelGroups> groupLabels = new List<AddLabelGroups>();
    public ToolContent downloadedData;

    public List<GameObject> scenePropertyObjects = new List<GameObject>();
    public Coroutine coroutine, immersiveContentRoutine, hotspotRoutine, downloadProjectRoutine;
    public GameObject projectLoaderPanel;

    private DateTime localTime, universalTime;
    private ScriptsManager m_ScriptsManager;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        ScriptsManager.OnCloseClicked += ClearJSON;
    }

    private void OnDestroy()
    {
        ScriptsManager.OnCloseClicked -= ClearJSON;
    }

    private void ClearJSON()
    {
        ScriptsManager._toolContent = null;
        downloadedData = null;

        ClearLists();
        RemoveScenePropertyObjects();
        StopAllCoroutines();
    }

    public void OpenExistingProject()
    {
        StartCoroutine(ContentListRequest());
    }

    private void ClearLists()
    {
        label2Objects.Clear();
        labelName.Clear();
        spawnedLabels.Clear();
        name.Clear();
        lineStart.Clear();
        lineEnd.Clear();
        videoEnabled.Clear();
        videoDetails.Clear();

        if (listLabel.Count > 0)
        {
            foreach (GameObject deletables in listLabel)
            {
                Destroy(deletables);
            }

            listLabel.Clear();
        }

        if (groupLabels.Count > 0)
        {
            groupLabels.Clear();
        }
    }

    private void RemoveScenePropertyObjects()
    {
        if (scenePropertyObjects.Count > 0)
        {
            foreach (GameObject item in scenePropertyObjects)
            {
                if (item != null)
                {
                    Destroy(item);
                }
            }

            scenePropertyObjects.Clear();
        }
    }

    public void CloseExistingProject()
    {
        ScriptsManager.Instance.currentObjectName.gameObject.SetActive(false);
        existingProject.SetActive(false);
        selectedProjectName = "";

        RemoveButtons();
        DownloadedAsset = 0;
    }

    private IEnumerator ContentListRequest()
    {
        UnityWebRequest www = UnityWebRequest.Get(ScriptsManager.Instance.APIBaseURL + "api/GetContentList?clientid=" + ScriptsManager.Instance.userDetails.clientId);

        DownloadFromServer.Instance.DownloadingProgress(www);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            string dataAsJson = www.downloadHandler.text;
            FileListResponseData res = JsonUtility.FromJson<FileListResponseData>(dataAsJson);
            List<PublishedContentFileList> list = res.Data;
            Debug.Log($"Existing - {dataAsJson}");
            CreateButtons(list.Count, list);
            existingProject.SetActive(true);
        }
    }

    public void DownloadProject()
    {
        projectLoaderPanel.SetActive(true);

        //StopAllCoroutines();
        downloadProjectRoutine = StartCoroutine(DownloadJSONProject(selectedProjectName, (isDone) =>
        {
            if (isDone)
            {
                //StopCoroutine(downloadProjectRoutine);

                //check if the scenes and scene objects downloaded completely  
                if (ScriptsManager.Instance.projectTypeDropdown.value.Equals(1))
                {

                    for (int i = 0; i < ControlFlowManagerV2.Instance.sceneLineItems.Count; i++)
                    {
                        ControlFlowManagerV2.Instance.sceneLineItems[i].DeSelect();
                    }

                }
                else if (ScriptsManager.Instance.projectTypeDropdown.value.Equals(0))
                {
                    if (ScriptsManager.Instance.modelObjectLineItems.Count > 0)
                        ScriptsManager.Instance.modelObjectLineItems[ScriptsManager.Instance.modelObjectLineItems.Count - 1].DeSelect();
                }
                ScriptsManager.Instance.DeactivateAllComponent();
                projectLoaderPanel.SetActive(false);
            }
        }));
    }

    private IEnumerator DownloadJSONProject(string fileName, Action<bool> isDone)
    {
        UnityWebRequest www = UnityWebRequest.Get(ScriptsManager.Instance.APIBaseURL + "/api/GetContent?ClientId=" + ScriptsManager.Instance.userDetails.clientId + "&FileName=" + fileName);
        yield return www.SendWebRequest();

        if (!www.isNetworkError || !www.isHttpError)
        {
            string dataAsJson = www.downloadHandler.text;
            ReceivedContent fetchedData = JsonUtility.FromJson<ReceivedContent>(dataAsJson);
#if UNITY_EDITOR
            TextAsset textAsset = new TextAsset();
            File.WriteAllText(Application.dataPath + "/LoadedProject.json", dataAsJson);
#endif
            ScriptsManager.Instance.IsExistingProject = true;
            if (fetchedData.ResponseCode != 0)
            {
                ScriptsManager._toolContent = fetchedData.Data;
                downloadedData = fetchedData.Data;
                ScriptsManager.Instance.AssetCount = ScriptsManager._toolContent.modelCount;
                ScriptsManager.Instance.projectTypeDropdown.value = ScriptsManager._toolContent.projectType;
                ScriptsManager.Instance.clearError.SetActive(true);

                switch (downloadedData.projectType)
                {
                    
                    case 1:
                        CreateDSButtons();
                        yield return StartCoroutine(DownloadAssets(ScriptsManager._toolContent.bundleDetails, ScriptsManager._toolContent.objectNewTransforms));
                        OpenScenes();
                        break;
                    
                }

                CloseExistingProject();
            }
        }
        else
        {
            Debug.Log(www.error);
        }

        if (ScriptsManager._toolContent.isExplode)
        {
            AddExplodeEffect(true);
        }
      

        isDone(true);
    }

    public NewButton returnNewButton(int id)
    {
        NewButton[] a = ScriptsManager.Instance.objectCollection.GetComponentsInChildren<NewButton>(true);
        NewButton[] b = ScriptsManager.Instance.objectCollection.GetComponentsInChildren<NewButton>(false);

        for (int i = 0; i < a.Length; i++)
        {
            if (a[i].customButton.ID == id)
            {
                return a[i];
            }
        }
        for (int i = 0; i < b.Length; i++)
        {
            if (b[i].customButton.ID == id)
            {
                return b[i];
            }
        }
        return null;
    }

  
    //public void AddExplodeEffect(bool enable, List<ExplodeProperty> temp)
    public void AddExplodeEffect(bool enable)
    {
        if (ScriptsManager.Instance.enableDisableComponent.explodePrefab != null)
        {
            ScriptsManager.Instance.isExplodable = true;

            GameObject explodePrefab = new GameObject();
            explodePrefab.name = "Explode Effect";
            explodePrefab.tag = "SceneProperty";
            explodePrefab.transform.SetParent(ScriptsManager.Instance.scenePropertyRect);
            explodePrefab.AddComponent<NoTransform>();

            scenePropertyObjects.Add(explodePrefab);

            ScriptsManager.Instance.featureScript.explosion.sprite = ScriptsManager.Instance.featureScript.explosionSprites[0];



            foreach (var button in ScriptsManager._toolContent.dsButtons)
            {
                ControlFlowButton controlFlowButton = ScriptsManager.Instance.dsButtons[ScriptsManager._toolContent.dsButtons.IndexOf(button)].GetComponentInChildren<ControlFlowButton>();
                if (controlFlowButton.referencedTo.Equals(button.referencedTo))
                {
                    if (button.referencedTo.Equals(AssignButton.Explode.ToString()))
                    {
                        ExplodeComponent explodeComponent = ScriptsManager.Instance.enableDisableComponent.explodePrefab.GetComponent<ExplodeComponent>();
                        explodeComponent.explodeButton = ScriptsManager.Instance.dsButtons[ScriptsManager._toolContent.dsButtons.IndexOf(button)];
                        explodeComponent.explode.isOn = ScriptsManager._toolContent.isExplode;
                        //explodeComponent.ExplodeFunc(explodeComponent.explode);
                        explodeComponent.editExplode.interactable = true;
                        explodeComponent.range.value = ScriptsManager._toolContent.explodeRange;
                        explodeComponent.speed.value = ScriptsManager._toolContent.explodeSpeed;
                        explodeComponent.explodeButton.GetComponentInChildren<Button>().onClick.AddListener(() => { explodeComponent.ExplodeEffect(); });
                        break;
                    }

                }
            }


            ScriptsManager.Instance.enableDisableComponent.instantiateScript.explode = ScriptsManager.Instance.enableDisableComponent.explodePrefab.GetComponentInChildren<UnityEngine.UI.Toggle>();

            if (ScriptsManager.Instance.objectCollection != null && ScriptsManager.Instance.objectCollection.transform.childCount > 0)
            {
                //ThreeDModelFunctions[] threeDFunction = ScriptsManager.Instance.objectCollection.GetComponentsInChildren<ThreeDModelFunctions>();

                //if (threeDFunction.Length > 0)
                //{
                //    for (int i = 0; i < threeDFunction.Length; i++)
                //    {
                //        Transform local = threeDFunction[i].transform;

                //        if (local != null)
                //        {
                //            if (!local.GetComponent<TextContentCanvasHandler>() && !local.CompareTag("Label"))
                //            {
                //                if (local.GetComponent<ThreeDModelFunctions>() && local.gameObject.GetComponentInChildren<Animation>())
                //                {
                //                    local.GetComponent<ThreeDModelFunctions>().canExplode = temp[i].explodeEffect;
                //                    local.GetComponent<ThreeDModelFunctions>().explosionSpeed = temp[i].explodeSpeed;
                //                    local.GetComponent<ThreeDModelFunctions>().explosionDistance = temp[i].explodeRange;

                //                    ScriptsManager.Instance.enableDisableComponent.explodePrefab.GetComponent<ExplodeComponent>().range.value = temp[i].explodeRange;
                //                    ScriptsManager.Instance.enableDisableComponent.explodePrefab.GetComponent<ExplodeComponent>().speed.value = temp[i].explodeSpeed;

                //                    if (local.gameObject.GetComponentInChildren<Animation>().GetClipCount() == 0)
                //                    {
                //                        ScriptsManager.Instance.enableDisableComponent.PopulateCollectionChildren(local);
                //                    }
                //                }
                //            }
                //        }
                //    }
                //}

                //DropReference dropReference = ScriptsManager.Instance.enableDisableComponent.explodePrefab.GetComponentInChildren<DropReference>();
                //InitializeReferenceForButtons(AssignButton.Explode, dropReference);
            }
            else if (ScriptsManager.Instance.enableDisableComponent.objectCollection.transform.childCount == 0)
            {
                Debug.Log("There's no children!");
            }
        }
    }

    //rework on the load a existing project set up
    //by saravanan
    //there are multiple property added .. editing the code to avoid that 
    public void EnableDisableTextContentFeature(List<TextContent> temp, Action<GameObject> gameObjOfButtons = null)
    {
        if (temp.Count > 0)
        {
            ScriptsManager.Instance.panelCount = temp[0].Count;
            ScriptsManager.Instance.textPropertyObj.SetActive(true);

            for (int i = 0; i < temp.Count; i++)
            {

                //   GameObject textProperty = Instantiate(ScriptsManager.Instance.enableDisableComponent.textContentPrefab, ScriptsManager.Instance.propertyRect.transform);
                GameObject panel = Instantiate(ScriptsManager.Instance.descriptionPanelPrefab, ScriptsManager.Instance.scenePropertyRect);

                TextContentComponent propScript = ScriptsManager.Instance.textPropertyObj.GetComponent<TextContentComponent>();

                //ScriptsManager.Instance.enableDisableComponent.AllDesccriptionPanel.Add(propScript);
                ScriptsManager.Instance.enableDisableComponent.AllDesccriptionPanelHierarchy.Add(panel);

                propScript.SubscribeToDelegate();
                propScript.panelProperty = panel.GetComponent<TextContentCanvasHandler>();

                panel.AddComponent<ObjectTransformComponent>();

                panel.name = temp[i].panelName; //"Description panel(" + ScriptsManager.Instance.panelCount.ToString() + ")";
                                                // textProperty.name = "Property_" + temp[i].panelName;
                propScript.panelProperty.gameObject.GetComponent<ObjectTransformComponent>().currentTransform.objectName = panel.name;

                propScript.panelProperty.transform.position = new Vector3(temp[i].transform.position.x, temp[i].transform.position.y, temp[i].transform.position.z);
                propScript.panelProperty.transform.eulerAngles = new Vector3(temp[i].transform.rotation.x, temp[i].transform.rotation.y, temp[i].transform.rotation.z);
                propScript.panelProperty.transform.localScale = new Vector3(temp[i].transform.scale.x, temp[i].transform.scale.y, temp[i].transform.scale.z);

                propScript.panelProperty.gameObject.GetComponent<ObjectTransformComponent>().SetTransform(propScript.panelProperty.transform.position,
                    propScript.panelProperty.transform.eulerAngles, propScript.panelProperty.transform.localScale);

                //propScript.GetContentValue(temp[i]);
                propScript.panelProperty.textContentCurrentObjDetails = temp[i];
                propScript.panelProperty.AssignValues();
                //   textProperty.SetActive(true);

                //  ScriptsManager.Instance.panelCount++;
                // textProperty.GetComponent<TextContentComponent>().SetInactive();

                if (temp[i].visible == false)
                    propScript.panelProperty.gameObject.SetActive(false);

                if (gameObjOfButtons != null)
                {
                    gameObjOfButtons(panel);
                }

                if (ScriptsManager.Instance.projectTypeDropdown.value == 1)
                {
                    //pushing the content type obj to control flow scene objects list
                    ControlFlowManagerV2.Instance.CreateSeneObjectLineItem(panel, ActionStates.TextPanel);
                }

                ScriptsManager.Instance.runtimeHierarchy.Refresh();
                ScriptsManager.Instance.runtimeHierarchy.Select(propScript.panelProperty.transform);
                panel.SetActive(false);
            }
            ScriptsManager.Instance.textPropertyObj.SetActive(false);
        }
    }

    //public void EnableDisableSkyboxFeature(string s)
    //{
    //    if (ScriptsManager.Instance.enableDisableComponent.skyboxPrefab == null)
    //    {
    //        ScriptsManager.Instance.enableDisableComponent.skyboxPrefab = Instantiate(ScriptsManager.Instance.enableDisableComponent.compChangeSkybox, ScriptsManager.Instance.enableDisableComponent.scenePropertiesContent) as GameObject;
    //        ScriptsManager.Instance.enableDisableComponent.skyboxPrefab.transform.SetAsFirstSibling();
    //        ScriptsManager.Instance.enableDisableComponent.skyboxPrefab.GetComponent<ChangeSkybox>().urlInput.text = s;
    //        ScriptsManager.Instance.featureScript.skyBox.sprite = ScriptsManager.Instance.featureScript.skyBoxSprites[1];
    //        if (!ScriptsManager.Instance.enableDisableComponent.skyboxPrefab.activeSelf)
    //        {
    //            ScriptsManager.Instance.enableDisableComponent.skyboxPrefab.SetActive(true);
    //        }
    //        ScriptsManager.Instance.skyAudVid.selectedInputField = ScriptsManager.Instance.enableDisableComponent.skyboxPrefab.GetComponent<ChangeSkybox>().urlInput;
    //    }
    //}

    public void EnableDisableTextToSpeech(List<XRTextToSpeech> temp)
    {
        if (temp.Count > 0)
        {
            if (ScriptsManager.Instance.enableDisableComponent.textToSpeechPrefab != null)
            {
                //ScriptsManager.Instance.enableDisableComponent.textToSpeechPrefab = Instantiate(ScriptsManager.Instance.enableDisableComponent.compTextToSpeech, ScriptsManager.Instance.enableDisableComponent.scenePropertiesContent) as GameObject;
                //ScriptsManager.Instance.enableDisableComponent.textToSpeechPrefab.transform.SetAsFirstSibling();
                ScriptsManager.Instance.enableDisableComponent.textToSpeechPrefab.GetComponent<TextToSpeechComponent>().SubscribeDelegates();
                ScriptsManager.Instance.enableDisableComponent.textToSpeechPrefab.GetComponent<TextToSpeechComponent>().speakTextField.text = temp[0].text;
                ScriptsManager.Instance.enableDisableComponent.textToSpeechPrefab.GetComponent<TextToSpeechComponent>().speechManager.Text = temp[0].text;
                //ScriptsManager.Instance.enableDisableComponent.textToSpeechPrefab.GetComponent<TextToSpeechComponent>().speechManager.PlayOnStart = temp[0].playOnStart;


                ScriptsManager.Instance.featureScript.textToSpeech.sprite = ScriptsManager.Instance.featureScript.textToSpeechSprites[1];

                //if (!ScriptsManager.Instance.enableDisableComponent.textToSpeechPrefab.activeSelf)
                //{
                //    ScriptsManager.Instance.enableDisableComponent.textToSpeechPrefab.SetActive(true);
                //}

                GameObject obj = new GameObject();
                obj.transform.SetParent(ScriptsManager.Instance.scenePropertyRect);
                obj.name = "TextToSpeech";
                obj.tag = "SceneProperty";
                obj.AddComponent<NoTransform>();
                //ScriptsManager.Instance.runtimeHierarchy.Refresh();
                //ScriptsManager.Instance.runtimeHierarchy.Select(obj.transform);
            }
        }
    }

    public IEnumerator EnableDisableAudioFeature(List<AddAudio> temp)
    {
        if (temp.Count > 0)
        {
            ScriptsManager.Instance.enableDisableComponent.EnableDisableAudioFeature(false);

            for (int i = 0; i < temp.Count; i++)
            {
                scenePropertyObjects.Add(ScriptsManager.Instance.enableDisableComponent.CreateAudio_HierarchyObject(temp[i].attachedObject, temp[i].audioURL));


                yield return StartCoroutine(ScriptsManager.Instance.skyAudVid.AddWebLinkToGameObject(temp[i].audioURL));

            }
            ScriptsManager.Instance.isAudio = false;
        }
    }

    //normal video...  not label video data
    private IEnumerator ApplyVideoAttributes(List<AddVideo> addvideos)
    {
        if (addvideos != null && addvideos.Count > 0)
        {
            ScriptsManager.Instance.enableDisableComponent.videoCount = (short)(addvideos[0].totalcount - 1);

            ScriptsManager.Instance.enableDisableComponent.EnableDisableVideoFeature(false);
            for (int i = 0; i < addvideos.Count; i++)
            {
                GameObject videoPrefab = ScriptsManager.Instance.enableDisableComponent.CreateVideo_HierarchyObject(addvideos[i].attachedObject, addvideos[i].videoURL, addvideos[i]);
                //   ScriptsManager.Instance.runtimeHierarchy.Select(obj.transform);

                videoPrefab.GetComponent<NoTransform>().videoComponent.gameObject.SetActive(false);
                videoPrefab.GetComponent<NoTransform>().videoID = addvideos[i].videoID;
                scenePropertyObjects.Add(videoPrefab);
                ApplyGameObjectPositions(videoPrefab);
                //videoPrefab.transform.localScale = addvideos[i].scale;
                //videoPrefab.transform.eulerAngles = addvideos[i].rotation;
                //videoPrefab.transform.position = addvideos[i].position;

                videoPrefab.gameObject.SetActive(addvideos[i].visible);

                yield return StartCoroutine(DownloadFromServer.Instance.VideoGrabber( addvideos[i].videoURL, videoPrefab.GetComponent<NoTransform>().videoComponent));

                //ScriptsManager.Instance.skyAudVid.AddWebLinkToGameObject(addvideos[i].videoURL);

                if (ScriptsManager.Instance.projectTypeDropdown.value == 1)
                {

                    //pushing the content type obj to control flow scene objects list
                    ControlFlowManagerV2.Instance.CreateSeneObjectLineItem(videoPrefab, ActionStates.Video);
                }
            }
            #region comnded for future use 
            //ScriptsManager.Instance.enableDisableComponent.audioPrefab.GetComponent<AudioComponent>().inputAudioClipPath.text = temp[0].audioURL;
            //ScriptsManager.Instance.enableDisableComponent.audioPrefab.SetActive(true);


            //if (ScriptsManager.Instance.enableDisableComponent.videoPrefab == null)
            //{
            //    ScriptsManager.Instance.enableDisableComponent.videoPrefab = Instantiate(ScriptsManager.Instance.enableDisableComponent.compAddVideo, ScriptsManager.Instance.enableDisableComponent.scenePropertiesContent) as GameObject;
            //    ScriptsManager.Instance.enableDisableComponent.videoPrefab.transform.SetAsFirstSibling();
            //    Canvas.ForceUpdateCanvases();

            //    //ScriptsManager.Instance.instantiateScript.obj = Instantiate(ScriptsManager.Instance.instantiateScript.prefab.gameObject);
            //    ScriptsManager.Instance.enableDisableComponent.videoPrefab.GetComponent<VideoComponentScene>().instantiateScript = ScriptsManager.Instance.enableDisableComponent.instantiateScript;
            //    ScriptsManager.Instance.enableDisableComponent.videoPrefab.GetComponent<VideoComponentScene>().inputVideoClipPath.text = addvideos[0].videoURL;
            //    ScriptsManager.Instance.enableDisableComponent.instantiateScript.url = ScriptsManager.Instance.enableDisableComponent.videoPrefab.GetComponent<VideoComponentScene>().inputVideoClipPath;
            //    ScriptsManager.Instance.enableDisableComponent.videoPrefab.GetComponent<VideoComponentScene>().width.value = addvideos[0].width;
            //    ScriptsManager.Instance.enableDisableComponent.instantiateScript.widthSlider = ScriptsManager.Instance.enableDisableComponent.videoPrefab.GetComponent<VideoComponentScene>().width;
            //    ScriptsManager.Instance.enableDisableComponent.instantiateScript.heightSlider = ScriptsManager.Instance.enableDisableComponent.videoPrefab.GetComponent<VideoComponentScene>().height;
            //    ScriptsManager.Instance.enableDisableComponent.instantiateScript.dropDown = ScriptsManager.Instance.enableDisableComponent.videoPrefab.GetComponent<VideoComponentScene>().dropdown;
            //    ScriptsManager.Instance.enableDisableComponent.instantiateScript.heightSlider.value = addvideos[0].height;
            //    ScriptsManager.Instance.enableDisableComponent.instantiateScript.dropDown.value = addvideos[0].AspectRation;
            //    ScriptsManager.Instance.enableDisableComponent.instantiateScript.heightSlider = ScriptsManager.Instance.enableDisableComponent.videoPrefab.GetComponent<VideoComponentScene>().height;
            //    ScriptsManager.Instance.enableDisableComponent.instantiateScript.dropDown = ScriptsManager.Instance.enableDisableComponent.videoPrefab.GetComponent<VideoComponentScene>().dropdown;
            //    ScriptsManager.Instance.featureScript.video.sprite =   ScriptsManager.Instance.featureScript.videoSprites[1];
            //} 
            #endregion
        }
    }

    private IEnumerator DownloadAssets(List<BundleDetails> bundleDetails, List<ObjectTransform> objectTransforms)
    {
        totalAsset = bundleDetails.Count;

        if (totalAsset != 0)
        {
            for (int i = 0; i < totalAsset; i++)
            {
                ScriptsManager.Instance.bundleDetails.Add(bundleDetails[i]);
                yield return StartCoroutine(DownloadAsset(bundleDetails[i], objectTransforms));
            }
        }

        yield return StartCoroutine(GetBundleDetails(bundleDetails));
    }

    void AddLabelData(LabelArrowDetails arrowDetails)
    {
        lineEnd = arrowDetails.lineEnd;
        lineStart = arrowDetails.lineStart;
        label2Objects = arrowDetails.label2Objects;
    }

    private IEnumerator NonModelCall()
    {
        ApplyCustomButtons(ScriptsManager._toolContent.customButtons);
        ApplyLights(ScriptsManager._toolContent);
        AddLabelData(ScriptsManager._toolContent.addArrowDetails);
        yield return StartCoroutine(ApplyLabelwithAudio(ScriptsManager._toolContent.labelWithAudios));
        ApplyLabelwithText(ScriptsManager._toolContent.labelWithTextContents);
        ApplyLabelwithTTS(ScriptsManager._toolContent.labelWithTextToSpeeches);
        

        yield return StartCoroutine(ApplySkyboxChange(ScriptsManager._toolContent.skyboxURL));
        EnableDisableTextContentFeature(ScriptsManager._toolContent.textContent);
        ApplyManipulation(ScriptsManager._toolContent.moveScript, ScriptsManager._toolContent.rotScript, ScriptsManager._toolContent.scaleScript, ScriptsManager._toolContent);

        EnableDisableTextToSpeech(ScriptsManager._toolContent.textToSpeech);
        yield return StartCoroutine(EnableDisableAudioFeature(ScriptsManager._toolContent.addAudio));

      
        //yield return StartCoroutine(ButtonDelay());
        yield return StartCoroutine(ApplyLabel(ScriptsManager._toolContent.addLabel));
        applyVideoCount(ScriptsManager._toolContent);
        ScriptsManager.Instance.GetComponent<raycasthitting>().Count = ScriptsManager._toolContent.totalLabels;
    }

    public void applyVideoCount(ToolContent content)
    {
        ScriptsManager.Instance.videoCount = content.videoCount;
    }

    public void ApplyCustomButtons(List<CustomButtons> customButtons = null, Action<GameObject> gameObjOfButtons = null)
    {
        int buttonCount = customButtons.Count;

        if (buttonCount > 0)
        {
            ButtonCustomizationManager.Instance.uniqueID = buttonCount;

            for (int p = 0; p < buttonCount; p++)
            {
                Debug.Log("........customButtons[p].customButtonName : " + customButtons[p].customButtonName);
                GameObject buttonPreview = Instantiate(ButtonCustomizationManager.Instance.previewPrefab, ButtonCustomizationManager.Instance.parentTransform);
                CustomButton cb = buttonPreview.GetComponent<CustomButton>();

                GameObject button = Instantiate(ButtonCustomizationManager.Instance.buttonPrefab, ScriptsManager.Instance.objectCollection.transform);
                NewButton newButton = button.GetComponentInChildren<NewButton>();

                button.name = customButtons[p].customButtonName;
                button.AddComponent<ObjectTransformComponent>();
                button.GetComponent<ObjectTransformComponent>().currentTransform.objectFullPath = ScriptsManager.Instance.GetObjectFullPath(button);
                button.GetComponent<ObjectTransformComponent>().currentTransform.objectName = button.name;


                ApplyGameObjectPositions(button);

                cb.buttonType = (ButtonType)customButtons[p].buttonType;
                cb.referencedTo = customButtons[p].referencedTo;
                cb.buttonObj = button;
                buttonPreview.name = cb.customButtonName = button.name;
                cb.attributesManager = ButtonCustomizationManager.Instance.attributesManager;
                cb.image = button.GetComponentInChildren<Image>();
                cb.imageText = button.GetComponentInChildren<TMP_Text>();
                cb.ID = customButtons[p].ID;
                cb.name = customButtons[p].name;
                cb.alternateName = customButtons[p].alternateName;
                cb.width = customButtons[p].width;
                cb.height = customButtons[p].height;
                cb.fontSize = customButtons[p].fontSize;
                cb.normalColor = customButtons[p].normalColor;
                cb.highlightColor = customButtons[p].highlightColor;
                cb.pressedColor = customButtons[p].pressedColor;
                cb.textNormal = customButtons[p].textNormal;
                cb.textHighlight = customButtons[p].textHighlight;
                cb.path = customButtons[p].path;
                cb.nameText.text = cb.imageText.text = cb.name;
                cb.defaultSprite = cb.attributesManager.defaultSprite;
                cb.nameText.gameObject.SetActive(false);
                cb.buttonObj.GetComponentInChildren<RectTransform>().sizeDelta = new Vector2(cb.width, cb.height);
                cb.imageText.fontSize = cb.fontSize;
                cb.image.color = cb.normalColor;
                cb.normalImageURL = customButtons[p].normalImageURL;
                cb.highlightImageURL = customButtons[p].highlightImageURL;
                cb.pressedImageURL = customButtons[p].pressedImageURL;
                cb.normalImageText = ScriptsManager.Instance.ExtensionRemover(customButtons[p].normalImageURL);
                cb.highLightImageText = ScriptsManager.Instance.ExtensionRemover(customButtons[p].highlightImageURL);
                cb.pressedImageText = ScriptsManager.Instance.ExtensionRemover(customButtons[p].pressedImageURL);

                ButtonCustomizationManager.Instance.prefabsList.Add(cb);

                newButton.customButton = cb;
                newButton.buttonType = cb.buttonType;
                newButton.normalName = customButtons[p].name;
                newButton.alternateName = customButtons[p].alternateName;
                newButton.normalColor = customButtons[p].normalColor;
                newButton.highlightColor = customButtons[p].highlightColor;
                newButton.pressedColor = customButtons[p].pressedColor;
                newButton.textNormalColor = newButton.buttonText.color = customButtons[p].textNormal;
                newButton.textHighlightColor = customButtons[p].textHighlight;
                newButton.eventTrigger = button.GetComponentInChildren<EventTrigger>();
                newButton.GetComponentInChildren<EventTrigger>().enabled = newButton.customButton.referencedTo != string.Empty ? true : false;
                if (!ButtonCustomizationManager.Instance.dict.ContainsKey(newButton.customButton.ID))
                    ButtonCustomizationManager.Instance.dict.Add(newButton.customButton.ID, newButton);
                StartCoroutine(DownloadButSprites(newButton, customButtons[p], cb, (boool) =>
                {
                    if (boool == true)
                    {
                        if (gameObjOfButtons != null)
                        {
                            gameObjOfButtons(button);
                        }
                    }
                }));
            }
        }
    }

    

    private IEnumerator DownloadButSprites(NewButton newButton, CustomButtons customButtons, CustomButton cb, Action<bool> completed)
    {
        yield return StartCoroutine(DownloadImageRoutine(customButtons.normalImageURL, (data) =>
        {
            newButton.normalImage = data;
        }));

        yield return StartCoroutine(DownloadImageRoutine(customButtons.highlightImageURL, (data) =>
        {
            newButton.highlightImage = data;
        }));

        yield return StartCoroutine(DownloadImageRoutine(customButtons.pressedImageURL, (data) =>
        {
            newButton.pressedImage = data;
        }));


        cb.normalImage = newButton.normalImage;
        cb.highlightImage = newButton.highlightImage;
        cb.pressedImage = newButton.pressedImage;
        cb.thisButton.sprite = cb.normalImage;
        cb.nameText.gameObject.SetActive(false);

        cb.AssignAttributeValues();
        cb.InitAndSubscribeDelegate();

        newButton.TypeOfButton();

        completed(true);
    }

    private IEnumerator DownloadImageRoutine(string url, Action<Sprite> finished)
    {
        Texture2D texture = new Texture2D(4, 4, TextureFormat.DXT1, false);

        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        DownloadFromServer.Instance.DownloadingProgress(www);
        yield return www.SendWebRequest();

        if (!www.isNetworkError || !www.isHttpError)
        {
            if (!String.IsNullOrEmpty(url))
            {
                DownloadHandlerTexture handler = (DownloadHandlerTexture)www.downloadHandler;
                yield return handler.isDone;

                if (handler.isDone)
                {
                    texture = ((DownloadHandlerTexture)www.downloadHandler).texture as Texture2D;

                    Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one / 2);

                    newSprite = sprite;
                }
            }
        }
        finished(newSprite);
    }

    protected void ApplyLights(ToolContent data)
    {
        if (data.lightComponents.Count > 0)
        {
            ScriptsManager.Instance.sceneLight.SetActive(false);
            for (int p = 0; p < data.lightComponents.Count; p++)
            {
                try
                {
                    if (data.lightComponents[p].path != "")
                    {
                        string name = ScriptsManager.Instance.ExtensionRemover(data.lightComponents[p].path);

                        GameObject light = Instantiate(ScriptsManager.Instance.lightPrefab, ScriptsManager.Instance.scenePropertyRect);
                        light.name = name;
                        light.AddComponent<NoTransform>();

                        NoTransform noTransform = light.GetComponent<NoTransform>();

                        noTransform.propertyType = TypesOfProperty.Light;
                        noTransform.componentObject = light;

                        scenePropertyObjects.Add(light);

                        LightComponent componentLight = ScriptsManager.Instance.lightComponent.GetComponent<LightComponent>();
                        componentLight.addLights.lightsCount = data.lightComponents.Count;
                        componentLight.noTransform = light.GetComponent<NoTransform>();
                        componentLight.light = light.GetComponent<Light>();

                        noTransform.lightComponent = componentLight;
                        noTransform.light = componentLight.light;
                        noTransform.lightValue = data.lightComponents[p].lightDropDownValue;
                        noTransform.light.type = (LightType)noTransform.lightValue;
                        noTransform.colorValue = data.lightComponents[p].lightColor;
                        noTransform.intensity = data.lightComponents[p].intensity;
                        noTransform.indirectMultiplier = data.lightComponents[p].indirectMultiplier;
                        noTransform.angleValue = data.lightComponents[p].angle;
                        noTransform.rangeValue = data.lightComponents[p].range;
                        noTransform.angleValue = data.lightComponents[p].sliderValue;
                        noTransform.GetLightPath(noTransform.light.gameObject);

                        noTransform.light.color = noTransform.colorValue;
                        noTransform.light.intensity = noTransform.intensity;
                        noTransform.light.bounceIntensity = noTransform.indirectMultiplier;
                        noTransform.light.spotAngle = noTransform.angleValue;
                        noTransform.light.range = noTransform.rangeValue;

                        ApplyGameObjectPositions(light);
                        light.SetActive(data.lightComponents[p].isActive);

                        componentLight.lightDropdown.value = data.lightComponents[p].lightDropDownValue;
                        componentLight.rangeField.text = data.lightComponents[p].range.ToString();
                        componentLight.angleSlider.value = data.lightComponents[p].sliderValue;
                        componentLight.angleField.text = data.lightComponents[p].sliderValue.ToString();
                        componentLight.intensityField.text = data.lightComponents[p].intensity.ToString();
                        componentLight.indirectMultiplierField.text = data.lightComponents[p].indirectMultiplier.ToString();
                        componentLight.colorField.color = data.lightComponents[p].lightColor;

                        ScriptsManager.Instance.featureScript.lightImg.sprite = ScriptsManager.Instance.featureScript.lightSprites[0];
                    }
                }
                catch (Exception)
                {

                }
            }
        }
    }

    private IEnumerator ApplyLabel(List<AddLabel> addlabel)
    {
        yield return StartCoroutine(ApplyVideoAttributes(ScriptsManager._toolContent.addVideo));

        if (addlabel.Count > 0)
        {
            for (int i = 0; i < addlabel.Count; i++)
            {
                labelName.Add(addlabel[i].labelName);
                name.Add(addlabel[i].name);
                videoEnabled.Add(addlabel[i].videoEnabled);
                audioEnabled.Add(addlabel[i].audioEnabled);
                descriptionEnabled.Add(addlabel[i].descriptionEnabled);
                text2SpeechEnabled.Add(addlabel[i].text2SpeechEnabled);
                audioDetails.Add(addlabel[i].audioDetails);
                videoDetails.Add(addlabel[i].videoDetails);
                textDetails.Add(addlabel[i].textDetails);
                textToSpeechDetails.Add(addlabel[i].textToSpeechDetails);
                labelgrp.Add(addlabel[i].name);
            }

            

        }
    }

    void ApplyManipulation(bool move, bool rotate, bool scale, ToolContent temp)
    {
        if (temp.isGesture)
        {
            m_ScriptsManager = ScriptsManager.Instance;

            GameObject manipulationPrefab = new GameObject("Gestures");
            manipulationPrefab.tag = "SceneProperty";
            manipulationPrefab.transform.SetParent(m_ScriptsManager.scenePropertyRect);
            manipulationPrefab.AddComponent<NoTransform>();

            ManipulationComponent manipulationComponent = m_ScriptsManager.manipulationComponent;
            m_ScriptsManager.isGesture = temp.isGesture;
            manipulationComponent.SubscribeToDelegate();

            foreach (var button in temp.dsButtons)
            {
                ControlFlowButton controlFlowButton = m_ScriptsManager.dsButtons[temp.dsButtons.IndexOf(button)].GetComponentInChildren<ControlFlowButton>();
                if (controlFlowButton.referencedTo.Equals(button.referencedTo))
                {
                    if (button.referencedTo.Equals(AssignButton.GestureMove.ToString()))
                    {
                        manipulationComponent.moveButton = m_ScriptsManager.dsButtons[temp.dsButtons.IndexOf(button)];
                        manipulationComponent.move.isOn = move;
                        manipulationComponent.MoveFunc(manipulationComponent.move);
                    }
                    else if (button.referencedTo.Equals(AssignButton.GestureRotate.ToString()))
                    {
                        manipulationComponent.rotateButton = m_ScriptsManager.dsButtons[temp.dsButtons.IndexOf(button)];
                        manipulationComponent.rotate.isOn = rotate;
                        manipulationComponent.RotateFunc(manipulationComponent.rotate);
                    }
                    else if (button.referencedTo.Equals(AssignButton.GestureScale.ToString()))
                    {
                        manipulationComponent.scaleButton = m_ScriptsManager.dsButtons[temp.dsButtons.IndexOf(button)];
                        manipulationComponent.scale.isOn = scale;
                        manipulationComponent.ScaleFunc(manipulationComponent.scale);
                    }
                }
            }
        }
    }

    public IEnumerator ApplySkyboxChange(string skyboxUrl)
    {
        if (!string.IsNullOrEmpty(skyboxUrl))
        {
            ScriptsManager.Instance.skuboxURL = skyboxUrl;

            yield return StartCoroutine(DownloadSkybox(skyboxUrl));
            //EnableDisableSkyboxFeature(skyboxUrl);
            ScriptsManager.Instance.enableDisableComponent.skyboxPrefab.GetComponent<ChangeSkybox>().urlInput.text = skyboxUrl;
            ScriptsManager.Instance.enableDisableComponent.EnableDisableSkyboxFeature(false);
            ScriptsManager.Instance.isSkybox = false;
            ScriptsManager.Instance.enableDisableComponent.skyboxPrefab.SetActive(false);
        }

        ScriptsManager.Instance.currentObjectName.text = "";
    }

    IEnumerator DownloadSkybox(string url)
    {
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(url))
        {
            yield return uwr.SendWebRequest();

            if (uwr.isNetworkError || uwr.isHttpError)
            {
                Debug.Log(uwr.error);
            }
            else
            {
                //   Debug.LogError("skyboxurl"+url);
                //   ScriptsManager._XRStudioContent.skyboxURL = url;
                var texture = DownloadHandlerTexture.GetContent(uwr);
                skyboxMaterial.SetTexture("_Tex", texture);
                RenderSettings.skybox = skyboxMaterial;
            }
        }
    }

    private IEnumerator DownloadAsset(BundleDetails bundleDetails = null, List<ObjectTransform> objectTransforms = null)
    {
        string fileName = bundleDetails.fileName.Split('.')[0];

        ObjectTransform objectTransform = null;

        foreach (var item in objectTransforms)
        {
            if (item.objectName == bundleDetails.gameObjectName)
            {
                objectTransform = item;
                break;
            }
        }

        yield return StartCoroutine(GetAssetBundle(bundleDetails, fileName, objectTransform));
    }

    private IEnumerator GetAssetBundle(BundleDetails bundleDetails, string assetName, ObjectTransform objectTransform)
    {
        UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle("http://xrstudio.eastus.cloudapp.azure.com/" + bundleDetails.bundleWindowsURL);
        DownloadFromServer.Instance.DownloadingProgress(www);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(www);

            GameObject emptyObject = new GameObject();
            emptyObject.transform.parent = ScriptsManager.Instance.objectCollection.transform;

            GameObject newObject = Instantiate(bundle.LoadAsset(assetName)) as GameObject;
            if (newObject.GetComponent<Animation>())
            {
                Destroy(newObject.GetComponent<Animation>());
            }
            else if (newObject.GetComponent<Animator>())
            {
                Destroy(newObject.GetComponent<Animator>());
            }

            ScriptsManager.Instance.bundleCount++;
            Renderer[] meshRenderer = newObject.GetComponentsInChildren<Renderer>();
            Bounds bounds = meshRenderer[0].bounds;

            foreach (Renderer rend in meshRenderer)
            {
                if (rend.enabled)
                {
                    bounds = bounds.GrowBounds(rend.bounds);
                }
            }
            Vector3 center = bounds.center;

            List<GameObject> renderObjects = new List<GameObject>();
            for (int i = 0; i < meshRenderer.Length; i++)
            {
                renderObjects.Add(meshRenderer[i].gameObject);
            }
            for (int i = 0; i < renderObjects.Count; i++)
            {
                renderObjects[i].AddComponent<MeshCollider>();
            }
            bundle.Unload(false);
            if (newObject.GetComponentInChildren<Camera>())
            {
                newObject.GetComponentInChildren<Camera>().enabled = false;
            }
            emptyObject.name = bundleDetails.gameObjectName;
            newObject.name = assetName;
            newObject.transform.position = Vector3.zero;
            newObject.transform.parent = emptyObject.transform;

            if (!emptyObject.GetComponent<ThreeDModelFunctions>())
            {
                emptyObject.AddComponent<ThreeDModelFunctions>();
            }

            emptyObject.GetComponent<ThreeDModelFunctions>().objDetails = bundleDetails.assetDetails;

            ScriptsManager.Instance.downloadedObjects.Add(emptyObject);
            ScriptsManager.Instance.AssetCount++;
            DownloadedAsset++;

            //SetObjectNewTransforms();
            Transform[] childTransforms = emptyObject.GetComponentsInChildren<Transform>();

            for (int p = 0; p < childTransforms.Length; p++)
            {
                ApplyGameObjectPositions(childTransforms[p].gameObject);
            }

            if (ScriptsManager.Instance.projectTypeDropdown.value == 1)
            {
                //pushing the content type obj to control flow scene objects list
                ControlFlowManagerV2.Instance.CreateSeneObjectLineItem(emptyObject, ActionStates.Model);
            }
            else if (ScriptsManager.Instance.projectTypeDropdown.value == 0)
            {
                ScriptsManager.Instance.CreateModelObjectLineItem(emptyObject);
            }
        }
    }

    private IEnumerator GetBundleDetails(List<BundleDetails> bundleDetails)// Need to check this.
    {
        yield return StartCoroutine(NonModelCall());

        if (ScriptsManager.Instance.downloadedObjects.Count != 0)
        {
            for (int i = 0; i < ScriptsManager.Instance.downloadedObjects.Count; i++)
            {
                ChildActiveState(ScriptsManager.Instance.downloadedObjects[i].transform, bundleDetails[i]);
            }
        }
    }

    private void ChildActiveState(Transform parent, BundleDetails bundleDetails)
    {
        Transform[] childTransforms = parent.GetComponentsInChildren<Transform>(true);

        for (int p = 0; p < ScriptsManager._toolContent.activeObjects.Count; p++)
        {
            if (bundleDetails.gameObjectFullPath == ScriptsManager._toolContent.activeObjects[p].parentFullPath)
            {
                for (int j = 0; j < ScriptsManager._toolContent.activeObjects[p].activeObjectName.Count; j++)
                {
                    if (childTransforms[j].name == ScriptsManager._toolContent.activeObjects[p].activeObjectName[j])
                    {
                        if (childTransforms[j].name.Equals("Canvas (1)"))
                        {
                            childTransforms[j].gameObject.SetActive(true);
                        }
                        else if (childTransforms[j].GetComponent<TextContentCanvasHandler>())
                        {
                            childTransforms[j].gameObject.SetActive(false);
                        }
                        else
                        {
                            childTransforms[j].gameObject.SetActive(ScriptsManager._toolContent.activeObjects[p].activeState[j]);
                        }
                    }
                }
            }
        }
    }

    private void CreateButtons(int count, List<PublishedContentFileList> list)
    {
        for (int p = 0; p < count; p++)
        {
            GameObject button = Instantiate(buttonPrefab, buttonParent);
            ExistingProjectButton existingButton = button.GetComponent<ExistingProjectButton>();

            existingButton.buttonID = p;
            existingButton.projectManager = this;
            existingButton.normalColor = normalColor;
            existingButton.selectedColor = selectedColor;
            existingButton.status.text = list[p].Status;
            existingButton.projectName = list[p].FileName;
            existingButton.buttonName.text = list[p].FileName.Split('.')[0];

            universalTime = DateTime.Parse(list[p].UpdatedOn);
            localTime = universalTime.ToLocalTime();
            existingButton.lastModified.text = localTime.ToString();

            buttons.Add(button);
        }
    }

    private string lastModified(DateTime lastModified)
    {
        string time = "";

        string hour = PaddingText(lastModified.Hour);
        string minute = PaddingText(lastModified.Minute);
        string second = PaddingText(lastModified.Second);

        time = hour + ":" + minute + ":" + second;

        Debug.Log($"Time - {time}");
        return time;
    }

    private string PaddingText(int time)
    {
        return time.ToString().PadLeft(2, '0');
    }

    private void RemoveButtons()
    {
        if (buttons.Count > 0)
        {
            foreach (GameObject item in buttons)
            {
                Destroy(item);
            }

            submitButton.interactable = false;
            buttons.Clear();
        }
    }

    public void Submit()
    {
        if (selectedProjectName.Length > 0)
        {
            DownloadProject();
        }
    }

 

    private void OpenScenes()
    {
        ScriptsManager scriptsManager = ScriptsManager.Instance;

        ControlFlowManagerV2.Instance.EnableCameraFrustum(true);
        scriptsManager.projectSelector.SetActive(false);
        scriptsManager.cloudSpace.SetActive(true);
        scriptsManager.mainScene.SetActive(true);
        scriptsManager.rtEditor.SetActive(true);
        if (scriptsManager.rte != null)
        {
            scriptsManager.rte.SetActive(true);
            scriptsManager.runtimeComponent.SetActive(true);
        }
        scriptsManager.featuresPanel.SetActive(false);
        scriptsManager.featuresPropertiesPanel.SetActive(true);
        scriptsManager.modelHierarchyPanel.SetActive(false);

        scriptsManager.previewButton.SetActive(true);
        scriptsManager.controlFlowUI.SetActive(true);
        scriptsManager.mainCamera.GetComponent<TransformGizmo>().enabled = true;
        scriptsManager.projectTitleText.text = selectedProjectName.Split('.')[0];
        scriptsManager.objectCollection.SetActive(true);
    }

    

   

    public IEnumerator PopulateLabels()
    {
        if (lineEnd != null)
        {
            for (int i = 0; i < lineEnd.Count; i++)
            {
                //  GameObject temp =  GameObject.Find(addlabel[0].label2Objects[0]);
                // print(temp==null);
                GameObject startL = new GameObject();
                startL.transform.position = lineStart[i];
                startL.name = "Start";

                GameObject temp = null;

                yield return new WaitUntil(() => temp = GameObject.Find(label2Objects[i]));

                startL.transform.SetParent(temp.transform);
                GameObject obj = Instantiate(prefab, lineEnd[i], Quaternion.identity);
                obj.name = ScriptsManager._toolContent.addLabel[i].name;//i.ToString() + " Label";
             //   obj.GetComponent<ObjectNavigator>().root = obj;

               
                obj.transform.parent = startL.transform.parent;
                obj.transform.GetComponentInChildren<UnityEngine.UI.Text>().text = labelName[i];

                ApplyGameObjectPositions(obj);

                GameObject endL = new GameObject();
                endL.transform.position = lineEnd[i];
                endL.name = "End";
                endL.transform.SetParent(obj.transform);

                LineRenderer lineR = obj.AddComponent<LineRenderer>();

                lineR.material = new Material(Shader.Find("Sprites/Default"));
                lineR.widthMultiplier = 0.01f;
                lineR.positionCount = 2;
                float alpha = 1.0f;
                Gradient gradient = new Gradient();
                gradient.SetKeys(
                    new GradientColorKey[] { new GradientColorKey(c1, 0.0f), new GradientColorKey(c2, 1.0f) },
                    new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
                );

                //  Debug.Log(addlabel[0].label2Objects);
                lineR.colorGradient = gradient;
                lineR.SetPosition(0, startL.transform.position);
                lineR.SetPosition(1, endL.transform.position);
                lineR.sortingOrder = -1;
                // lineTemp.LR = lineR;
                listLabel.Add(obj);
                spawnedLabels.Add(startL);
                spawnedLabels.Add(endL);
                spawnedLabels.Add(obj);

             

                Label_Publish local = obj.GetComponent<Label_Publish>();
                local.componentCombo = 0;
                local.parentTransform = temp.transform;
                local.labelToObject = label2Objects[i];

                //label video data
                if (videoEnabled[i])
                {
                    local.EnableVideoReload();
                    local.applyIcon(1);
                    local.componentCombo = 2;
                    local.videoComponent = ScriptsManager.Instance.videoPropertyObj.GetComponent<VideoComponentScene>();
                    //local.videoHeight = videoDetails[i].height;
                    local.aspectRatioValue = videoDetails[i].aspectRatio;
                    local.videoComponent.videoType = ComponentType.Label;
                    local.videoComponent.labelPublish = local;
                    local.CreateVideoPlane(videoDetails[i].position, false, true);
                    local.ThisVideo.transform.eulerAngles = videoDetails[i].rotation;
                    local.ThisVideo.transform.localScale = videoDetails[i].scale;
                    yield return StartCoroutine(DownloadFromServer.Instance.VideoGrabber(videoDetails[i].videoURL, local.videoComponent));
                }

                if (isBatchingEnabled[i])
                {
                    obj.GetComponent<Label_Publish>().isBatchingEnable = isBatchingEnabled[i];
                }
            }

       
            //if (ScriptsManager._XRStudioContent.fromSavedFile.batchingButtonsActive)
                //SetGroups();
        }
        else
        {
            print("linestart or end null");
        }
    }

  

  
   

    public void ProcessLabelEvent(int i)
    {

        if (videoEnabled[i])
        {
            ProcessLabelData(enumData.video, i);
        }
        else if (audioEnabled[i])
        {
            ProcessLabelData(enumData.audio, i);
        }
        else if (text2SpeechEnabled[i])
        {
            ProcessLabelData(enumData.text2speech, i);
        }
        else if (descriptionEnabled[i])
        {
            ProcessLabelData(enumData.text, i);
        }
        else
        {
            ProcessLabelData(enumData.label, i);
        }
    }

    void ProcessLabelData(enumData value, int index)
    {
        switch (value)
        {
            case enumData.label:
                break;
            case enumData.video:
                videoPrefab.SetActive(true);
                vidPlayerObj = videoPrefab.GetComponent<VideoPlayer>();
                vidPlayerObj.url = videoDetails[index].videoURL;
                ScriptsManager.Instance.LocalLogs(videoDetails[index].position.ToString());
                Vector3 vidTemp = vidPlayerObj.transform.localScale;
                vidTemp.x = videoDetails[index].height * 1f;
                vidTemp.z = videoDetails[index].height * 0.57f;
                vidPlayerObj.transform.localScale = vidTemp;

                Debug.Log($"Height Value - {videoDetails[index].height}.");

                //videoPrefab.transform.localPosition = videoDetails [index].position;
                //videoPrefab.transform.localEulerAngles = videoDetails [index].rotation;
                //prefab.GetComponent<VideoPlayer>().targetTexture.width = 700;

                RenderTexture newText = new RenderTexture(1268, 720, 24);
                newText.useDynamicScale = true;
                newText.dimension = UnityEngine.Rendering.TextureDimension.Tex2D;
                vidPlayerObj.targetTexture = newText;
                videoPrefab.GetComponent<MeshRenderer>().sharedMaterial.mainTexture = newText;
                vidPlayerObj.aspectRatio = (VideoAspectRatio)videoDetails[index].aspectRatio;// aspectRatioFind(aspectR);
                                                                                             //vidPlayerObj.Play();
                break;
            case enumData.audio:
                break;
            case enumData.text:
                break;
            case enumData.text2speech:
                break;
        }
    }

    private IEnumerator ApplyLabelwithAudio(List<LabelWithAudio> data)
    {
        if (data.Count > 0)
        {
            for (int i = 0; i < data.Count; i++)
            {
                GameObject startL = new GameObject();
                startL.transform.position = data[i].lineStartPoint;
                startL.name = "Start";

                GameObject parentTransform = GameObject.Find(data[i].attachedObjectFullPath.Replace("/" + data[i].attachedObjectName, ""));
                startL.transform.SetParent(parentTransform.transform);

                GameObject obj = Instantiate(prefab, data[i].lineEndPoint, Quaternion.identity);

                Label_Publish temp = obj.GetComponent<Label_Publish>();
                temp.applyIcon(2);
                temp.componentCombo = 3;
                temp.parentTransform = parentTransform.transform;
                temp.labelWithAudio = data[i];
                temp.selectedPropertyType = PropertyType.LabelWithAudio;
                ScriptsManager.Instance.GetComponent<Label_components>().labelWithAudioProperty.GetComponent<LabelWithAudioProperty>().label_Publish = temp;
                yield return StartCoroutine(DownloadFromServer.Instance.AudioGrabber( temp.labelWithAudio.audioDetails.audioURL, null
                         , ScriptsManager.Instance.GetComponent<Label_components>().labelWithAudioProperty.GetComponent<LabelWithAudioProperty>()));
                temp.textObj.GetComponentInParent<Button>().onClick.RemoveListener(temp.reset);
                temp.textObj.GetComponentInParent<Button>().onClick.AddListener(temp.ShowProperty);
                temp.textObj.text = data[i].labelText.Equals("") ? "Enter label name" : data[i].labelText;

                obj.name = data[i].attachedObjectName;
             //   obj.GetComponent<ObjectNavigator>().root = obj;
                obj.transform.SetParent(startL.transform.parent);

                ApplyGameObjectPositions(obj);

                GameObject endL = new GameObject();
                endL.transform.position = data[i].lineEndPoint;
                endL.name = "End";
                endL.transform.SetParent(obj.transform);

                LineRenderer lineR = obj.AddComponent<LineRenderer>();

                lineR.material = new Material(Shader.Find("Sprites/Default"));
                lineR.widthMultiplier = 0.01f;
                lineR.positionCount = 2;
                float alpha = 1.0f;
                Gradient gradient = new Gradient();
                gradient.SetKeys(
                    new GradientColorKey[] { new GradientColorKey(c1, 0.0f), new GradientColorKey(c2, 1.0f) },
                    new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
                );

                //  Debug.Log(addlabel[0].label2Objects);
                lineR.colorGradient = gradient;
                lineR.SetPosition(0, startL.transform.position);
                lineR.SetPosition(1, endL.transform.position);
                lineR.sortingOrder = -1;
                // lineTemp.LR = lineR;
                listLabel.Add(obj);
                spawnedLabels.Add(startL);
                spawnedLabels.Add(endL);
                spawnedLabels.Add(obj);

                label2Objects.Add(data[i].attachedObjectFullPath);

               
            }

        

            //if (ScriptsManager._XRStudioContent.fromSavedFile.batchingButtonsActive)
            //    SetGroups();
            // DisableLabels();

        }
        else
        {
            print("linestart or end null");
        }
    }

    void ApplyLabelwithText(List<LabelWithTextContent> data)
    {
        if (data.Count > 0)
        {
            print(" populate labels called 1");
            for (int i = 0; i < data.Count; i++)
            {
                GameObject startL = new GameObject();
                startL.transform.position = data[i].lineStartPoint;
                startL.name = "Start";

                Transform parentTransform = GameObject.Find(data[i].attachedObjectFullPath.Replace("/" + data[i].attachedObjectName, "")).transform;
                startL.transform.SetParent(parentTransform);

                GameObject obj = Instantiate(prefab, data[i].lineEndPoint, Quaternion.identity);

                Label_Publish temp = obj.GetComponent<Label_Publish>();
                temp.applyIcon(4);
                temp.componentCombo = 3;
                temp.parentTransform = parentTransform;
                temp.labelWithTextContent = data[i];
                temp.selectedPropertyType = PropertyType.LabelWithDescription;
                temp.textObj.GetComponentInParent<Button>().onClick.RemoveListener(temp.reset);
                temp.textObj.GetComponentInParent<Button>().onClick.AddListener(temp.ShowProperty);
                temp.DescriptionPanelReload(data[i].contentDetails.transform);
                temp.textObj.text = data[i].labelText.Equals("") ? "Enter label name" : data[i].labelText;

                obj.name = data[i].attachedObjectName;
               // obj.GetComponent<ObjectNavigator>().root = obj;
                obj.transform.SetParent(startL.transform.parent);

                LabelWithTextContentProperty labelWithTextContentProperty = ScriptsManager.Instance.GetComponent<Label_components>().labelWithTextContentProperty.GetComponent<LabelWithTextContentProperty>();
                labelWithTextContentProperty.gameObject.SetActive(!labelWithTextContentProperty.gameObject.activeSelf);
                labelWithTextContentProperty.label_Publish = temp;
                labelWithTextContentProperty.SetPropertyValue();
                labelWithTextContentProperty.label_Publish.descriptionPanelObj.SetActive(false);
                labelWithTextContentProperty.gameObject.SetActive(false);

                ApplyGameObjectPositions(obj);

                GameObject endL = new GameObject();
                endL.transform.position = data[i].lineEndPoint;
                endL.name = "End";
                endL.transform.SetParent(obj.transform);

                LineRenderer lineR = obj.AddComponent<LineRenderer>();

                lineR.material = new Material(Shader.Find("Sprites/Default"));
                lineR.widthMultiplier = 0.01f;
                lineR.positionCount = 2;
                float alpha = 1.0f;
                Gradient gradient = new Gradient();
                gradient.SetKeys(
                    new GradientColorKey[] { new GradientColorKey(c1, 0.0f), new GradientColorKey(c2, 1.0f) },
                    new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
                );

                //  Debug.Log(addlabel[0].label2Objects);
                lineR.colorGradient = gradient;
                lineR.SetPosition(0, startL.transform.position);
                lineR.SetPosition(1, endL.transform.position);
                lineR.sortingOrder = -1;
                // lineTemp.LR = lineR;
                listLabel.Add(obj);
                spawnedLabels.Add(startL);
                spawnedLabels.Add(endL);
                spawnedLabels.Add(obj);

                label2Objects.Add(data[i].attachedObjectFullPath);

               
            }

            //if (ScriptsManager._XRStudioContent.fromSavedFile.batchingButtonsActive)
            //    SetGroups();
            // DisableLabels();

        }
        else
        {
            print("linestart or end null");
        }



    }

    void ApplyLabelwithTTS(List<LabelWithTextToSpeech> data)
    {
        //  print(" populate labels called 0");
        if (data.Count > 0)
        {
            print(" populate labels called 1");
            for (int i = 0; i < data.Count; i++)
            {
                GameObject startL = new GameObject();
                startL.transform.position = data[i].lineStartPoint;
                startL.name = "Start";

                Transform parentTransform = GameObject.Find(data[i].attachedObjectFullPath.Replace("/" + data[i].attachedObjectName, "")).transform;
                startL.transform.SetParent(parentTransform);

                GameObject obj = Instantiate(prefab, data[i].lineEndPoint, Quaternion.identity);
                Label_Publish temp = obj.GetComponent<Label_Publish>();
                temp.applyIcon(3);
                temp.componentCombo = 3;
                temp.parentTransform = parentTransform;
                temp.labelWithTTS = data[i];
                temp.selectedPropertyType = PropertyType.LabelWithDescription;
                temp.textObj.GetComponentInParent<Button>().onClick.RemoveListener(temp.reset);
                temp.textObj.GetComponentInParent<Button>().onClick.AddListener(temp.ShowProperty);
                temp.selectedPropertyType = PropertyType.LabelWithTextToSpeech;
                temp.textObj.text = data[i].labelText.Equals("") ? "Enter label name" : data[i].labelText;

                obj.name = data[i].attachedObjectName;
              //  obj.GetComponent<ObjectNavigator>().root = obj;
                obj.transform.SetParent(startL.transform.parent);

                ApplyGameObjectPositions(obj);

                GameObject endL = new GameObject();
                endL.transform.position = data[i].lineEndPoint;
                endL.name = "End";
                endL.transform.SetParent(obj.transform);

                LineRenderer lineR = obj.AddComponent<LineRenderer>();

                lineR.material = new Material(Shader.Find("Sprites/Default"));
                lineR.widthMultiplier = 0.01f;
                lineR.positionCount = 2;
                float alpha = 1.0f;
                Gradient gradient = new Gradient();
                gradient.SetKeys(
                    new GradientColorKey[] { new GradientColorKey(c1, 0.0f), new GradientColorKey(c2, 1.0f) },
                    new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
                );

                //  Debug.Log(addlabel[0].label2Objects);
                lineR.colorGradient = gradient;
                lineR.SetPosition(0, startL.transform.position);
                lineR.SetPosition(1, endL.transform.position);
                lineR.sortingOrder = -1;
                // lineTemp.LR = lineR;
                listLabel.Add(obj);
                spawnedLabels.Add(startL);
                spawnedLabels.Add(endL);
                spawnedLabels.Add(obj);

                label2Objects.Add(data[i].attachedObjectFullPath);

               
            }

            //ObjectNav_Manager.Instance.objRayCast.labels = listLabel;

            //if (ScriptsManager._XRStudioContent.fromSavedFile.batchingButtonsActive)
            //    SetGroups();
            // DisableLabels();

        }
        else
        {
            print("linestart or end null");
        }
    }

   

    /// <summary>
    /// Positions the object to the saved locations. Created by Jeffri.
    /// </summary>
    /// <param name="obj"></param>
    private void ApplyGameObjectPositions(GameObject obj)
    {
        var objectTransforms = ScriptsManager._toolContent.objectNewTransforms;

        for (int j = 0; j < objectTransforms.Count; j++)
        {
            GameObject temp = GameObject.Find(objectTransforms[j].objectFullPath);

            if (temp != null)
            {
                Debug.Log($"Name: {temp.name}");
                temp.name = objectTransforms[j].objectName;

                if (obj.name == temp.name)
                {
                    if (!string.IsNullOrEmpty(temp.name))
                    {
                        Vector3 position = new Vector3(objectTransforms[j].position.x, objectTransforms[j].position.y, objectTransforms[j].position.z);
                        Vector3 rotation = new Vector3(objectTransforms[j].rotation.x, objectTransforms[j].rotation.y, objectTransforms[j].rotation.z);
                        Vector3 scale = new Vector3(objectTransforms[j].scale.x, objectTransforms[j].scale.y, objectTransforms[j].scale.z);

                        temp.GetComponent<ObjectTransformComponent>();

                        ObjectTransformComponent objectTransformComponent = temp.GetComponent<ObjectTransformComponent>();
                        objectTransformComponent.SetTransform(position, rotation, scale);

                        temp.transform.position = position;
                        temp.transform.eulerAngles = rotation;
                        temp.transform.localScale = scale;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Use to give the references back to the buttons while reloading.
    /// </summary>
    /// <param name="assignButton"></param>
    private void InitializeReferenceForButtons(AssignButton assignButton)
    {
        if (ScriptsManager._toolContent.customButtons.Count != 0)
        {
            foreach (CustomButtons item in ScriptsManager._toolContent.customButtons)
            {
                Debug.Log("item.pathitem.pathitem.pathitem.path : " + item.path);
                if (item.referencedTo == assignButton.ToString())
                {
                    GameObject buttonObject = GameObject.Find(item.path);
                   // dropReference.OnLoading(item.referencedTo, buttonObject);
                }
            }
        }
    }

    /// <summary>
    /// Reload all the buttons for the Design Studio.
    /// </summary>
    private void CreateDSButtons()
    {
        m_ScriptsManager = ScriptsManager.Instance;
        m_ScriptsManager.dsButtons.Clear();
        if (m_ScriptsManager.projectTypeDropdown.value == 1)
        {
            ControlFlowManagerV2.Instance.SubscribeToDelegate();
        }

        if (ScriptsManager._toolContent.dsButtons.Count > 0)
        {
            PopupManager.uniqueButtonID = ScriptsManager._toolContent.totalDSButtons;

            foreach (var dsButton in ScriptsManager._toolContent.dsButtons)
            {
                if (ScriptsManager.Instance.projectTypeDropdown.value.Equals(1))
                {
                    //for line item Creation in the scene vertical panel (saravanan)
                    PopupManager.ButtonNameAction += ControlFlowManagerV2.Instance.CreateScene;
                }

                GameObject button = Instantiate(PopupManager.Instance.buttonSceneItem);
                button.transform.SetParent(m_ScriptsManager.scenePropertyRect);
                m_ScriptsManager.dsButtons.Add(button);
                //Debug.Log("dsButton...Name.."+dsButton.givenName);
                ControlFlowButton controlFlowButton = button.GetComponentInChildren<ControlFlowButton>();
                DSButton(controlFlowButton, dsButton);
            }
        }

    }

    /// <summary>
    /// Sub method for CreateDSButtons().
    /// </summary>
    /// <param name="controlFlowButton"></param>
    /// <param name="dsButton"></param>
    private void DSButton(ControlFlowButton controlFlowButton, DSButton dsButton)
    {
        controlFlowButton.type = dsButton.type;
        controlFlowButton.uniqueID = dsButton.uniqueID;
        controlFlowButton.width = dsButton.width;
        controlFlowButton.height = dsButton.height;
        controlFlowButton.fontSize = dsButton.fontSize;
        controlFlowButton.normalColor = dsButton.normalColor;
        controlFlowButton.highlightColor = dsButton.highlightColor;
        controlFlowButton.pressedColor = dsButton.pressedColor;
        controlFlowButton.textColor = dsButton.textColor;
        controlFlowButton.parentTransform.parent.name = controlFlowButton.givenName = dsButton.givenName;
        controlFlowButton.alternateName = dsButton.alternateName;
        controlFlowButton.referencedTo = dsButton.referencedTo;
        ApplyGameObjectPositions(controlFlowButton.parentTransform.parent.gameObject);
        controlFlowButton.ApplyChanges();

        if (ScriptsManager.Instance.projectTypeDropdown.value.Equals(1))
        {
            PopupManager.ButtonNameAction(dsButton.givenName, controlFlowButton.transform.parent.parent.gameObject);
        }
    }
}