using Crosstales.RTVoice.Tool;
using RuntimeInspectorNamespace;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Added on: 23/07/2018
/// Added by: Periyasamy
/// Purpose: To handle text content implementation
/// </summary>
public class TextContentComponent : MonoBehaviour
{
    public TMP_InputField titleTextInput;
    public TMP_InputField textInput;

    public TMP_InputField contentWidthInput;
    public TMP_InputField fontSizeInput;
    public TMP_InputField titleFontSize;
    public TextMeshProUGUI playTTSButtonText;
    public Image contentColorImage;
    public Color contentColorValue;
    public Image headingColorImage;
    public Color headingColorValue;
    public Image panelBGColorImage;
    public Color panelBGColorValue;
    public TextContentCanvasHandler panelProperty;
    public TMP_Dropdown alignmentDropdown;

    [SerializeField]
    private TMP_Text showHideButtonText;

    [SerializeField]
    private Button testTTSButton;

    public Toggle enableTTSToggle;

    private SpeechText speechManager;

    private void CloseProject()
    {
        titleTextInput.text = "";
        textInput.text = "";

        contentWidthInput.text = "";
        fontSizeInput.text = "";
        titleFontSize.text = "";
        contentColorValue = headingColorValue = Color.white;
        panelBGColorValue = Color.black;
        alignmentDropdown.value = 0;
    }

    private void Awake()
    {
        speechManager = gameObject.GetComponent<SpeechText>();
    }

    private void Start()
    {
        enableTTSToggle.onValueChanged.AddListener(ttsToggle);
        //textInput.onValueChanged.AddListener(val);
    }

    void ttsToggle(bool temp)
    {
        if (temp == false)
        {
            // gameObject.GetComponent<SpeechText>().StopAllCoroutines();
        }
    }

    private void onSaveContent()
    {
        //  ScriptsManager._XRStudioContent.textContent = new List<TextContent>();

        TextContent textContent = new TextContent();
        if (panelProperty)
            textContent.visible = panelProperty.gameObject.activeSelf;
        textContent.text = textInput.text;
        textContent.contentWidth = float.Parse(contentWidthInput.text);
        textContent.fontSize = int.Parse(fontSizeInput.text);
        textContent.titleText = titleTextInput.text;
        textContent.titleFontSize = int.Parse(titleFontSize.text);
        textContent.headingColor = headingColorValue;
        textContent.bgColor = panelBGColorValue;
        //   print(panelProperty.panelBg.GetComponent<RectTransform>().sizeDelta);
        //   print(panelProperty.panelBg.GetComponent<RectTransform>().rect);
        //  print(panelProperty.panelBg.GetComponent<RectTransform>().position);
        //  print(panelProperty.panelBg.GetComponent<RectTransform>().localPosition);

        //   print(panelProperty.panelBg.GetComponent<RectTransform>().anchoredPosition);


        textContent.bgrectValue = panelProperty.panelBg.GetComponent<RectTransform>().sizeDelta;
        if (panelProperty != null)
            textContent.panelName = panelProperty.gameObject.name;
        else
            return;

        if (panelProperty.gameObject.GetComponent<ObjectTransformComponent>())
            textContent.transform = panelProperty.gameObject.GetComponent<ObjectTransformComponent>().currentTransform;

        textContent.enableTTS = enableTTSToggle.isOn;
        textContent.Count = ScriptsManager.Instance.panelCount;

        ScriptsManager._toolContent.textContent.Add(textContent);
        //   Debug.LogError("Text Component!");
    }

    public void SubscribeToDelegate()
    {
        //PublishContent.OnSaveContentClick += onSaveContent;
        ScriptsManager.OnCloseClicked += CloseProject;
    }

    private void OnDestroy()
    {
        // PublishContent.OnSaveContentClick -= onSaveContent;
        ScriptsManager.OnCloseClicked -= CloseProject;
    }

    public void GetContentValue(TextContent textContent)
    {
        textInput.text = textContent.text;
        contentWidthInput.text = textContent.contentWidth.ToString();
        fontSizeInput.text = textContent.fontSize.ToString();
        titleTextInput.text = textContent.titleText;
        titleFontSize.text = textContent.titleFontSize.ToString();
        headingColorValue = textContent.headingColor;
        headingColorImage.color = textContent.headingColor;
        contentColorValue = textContent.contentColor;
        contentColorImage.color = textContent.contentColor;
        panelBGColorValue = textContent.bgColor;
        panelBGColorImage.color = textContent.bgColor;
        panelProperty.panelBg.color = textContent.bgColor;
        panelProperty.heading.color = textContent.headingColor;
        panelProperty.content.color = textContent.contentColor;
        enableTTSToggle.isOn = textContent.enableTTS;
        alignmentDropdown.value = textContent.alignmentValue;

        OnContentChanged();
        OnContentWidthChanged();
        OnFontSizeChanged();
        OnTitleFontSizeChanged();
        OnTitleTextChanged();
        OnTextAlignmentChange();
    }

