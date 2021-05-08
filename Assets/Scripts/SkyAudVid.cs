using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using System;

/// <summary>
/// For pop up box managing, getting data to use for the engine  
/// </summary>
public class SkyAudVid : MonoBehaviour
{
    public RectTransform skyAudVidPanel, libraryContent, cloudContent;
    public GameObject skyAudVidParent, library, cloud, urlField, audioHeader, errorMessage, refreshButton, upload;
    public TMP_InputField urlInputField;
    [HideInInspector] public TMP_InputField selectedInputField;
    public GameObject videoButtPrefab, audioButtPrefab, skyBoxPrefab;
    public TMP_Text titleText, addButtonText, noteText;
    public Image icon;
    public Sprite videoSprite, skyBoxSprite, audioSprite;
    public ChangeSkybox changeSkybox;

    [Space(15)]
    public float normalHeight = 0.0f;
    public float extendedHeight = 0.0f;

    [Space(15)]
    public GridLayoutGroup gridLayout;

    public string url;
    public Color normalColor, selectedColor;
    public List<GameObject> buttons = new List<GameObject>();
    public List<Image> linkButtons = new List<Image>();

    [HideInInspector] public GameObject linkHolder;

    private ScriptsManager scriptsManager;
    private Coroutine routine;

    public void ToggleVidSkyLibrary()
    {
        audioHeader.SetActive(false);

        gridLayout.startAxis = GridLayoutGroup.Axis.Horizontal;
        gridLayout.spacing = new Vector2(30.0f, 3.0f);
        gridLayout.cellSize = new Vector2(254.0f, 220.0f);

        var cloudHeight = cloud.GetComponent<RectTransform>().rect.height;
        cloudHeight = 388.0f;
        cloud.GetComponent<RectTransform>().sizeDelta = new Vector2(cloud.GetComponent<RectTransform>().rect.width, cloudHeight);

        var libraryHeight = library.GetComponent<RectTransform>().rect.height;
        libraryHeight = 388.0f;
        library.GetComponent<RectTransform>().sizeDelta = new Vector2(library.GetComponent<RectTransform>().rect.width, libraryHeight);

        var cloudYPosition = cloud.transform.localPosition;
        cloudYPosition.y = 0.0f;
        cloud.transform.localPosition = cloudYPosition;

        var libraryYPosition = library.transform.localPosition;
        libraryYPosition.y = 0.0f;
        library.transform.localPosition = libraryYPosition;
    }

    public void ToggleAudioLibrary()
    {
        audioHeader.SetActive(true);

        gridLayout.startAxis = GridLayoutGroup.Axis.Vertical;
        gridLayout.spacing = new Vector2(0.0f, 2.0f);
        gridLayout.cellSize = new Vector2(825.0f, 40.0f);

        var height = cloud.GetComponent<RectTransform>().rect.height;
        height = 335.0f;
        cloud.GetComponent<RectTransform>().sizeDelta = new Vector2(cloud.GetComponent<RectTransform>().rect.width, height);

        var libraryHeight = library.GetComponent<RectTransform>().rect.height;
        libraryHeight = 335.0f;
        library.GetComponent<RectTransform>().sizeDelta = new Vector2(library.GetComponent<RectTransform>().rect.width, libraryHeight);

        var libraryYPosition = library.transform.localPosition;
        libraryYPosition.y = -53.0f;
        library.transform.localPosition = libraryYPosition;

        var cloudYPosition = cloud.transform.localPosition;
        cloudYPosition.y = -53.0f;
        cloud.transform.localPosition = cloudYPosition;
    }

    public void LibraryButton()
    {
        scriptsManager = ScriptsManager.Instance;

        library.SetActive(true);
        upload.SetActive(false);
        int j = 1;
        ClearButtons(libraryContent);

        if (scriptsManager.isAudio)
        {
            audioHeader.SetActive(true);
        }
        else if (scriptsManager.isVideo || scriptsManager.isSkybox)
        {
            audioHeader.SetActive(false);
        }

        gridLayout = libraryContent.GetComponent<GridLayoutGroup>();

        urlField.SetActive(false);
        cloud.SetActive(false);
        errorMessage.SetActive(false);
        refreshButton.SetActive(true);

        addButtonText.text = scriptsManager.isSkybox ? "Apply" : "Add";

        ChangeColor(j);
    }

