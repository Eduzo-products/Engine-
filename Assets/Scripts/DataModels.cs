using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class GestureString
{
    public string move;
    public string rotate;
    public string scale;
}

[Serializable]
public class Browser
{
    public int Id;
    public string BtnPath;
    public string referencedTo = "";
    public bool enable;
}

[Serializable]
public class ToolContent
{
    public List<SceneSequence> allSequence = new List<SceneSequence>();
    public string skyboxURL;
    public bool moveScript;
    public bool rotScript;
    public bool scaleScript;
    public bool isExplode;
    public bool isGesture;
    public int modelCount;
    public int projectType;
    public int totalDSButtons;
    public long totalLabels;
    public List<DSButton> dsButtons = new List<DSButton>();
    //for control flow - scene object data (saravanan)
    public List<SceneLineItemData> sceneLineItemsToSave = new List<SceneLineItemData>();
    public List<BundleDetails> bundleDetails = new List<BundleDetails>();
    public List<AssetDetails> assetDetails = new List<AssetDetails>();
    public List<TextContent> textContent = new List<TextContent>();
    public List<XRTextToSpeech> textToSpeech = new List<XRTextToSpeech>();
    public List<AddAudio> addAudio = new List<AddAudio>();
    public List<ObjectTransform> objectNewTransforms = new List<ObjectTransform>();
    public List<AddVideo> addVideo = new List<AddVideo>();
    public List<AddVideoLabel> addVideoLabel = new List<AddVideoLabel>();
    public List<AddLabel> addLabel = new List<AddLabel>();
    //not needed
    public LabelArrowDetails addArrowDetails = new LabelArrowDetails();
    public List<LabelWithAudio> labelWithAudios = new List<LabelWithAudio>();
    public List<LabelWithTextToSpeech> labelWithTextToSpeeches = new List<LabelWithTextToSpeech>();
    public List<LabelWithTextContent> labelWithTextContents = new List<LabelWithTextContent>();
    public List<ActiveObjects> activeObjects = new List<ActiveObjects>();
    public List<CustomButtons> customButtons = new List<CustomButtons>();
    public List<LightComponents> lightComponents = new List<LightComponents>();
    public GestureString gesture;
    public int videoCount;
    public float explodeRange;
    public float explodeSpeed;
}

[Serializable]
public class DSButton
{
    public int type;
    public int uniqueID;
    public float width, height, fontSize;
    public Color normalColor, highlightColor, pressedColor;
    public Color textColor;
    public string givenName, alternateName;
    public string referencedTo = "";
}

//for control flow - scene data (from saravnan)
[Serializable]
public class SceneLineItemData
{
    public Properties _thisSceneProperty;
    public string sceneUnqiueId;
    public short _idNumeric;
    public List<SceneObjectLineItemData> sceneObjList = new List<SceneObjectLineItemData>();

}

[Serializable]
public class SceneObjectLineItemData
{

    public string objName;
    public short id;

}




[Serializable]
public class Timer
{
    public int time;
}


[Serializable]
public class ExplodeProperty
{
    public string gameObjectName;
    public string fullPath;
    public bool explodeEffect;
    public float explodeRange;
    public float explodeSpeed;
}



[Serializable]
public class TextContent
{
    public ObjectTransform transform;
    public string titleText;
    public string text;
    public float contentWidth;
    
    public int titleFontSize;
    public int fontSize;
    public Color contentColor;
    public Color headingColor;
    public Color bgColor;
    public string panelName;
    public bool visible;
    public bool enableTTS;
    public int Count;
    public Vector2 bgrectValue;
    public bool isSceneTextpanel;
    public int alignmentValue;
}

[Serializable]
public class LabelWithTextContent
{
    public Vector3 lineStartPoint;
    public Vector3 lineEndPoint;
    public string labelText;
    public string attachedObjectName;
    public string attachedObjectFullPath;
    public TextContent contentDetails;
}

[Serializable]
public class AddAudio
{
    public string audioURL;
    public float startTime;
    public float endTime;
    public bool playOnStart;
    public bool playOnObjectEnable;
    public bool playOnObjectClick;
    public string attachedObject;
    public string name;
}

