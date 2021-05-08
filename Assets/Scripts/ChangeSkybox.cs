using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

/// <summary>
/// Added on: 22/07/2018
/// Added by: Periyasamy
/// Purpose: To handle skybox feature
/// </summary>
/// 
public partial class ChangeSkybox : MonoBehaviour
{
    //[SerializeField]//
    public Material skyboxMaterial;
    public TMP_InputField urlInput;

    // Start is called before the first frame update
    void Start()
    {
        PublishContent.OnSaveContentClick += onSaveContent;
        ScriptsManager.OnCloseClicked += CloseProject;
    }

    private void CloseProject()
    {
        RemoveSkyboxComponent();
    }

    private void onSaveContent()
    {
        ScriptsManager._toolContent.skyboxURL = urlInput.text;

        Debug.Log("Change Skybox!");
    }
    private void OnDestroy()
    {
        PublishContent.OnSaveContentClick -= onSaveContent;
        ScriptsManager.OnCloseClicked -= CloseProject;
    }

    //Calling on ChangeButton click event
    public void ApplyChange()
    {
        if (!string.IsNullOrEmpty(urlInput.text.Trim()))
            StartCoroutine(DownloadSkybox(urlInput.text));
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
            skyboxMaterial.SetTexture("_Tex", texture);
            RenderSettings.skybox = skyboxMaterial;
        }
    }

    public void RemoveSkyboxComponent()
    {
        ScriptsManager.Instance.featureScript.skyBox.sprite = ScriptsManager.Instance.featureScript.skyBoxSprites[0];
        urlInput.text = ScriptsManager.Instance.skuboxURL = "";

        ScriptsManager.Instance.RevertSkybox();
        if (ScriptsManager.Instance.scenePropertyRect.Find("Skybox") != null)
            Destroy(ScriptsManager.Instance.scenePropertyRect.Find("Skybox").gameObject);

        gameObject.SetActive(false);
        ScriptsManager.Instance.currentObjectName.text = "";
    }

    public void ClearInputField()
    {
        urlInput.text = "";
    }
}
