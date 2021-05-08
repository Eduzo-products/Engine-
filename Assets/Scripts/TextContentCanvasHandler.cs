using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextContentCanvasHandler : MonoBehaviour
{
    public Image panelBg;
    public TMP_Text heading;
    public TMP_Text content;
    public RectTransform canvasRect;
    public TextContent textContentCurrentObjDetails;
    public int alignmentValue = 0;
    public SceneObjectLineItem thisSceneObjLineItem;

    //differentiating between label text panel and normal text panel in the space
    public bool isSceneTextpanel = false;

    public void OnUserClick()
    {
        if(!XR_studio_ControlFlow.flowDefault.currentStateAlive)
        ScriptsManager.Instance.runtimeHierarchy.Select(transform);
    }

    private void Start()
    {
        PublishContent.OnSaveContentClick += OnSaveContent;
    }
    private void OnDestroy()
    {
        PublishContent.OnSaveContentClick -= OnSaveContent;
    }

    public void AssignValues()
    {
        panelBg.color = textContentCurrentObjDetails.bgColor;
        heading.text = textContentCurrentObjDetails.titleText;
        content.text = textContentCurrentObjDetails.text;
        content.fontSize = textContentCurrentObjDetails.fontSize;
        heading.fontSize = textContentCurrentObjDetails.titleFontSize;
        content.color = textContentCurrentObjDetails.contentColor;
        heading.color = textContentCurrentObjDetails.headingColor;
        isSceneTextpanel = textContentCurrentObjDetails.isSceneTextpanel;
        alignmentValue = textContentCurrentObjDetails.alignmentValue;
        //canvasRect.sizeDelta = new Vector2(textContentCurrentObjDetails.contentWidth, canvasRect.rect.height);
    }

    public void OnSaveContent()
    {
        textContentCurrentObjDetails.bgrectValue = panelBg.GetComponent<RectTransform>().sizeDelta;
        textContentCurrentObjDetails.panelName = gameObject.name;

        if (GetComponent<ObjectTransformComponent>())
        {
            textContentCurrentObjDetails.transform = GetComponent<ObjectTransformComponent>().currentTransform;
        }

        textContentCurrentObjDetails.Count = ScriptsManager.Instance.panelCount;
        textContentCurrentObjDetails.isSceneTextpanel = isSceneTextpanel;
        textContentCurrentObjDetails.alignmentValue = alignmentValue;

        if (isSceneTextpanel)
        {
            ScriptsManager._toolContent.textContent.Add(textContentCurrentObjDetails);
        }
    }
}