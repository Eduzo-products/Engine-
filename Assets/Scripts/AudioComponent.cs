using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;


public class AudioComponent : MonoBehaviour
{
    [HideInInspector] public GameObject hierarchyReference;
    [HideInInspector] public GameObject linkHolder;

    public AudioClip audioClip;
    [SerializeField]
    public TMP_InputField inputAudioClipPath;
    //[SerializeField]
    //private TMP_InputField inputStartTime;
    //[SerializeField]
    //private TMP_InputField inputEndTime;
    public bool isPlaying;
    public TextMeshProUGUI playButtonText;

    private Coroutine routine;

    private void Start()
    {
        ScriptsManager.OnCloseClicked += CloseProject;
    }

    private void CloseProject()
    {
        RemoveComponent();
    }

    private void OnDestroy()
    {
        ScriptsManager.OnCloseClicked -= CloseProject;
    }

    public void PlayPushSample()
    {
        if (audioClip != null)
        {
            isPlaying = !isPlaying;

            if (isPlaying)
            {
                ScriptsManager.Instance.audioManager.clip = audioClip;
                ScriptsManager.Instance.audioManager.Play();
                playButtonText.text = "stop";
                routine = StartCoroutine(EndOfAudio());
            }
            else
            {
                StopPlayingAudio();
                if (routine != null)
                {
                    StopCoroutine(routine);
                }
            }
        }
    }

    private IEnumerator EndOfAudio()
    {
        yield return new WaitUntil(() => ScriptsManager.Instance.audioManager.isPlaying);
        while (ScriptsManager.Instance.audioManager.isPlaying)
        {
            yield return null;
        }
        playButtonText.text = "play";
    }

    private void StopPlayingAudio()
    {
        playButtonText.text = "play";
        ScriptsManager.Instance.audioManager.Stop();
        ScriptsManager.Instance.audioManager.clip = null;
    }

    public void RemoveComponent()
    {
        ScriptsManager.Instance.featureScript.audioImg.sprite = ScriptsManager.Instance.featureScript.audioSprites[0];

        if (routine != null)
        {
            StopCoroutine(routine);
        }

        isPlaying = false;
        ScriptsManager.Instance.audioManager.volume = 0.0f;
        ScriptsManager.Instance.audioManager.Stop();
        ScriptsManager.Instance.audioManager.clip = null;
        Destroy(audioClip);

        gameObject.SetActive(false);
        playButtonText.text = "play";
        DestroyImmediate(hierarchyReference, true);
        inputAudioClipPath.text = ScriptsManager.Instance.currentObjectName.text = "";
    }
}