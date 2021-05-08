using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ControlFlowButton : MonoBehaviour
{
    public int type;
    public int uniqueID;
    public float width, height, fontSize;
    public Color normalColor, highlightColor, pressedColor;
    public Color textColor;
    public string givenName, alternateName;
    public string referencedTo = "";
    public RectTransform parentTransform;
    public Image image;
    public Sprite rectangleSprite;
    public Sprite curvedSprite;
    public Button button;
    public TextMeshProUGUI text;

    public bool isClicked = false;
    private ScriptsManager m_ScriptsManager;

    public void OnSave()
    {
        DSButton btn = new DSButton
        {
            type = type,
            uniqueID = uniqueID,
            width = width,
            height = height,
            fontSize = fontSize,
            normalColor = normalColor,
            highlightColor = highlightColor,
            pressedColor = pressedColor,
            textColor = textColor,
            givenName = givenName,
            alternateName = alternateName,
            referencedTo = referencedTo
        };
        if (!ScriptsManager._toolContent.dsButtons.Contains(btn))
            ScriptsManager._toolContent.dsButtons.Add(btn);
    }

    public void ApplyChanges()
    {
        parentTransform.sizeDelta = new Vector2(width, height);
        switch (type)
        {
            case 0:
                image.sprite = curvedSprite;
                break;
            case 1:
                image.sprite = rectangleSprite;
                break;
        }

        // image.color = normalColor;

        ColorBlock colorBlock = button.colors;
        colorBlock.normalColor = normalColor;
        colorBlock.highlightedColor = highlightColor;
        colorBlock.pressedColor = pressedColor;
        button.colors = colorBlock;

        text.text = givenName;
        text.fontSize = fontSize;
        text.color = textColor;
    }

    public void OnUserClick()
    {
        m_ScriptsManager = ScriptsManager.Instance;
        m_ScriptsManager.runtimeHierarchy.Refresh();
        m_ScriptsManager.runtimeHierarchy.Select(parentTransform.parent);

        if (m_ScriptsManager.projectTypeDropdown.value.Equals(0))
        {
            if (referencedTo.Equals(AssignButton.Explode.ToString()))
            {
                isClicked = !isClicked;
                switch (isClicked)
                {
                    case true:
                        text.text = alternateName;
                        image.color = highlightColor;
                        break;
                    case false:
                        text.text = givenName;
                        image.color = normalColor;
                        break;
                }
            }
            m_ScriptsManager.eventSystem.SetSelectedGameObject(null);
        }
    }
}