    public void CloudButton()
    {
        scriptsManager = ScriptsManager.Instance;

        cloud.SetActive(true);
        upload.SetActive(true);
        int j = 2;
        ClearButtons(cloudContent);

        if (scriptsManager.isAudio)
        {
            audioHeader.SetActive(true);
        }
        else if (scriptsManager.isVideo || scriptsManager.isSkybox)
        {
            audioHeader.SetActive(false);
        }

        gridLayout = cloudContent.GetComponent<GridLayoutGroup>();

        library.SetActive(false);
        urlField.SetActive(false);
        errorMessage.SetActive(false);
        refreshButton.SetActive(true);

        addButtonText.text = scriptsManager.isSkybox ? "Apply" : "Add";

        ChangeColor(j);
    }

    public void URLButton()
    {
        scriptsManager = ScriptsManager.Instance;

        urlField.SetActive(true);
        upload.SetActive(false);
        urlInputField.text = "";
        int j = 0;

        if (selectedInputField != null && selectedInputField.text.Length > 0)
        {
            urlInputField.text = selectedInputField.text;
        }

        audioHeader.SetActive(false);
        library.SetActive(false);
        cloud.SetActive(false);
        errorMessage.SetActive(false);
        refreshButton.SetActive(false);

        addButtonText.text = scriptsManager.isSkybox ? "Apply" : "Add";

        ChangeColor(j);
    }

    private void ClearButtons(Transform parent = null)
    {
        if (buttons.Count > 0)
        {
            parent.DetachChildren();

            int count = buttons.Count;

            for (int p = 0; p < count; p++)
            {
                Destroy(buttons[p]);
            }

            buttons.Clear();
        }
    }

    private void ChangeColor(int j = 0)
    {
        for (int p = 0; p < linkButtons.Count; p++)
        {
            if (p == j)
            {
                linkButtons[p].sprite = ResourceManager.Instance.radialSelected;
            }
            else
            {
                linkButtons[p].sprite = ResourceManager.Instance.radialDeselected;
            }
        }
    }

    public void ExpandPanel()
    {
        if (skyAudVidPanel.rect.height != extendedHeight)
        {
            RectTransform panelRect = skyAudVidPanel;
            panelRect.sizeDelta = new Vector2(panelRect.rect.width, extendedHeight);
        }
    }

