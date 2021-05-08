using Crosstales.RTVoice.Tool;
using RuntimeInspectorNamespace;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Added on: 23/07/2018
/// Added by: Periyasamy
/// Purpose: To handle text content implementation
/// </summary>

public class LabelWithTextContentProperty : MonoBehaviour
{
    public TMP_InputField labelTextInput;
    public TMP_InputField titleTextInput;
    public TMP_InputField textContentInput;
    public TMP_InputField contentWidthInput;

    public TMP_InputField fontSize;
    public TMP_InputField titleFontSize;
    public Image contentColorImage;
    public Color contentColorValue;
    public Image headingColorImage;
    public Color headingColorValue;
    public Image panelBGColorImage;
    public Color panelBGColorValue;
    public TMP_Dropdown alignmentDropdown;
    private TextContentCanvasHandler panelProperty;
    [SerializeField]
    private TMP_Text showHideButtonText;
    public Label_Publish label_Publish;

    [SerializeField]
    private Button testTTSButton;
    public TextMeshProUGUI playTTSButtonText;
    public Toggle enableTTSToggle;
    public SpeechText speechManager;

    // Start is called before the first frame update
    void Start()
    {
        ScriptsManager.OnCloseClicked += CloseProject;
    }

    private void CloseProject()
    {
        RemoveComponent();
        ClearFields();
    }

    private void OnDestroy()
    {
        ScriptsManager.OnCloseClicked -= CloseProject;
    }

    public void SetPropertyValue()
    {
        label_Publish.descriptionPanelObj.SetActive(!label_Publish.descriptionPanelObj.activeSelf);
        panelProperty = label_Publish.descriptionPanelObj.GetComponent<TextContentCanvasHandler>();
        panelProperty.content.text = label_Publish.labelWithTextContent.contentDetails.text;
        labelTextInput.text = label_Publish.labelWithTextContent.labelText;

        textContentInput.text = label_Publish.labelWithTextContent.contentDetails.text;
        contentWidthInput.text = label_Publish.labelWithTextContent.contentDetails.contentWidth.ToString();
        fontSize.text = label_Publish.labelWithTextContent.contentDetails.fontSize.ToString();
        panelProperty.heading.text = label_Publish.labelWithTextContent.contentDetails.titleText;
        titleTextInput.text = label_Publish.labelWithTextContent.contentDetails.titleText;
        panelProperty.content.fontSize = label_Publish.labelWithTextContent.contentDetails.fontSize;
        panelProperty.heading.fontSize = label_Publish.labelWithTextContent.contentDetails.titleFontSize;
        panelProperty.content.color = label_Publish.labelWithTextContent.contentDetails.contentColor;
        panelProperty.heading.color = label_Publish.labelWithTextContent.contentDetails.headingColor;

        contentColorValue = contentColorImage.color = label_Publish.labelWithTextContent.contentDetails.contentColor;
        headingColorValue = headingColorImage.color = label_Publish.labelWithTextContent.contentDetails.headingColor;
        panelProperty.panelBg.color = panelBGColorValue = panelBGColorImage.color = label_Publish.labelWithTextContent.contentDetails.bgColor;
        titleFontSize.text = label_Publish.labelWithTextContent.contentDetails.titleFontSize.ToString();

        enableTTSToggle.isOn = label_Publish.labelWithTextContent.contentDetails.enableTTS;
        alignmentDropdown.value = label_Publish.labelWithTextContent.contentDetails.alignmentValue;
        refresh(panelProperty);
    }

    public void OnLabelTextChanged()
    {
        if (labelTextInput.text.Length > 0)
        {
            label_Publish.textObj.text = label_Publish.labelWithTextContent.labelText = labelTextInput.text;
        }
        else
        {
            label_Publish.textObj.text = "Enter label name";
            label_Publish.labelWithTextContent.labelText = "";
        }
    }

    //Calling text content input field value changed
    public void OnContentChanged()
    {
        panelProperty.content.text = textContentInput.text;
        label_Publish.labelWithTextContent.contentDetails.text = textContentInput.text;
        //  contentwidth();
        refresh(panelProperty);
    }

