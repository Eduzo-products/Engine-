using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class saveAsset
{
    public AssetDetails assetDetails;
    public string name;
}

public class MyAssets : MonoBehaviour
{
    public GameObject ModelBtnPrefab;
    public Button libraryBtn;
    public Button myCloudBtn;
    public Button addBtn;
    public Button remove;
    public Button cloudRefresh;
    public Button LocalRefresh;

    public List<Image> title = new List<Image>();
    public Color highlight;


    public GameObject specialTab;

    public void titleColor()
    {
        for (int i = 0; i < title.Count; i++)
        {
            title[i].sprite = ResourceManager.Instance.radialDeselected;
        }

    }

    public void initEnable()
    {
        ScriptsManager.Instance.enableDisableComponent.Reset();
        ScriptsManager.Instance.ModuleVideoStop();

        ScriptsManager.Instance.featureselectFunc();
        RefreshBtn();
        titleColor();
        ScriptsManager.Instance.isAssets = true;
        ScriptsManager.Instance.showAssetFunc(myCloudBtn.GetComponent<Image>());
        //ScriptsManager.Instance.showLibraryAsset(libraryBtn.GetComponent<Image>());
        LocalRefresh.gameObject.SetActive(true);

        //  ScriptsManager.Instance.showAssetFunc(myCloudBtn.GetComponent<Image>());
    }
    // Start is called before the first frame update
    void Start()
    {

        myCloudBtn.onClick.AddListener(RefreshBtn);
        myCloudBtn.onClick.AddListener(titleColor);
        libraryBtn.onClick.AddListener(titleColor);
        LocalRefresh.onClick.AddListener(titleColor);
        //  myCloudBtn.onClick.AddListener(ScriptsManager.Instance.showAssetFunc);
        myCloudBtn.onClick.AddListener(() => ScriptsManager.Instance.showAssetFunc(myCloudBtn.GetComponent<Image>()));
        cloudRefresh.onClick.AddListener(() => ScriptsManager.Instance.showAssetFunc(null));

        cloudRefresh.onClick.AddListener(RefreshBtn);
        //  cloudRefresh.onClick.AddListener(titleColor);

        libraryBtn.onClick.AddListener(RefreshBtn);
        LocalRefresh.onClick.AddListener(RefreshBtn);

        remove.onClick.AddListener(titleColor);

        remove.onClick.AddListener(refresh);
        remove.onClick.AddListener(refresh);
        remove.onClick.AddListener(ScriptsManager.Instance.RoutineStop);

        libraryBtn.onClick.AddListener(() => ScriptsManager.Instance.showLibraryAsset(libraryBtn.GetComponent<Image>()));
        LocalRefresh.onClick.AddListener(() => ScriptsManager.Instance.showLibraryAsset(libraryBtn.GetComponent<Image>()));
    }

    public List<FileDetails> prefabBtn = new List<FileDetails>();
    [SerializeField]
    public saveAsset LastClicked;

    public void RefreshBtn()
    {
        //colorReset();
        addBtn.gameObject.SetActive(false);

        for (int i = 0; i < prefabBtn.Count; i++)
        {
            Destroy(prefabBtn[i].gameObject);
        }
        prefabBtn.Clear();
    }


    public void myAssetPrefabInst(AssetDetails AD, string name)
    {
        LastClicked = null;
        GameObject temp = Instantiate(ModelBtnPrefab, ModelBtnPrefab.transform.parent);
        temp.gameObject.SetActive(true);
        temp.GetComponent<FileDetails>().assetDetails = AD;
        ScriptsManager.Instance.TextureDownloadInit(AD.ThumbnailURL, temp.GetComponent<FileDetails>().buttonImage);
        temp.GetComponent<FileDetails>().fileName.text = name;
        temp.GetComponent<Button>().onClick.RemoveAllListeners();
        temp.GetComponent<Button>().onClick.AddListener(() => listenLastClickFunc(AD, name, temp));
        prefabBtn.Add(temp.GetComponent<FileDetails>());
    }
    [SerializeField]
    //Button addBtn;

    public void listenLastClickFunc(AssetDetails det, string name, GameObject obj)
    {
        saveAsset temp = new saveAsset();
        temp.assetDetails = det;
        temp.name = name;
        //  Debug.LogError(temp.assetDetails);
        LastClicked = temp;
        addBtn.onClick.RemoveAllListeners();
        colorReset();
        obj.transform.GetChild(0).GetComponent<Image>().color = highlight;
        //obj.GetComponent<Image>().color = highlight;
        addBtn.onClick.AddListener(() => addBtnFunc());
        addBtn.gameObject.SetActive(true);

    }

    void colorReset()
    {
        addBtn.gameObject.SetActive(false);

        for (int i = 0; i < prefabBtn.Count; i++)
        {
            prefabBtn[i].transform.GetChild(0).GetComponent<Image>().color = Color.white;
        }
    }
    void addBtnFunc()
    {
        if (LastClicked == null)
        {
            return;
        }
        ScriptsManager.Instance.isAssets = false;
        ScriptsManager.Instance.DownloadFiles(LastClicked.assetDetails, LastClicked.name, null);
        //


        //RoutineStop();
        RefreshBtn();
        this.gameObject.SetActive(false);
        LastClicked = null;
    }
    public void refresh()
    {
        RefreshBtn();
        LastClicked = null;


    }
}