    public void PopulateLibrary()
    {
        if (scriptsManager.libraryData.Count > 0)
        {
            if (libraryContent.childCount == 0)
            {
                for (int p = 0; p < scriptsManager.libraryData.Count; p++)
                {
                    if (scriptsManager.isVideo && scriptsManager.libraryData[p].AssetTypeId == (int)AssetTypes.Video)
                    {
                        ToggleVidSkyLibrary();
                        GameObject video = Instantiate(videoButtPrefab, libraryContent);
                        FileDetails fd = video.GetComponent<FileDetails>();

                        fd.GetComponent<DownloadTextures>().AssignFileDetails(fd, p, videoSprite, null, scriptsManager.libraryData, this);

                        buttons.Add(video);
                    }
                    else if (scriptsManager.isAudio && scriptsManager.libraryData[p].AssetTypeId == (int)AssetTypes.Audio)
                    {
                        ToggleVidSkyLibrary();
                        GameObject audio = Instantiate(videoButtPrefab, libraryContent);
                        FileDetails fd = audio.GetComponent<FileDetails>();

                        fd.GetComponent<DownloadTextures>().AssignFileDetails(fd, p, audioSprite, null, scriptsManager.libraryData, this);

                        buttons.Add(audio);
                    }
                    else if (scriptsManager.isSkybox && scriptsManager.libraryData[p].AssetTypeId == (int)AssetTypes.SkyBox)
                    {
                        ToggleVidSkyLibrary();
                        GameObject skyBox = Instantiate(skyBoxPrefab, libraryContent);
                        FileDetails fd = skyBox.GetComponent<FileDetails>();

                        fd.GetComponent<DownloadTextures>().AssignFileDetails(fd, p, skyBoxSprite, null, scriptsManager.libraryData, this);

                        buttons.Add(skyBox);
                    }
                    else if (scriptsManager.isImage && scriptsManager.libraryData[p].AssetTypeId == (int)AssetTypes.Image)
                    {
                        ToggleVidSkyLibrary();
                        GameObject image = Instantiate(skyBoxPrefab, libraryContent);
                        FileDetails fd = image.GetComponent<FileDetails>();

                        fd.GetComponent<DownloadTextures>().AssignFileDetails(fd, p, skyBoxSprite, null, scriptsManager.libraryData, this);

                        buttons.Add(image);
                    }
                    else if (scriptsManager.isImmersiveImage && scriptsManager.libraryData[p].AssetTypeId == (int)AssetTypes.ImmersiveImage)
                    {
                        ToggleVidSkyLibrary();
                        GameObject image = Instantiate(skyBoxPrefab, libraryContent);
                        FileDetails fd = image.GetComponent<FileDetails>();

                        fd.GetComponent<DownloadTextures>().AssignFileDetails(fd, p, skyBoxSprite, null, scriptsManager.libraryData, this);

                        buttons.Add(image);
                    }
                    else if (scriptsManager.isImmersiveVideo && scriptsManager.libraryData[p].AssetTypeId == (int)AssetTypes.ImmersiveVideo)
                    {
                        ToggleVidSkyLibrary();
                        GameObject image = Instantiate(videoButtPrefab, libraryContent);
                        FileDetails fd = image.GetComponent<FileDetails>();

                        fd.GetComponent<DownloadTextures>().AssignFileDetails(fd, p, skyBoxSprite, null, scriptsManager.libraryData, this);

                        buttons.Add(image);
                    }
                    else if (scriptsManager.isAudio && scriptsManager.libraryData[p].AssetTypeId != (int)AssetTypes.Audio)
                    {
                        ToggleAudioLibrary();
                    }
                    else if (scriptsManager.isVideo && scriptsManager.libraryData[p].AssetTypeId != (int)AssetTypes.Video)
                    {
                        ToggleVidSkyLibrary();
                    }
                    else if (scriptsManager.isSkybox && scriptsManager.libraryData[p].AssetTypeId != (int)AssetTypes.SkyBox)
                    {
                        ToggleVidSkyLibrary();
                    }
                    else if (scriptsManager.isImage && scriptsManager.libraryData[p].AssetTypeId != (int)AssetTypes.Image)
                    {
                        ToggleVidSkyLibrary();
                    }
                    else if (scriptsManager.isImmersiveImage && scriptsManager.libraryData[p].AssetTypeId != (int)AssetTypes.ImmersiveImage)
                    {
                        ToggleVidSkyLibrary();
                    }
                    else if (scriptsManager.isImmersiveVideo && scriptsManager.libraryData[p].AssetTypeId != (int)AssetTypes.ImmersiveVideo)
                    {
                        ToggleVidSkyLibrary();
                    }
                }
            }
        }
    }

