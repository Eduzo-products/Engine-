using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;


/// <summary>
/// Added by Periyasamy on 2/8/2019 for publishing the content
/// </summary>
public class PublishContent : MonoBehaviour
{
    public ToolContent downloadedData;
    public delegate void XRContentSaveDelegate();
    private static XRContentSaveDelegate _XRContentSaveDelegate;
    public static event XRContentSaveDelegate OnSaveContentClick
    {
        add
        {
            _XRContentSaveDelegate += value;
        }
        remove
        {
            _XRContentSaveDelegate -= value;
        }
    }

    void Start()
    {
        ScriptsManager.OnCloseClicked += ClearJSON;
    }

    private void OnDestroy()
    {
        ScriptsManager.OnCloseClicked -= ClearJSON;
    }

    private void ClearJSON()
    {
        ScriptsManager._toolContent = downloadedData = null;
    }

    public void SaveContent()
    {
        SaveContent("Save");
    }

    /// <summary>
    /// Delegate to save content in json called 
    /// </summary>
    public void SaveContent(string statusText)
    {
        ScriptsManager.Instance.RemoveUnnecessaryStartPoint();

        if (!ScriptsManager.Instance.CheckForEmptyLabelName())
        {
            StartCoroutine(SaveFunctionality(_XRContentSaveDelegate, statusText));
            ScriptsManager.Instance.confirmationPanel.CloseConfirmationPanel();
        }
        else
        {
            ConfirmationPanel confirmationPanel = ScriptsManager.Instance.confirmationPanel;
            confirmationPanel.confirmationTitleText.text = "Attention";
            confirmationPanel.confirmationTextFieldText.text = "Please enter the missing label name(s).";
            confirmationPanel.submitButtonText.text = "Okay";
            confirmationPanel.cancelButtonText.transform.parent.gameObject.SetActive(false);
            confirmationPanel.confirmationIcon.sprite = Helper.Instance.PopUpSprite("Attention");

            confirmationPanel.OpenConfirmationPanel();
        }
    }

  

    private IEnumerator SaveFunctionality(XRContentSaveDelegate myRef, string statusText)
    {

        GetComponent<CurrentObjectTransform>().SaveDetails();

        yield return new WaitForSeconds(0.2f);

        if (myRef != null)
        {
            Debug.Log($"Start myRef : {myRef}.");

            ScriptsManager._toolContent = new ToolContent();
            _XRContentSaveDelegate();

            Debug.Log($"End myRef : {myRef}.");

            //foreach (XRContentSaveDelegate item in myRef.GetInvocationList())
            //{
            //    IAsyncResult result = item.BeginInvoke(new AsyncCallback(ProcessDnsInformation), null);
            //    yield return result.IsCompleted;
            //}
            ScriptsManager._toolContent.dsButtons.Clear();
            ScriptsManager.Instance.SaveDSButtons();


            ScriptsManager._toolContent.projectType = ScriptsManager.Instance.projectTypeDropdown.value;
            ScriptsManager._toolContent.isExplode = ScriptsManager.Instance.isExplodable;
            ScriptsManager._toolContent.isGesture = ScriptsManager.Instance.isGesture;
            ScriptsManager._toolContent.bundleDetails = ScriptsManager.Instance.bundleDetails;
            ScriptsManager._toolContent.skyboxURL = ScriptsManager.Instance.skuboxURL;
            ScriptsManager._toolContent.videoCount = ScriptsManager.Instance.videoCount;
            ScriptsManager._toolContent.explodeRange = 0;
            ScriptsManager._toolContent.explodeSpeed = 0;

            ExplodeComponent explodeComponent = ScriptsManager.Instance.enableDisableComponent.explodePrefab.GetComponent<ExplodeComponent>();
            if (explodeComponent != null)
            {
                if (explodeComponent.explode.isOn)
                {
                    ScriptsManager._toolContent.explodeRange = explodeComponent.range.value;
                    ScriptsManager._toolContent.explodeSpeed = explodeComponent.speed.value;
                }
            }


            //ScriptsManager._XRStudioContent.allSequence = ScriptsManager.Instance.enableDisableComponent.layerEventsParent.SaveSequence();

            //sending data to save (saravanan code)
            ScriptsManager._toolContent.allSequence = ControlFlowManagerV2.Instance.SaveMultiScene();

            // if (ScriptsManager.Instance.scenePropertyRect)
            GameObject temp1 = ScriptsManager.FindObject(ScriptsManager.Instance.scenePropertyRect.gameObject, "WalkThrough");
         
            
           
            ScriptsManager._toolContent.modelCount = ScriptsManager.Instance.AssetCount;

            //for(int i=0;i<scriptsManager.txCurrent.Count;i++)
            //{
            //  //  ScriptsManager._XRStudioContent.textContent.Add(scriptsManager.txCurrent[i]);

            //}

            Debug.Log(ScriptsManager._toolContent);
            Debug.Log(JsonUtility.ToJson(ScriptsManager._toolContent));
#if UNITY_EDITOR
            TextAsset textAsset = new TextAsset();
            File.WriteAllText(Application.dataPath + "/SavedProject.json", JsonUtility.ToJson(ScriptsManager._toolContent));
            Debug.Log($"Path: {Application.dataPath}");
#endif
            ScriptsManager.Instance.errorMessage.text = "Saved successfully";

            downloadedData = ScriptsManager._toolContent;
            PublishContentIntoServer(statusText);
        }
    }

