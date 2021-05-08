using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.IO;
using System;
using UnityEngine.Video;
using TMPro;
using System.Text.RegularExpressions;
using System.Net;

public class DownloadFromServer : Singleton<DownloadFromServer>
{
    public string APIURL = "http://xrstudio.eastus.cloudapp.azure.com/";
    public GameObject downloadProgress;
    public Slider progressSlider;
    public TextMeshProUGUI progressText;

    private WebRequest webRequest;
    private WebResponse webResponse;
    private float progress = 0.0f;
    private Coroutine coroutine, downloadProgressRoutine;

    private void Start()
    {
        DontDestroyOnLoad(this);
    }

    public bool IsValidURL(string URL)
    {
        try
        {
            webRequest = WebRequest.Create(URL);
            using (webResponse = webRequest.GetResponse())
            {
                return true;
            }
        }
        catch (Exception exception)
        {
            Debug.Log(exception.Message);
            return false;
        }
    }

    public void GetCloudImages()
    {
        StartCoroutine(DownloadCloudButtonImages((int)AssetTypes.Image));
    }

    public void GetLibraryImages()
    {
        StartCoroutine(DownloadLibraryButtonImages((int)AssetTypes.Image));
    }

    private IEnumerator DownloadCloudButtonImages(int assetID)
    {
        WWWForm form = new WWWForm();
        form.AddField("clientId", ScriptsManager.Instance.userDetails.clientId);
        form.AddField("AssetTypeId", assetID);

        UnityWebRequest www = UnityWebRequest.Post(APIURL + "api/GetMyAssets?clientid=" + ScriptsManager.Instance.userDetails.clientId + "&AssetTypeId=" + assetID, form);
        Debug.Log($"Images : {www.url}");
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
                for (int p = 0; p < assetFromJson.Data.Count; p++)
                {
                    ScriptsManager.Instance.cloudData.Add(assetFromJson.Data[p]);
                }
            }
        }
    }

    private IEnumerator DownloadLibraryButtonImages(int assetID)
    {
        WWWForm form = new WWWForm();
        form.AddField("AssetTypeId", assetID);

        UnityWebRequest www = UnityWebRequest.Post(APIURL + "api/GetPublicAssets?AssetTypeId=" + assetID, form);
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
                for (int p = 0; p < assetFromJson.Data.Count; p++)
                {
                    ScriptsManager.Instance.libraryData.Add(assetFromJson.Data[p]);
                }
            }
        }
    }

    public IEnumerator VideoGrabber(string link = "", VideoComponentScene videoComponent = null, Action<bool> downloadCompleted = null)
    {
        if (link != string.Empty)
        {
            //var uwr = UnityWebRequest.Get(link);
            string videoName = ScriptsManager.Instance.ExtensionRemover(link);

            //DownloadingProgress(uwr);
            //yield return uwr.SendWebRequest();

            //if (uwr.isNetworkError || uwr.isHttpError)
            //{
            //    if (setTransitionValues != null)
            //    {
            //        setTransitionValues.videoServerURL = link;
            //    }
            //    else if (videoComponent != null)
            //    {
            //        if (videoComponent.videoType == VideoType.Scene)
            //        {
            //            videoComponent.noTransform.isVideoPlaying = false;
            //            videoComponent.localURL = videoComponent.noTransform.serverURL = link;
            //        }
            //        else if (videoComponent.videoType == VideoType.Label)
            //        {
            //            videoComponent.labelPublish.isVideoPlaying = false;
            //            videoComponent.localURL = videoComponent.labelPublish.serverURL = link;
            //        }
            //    }

            //    ScriptsManager.Instance.errorMessage.text = String.IsNullOrEmpty(link) ? "" : $"We couldn't download the content from the link, {uwr.error}";
            //    yield break;
            //}
            if (IsValidURL(link))
            {
                
                 if (videoComponent != null)
                {
                    if (videoComponent.videoType == ComponentType.Scene)
                    {
                        videoComponent.noTransform.isVideoPlaying = false;
                        videoComponent.localURL = videoComponent.noTransform.serverURL = link;
                    }
                    else if (videoComponent.videoType == ComponentType.Label)
                    {
                        videoComponent.labelPublish.isVideoPlaying = false;
                        videoComponent.localURL = videoComponent.labelPublish.serverURL = link;
                    }
                }
                else
                {
                    ScriptsManager.Instance.errorMessage.text = String.IsNullOrEmpty(link) ? "" : $"We couldn't process the content from the provided link.";
                }

                yield return null;

                if (downloadCompleted != null)
                    downloadCompleted(true);
            }
        }
    }

    public IEnumerator AudioGrabber( string link = "", AudioComponent audioComponent = null, LabelWithAudioProperty labelWithAudioProperty = null)
    {
        Debug.Log($"Audio link: {link}");
        var uwr = UnityWebRequestMultimedia.GetAudioClip(link, AudioType.WAV);
        DownloadingProgress(uwr);
        yield return uwr.SendWebRequest();

        if (uwr.isNetworkError || uwr.isHttpError)
        {
             if (audioComponent != null)
            {
                audioComponent.audioClip = null;
                audioComponent.hierarchyReference.GetComponent<NoTransform>().audioClip = null;
            }
            else if (labelWithAudioProperty != null)
            {
                labelWithAudioProperty.label_Publish.audioClip = labelWithAudioProperty.audioClip = null;
            }

            ScriptsManager.Instance.errorMessage.text = String.IsNullOrEmpty(link) ? "" : $"We couldn't download the content from the link, {uwr.error}";
            yield break;
        }
        else
        {
            AudioClip clip = DownloadHandlerAudioClip.GetContent(uwr);
            clip.name = ScriptsManager.Instance.ExtensionRemover(link);

            if (audioComponent != null)
            {
                audioComponent.audioClip = clip;
                audioComponent.hierarchyReference.GetComponent<NoTransform>().audioClip = clip;
            }
            else if (labelWithAudioProperty != null)
            {
                labelWithAudioProperty.label_Publish.audioClip = labelWithAudioProperty.audioClip = clip;
            }
        }
    }

   

  
    

    public void DownloadingProgress(UnityWebRequest unityWebRequest)
    {
        downloadProgress.SetActive(true);
        downloadProgressRoutine = StartCoroutine(ProgressBarPercentage(unityWebRequest, (isDone) =>
        {
            if (isDone)
            {
                downloadProgress.SetActive(false);
                progress = 0.0f;
                progressSlider.value = progress;
                progressText.text = progressSlider.value.ToString();
                StopCoroutine(downloadProgressRoutine);
            }
        }));
    }

    private IEnumerator ProgressBarPercentage(UnityWebRequest unityWebRequest, Action<bool> isDone)
    {
        if (unityWebRequest != null)
        {
            progressSlider.value = 0;
            progressText.text = progressSlider.value.ToString();
            progress = 0.0f;

            while (!unityWebRequest.isDone)
            {
                progress = (unityWebRequest.downloadProgress * 100.0f);
                if (progress > 99.7f)
                {
                    progress = 100.0f;
                }
                progressSlider.value = progress;
                progressText.text = progressSlider.value.ToString() + "%";
                yield return null;
            }
        }
        isDone(true);
    }
}

