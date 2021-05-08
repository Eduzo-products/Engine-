using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.EventSystems;

public class FileDetails : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public ScriptsManager scriptsManager;
    public SkyAudVid skyAudVid;
    public int buttonID;
    public string url;
    public TMP_Text fileName, duration, size;
    public AudioClip audioClip;
    public Image buttonImage;
    public Color normalColor, selectedColor;


    //assetTab
    [Header("AssetTab")]
    public AssetDetails assetDetails;


    public void AssignURL()
    {
        scriptsManager.skyAudVid.url = url;

        ButtonColors();
    }
    public enum IconType { None, Asset };
    public IconType iconType;
    bool Hovered;

    public void OnPointerEnter(PointerEventData eventData)
    {
        Hovered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Hovered = false;
        //    throw new System.NotImplementedException();
    }
    public float InitialTouch;

    //private void Update()
    //{
        //if (iconType == IconType.Asset && Hovered)
        //    if (Input.GetMouseButtonDown(0))
        //    {
        //        if (Time.time < InitialTouch + 0.5f)
        //        {
        //            //  Debug.Log("DoubleTouch");
        //            ScriptsManager.Instance.DownloadFiles(ScriptsManager.Instance.myAssets.LastClicked.assetDetails, ScriptsManager.Instance.myAssets.LastClicked.name, null);
        //            ScriptsManager.Instance.myAssets.titleColor();
        //            ScriptsManager.Instance.myAssets.refresh();
        //           ScriptsManager.Instance.myAssets.gameObject.SetActive(false);
        //            ScriptsManager.Instance.isAssets = false;



        //        }
        //        InitialTouch = Time.time;
        //    }
    //}

    public void PlayAudio()
    {
        StartCoroutine(DownloadAudio());
    }

    private void ButtonColors()
    {
        for (int p = 0; p < skyAudVid.buttons.Count; p++)
        {
            if (skyAudVid.buttons[p].GetComponent<FileDetails>().buttonID == buttonID)
            {
                skyAudVid.buttons[p].GetComponent<FileDetails>().buttonImage.color = selectedColor;

            }
            else
            {
                skyAudVid.buttons[p].GetComponent<FileDetails>().buttonImage.color = normalColor;
            }
        }
    }

    private IEnumerator DownloadAudio()
    {
        var uwr = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.WAV);
        DownloadFromServer.Instance.DownloadingProgress(uwr);
        yield return uwr.SendWebRequest();

        if (uwr.isNetworkError || uwr.isHttpError)
        {
            //  Debug.LogError(uwr.error);
            yield break;
        }

        audioClip = DownloadHandlerAudioClip.GetContent(uwr);

        scriptsManager.audioManager.clip = audioClip;

        scriptsManager.audioManager.Play();
    }
}