using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using CommandUndoRedo;

public class InputFieldTest : ICommand
{
    private TMP_InputField inputField;
    private string undoText;
    private string redoText;

    public InputFieldTest(TMP_InputField inputField, string undoText)
    {
        this.inputField = inputField;
        this.undoText = undoText;
        
    }
    public void StoreNewValues(string redoTxt)
    {
       redoText = redoTxt;
    }

    public void Execute()
    {
        OnRedo();
    }

    public void UnExecute()
    {
        OnUndo();
    }

    private void OnUndo()
    {
        inputField.text = undoText;
    }

    private void OnRedo()
    {
        inputField.text = redoText;
    }
}