[Serializable]
public class LabelWithAudio
{
    public Vector3 lineStartPoint;
    public Vector3 lineEndPoint;
    public string labelText;
    public string attachedObjectName;
    public string attachedObjectFullPath;
    public AddAudio audioDetails;
}

[Serializable]
public class AddVideo
{
    public string videoURL;
    public int videoID;
    public float startTime;
    public float endTime;
    public bool playOnStart;
    public bool playOnObjectEnable;
    public bool playOnObjectClick;
    public string attachedObject;
    public int aspectRatio;
    public float width;
    public float height;
    public bool enabled;
    public string videoType;
    public Vector3 position, scale, rotation;
    public int totalcount;
    public ObjectTransform ObjectTransform;
    public bool visible;
    public string name;
}

[Serializable]
public class AddVideoLabel
{
    public string videoURL;
    public float startTime;
    public float endTime;
    public bool playOnStart;
    public bool playOnObjectEnable;
    public bool playOnObjectClick;
    public string attachedObject;
    public int aspectRatio;
    public float width;
    public float height;
    public bool enabled;
    public string videoType;
    public Vector3 position, scale, rotation;


}

[Serializable]
public class AddLabel
{
    public List<GameObject> labels;
    public string label2Objects;
    public string labelName;
    public bool videoEnabled;
    public bool audioEnabled;
    public bool text2SpeechEnabled;
    public bool descriptionEnabled;
    public AddVideoLabel videoDetails;
    public AddAudio audioDetails;
    public TextContent textDetails;
    public XRTextToSpeech textToSpeechDetails;
    public string name;
    public List<string> parentNames;
    public List<string> childNames;
}



[Serializable]
public class AddLabelGroups
{
    public string name;
    public List<string> parentsname;
    public List<string> childsname;
    public bool parentflag;
}
[Serializable]
public class LabelArrowDetails
{
    public List<Vector3> lineStart;
    public List<Vector3> lineEnd;
    public List<string> label2Objects;
}
[Serializable]
public class AddLabel_new
{

    public Vector3 lineStart;
   // public Vector3 lineEnd;
    public string label2Objects;
    public string labelName;
    public bool videoEnabled;
    public bool audioEnabled;
    public bool text2SpeechEnabled;
    public bool descriptionEnabled;
    public AddVideo videoDetails;
    public AddAudio audioDetails;
    public TextContent textDetails;
    public XRTextToSpeech textToSpeechDetails;

}
[Serializable]
public class AddLabelVideo
{
    public List<string> labelParent;
    public List<AddVideo> labelVideo;
}
[Serializable]
public class AddLabelName
{
    public List<string> labelParent;
    public List<string> labelName;
    public List<bool> labelClickable;
}
[Serializable]
public class AddLabelAudio
{
    public List<string> labelParent;
    public List<string> audioURL;
}
[Serializable]
public class AddLabelDescription
{
    public List<string> labelParent;
    public List<TextContent> textContent;
}
[Serializable]
public class AddLabelText2Audio
{
    public List<string> labelParent;
    public List<XRTextToSpeech> labelText2Speech;
}

[Serializable]
public class XRTextToSpeech
{
    public string text;
    public bool playOnStart;
    public bool playOnObjectEnable;
    public bool playOnObjectClick;
    public string attachedObject;
}

[Serializable]
public class LabelWithTextToSpeech
{
    public Vector3 lineStartPoint;
    public Vector3 lineEndPoint;
    public string labelText;
    public string attachedObjectName;
    public string attachedObjectFullPath;
    public XRTextToSpeech textToSpeechDetails;
}

[Serializable]
public class PublishedContentFileList
{
    public string FileName;
    public string Status;
    public string UpdatedOn;
}

[Serializable]
public class FileListResponseData
{
    public int ResponseCode;
    public string Message;
    public List<PublishedContentFileList> Data;
}

[Serializable]
public class ObjectTransform
{
    public string objectName;
    public string objectFullPath;
    public ObjectPosition position;
    public ObjectRotation rotation;
    public ObjectScale scale;
    // public int id;
}



[Serializable]
public class ObjectPosition
{
    public float x = 0.0f;
    public float y = 0.0f;
    public float z = 0.0f;