    public void PopulateCloud()
    {
        if (scriptsManager.cloudData.Count > 0)
        {
            if (cloudContent.childCount == 0)
            {
                for (int p = 0; p < scriptsManager.cloudData.Count; p++)
                {
                    if (scriptsManager.isVideo && scriptsManager.cloudData[p].AssetTypeId == (int)AssetTypes.Video)
                    {
                        ToggleVidSkyLibrary();
                        GameObject video = Instantiate(videoButtPrefab, cloudContent);
                        FileDetails fd = video.GetComponent<FileDetails>();

                        fd.GetComponent<DownloadTextures>().AssignFileDetails(fd, p, videoSprite, scriptsManager.cloudData, null, this);

                        buttons.Add(video);
                    }
                    else if (scriptsManager.isAudio && scriptsManager.cloudData[p].AssetTypeId == (int)AssetTypes.Audio)
                    {
                        ToggleVidSkyLibrary();
                        GameObject audio = Instantiate(videoButtPrefab, cloudContent);
                        FileDetails fd = audio.GetComponent<FileDetails>();

                        fd.GetComponent<DownloadTextures>().AssignFileDetails(fd, p, audioSprite, scriptsManager.cloudData, null, this);

                        buttons.Add(audio);
                    }
                    else if (scriptsManager.isSkybox && scriptsManager.cloudData[p].AssetTypeId == (int)AssetTypes.SkyBox)
                    {
                        ToggleVidSkyLibrary();
                        GameObject skyBox = Instantiate(skyBoxPrefab, cloudContent);
                        FileDetails fd = skyBox.GetComponent<FileDetails>();

                        fd.GetComponent<DownloadTextures>().AssignFileDetails(fd, p, skyBoxSprite, scriptsManager.cloudData, null, this);

                        buttons.Add(skyBox);
                    }
                    else if (scriptsManager.isImage && scriptsManager.cloudData[p].AssetTypeId == (int)AssetTypes.Image)
                    {
                        ToggleVidSkyLibrary();
                        GameObject image = Instantiate(skyBoxPrefab, cloudContent);
                        FileDetails fd = image.GetComponent<FileDetails>();

                        fd.GetComponent<DownloadTextures>().AssignFileDetails(fd, p, skyBoxSprite, scriptsManager.cloudData, null, this);

                        buttons.Add(image);
                    }
                    else if (scriptsManager.isImmersiveImage && scriptsManager.cloudData[p].AssetTypeId == (int)AssetTypes.ImmersiveImage)
                    {
                        ToggleVidSkyLibrary();
                        GameObject image = Instantiate(skyBoxPrefab, cloudContent);
                        FileDetails fd = image.GetComponent<FileDetails>();

                        fd.GetComponent<DownloadTextures>().AssignFileDetails(fd, p, skyBoxSprite, scriptsManager.cloudData, null, this);

                        buttons.Add(image);
                    }
                    else if (scriptsManager.isImmersiveVideo && scriptsManager.cloudData[p].AssetTypeId == (int)AssetTypes.ImmersiveVideo)
                    {
                        ToggleVidSkyLibrary();
                        GameObject image = Instantiate(videoButtPrefab, cloudContent);
                        FileDetails fd = image.GetComponent<FileDetails>();

                        fd.GetComponent<DownloadTextures>().AssignFileDetails(fd, p, videoSprite, scriptsManager.cloudData, null, this);

                        buttons.Add(image);
                    }
                    else if (scriptsManager.isAudio && scriptsManager.cloudData[p].AssetTypeId != (int)AssetTypes.Audio)
                    {
                        ToggleAudioLibrary();
                    }
                    else if (scriptsManager.isVideo && scriptsManager.cloudData[p].AssetTypeId != (int)AssetTypes.Video)
                    {
                        ToggleVidSkyLibrary();
                    }
                    else if (scriptsManager.isSkybox && scriptsManager.cloudData[p].AssetTypeId != (int)AssetTypes.SkyBox)
                    {
                        ToggleVidSkyLibrary();
                    }
                    else if (scriptsManager.isImage && scriptsManager.cloudData[p].AssetTypeId != (int)AssetTypes.Image)
                    {
                        ToggleVidSkyLibrary();
                    }
                    else if (scriptsManager.isImmersiveImage && scriptsManager.cloudData[p].AssetTypeId != (int)AssetTypes.ImmersiveImage)
                    {
                        ToggleVidSkyLibrary();
                    }
                    else if (scriptsManager.isImmersiveVideo && scriptsManager.cloudData[p].AssetTypeId != (int)AssetTypes.ImmersiveVideo)
                    {
                        ToggleVidSkyLibrary();
                    }
                }
            }
        }
    }

    public void CollapsePanel()
    {
        if (skyAudVidPanel.rect.height != normalHeight)
        {
            RectTransform panelRect = skyAudVidPanel;
            panelRect.sizeDelta = new Vector2(panelRect.rect.width, normalHeight);
        }
    }

    public void ClosePanel()
    {
        ScriptsManager.Instance.featureScript.skyBox.sprite = ScriptsManager.Instance.featureScript.skyBoxSprites[0];
        scriptsManager = ScriptsManager.Instance;
        scriptsManager.isPopedUp = false;
        scriptsManager.transformGizmo.enabled = true;
        scriptsManager.isVideo = scriptsManager.isAudio = scriptsManager.isSkybox = scriptsManager.isImage = false;

        url = "";
        selectedInputField = null;
        errorMessage.SetActive(false);
        skyAudVidParent.SetActive(false);

        if (libraryContent.childCount > 0)
        {
            for (int j = 0; j < libraryContent.childCount; j++)
            {
                Destroy(libraryContent.GetChild(j).gameObject);
            }
        }
        if (cloudContent.childCount > 0)
        {
            for (int j = 0; j < cloudContent.childCount; j++)
            {
                Destroy(cloudContent.GetChild(j).gameObject);
            }
        }
    }

