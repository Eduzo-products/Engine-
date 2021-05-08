using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DeletePopup : PopupBase, IPopupManager
{
    private TextMeshProUGUI title;
    private TextMeshProUGUI message;
    private Button confirmation;
    private Button cancellation;
    private TextMeshProUGUI confirmationButtonText;
    private TextMeshProUGUI cancellationButtonText;
    private bool isDeletable;

    public DeletePopup(DeletePopupVariable deletePopupVariable, string title, string message, string confirm, string cancel)
    {
        popup = deletePopupVariable.popup;
        this.title = deletePopupVariable.title;
        this.message = deletePopupVariable.message;
        confirmation = deletePopupVariable.confirmation;
        cancellation = deletePopupVariable.cancellation;
        confirmationButtonText = deletePopupVariable.confirmationButtonText;
        cancellationButtonText = deletePopupVariable.cancellationButtonText;
        isDeletable = true;

        this.title.text = title;
        this.message.text = message;
        confirmationButtonText.text = confirm;
        cancellationButtonText.text = cancel;
        confirmation.onClick.AddListener(() => { Confirmation(); });
        cancellation.onClick.AddListener(() => { Cancellation(); });
    }

    public override void OpenPopUp()
    {
        popup.SetActive(true);
    }

    public override void ClosePopUp()
    {
        popup.SetActive(false);
        isDeletable = false;
        RemoveAllListenres();
    }

    private void RemoveAllListenres()
    {
        confirmation.onClick.RemoveListener(() => { Cancellation(); });
        cancellation.onClick.RemoveListener(() => { Cancellation(); });
    }

    public void Cancellation()
    {
        ClosePopUp();
    }

    public void Confirmation()
    {
        ClosePopUp();
    }
}