    public Vector3 getVec()
    {
        return new Vector3(x, y, z);
    }
}

[Serializable]
public class ObjectScale
{
    public float x = 0.0f;
    public float y = 0.0f;
    public float z = 0.0f;

    public Vector3 getVec()
    {
        return new Vector3(x, y, z);
    }
}

[Serializable]
public class ObjectRotation
{
    public float x = 0.0f;
    public float y = 0.0f;
    public float z = 0.0f;

    public Vector3 getVec()
    {
        return new Vector3(x, y, z);
    }
}

[Serializable]
public class ReceivedContent
{
    public int ResponseCode;
    public string Message;

    public ToolContent Data;
}

[Serializable]
public class ClientDetails
{
    public int clientId;
    public string firstName;
    public string lastName;
    public string emailId;
}
[Serializable]
public class LoginResponseData
{
    public int ResponseCode;
    public string Message;
    public ClientDetails Data;
}

[Serializable]
public class ResponseContent
{
    public int ResponseCode;
    public string Message;
    public object Data;
}

[Serializable]
public class ExistingProject
{
    public int ResponseCode;
    public string Message;
    public bool Data;
}

public enum PropertyType
{
    none,
    LabelWithAudio,
    LabelWithDescription,
    LabelWithTextToSpeech,
    Image
}

[Serializable]
public class ActiveObjects
{
    public string parentFullPath = "";
    public List<string> activeObjectName = new List<string>();
    public List<bool> activeState = new List<bool>();
}

[Serializable]
public class ConfirmationPanelIcons
{
    public string iconName;
    public Sprite iconSprite;
}

[Serializable]
public class SaveData
{
    public string userName, passWord, projectName;
    public float tranX, tranY, tranZ, rotX, rotY, rotZ, scaleX, scaleY, scaleZ;
    public string colorValue;
}

[Serializable]
public class DownloadAsset
{
    public int ResponseCode;
    public string Message;

    public List<AssetDetails> Data;
}

//[Serializable]
//public class AnimationControlFlow
//{
//    public AssetDetails assetDetails;
//    public float totalLength;
//    public bool frame;
//    public float startSec;
//    public float endSec;
//    public string path;
//}


[Serializable]
public class BundleDetails
{
    public int assetId;
    public int clientId;
    public string bundleWindowsURL;
    public string bundleAndroidURL;
    public string bundleIOSURL;
    public string fileName;
    public string gameObjectName;
    public string gameObjectFullPath;

    public AssetDetails assetDetails;
}

[Serializable]
public class AssetDetails
{
    public int AssetId;
    public int clientId;

    public int AssetTypeId;
    public float FileSize;
    public string Duration;
    public string FileName;
    public string FileURL;
    public string WindowsURL;
    public string AndroidURL;
    public string IOSURL;
    public string ThumbnailURL;
    public bool InQueue;
    public List<string> childName = new List<string>();
    public List<bool> childActiveState = new List<bool>();
    public Dictionary<string, bool> childVisibility = new Dictionary<string, bool>();
    public ObjectTransform ObjectTransform;
}

[Serializable]
public class DownloadCloudAssets
{
    public int ResponseCode;
    public string Message;

    public List<AssetDetails> Data;
}

[Serializable]
public class DownloadLibraryAssets
{
    public int ResponseCode;
    public string Message;

    public List<AssetDetails> Data;
}

[Serializable]
public class CustomButtons
{
    public int ID = 0;
    public string customButtonName = "", name = "", alternateName = "";
    public float width = 0.0f, height = 0.0f, fontSize = 0.0f;
    public Color normalColor, highlightColor, pressedColor;
    public Color textNormal, textHighlight;
    public string referencedTo = "", path = "";
    public string normalImageURL = "";
    public string highlightImageURL = "";
    public string pressedImageURL = "";
    public int buttonType;
    public ObjectTransform objectTransform;
}

[Serializable]
public class LightComponents
{
    public int lightDropDownValue;
    public Color lightColor;
    public float intensity;
    public float indirectMultiplier;
    public float angle;
    public float range;
    public float sliderValue;
    public string path;
    public bool isActive;
}