    public void AddURLs()
    {
        StartCoroutine(AddURLRoutine());
    }

    public IEnumerator AddURLRoutine()
    {
        scriptsManager = ScriptsManager.Instance;

        if (urlField.activeSelf)
        {
            if (urlInputField.text.Length > 0) //&& Uri.IsWellFormedUriString(selectedInputField.text, UriKind.Absolute))
            {
                selectedInputField.text = urlInputField.text;

                if (scriptsManager.isSkybox)
                {
                    if (ScriptsManager.Instance.projectTypeDropdown.value == 0)
                    {
                        ScriptsManager.Instance.ChangeSkybox(urlInputField.text);
                    }
                    else
                    {
                        changeSkybox.ApplyChange();
                    }
                }
                else if (scriptsManager.isImage)
                {
                    if (selectedInputField.gameObject.GetComponentInParent<ImageComponent>())
                    {
                        selectedInputField.gameObject.GetComponentInParent<ImageComponent>().DownloadImage();
                    }
                   
                }

                yield return StartCoroutine(AddWebLinkToGameObject(selectedInputField.text));
                ClosePanel();
            }
            else
            {
                errorMessage.SetActive(true);
                errorMessage.GetComponent<TMP_Text>().text = "Enter a valid URL";
            }
        }
        else if (library.activeSelf || cloud.activeSelf)
        {
            if (url.Length > 0)
            {
                selectedInputField.text = url;

                if (scriptsManager.isSkybox)
                {
                    if (ScriptsManager.Instance.projectTypeDropdown.value == 0)
                    {
                        ScriptsManager.Instance.ChangeSkybox(url);
                    }
                    else
                    {
                        changeSkybox.ApplyChange();
                    }
                }
                else if (scriptsManager.isImage)
                {
                    if (selectedInputField.gameObject.GetComponentInParent<ImageComponent>())
                    {
                        selectedInputField.gameObject.GetComponentInParent<ImageComponent>().DownloadImage();
                    }
                   
                }

                yield return StartCoroutine(AddWebLinkToGameObject(selectedInputField.text));
                ClosePanel();
            }
            else
            {
                errorMessage.SetActive(true);
                errorMessage.GetComponent<TMP_Text>().text = "Please select a file to add";
            }
        }
    }

    public IEnumerator AddWebLinkToGameObject(string link)
    {
        scriptsManager = ScriptsManager.Instance;

        string contentName = scriptsManager.ExtensionRemover(link);

        if (linkHolder && linkHolder.GetComponent<NoTransform>())
        {
            NoTransform noTransform = linkHolder.GetComponent<NoTransform>();

            if (scriptsManager.isAudio)
            {
                noTransform.webLink = link;

                noTransform.componentObject = scriptsManager.audioPropertyObj;
                noTransform.audioComponent = noTransform.componentObject.GetComponent<AudioComponent>();
                noTransform.audioComponent.inputAudioClipPath.text = noTransform.webLink;
                yield return StartCoroutine(DownloadFromServer.Instance.AudioGrabber(link, noTransform.audioComponent));
            }
            else if (scriptsManager.isVideo)
            {
                noTransform.webLink = link;

                noTransform.componentObject = scriptsManager.videoPropertyObj;
                noTransform.videoComponent = noTransform.componentObject.GetComponent<VideoComponentScene>();
                noTransform.videoComponent.inputVideoClipPath.text = noTransform.webLink;
                yield return StartCoroutine(DownloadFromServer.Instance.VideoGrabber(link, noTransform.videoComponent));
            }
        }
        else if (linkHolder && linkHolder.GetComponent<LabelWithAudioProperty>())
        {
            LabelWithAudioProperty labelWithAudioProperty = linkHolder.GetComponent<LabelWithAudioProperty>();
            labelWithAudioProperty.inputAudioClipPath.text = link;

            yield return StartCoroutine(DownloadFromServer.Instance.AudioGrabber(link, null, labelWithAudioProperty));
        }
        else if (linkHolder && linkHolder.GetComponent<Label_Publish>())
        {
            Label_Publish labelPublish = linkHolder.GetComponent<Label_Publish>();

            if (scriptsManager.isVideo)
            {
                labelPublish.videoComponent = labelPublish.videoComponent.GetComponent<VideoComponentScene>();
                labelPublish.videoComponent.inputVideoClipPath.text = link;
                yield return StartCoroutine(DownloadFromServer.Instance.VideoGrabber(link, labelPublish.videoComponent));
            }
            else if (scriptsManager.isAudio)
            {
                yield return StartCoroutine(DownloadFromServer.Instance.AudioGrabber( link, null, null));
            }
        }
       
       
    }

