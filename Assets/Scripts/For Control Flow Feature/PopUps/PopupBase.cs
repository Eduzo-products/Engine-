using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupBase
{
    protected GameObject popup;

    public virtual void OpenPopUp()
    {
        popup.SetActive(true);
    }

    public virtual void ClosePopUp()
    {
        popup.SetActive(false);
    }
}