using CommandUndoRedo;
using TMPro;
using UnityEngine;

public class InputFieldCommand : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;
    ICommand command = null;

    private void Awake()
    {
        inputField = GetComponent<TMP_InputField>();
        inputField.onEndEdit.AddListener(delegate { OnEndEdit(); });
        inputField.onSelect.AddListener(delegate { OnSelect(); });
    }

    private void OnSelect()
    {
        command = new InputFieldTest(inputField, inputField.text);
    }

    private void OnEndEdit()
    {
        if (inputField != null && command != null)
        {

            ((InputFieldTest)command).StoreNewValues(inputField.text);
            UndoRedoManager.Insert(command);
        }
    }

    private void OnDestroy()
    {
        inputField.onEndEdit.RemoveListener(delegate { OnEndEdit(); });
        inputField.onSelect.RemoveListener(delegate { OnSelect(); });

    }
}