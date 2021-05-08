using CommandUndoRedo;
using Oculus.Platform;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/*
 * Created by Jeffri on 7-11-2019.
 */
public class ConfirmationPanel : MonoBehaviour
{
    [Header("<------- References ------->")]
    public GameObject confirmationPanel = null;
    public TMP_Text confirmationTitleText, confirmationTextFieldText, cancelButtonText, submitButtonText;
    public Image confirmationIcon;
    public PublishContent publishContent;
    public CurrentObjectTransform currentObjectTransform;

    [Header("<------- Bolleans ------->")]
    public bool isPublishable;
    public bool signOut, closeBool, mainSceneBool;

    [Header("<------- Duplicate Component ------->")]
    public GameObject duplicatePopUp;
    public TextMeshProUGUI messageText;


    private ScriptsManager m_ScriptsManager;

    private void Start()
    {
        m_ScriptsManager = ScriptsManager.Instance;
        isPublishable = signOut = closeBool = mainSceneBool = false;
    }

    public void OpenConfirmationPanel()
    {
        m_ScriptsManager = ScriptsManager.Instance;
        m_ScriptsManager.isPopedUp = true;
        confirmationPanel.SetActive(true);
    }

    public void CloseConfirmationPanel()
    {
        m_ScriptsManager = ScriptsManager.Instance;
        m_ScriptsManager.isPopedUp = false;
        ButtonCustomizationManager l_buttonCustomizationManager = ButtonCustomizationManager.Instance;

        confirmationPanel.SetActive(false);
        cancelButtonText.transform.parent.gameObject.SetActive(true);
        isPublishable = signOut = closeBool = mainSceneBool = m_ScriptsManager.isHotspotDeletable =
            m_ScriptsManager.isDeletable = false;

        ControlFlowManagerV2.confirmDeleteSceneObjLineItem = false;
        ControlFlowManagerV2.confirmDeleteSceneLineItem = false;

        if (!l_buttonCustomizationManager.attributesPanel.gameObject.activeSelf && !l_buttonCustomizationManager.mainPanel.gameObject.activeSelf)
        {
            m_ScriptsManager.saveButton = m_ScriptsManager.resetButton = m_ScriptsManager.cancelButton = false;
        }
    }

    public void YesClicked()
    {
        m_ScriptsManager = ScriptsManager.Instance;
        if (ScriptsManager.Instance.confirmationPanel.confirmationTitleText.text == "Attention")
        {
            isPublishable = false;
        }

        if (isPublishable)
        {
            UndoRedoManager.Clear();
            ScriptsManager.Instance.RemoveUnnecessaryStartPoint();
            //publishContent.PublishContentIntoServer("Publish");
            publishContent.SaveContent("Publish");
        }
        else if (closeBool)
        {
            m_ScriptsManager.CloseProject();
        }
        else if (signOut)
        {
            UndoRedoManager.Clear();
            m_ScriptsManager.SignOut();
        }
        else if (mainSceneBool)
        {
            m_ScriptsManager.OpenScene();
        }
        else if (m_ScriptsManager.isDeletable)
        {
            m_ScriptsManager.DeleteObject();
        }
        else if (m_ScriptsManager.saveButton)
        {
            UndoRedoManager.Clear();
            ButtonCustomizationManager.Instance.attributesManager.SaveChanges();
        }
        else if (m_ScriptsManager.resetButton)
        {
            ButtonCustomizationManager.Instance.attributesManager.ResetValues();
            ButtonCustomizationManager.Instance.attributesManager.SaveChanges();
        }
        else if (m_ScriptsManager.cancelButton)
        {
            ButtonCustomizationManager.Instance.CloseAttributesPanel();
        }
      
        else if (ControlFlowManagerV2.confirmDeleteSceneObjLineItem)
        {
            ControlFlowManagerV2.Instance.DeleteMethodForSceneObjLineItem(ControlFlowManagerV2.Instance.ObjTobeDeleted);
        }
        else if (ControlFlowManagerV2.confirmDeleteSceneLineItem)
        {
            ControlFlowManagerV2.Instance.DeleteMethodForSceneLineItem();
        }
        if (!isPublishable)
        {
            CloseConfirmationPanel();
        }
    }

    public void PublishContent()
    {
        isPublishable = true;

        confirmationTitleText.text = "Publish";
        submitButtonText.text = "Publish";
        confirmationTextFieldText.text = "Would you like to publish your work?";
        cancelButtonText.text = "Cancel";
        confirmationIcon.sprite = Helper.Instance.PopUpSprite("Publish");

        OpenConfirmationPanel();
    }


    public void CloseButton()
    {
        closeBool = true;

        confirmationTitleText.text = "Close Project";
        confirmationTextFieldText.text = "Would you like to close your project? Any unsaved changes will be lost.";
        submitButtonText.text = "Close";
        cancelButtonText.text = "Cancel";
        confirmationIcon.sprite = Helper.Instance.PopUpSprite("Close");

        OpenConfirmationPanel();
    }

    public void SignOutButton()
    {
        signOut = true;

        confirmationTitleText.text = "Sign Out";
        confirmationTextFieldText.text = "Would you like to sign out?";
        submitButtonText.text = "Sign Out";
        cancelButtonText.text = "Cancel";
        confirmationIcon.sprite = Helper.Instance.PopUpSprite("SignOut");

        OpenConfirmationPanel();
    }

    public void OpenDuplicatePopUp(bool isComponentAdded)
    {
        duplicatePopUp.SetActive(true);

        switch (isComponentAdded)
        {
            case true:
                messageText.text = "You may only add one component to the selected label. Please try removing the existing component before adding a new one.";
                break;
            case false:
                messageText.text = "The transition label you are trying to add is already added to the selected object.";
                break;
        }
    }

    public void CloseDuplicatePopUp()
    {
        duplicatePopUp.SetActive(false);
    }
}