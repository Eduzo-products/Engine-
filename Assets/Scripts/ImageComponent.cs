using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using System;

public class ImageComponent : MonoBehaviour
{
    public TMP_InputField imageURLField;

    [Header("<Manual References>")]
    public SpriteRenderer icon;

    [Header("<RunTime References>")]
    public string url;
    public string imageName;
    public Sprite iconSprite;

    public ComponentType imageType;

    private Coroutine routine;

    public void DownloadImage()
    {
        if (imageURLField.text.Length > 0)
        {
            url = imageURLField.text;
        }
        else
        {
            Debug.Log("Nothing to download!");
            return;
        }

        routine = StartCoroutine(DownloadImageRoutine(url, (stopRoutine) =>
        {
            if (stopRoutine)
            {
                StopCoroutine(routine);
            }
        }));
    }

    private IEnumerator DownloadImageRoutine(string url, Action<bool> stopRoutine)
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

                    iconSprite = sprite;
                }
            }

            icon.sprite = iconSprite;
        }

        stopRoutine(true);
    }
}