    public void ProcessDnsInformation(IAsyncResult result)
    {
        Debug.Log($"{result} called.");
    }

    public void PublishContentIntoServer(string statusText)
    {
        StartCoroutine(publishContent(statusText));
    }

    IEnumerator publishContent(string statusText)
    {
        if (ScriptsManager._toolContent == null)
        {
            ScriptsManager._toolContent = new ToolContent();
        }
        if (ScriptsManager._toolContent != null)
        {
            string postdata = JsonUtility.ToJson(ScriptsManager._toolContent);
            byte[] bytes = Encoding.UTF8.GetBytes(postdata);
            Debug.Log("JSON..to Save..: " + postdata);
            UnityWebRequest www = new UnityWebRequest(ScriptsManager.Instance.APIBaseURL + "api/SaveContent");
            www.method = UnityWebRequest.kHttpVerbPOST;
            www.uploadHandler = new UploadHandlerRaw(bytes);

            www.SetRequestHeader("ClientId", ScriptsManager.Instance.userDetails.clientId.ToString());
            www.SetRequestHeader("FileName", ScriptsManager.Instance.projectTitleText.text + ".json");
            www.SetRequestHeader("Status", statusText);

            www.downloadHandler = new DownloadHandlerBuffer();
            www.uploadHandler.contentType = "application/json";
            www.chunkedTransfer = false;

            DownloadFromServer.Instance.DownloadingProgress(www);
            yield return www.SendWebRequest();

            if (!www.isNetworkError || !www.isHttpError)
            {
                //success
                Debug.Log(www.downloadHandler.text);

                string status = (statusText == "Save") ? "Saved" : "Published";
                ScriptsManager.Instance.errorMessage.text = status + " successfully";
            }
            else
            {
                //error
                Debug.Log(www.error);
                string status = (statusText == "Save") ? "Saving" : "Publishing";
                ScriptsManager.Instance.errorMessage.text = status + " failed " + www.error;
            }

            yield return null;
        }
    }

    public void TestGetPublishedContent()
    {
        StartCoroutine(DownloadPublishedJSON());
    }

    IEnumerator DownloadPublishedJSON()
    {
        //UnityWebRequest www = UnityWebRequest.Post("http://quizpluginservice.holopundits.com/api/Quiz/", form);
        UnityWebRequest www = UnityWebRequest.Get(ScriptsManager.Instance.APIBaseURL + "api/GetContent?ClientId=1&FileName=myprojectname.json");
        yield return www.SendWebRequest();

        try
        {
            if (!www.isNetworkError || !www.isHttpError)
            {
                string dataAsJson = www.downloadHandler.text;
                Debug.Log(dataAsJson);

                ReceivedContent fetchedData = JsonUtility.FromJson<ReceivedContent>(dataAsJson);
                Debug.Log(fetchedData);
                //if (fetchedData.ResponseCode != 0)
                //{
                //    data = fetchedData.Data;


                //}
                //else
                //{

                //}
            }
            else
            {
                Debug.Log(www.error);
            }
        }
        catch (Exception ee)
        {
            Debug.Log(ee.Message);
        }
    }
}
