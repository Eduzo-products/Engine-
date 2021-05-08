using RuntimeGizmos;
using RuntimeInspectorNamespace;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class raycasthitting : MonoBehaviour
{
    bool dragging = false;
    public static bool enable_label_flag = false;
    public Transform target;

    public bool firsthit = false;
    public GameObject prefab;
    GameObject gameObj2Label;
    public Color c1 = Color.blue;
    public Color c2 = Color.white;

    //list of labels available in the 3d space, dispite of models it is mapped 
    public List<GameObject> labels;

    public List<Transform> LineStart;
    //public List<Transform> LineEnd;
    public List<string> label2Obj;
    GameObject newObj2;
    GameObject newObj1;
    public long Count = 0;
    public RuntimeHierarchy runtimeHierarchy;
    public TransformGizmo transformGizmo;

    private GameObject startPoint;
    //    endPoint;
    private string labelToObject = "";

    private void Start()
    {
        labels = new List<GameObject>();
        LineStart = new List<Transform>();
      //  LineEnd = new List<Transform>();
        label2Obj = new List<string>();
        PublishContent.OnSaveContentClick += onSaveContent;
        ScriptsManager.OnCloseClicked += CloseProject;
   
        enable_label_flag = false;
        firsthit = false;
    }

    private void OnDestroy()
    {
        PublishContent.OnSaveContentClick -= onSaveContent;
        ScriptsManager.OnCloseClicked -= CloseProject;
    }

    private void onSaveContent()
    {
        List<Vector3> lineStartPoint = new List<Vector3>();
        List<string> label2Objects = new List<string>();

        for (int i = 0; i < labels.Count; i++)
        {
            Label_Publish prop = labels[i].GetComponent<Label_Publish>();

            if (prop.selectedPropertyType == PropertyType.none)
            {
                lineStartPoint.Add(LineStart[i].position);
                label2Objects.Add(label2Obj[i]);
            }
        }

       // ScriptsManager._toolContent.addArrowDetails.lineEnd = lineEndPoint;
        ScriptsManager._toolContent.addArrowDetails.lineStart = lineStartPoint;
        ScriptsManager._toolContent.addArrowDetails.label2Objects = label2Objects;
        ScriptsManager._toolContent.totalLabels = Count;
    }

    void CloseProject()
    {
        foreach (var obj in labels)
        {
            Destroy(obj);
        }
        labels.Clear();
        foreach (var obj in LineStart)
        {
            Destroy(obj.gameObject);
        }
        LineStart.Clear();
        // foreach (var obj in LineEnd)
        // {
        //     Destroy(obj.gameObject);
        // }
        // LineEnd.Clear();
        label2Obj.Clear();
        Count = 0;
    }

    public LayerMask selectionMask = Physics.DefaultRaycastLayers;

    void Update()
    {
        if (!ScriptsManager.Instance.isAssets && !ScriptsManager.Instance.isVideo && !ScriptsManager.Instance.isAudio && !ScriptsManager.Instance.isSkybox && !ScriptsManager.Instance.isImage && !ColorPicker.isActive)
        {
            if (enable_label_flag)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                    if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, selectionMask))
                    {
                        if (hitInfo.transform.tag == "videoPlane" || hitInfo.transform.gameObject.layer == 5)
                        {
                            return;
                        }
                        else
                        {
                            if (!firsthit)
                            {
                                firsthit = true;
                                startP = hitInfo.point;
                                target = hitInfo.transform;

                                gameObj2Label = target.gameObject;

                                runtimeHierarchy.Refresh();
                                runtimeHierarchy.Select(target);

                                transformGizmo.AddTargetHighlightedRenderers(target);

                                distance = (Camera.main.transform.position + startP).magnitude;
                                GameObject local = new GameObject("Start");
                                local.tag = "points";
                                local.transform.position = startP;
                                local.transform.parent = gameObj2Label.transform;

                                startPoint = local;
                                LineStart.Add(local.transform);
                                return;
                            }
                        }
                    }
                    else if (Battlehub.RTCommon.Pointer.isSceneWindowEnter)
                    {
                        ScriptsManager.Instance.runtimeHierarchy.Deselect();

                        ScriptsManager.Instance.DeactivateAllComponent();
                        //DisableTextPanels();
                        ScriptsManager.Instance.GetComponent<Label_components>().ClearComp();
                    }

                    if (firsthit)
                    {
                        if (Battlehub.RTCommon.Pointer.isSceneWindowEnter)
                        {
                            Vector3 direc = ray.direction * distance;
                            endP = direc + Camera.main.transform.position;

                            firsthit = false;

                            GameObject canvas = Instantiate(prefab, endP, Quaternion.identity);
                            canvas.transform.SetParent(gameObj2Label.transform);
                        //TransistionScript ts = ScriptsManager.Instance.transistionObject.GetComponent<TransistionScript>();

                            canvas.name = "Label " + Count.ToString();
                           // canvas.GetComponent<ObjectNavigator>().root = canvas;
                            canvas.GetComponent<Label_Publish>().currentObjectName = canvas.name;
                            canvas.GetComponent<Label_Publish>().parentTransform = gameObj2Label.transform;

                            if (!gameObj2Label.GetComponent<AddedContents>())
                            {
                                gameObj2Label.AddComponent<AddedContents>();
                            }

                            labels.Add(canvas);

                            ScriptsManager.Instance.GetComponent<Label_components>().ClearComp();
                            Count++;
                            labels[labels.Count - 1].transform.parent = gameObj2Label.transform;
                            string str = "";
                            while (gameObj2Label.transform != null)
                            {
                                str = str + "/" + gameObj2Label.name;
                                if (gameObj2Label.transform.parent != null)
                                    gameObj2Label = gameObj2Label.transform.parent.gameObject;
                                else
                                {
                                    label2Obj.Add(str);
                                    labelToObject = str;
                                    break;
                                }
                            }
                            string[] words = label2Obj[label2Obj.Count - 1].Split('/');
                            str = "";
                            for (int i = words.Length - 1; i > 0; i--)
                            {
                                str = str + "/" + words[i];
                            }

                            label2Obj[label2Obj.Count - 1] = str;
                             canvas.GetComponent<Label_Publish>().startPoint = startPoint.transform.position;
                         //   canvas.GetComponent<Label_Publish>().endPoint = endPoint.transform.position;
                            canvas.GetComponent<Label_Publish>().labelToObject = labelToObject;


                            transformGizmo.ClearAllHighlightedRenderers();
                        }
                    }
                }
            }
        }

        if (labels.Count > 0)
        {
            for (int i = 0; i < labels.Count; i++)
            {
                if (LineStart.Count <= 0)
                    break;

              //  labels[i].GetComponent<LineRenderer>().SetPosition(0, LineStart[i].position);
              //  labels[i].GetComponent<LineRenderer>().SetPosition(1, LineEnd[i].position);
            }
        }
    }

    void DrawL()
    {
        DrawLine(startP, endP, Color.green, 100f);
    }

    void DrawLine(Vector3 start, Vector3 end, Color color, float duration = 0.2f)
    {
        GameObject myLine = new GameObject();
        myLine.transform.position = start;
        myLine.AddComponent<LineRenderer>();
        LineRenderer lr = myLine.GetComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));
        lr.startColor = Color.blue;
        lr.endColor = Color.white;
        lr.startWidth = 0.1f;
        lr.endWidth = 0.1f;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        GameObject.Destroy(myLine, duration);
    }
    Vector3 startP;
    float distance = 0f;
    Vector3 endP;


    void OnMouseDown()
    {
        print("called");
    }

    public void remove(int i)
    {
        Destroy(LineStart[i].gameObject);
        LineStart.RemoveAt(i);
    //    Destroy(LineEnd[i].gameObject);
     //   LineEnd.RemoveAt(i);
        Destroy(labels[i]);
        labels.RemoveAt(i);
    }

    public void removeLableObject(int i)
    {
        label2Obj.RemoveAt(i);
        //rename();
    }

    public void rename()
    {
        for (int i = 0; i < labels.Count; i++)
            labels[i].name = i.ToString() + labels[i].name.Remove(0, 1);
    }

   

    public void DisableTextPanels()
    {
        if (labels.Count > 0)
        {
            for (int p = 0; p < labels.Count; p++)
            {
                Label_Publish label_Publish = labels[p].GetComponent<Label_Publish>();

                if (label_Publish.selectedPropertyType.Equals(PropertyType.LabelWithDescription))
                {
                    label_Publish.descriptionPanelObj.SetActive(false);
                }
            }
        }
    }
}