    public void RefreshList()
    {
        url = "";
        refreshButton.GetComponent<Button>().interactable = false;
        scriptsManager.cloudData.Clear();
        scriptsManager.libraryData.Clear();
        int assetType = 0;

        if (libraryContent.childCount > 0)
        {
            for (int p = 0; p < libraryContent.childCount; p++)
            {
                Destroy(libraryContent.GetChild(p).gameObject);
            }
        }

        if (cloudContent.childCount > 0)
        {
            for (int p = 0; p < cloudContent.childCount; p++)
            {
                Destroy(cloudContent.GetChild(p).gameObject);
            }
        }

        if (scriptsManager.isVideo)
        {
            assetType = (int)AssetTypes.Video;

            if (library.activeSelf)
            {
                LibraryButton();
                ExpandPanel();

                StartCoroutine(RefreshLibraryData(assetType));
            }
            else if (cloud.activeSelf)
            {
                CloudButton();
                ExpandPanel();

                StartCoroutine(RefreshCloudData(assetType));
            }
        }
        else if (scriptsManager.isAudio)
        {
            assetType = (int)AssetTypes.Audio;

            if (library.activeSelf)
            {
                LibraryButton();
                ExpandPanel();

                StartCoroutine(RefreshLibraryData(assetType));
            }
            else if (cloud.activeSelf)
            {
                CloudButton();
                ExpandPanel();

                StartCoroutine(RefreshCloudData(assetType));
            }
        }
        else if (scriptsManager.isSkybox)
        {
            assetType = (int)AssetTypes.SkyBox;

            if (library.activeSelf)
            {
                LibraryButton();
                ExpandPanel();

                StartCoroutine(RefreshLibraryData(assetType));
            }
            else if (cloud.activeSelf)
            {
                CloudButton();
                ExpandPanel();

                StartCoroutine(RefreshCloudData(assetType));
            }
        }
        else if (scriptsManager.isImage)
        {
            assetType = (int)AssetTypes.Image;

            if (library.activeSelf)
            {
                LibraryButton();
                ExpandPanel();

                StartCoroutine(RefreshLibraryData(assetType));
            }
            else if (cloud.activeSelf)
            {
                CloudButton();
                ExpandPanel();

                StartCoroutine(RefreshCloudData(assetType));
            }
        }
        else if (scriptsManager.isImmersiveImage)
        {
            assetType = (int)AssetTypes.ImmersiveImage;

            if (library.activeSelf)
            {
                LibraryButton();
                ExpandPanel();

                StartCoroutine(RefreshLibraryData(assetType));
            }
            else if (cloud.activeSelf)
            {
                CloudButton();
                ExpandPanel();

                StartCoroutine(RefreshCloudData(assetType));
            }
        }
        else if (scriptsManager.isImmersiveVideo)
        {
            assetType = (int)AssetTypes.ImmersiveVideo;

            if (library.activeSelf)
            {
                LibraryButton();
                ExpandPanel();

                StartCoroutine(RefreshLibraryData(assetType));
            }
            else if (cloud.activeSelf)
            {
                CloudButton();
                ExpandPanel();

                StartCoroutine(RefreshCloudData(assetType));
            }
        }
        else if (urlField.activeSelf)
        {
            CollapsePanel();
            urlInputField.text = "";
        }
    }

    private IEnumerator RefreshLibraryData(int assetTypes)
    {
        yield return StartCoroutine(scriptsManager.LibraryAssetURLs(assetTypes));
        PopulateLibrary();
        refreshButton.GetComponent<Button>().interactable = true;
    }

    private IEnumerator RefreshCloudData(int assetTypes)
    {
        yield return StartCoroutine(scriptsManager.CloudAssetURLs(assetTypes));
        PopulateCloud();
        refreshButton.GetComponent<Button>().interactable = true;
    }
}