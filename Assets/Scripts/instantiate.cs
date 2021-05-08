using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;


public class instantiate : MonoBehaviour
{
    public GameObject videoPanel;
    public Transform prefab;
    public UnityEngine.UI.Text width;
    public UnityEngine.UI.Text height;
    public TMPro.TMP_InputField url;
    public UnityEngine.UI.Slider widthSlider;
    public UnityEngine.UI.Slider heightSlider;
    public UnityEngine.UI.Slider angle;
    public TMPro.TMP_Dropdown dropDown;
    public UnityEngine.UI.Toggle move;
    public UnityEngine.UI.Toggle rotate;
    public UnityEngine.UI.Toggle scaling;
    public UnityEngine.UI.Toggle explode;
    public GameObject videoPlane;
    public Transform objectCollection;

    void Start()
    {
        //width.text = "1280";
        //height.text = "720";
        //url.text = "https://www.apps.holopundits.com/Holopundits.mp4";// "M:/new.mp4";

        // widthSlider.value = 2.209f;
        // heightSlider.value = 1.0983f;
        PublishContent.OnSaveContentClick += onSaveContent;
    }

    void onSaveContent()
    {
        if (move != null)
            ScriptsManager._toolContent.moveScript = move.isOn;
        if (rotate != null)
            ScriptsManager._toolContent.rotScript = rotate.isOn;
        if (scaling != null)
            ScriptsManager._toolContent.scaleScript = scaling.isOn;

        Debug.Log("Instantiate Script.");
    }

    void OnDestroy()
    {
        PublishContent.OnSaveContentClick -= onSaveContent;
    }
    public void CreateVideoPlayer()
    {
        // Instantiate(prefab, new Vector3(0, 0, 15f), Quaternion.Euler(-270,-90,90));
    }
    public bool isVideoDownloaded = false;
    //public CurvedUI.CurvedUISettings curvedObj;

    public void enableVPanel()
    {
        ScriptsManager.Instance.videoPanel.SetActive(true);
    }

    public void disableVPanel()
    {
        CreateVideoPlayer();
        //  changeResolution();
        // playerProperties();
        //  videoPanel.SetActive(false);

    }
    void changeCurveness()
    {

    }

    void changeResolution()
    {
        print(url.name);
        //return;
        ScriptsManager.Instance.videoPanel.GetComponent<VideoPlayer>().url = url.text.ToString();
        //prefab.GetComponent<VideoPlayer>().targetTexture.width = 700;


        RenderTexture newText = new RenderTexture(int.Parse(width.text.ToString()), int.Parse(height.text.ToString()), 24);
        newText.useDynamicScale = true;
        newText.dimension = UnityEngine.Rendering.TextureDimension.Tex2D;
        ScriptsManager.Instance.videoPanel.GetComponent<VideoPlayer>().targetTexture = newText;
        ScriptsManager.Instance.videoPanel.GetComponent<MeshRenderer>().sharedMaterial.mainTexture = newText;
    }

    public void playerProperties()
    {
        ScriptsManager.Instance.videoPanel.GetComponent<VideoPlayer>().url = url.text;

        RenderTexture newText = new RenderTexture(2000, 1540, 24);
        newText.useDynamicScale = true;
        newText.dimension = UnityEngine.Rendering.TextureDimension.Tex2D;
        ScriptsManager.Instance.videoPanel.GetComponent<VideoPlayer>().targetTexture = newText;
        ScriptsManager.Instance.videoPanel.GetComponent<MeshRenderer>().sharedMaterial.mainTexture = newText;

        isVideoDownloaded = true;
        StartCoroutine(VideoProgress());

        isVideoDownloaded = false;
        ScriptsManager.Instance.videoPanel.GetComponent<VideoPlayer>().Play();
    }

    private IEnumerator VideoProgress()
    {
        if (isVideoDownloaded)
        {
            while (!ScriptsManager.Instance.videoPanel.GetComponent<VideoPlayer>().isPrepared)
            {
                yield return null;
            }
        }
    }

    bool flag = false;
    void LateUpdate()
    {
       
        if (flag)
        {
            // flag = false;
            if (videoPlane.GetComponent<VideoPlayer>().isPlaying)
                videoPlane.SetActive(false);
        }
    }

    public void explodeEffect()
    {
        //explodeEffect local =  obj.AddComponent<explodeEffect>();
        // local.playEffect();
        if (objectCollection.childCount > 0)
        {
            for (int p = 0; p < objectCollection.childCount; p++)
            {
                Transform local = objectCollection.GetChild(p);

                if (local != null && local.GetComponent<ThreeDModelFunctions>())
                {
                    // local.GetComponent<explodeEffect> ( ).playEffect ( );
                    local.GetComponent<ThreeDModelFunctions>().ToggleExplodedView();
                }
            }
        }
    }

    void ApplyExplode(bool enable, float speed = 1, float MaxRange = 10, GameObject newObj = null)
    {
        //print(enable + " apply explode");
        // explodeBut.gameObject.SetActive(enable);
        if (enable && newObj != null)
        {
            // newObj.AddComponent<ThreeDModelFunctions>();
            //   vidply.explodeSpeed = speed;
            //  vidply.MaxRange = MaxRange;
        }
    }

    public void PlayVideo()
    {
        videoPlane.SetActive(true);
        videoPlane.GetComponent<VideoPlayer>().url = url.text;
        videoPlane.GetComponent<MeshRenderer>().sharedMaterial.mainTexture = videoPlane.GetComponent<VideoPlayer>().targetTexture;
        videoPlane.GetComponent<VideoPlayer>().Play();
        //flag = true;
    }
}
