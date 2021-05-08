using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneObjects : PopupBase, IPopupManager
{
    private Button cancelButton;
    private Button textButton, ttsButton, audioButton,
        videoButton, modelButton, imageButton;
    private ScriptsManager m_ScriptsManager;

    public SceneObjects(SceneObjectsPopup sceneObjectsPopup)
    {
        popup = sceneObjectsPopup.popup;
        cancelButton = sceneObjectsPopup.cancelButton;
        textButton = sceneObjectsPopup.textButton;
        ttsButton = sceneObjectsPopup.ttsButton;
        audioButton = sceneObjectsPopup.audioButton;
        videoButton = sceneObjectsPopup.videoButton;
        modelButton = sceneObjectsPopup.modelButton;
        imageButton = sceneObjectsPopup.imageButton;

        cancelButton.onClick.AddListener(() => { Cancellation(); });
        textButton.onClick.AddListener(() => { AddText(); });
        ttsButton.onClick.AddListener(() => { AddTTS(); });
        audioButton.onClick.AddListener(() => { AddAudio(); });
        videoButton.onClick.AddListener(() => { AddVideo(); });
        modelButton.onClick.AddListener(() => { AddModel(); });
        imageButton.onClick.AddListener(() => { AddImage(); });
    }

    public override void OpenPopUp()
    {
        popup.SetActive(true);
    }

    public override void ClosePopUp()
    {
        popup.SetActive(false);
        RemoveAllListeners();
    }

    public void Cancellation()
    {
        ClosePopUp();
    }

    public void Confirmation()
    {
        ClosePopUp();
    }

    private void RemoveAllListeners()
    {
        cancelButton.onClick.RemoveAllListeners();
        textButton.onClick.RemoveAllListeners();
        ttsButton.onClick.RemoveAllListeners();
        audioButton.onClick.RemoveAllListeners();
        videoButton.onClick.RemoveAllListeners();
        modelButton.onClick.RemoveAllListeners();
    }

    private void AddText()
    {
        m_ScriptsManager = ScriptsManager.Instance;
        m_ScriptsManager.enableDisableComponent.EnableDisableTextContentFeature(true);
        Cancellation();
    }

    private void AddTTS()
    {
        m_ScriptsManager = ScriptsManager.Instance;
        m_ScriptsManager.enableDisableComponent.EnableDisableTextToSpeech(true);
        Cancellation();
    }

    private void AddAudio()
    {
        m_ScriptsManager = ScriptsManager.Instance;
        m_ScriptsManager.enableDisableComponent.EnableDisableAudioFeature(true);
        Cancellation();
    }

    private void AddVideo()
    {
        m_ScriptsManager = ScriptsManager.Instance;
        m_ScriptsManager.enableDisableComponent.EnableDisableVideoFeature(true);
        Cancellation();
    }

    private void AddImage()
    {
        m_ScriptsManager = ScriptsManager.Instance;
        m_ScriptsManager.enableDisableComponent.AddImage();
        Cancellation();
    }

    private void AddModel()
    {
        m_ScriptsManager = ScriptsManager.Instance;
        m_ScriptsManager.myAssets.gameObject.SetActive(true);
        m_ScriptsManager.myAssets.initEnable();
        Cancellation();
    }
}