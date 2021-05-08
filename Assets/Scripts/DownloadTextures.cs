using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DownloadTextures : MonoBehaviour
{
    private Coroutine routine;

    public void AssignFileDetails(FileDetails fd, int buttonID, Sprite sprite = null, List<AssetDetails> cloudData = null, List<AssetDetails> libraryData = null, SkyAudVid skyAudVid = null)
    {
        fd.buttonID = buttonID;
        fd.buttonImage.sprite = sprite;
        fd.scriptsManager = ScriptsManager.Instance;
        fd.skyAudVid = skyAudVid;

        if (cloudData != null)
        {
            fd.url = ScriptsManager.Instance.APIBaseURL + cloudData[buttonID].FileURL;
            fd.fileName.text = cloudData[buttonID].FileName.Split('.')[0];
            fd.duration.text = cloudData[buttonID].Duration.ToString();

            fd.size.text = cloudData[buttonID].FileSize.ToString() + " mb";

            routine = StartCoroutine(ApplyImageTexture(cloudData[buttonID].ThumbnailURL, fd, (endBool) =>
           {
               if (endBool)
               {
                   StopCoroutine(routine);
               }
           }));
        }
        else if (libraryData != null)
        {
            fd.url = ScriptsManager.Instance.APIBaseURL + libraryData[buttonID].FileURL;
            fd.fileName.text = libraryData[buttonID].FileName.Split('.')[0];
            fd.duration.text = libraryData[buttonID].Duration.ToString();
            fd.size.text = libraryData[buttonID].FileSize.ToString() + "mb";

            routine = StartCoroutine(ApplyImageTexture(libraryData[buttonID].ThumbnailURL, fd, (endBool) =>
           {
               if (endBool)
               {
                   StopCoroutine(routine);
               }
           }));
        }
    }

    public IEnumerator ApplyImageTexture(string imageURL, FileDetails fd, Action<bool> endBool)
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

                    fd.buttonImage.sprite = sprite;
                }
            }
        }
        else
        {
            Debug.Log($"Error - {www.error}.");
        }
        endBool(true);
    }

    public Sprite SpriteFromTexture2D(Texture2D texture)
    {
        return Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
    }

}