    //Calling content width input field value changed
    public void OnContentWidthChanged()
    {
        if (contentWidthInput.text.Contains("-") || contentWidthInput.text.Contains("."))
        {
            contentWidthInput.text = "";
            return;
        }
        if (!string.IsNullOrEmpty(contentWidthInput.text) && contentWidthInput.text != "." && contentWidthInput.text != "-" && !contentWidthInput.text.StartsWith("-") && !contentWidthInput.text.StartsWith("."))
        {
            float inputValue = Helper.Instance.RestrictInputValue(contentWidthInput, 1.0f, 999.0f);
            float w = inputValue;
            panelProperty.panelBg.GetComponent<RectTransform>().sizeDelta = new Vector2(w + 20, panelProperty.panelBg.GetComponent<RectTransform>().sizeDelta.y);
            panelProperty.content.GetComponent<RectTransform>().sizeDelta = new Vector2(w, panelProperty.content.GetComponent<RectTransform>().sizeDelta.y);
            panelProperty.heading.GetComponent<RectTransform>().sizeDelta = new Vector2(w, panelProperty.heading.GetComponent<RectTransform>().sizeDelta.y);

            label_Publish.labelWithTextContent.contentDetails.contentWidth = w;
            refresh(panelProperty);
        }
    }

    void refresh(TextContentCanvasHandler temp)
    {
        if (temp != null)
        {
            RectTransform[] ar = temp.gameObject.GetComponentsInChildren<RectTransform>();
            LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());

            for (int i = 0; i < ar.Length; i++)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(ar[i]);
            }
        }
    }

    void contentwidth()
    {
        float w = float.Parse(contentWidthInput.text);
        panelProperty.panelBg.GetComponent<RectTransform>().sizeDelta = new Vector2(w + 20, panelProperty.panelBg.GetComponent<RectTransform>().sizeDelta.y);
        panelProperty.content.GetComponent<RectTransform>().sizeDelta = new Vector2(w, panelProperty.content.GetComponent<RectTransform>().sizeDelta.y);
        panelProperty.heading.GetComponent<RectTransform>().sizeDelta = new Vector2(w, panelProperty.heading.GetComponent<RectTransform>().sizeDelta.y);

        label_Publish.labelWithTextContent.contentDetails.contentWidth = w;
    }

    public void OnFontSizeChaned()
    {
        if (fontSize.text.Contains("-") || fontSize.text.Contains("."))
        {
            fontSize.text = "";
            return;
        }
        if (!string.IsNullOrEmpty(fontSize.text) && fontSize.text != "." && fontSize.text != "-")
        {
            panelProperty.content.fontSize = int.Parse(fontSize.text);
            label_Publish.labelWithTextContent.contentDetails.fontSize = int.Parse(fontSize.text);
            OnContentChanged(); //calling to fix content resize issue..
        }

        OnContentWidthChanged();
    }

    public void OnTitleFontSizeChanged()
    {
        if (titleFontSize.text.Contains("-") || titleFontSize.text.Contains("."))
        {
            titleFontSize.text = "";
            return;
        }
        if (!string.IsNullOrEmpty(titleFontSize.text) && titleFontSize.text != "." && titleFontSize.text != "-")
        {
            panelProperty.heading.fontSize = int.Parse(titleFontSize.text);
            label_Publish.labelWithTextContent.contentDetails.titleFontSize = int.Parse(titleFontSize.text);
            OnTitleTextChanged(); //calling to fix content resize issue..
        }

        OnContentWidthChanged();
    }

    public void OnTitleTextChanged()
    {
        panelProperty.heading.text = titleTextInput.text;
        label_Publish.labelWithTextContent.contentDetails.titleText = titleTextInput.text;
        OnContentWidthChanged();
        refresh(panelProperty);
    }

    public void OnShowBgColorPicker()
    {
        ScriptsManager.Instance.transformGizmo.ManipulationControl();
        ColorPicker.Instance.Show(OnBgColorChanged, (Color32)panelBGColorValue);
    }

    void OnBgColorChanged(Color32 color)
    {
        panelBGColorImage.color = color;
        panelBGColorValue = color;
        panelProperty.panelBg.color = color;
        label_Publish.labelWithTextContent.contentDetails.bgColor = color;
    }

    public void OnShowContentColorPicker()
    {
        ScriptsManager.Instance.transformGizmo.ManipulationControl();
        ColorPicker.Instance.Show(OnContentColorChanged, (Color32)contentColorValue);
    }

    void OnContentColorChanged(Color32 color)
    {
        contentColorImage.color = color;
        contentColorValue = color;
        panelProperty.content.color = color;
        label_Publish.labelWithTextContent.contentDetails.contentColor = color;
    }

    public void OnShowHeadingColorPicker()
    {
        ScriptsManager.Instance.transformGizmo.ManipulationControl();
        ColorPicker.Instance.Show(OnHeadingColorChanged, (Color32)headingColorValue);
    }

    private void OnHeadingColorChanged(Color32 color)
    {
        headingColorImage.color = color;
        headingColorValue = color;
        panelProperty.heading.color = color;
        label_Publish.labelWithTextContent.contentDetails.headingColor = color;
    }

    public void OnTextAlignmentChange()
    {
        switch (alignmentDropdown.value)
        {
            case 0:
                panelProperty.heading.alignment = panelProperty.content.alignment = TextAlignmentOptions.TopLeft;
                break;
            case 1:
                panelProperty.heading.alignment = panelProperty.content.alignment = TextAlignmentOptions.Center;
                break;
            case 2:
                panelProperty.heading.alignment = panelProperty.content.alignment = TextAlignmentOptions.TopRight;
                break;
            case 3:
                panelProperty.heading.alignment = panelProperty.content.alignment = TextAlignmentOptions.Justified;
                break;
            case 4:
                panelProperty.heading.alignment = panelProperty.content.alignment = TextAlignmentOptions.Flush;
                break;
        }
        panelProperty.alignmentValue = label_Publish.labelWithTextContent.contentDetails.alignmentValue = alignmentDropdown.value;
    }

    public void ShowHidePanel()
    {
        label_Publish.descriptionPanelObj.SetActive(!label_Publish.descriptionPanelObj.activeSelf);
        if (label_Publish.descriptionPanelObj.activeSelf)
            showHideButtonText.text = "Hide Panel";
        else
            showHideButtonText.text = "Show Panel";
    }

    public void RemoveComponent()
    {
        gameObject.SetActive(false);
        speechManager.Silence();
        playTTSButtonText.text = "play";
        StopCoroutine(StopTTS());
    }

    private void ClearFields()
    {
        labelTextInput.text = "";
        titleTextInput.text = "";
        textContentInput.text = "";
        contentWidthInput.text = "";
        fontSize.text = "";
        titleFontSize.text = "";
        alignmentDropdown.value = 0;
    }

    public void closeButton()
    {
        ScriptsManager.Instance.runtimeHierarchy.Deselect();
        ScriptsManager.Instance.transformComponent.SetActive(false);
        List<GameObject> labelGobject = ScriptsManager.Instance.GetComponent<raycasthitting>().labels;
        for (int i = 0; i < labelGobject.Count; i++)
        {
            if (labelGobject[i].name == label_Publish.gameObject.name)
            {

                Destroy(labelGobject[i]);
                ScriptsManager.Instance.GetComponent<raycasthitting>().remove(i);
                ScriptsManager.Instance.GetComponent<raycasthitting>().removeLableObject(i);
                break;
            }
        }

        ScriptsManager.Instance.GetComponent<Label_components>().labelComp.SetActive(false);
        gameObject.SetActive(false);
    }

    public void OnTTSToggleClick()
    {
        testTTSButton.gameObject.SetActive(enableTTSToggle.isOn);
        if (label_Publish != null)
        {
            label_Publish.labelWithTextContent.contentDetails.enableTTS = enableTTSToggle.isOn;
        }
    }

    public void OnTestTTSClick()
    {
        speechManager.CurrentText = $"{titleTextInput.text}\n{textContentInput.text}";

        if (!speechManager.Source.isPlaying)
        {
            speechManager.Speak();
            playTTSButtonText.text = "stop";
            StartCoroutine(StopTTS());
        }
        else
        {
            speechManager.Silence();
            playTTSButtonText.text = "play";
            StopCoroutine(StopTTS());
        }
    }

    private IEnumerator StopTTS()
    {
        yield return new WaitUntil(() => speechManager.Source.isPlaying);

        while (speechManager.Source.isPlaying)
        {
            yield return null;
        }
        playTTSButtonText.text = "play";
    }

    
}