    public TextContent SetContentValue(TextContent textContent)
    {
        textContent.text = textInput.text;
        textContent.contentWidth = int.Parse(contentWidthInput.text);
        textContent.fontSize = int.Parse(fontSizeInput.text);
        textContent.titleText = titleTextInput.text;
        textContent.titleFontSize = int.Parse(titleFontSize.text);
        textContent.headingColor = headingColorImage.color;
        textContent.bgColor = panelProperty.panelBg.color;
        textContent.headingColor = panelProperty.content.color;
        textContent.enableTTS = enableTTSToggle.isOn;

        return textContent;
    }

    bool textoperate;
    public KeyCode lastkey;

    //Calling text content input field value changed
    public void OnContentChanged()
    {
        panelProperty.content.text = panelProperty.textContentCurrentObjDetails.text = textInput.text;
        refresh(panelProperty);
        //DefaultPanelHeight();
    }

    //Calling content width input field value changed
    public void OnContentWidthChanged()
    {
        //if (textoperate == false)
        //{
        //    //  print("shift");
        //    //textoperate = false;
        //    return;
        //}
        if (contentWidthInput.text.Contains("-") || contentWidthInput.text.Contains("."))
        {
            print("sasas");
            contentWidthInput.text = "";
            return;
        }
        if (!string.IsNullOrEmpty(contentWidthInput.text) && contentWidthInput.text != "." && contentWidthInput.text != "-")
        {
            float inputValue = Helper.Instance.RestrictInputValue(contentWidthInput, 1.0f, 999.0f);
            float width = inputValue;
            panelProperty.textContentCurrentObjDetails.contentWidth = width;
            panelProperty.panelBg.GetComponent<RectTransform>().sizeDelta = new Vector2(width + 20, panelProperty.panelBg.GetComponent<RectTransform>().sizeDelta.y);
            panelProperty.content.GetComponent<RectTransform>().sizeDelta = new Vector2(width, panelProperty.content.GetComponent<RectTransform>().sizeDelta.y);
            panelProperty.heading.GetComponent<RectTransform>().sizeDelta = new Vector2(width, panelProperty.heading.GetComponent<RectTransform>().sizeDelta.y);
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

    public void OnFontSizeChanged()
    {
        if (fontSizeInput.text.Contains("-") || fontSizeInput.text.Contains("."))
        {
            print("sasas");
            fontSizeInput.text = "";
            return;
        }
        if (!string.IsNullOrEmpty(fontSizeInput.text) && fontSizeInput.text != "." && fontSizeInput.text != "-")
        {
            ScriptsManager.Instance.LocalLogs("yyyyyyyyyy");
            int inputValue = Helper.Instance.RestrictInputValue(fontSizeInput, 1, 500);


            panelProperty.content.fontSize = inputValue;
            panelProperty.textContentCurrentObjDetails.fontSize = inputValue;
            //   OnContentChanged(); //calling to fix content resize issue..
            OnContentWidthChanged();
        }
        if (fontSizeInput.text == "-")
        {
            fontSizeInput.text = "";
        }
    }

    public void OnTitleFontSizeChanged()
    {
        if (titleFontSize.text.Contains("-") || titleFontSize.text.Contains("."))
        {
            print("sasas");
            titleFontSize.text = "";
            return;
        }
        if (!string.IsNullOrEmpty(titleFontSize.text) && titleFontSize.text != "." && titleFontSize.text != "-")
        {
            int inputValue = Helper.Instance.RestrictInputValue(titleFontSize, 1, 500);
            panelProperty.heading.fontSize = inputValue;
            panelProperty.textContentCurrentObjDetails.titleFontSize = inputValue;
            OnTitleTextChanged(); //calling to fix content resize issue..
            OnContentWidthChanged();
            refresh(panelProperty);

        }
        if (titleFontSize.text == "-")
        {
            titleFontSize.text = "";
        }
    }

    public void OnTitleTextChanged()
    {
        panelProperty.heading.text = panelProperty.textContentCurrentObjDetails.titleText = titleTextInput.text;
        refresh(panelProperty);
        //DefaultPanelHeight();
    }

    private void DefaultPanelHeight()
    {
        if (panelProperty.heading.text.Length == 0 && panelProperty.content.text.Length == 0)
        {
            RectTransform rectTransform = panelProperty.panelBg.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(rectTransform.rect.width, 25.0f);
        }
    }

    public void OnShowBgColorPicker()
    {
        ScriptsManager.Instance.transformGizmo.ManipulationControl();
        ColorPicker.Instance.Show(OnBgColorChanged, (Color32)panelBGColorValue);
    }

    public void OnBgColorChanged(Color32 color)
    {
        panelBGColorImage.color = color;
        panelBGColorValue = color;
        panelProperty.panelBg.color = color;
        panelProperty.textContentCurrentObjDetails.bgColor = color;
    }

    public void OnShowContentColorPicker()
    {
        ScriptsManager.Instance.transformGizmo.ManipulationControl();
        ColorPicker.Instance.Show(OnContentColorChanged, (Color32)contentColorValue);
    }

    public void OnShowHeadingColorPicker()
    {
        ScriptsManager.Instance.transformGizmo.ManipulationControl();
        ColorPicker.Instance.Show(OnHeadingColorChange, (Color32)headingColorValue);
    }

    public void OnTextAlignmentChange()
    {
        switch (alignmentDropdown.value)
        {
            case 0:
                panelProperty.content.alignment = panelProperty.heading.alignment = TextAlignmentOptions.TopLeft;
                break;
            case 1:
                panelProperty.content.alignment = panelProperty.heading.alignment = TextAlignmentOptions.Center;
                break;
            case 2:
                panelProperty.content.alignment = panelProperty.heading.alignment = TextAlignmentOptions.TopRight;
                break;
            case 3:
                panelProperty.content.alignment = panelProperty.heading.alignment = TextAlignmentOptions.TopJustified;
                break;
            case 4:
                panelProperty.content.alignment = panelProperty.heading.alignment = TextAlignmentOptions.TopFlush;
                break;
        }
        panelProperty.alignmentValue = panelProperty.textContentCurrentObjDetails.alignmentValue = alignmentDropdown.value;
    }

    public void OnContentColorChanged(Color32 color)
    {
        contentColorImage.color = color;
        contentColorValue = color;
        panelProperty.content.color = color;
        panelProperty.textContentCurrentObjDetails.contentColor = color;
    }

    private void OnHeadingColorChange(Color32 color)
    {
        headingColorImage.color = color;
        headingColorValue = color;
        panelProperty.heading.color = color;
        panelProperty.textContentCurrentObjDetails.headingColor = color;
    }

    public void ShowHidePaenlNoRec()
    {
        panelProperty.gameObject.SetActive(!panelProperty.gameObject.activeSelf);
        if (panelProperty.gameObject.activeSelf)
        {

            showHideButtonText.text = "Hide Panel";

        }
        else
        {
            showHideButtonText.text = "Show Panel";

        }
        //   Debug.LogError("call");

    }
    public void ShowHidePanel()
    {
        panelProperty.gameObject.SetActive(!panelProperty.gameObject.activeSelf);
        if (panelProperty.gameObject.activeSelf)
        {

            showHideButtonText.text = "Hide Panel";
            if (ScriptsManager.Instance.lastCLickedHierarchyTransform != null)
            {
                //   ScriptsManager.Instance.lastCLickedHierarchyTransform.ToggleOnOff();
            }
        }
        else
        {
            showHideButtonText.text = "Show Panel";
            if (ScriptsManager.Instance.lastCLickedHierarchyTransform != null)
            {
                //  ScriptsManager.Instance.lastCLickedHierarchyTransform.ToggleOnOff();
            }
        }
    }

    public void RemoveComponent()
    {
        //   Debug.LogError(ScriptsManager.Instance);
        //  Debug.LogError(ScriptsManager.Instance.scenePropertyRect);
        // Debug.LogError(panelProperty);
        print(this.gameObject.name + "end");
        VoiceStop();
        //if (panelProperty != null)
        //     ScriptsManager.Instance.ControlFLowRefDelete(panelProperty.transform);

        //if(ScriptsManager.Instance.enableDisableComponent.AllDesccriptionPanel.Contains(this))
        //{
        //    ScriptsManager.Instance.enableDisableComponent.AllDesccriptionPanel.Remove(this);
        //} 

        // Debug.LogError();
        if (panelProperty != null)
        {
            Transform temp = ScriptsManager.Instance.scenePropertyRect.Find(panelProperty.gameObject.name);

            if (temp != null)
            {
                Destroy(temp.gameObject);
            }


            //if (panelProperty)
            Destroy(panelProperty.gameObject);
        }

        ScriptsManager.Instance.transformComponent.SetActive(false);
        ScriptsManager.Instance.currentObjectName.text = "";
        if (gameObject)
            Destroy(gameObject);
    }

    public void OnTTSToggleClick()
    {
        testTTSButton.gameObject.SetActive(enableTTSToggle.isOn);
        panelProperty.textContentCurrentObjDetails.enableTTS = enableTTSToggle.isOn;
        VoiceStop();
    }

    public void VoiceStop()
    {
        GameObject temp = GameObject.Find("RTVoice");
        if (temp != null)
        {
            if (temp.GetComponent<AudioSource>())
            {
                temp.GetComponent<AudioSource>().Stop();
            }
        }
        speechManager.Silence();
        playTTSButtonText.text = "play";
        StopCoroutine(StopTTS());
    }

    public void OnTestTTSClick()
    {
        speechManager.CurrentText = $"{titleTextInput.text}\n{textInput.text}";

        if (speechManager.CurrentText.Length > 0)
        {
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

    public void SetInactive()
    {
        gameObject.SetActive(false);
        //Start(); // fix to add deligate
    }
}
