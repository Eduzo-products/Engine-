using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public struct DeletePopupVariable
{
    public GameObject popup;
    public TextMeshProUGUI title;
    public TextMeshProUGUI message;
    public Button confirmation;
    public Button cancellation;
    public TextMeshProUGUI confirmationButtonText;
    public TextMeshProUGUI cancellationButtonText;
}