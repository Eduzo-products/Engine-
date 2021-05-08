using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SubMeshes
{
    public MeshRenderer meshRenderer;
    public Vector3 originalPosition;
    public Vector3 explodedPosition;
}

public class ThreeDModelFunctions : MonoBehaviour
{
    #region VARIABLES
    public List<SubMeshes> childMeshRenderers;
    public bool isInExplodedView = false;
    public float explosionSpeed = 0.1f;
    public float explosionDistance = 3.5f;
    public bool isMoving = false;
    public bool canExplode = false;
    public bool canAnimate = false;
    public bool saveAnimate = false;

    public ActiveObjects activeObjects = new ActiveObjects();
    public AssetDetails objDetails = new AssetDetails();
    public int totalChildCount = 0;

    public List<string> childsName = new List<string>();
    public List<bool> childsActivateStatus = new List<bool>();

    private string fullPath = "";
    private string objectName = "";
    public Animation AnimationComp;
    public bool HasAnimation;


    public SceneObjectLineItem thisSceneObjLineItem;

    public ModelObjectLineItem thisModelLineItem;



    #endregion

    #region UnityFunctions
    public void CollectClips()
    {
        if (GetComponent<Animation>())
        {
            if (GetComponent<Animation>().GetClipCount() > 0)
            {
                canAnimate = true;
            }
        }
    }

    private void Awake()
    {
        if (GetComponent<Animation>())
        {
            if (GetComponent<Animation>().GetClipCount() > 0)
            {
                canAnimate = true;
                GetComponent<Animation>().Stop();
            }
        }

        childMeshRenderers = new List<SubMeshes>();

        foreach (var item in GetComponentsInChildren<MeshRenderer>())
        {
            SubMeshes mesh = new SubMeshes();

            mesh.meshRenderer = item;
            mesh.originalPosition = item.transform.position;
            mesh.explodedPosition = item.bounds.center * explosionDistance;

            childMeshRenderers.Add(mesh);
        }
        GetAnimationObj();
    }

    private void Start()
    {
        fullPath = ScriptsManager.Instance.GetObjectFullPath(gameObject);
        objectName = gameObject.name;
        objDetails.ObjectTransform.objectFullPath = fullPath;
        objDetails.ObjectTransform.objectName = objectName;

        if (!GetComponent<ObjectTransformComponent>())
        {
            gameObject.AddComponent<ObjectTransformComponent>();
        }

        GetComponent<ObjectTransformComponent>().SetTransform(transform.position, transform.eulerAngles, transform.localScale);
        objDetails.ObjectTransform = GetComponent<ObjectTransformComponent>().currentTransform;

        PublishContent.OnSaveContentClick += OnSaveExplodeAndAnim;
    }

    private void OnDestroy()
    {
        PublishContent.OnSaveContentClick -= OnSaveExplodeAndAnim;
    }

    private void OnSaveExplodeAndAnim()
    {
        GetTotalChildren();

        if (ScriptsManager.Instance.isExplodable)
        {
            //ExplodeProperty explodeProperty = new ExplodeProperty();

            //explodeProperty.gameObjectName = objectName;
            //explodeProperty.explodeEffect = canExplode;
            //explodeProperty.explodeRange = explosionDistance;
            //explodeProperty.explodeSpeed = explosionSpeed;
            //explodeProperty.fullPath = fullPath;

            //ScriptsManager._XRStudioContent.explodeProperty.Add(explodeProperty);

            if (isInExplodedView)
            {
                ToggleExplodedView();
            }

        }

       
    }

    public void updateSettings()
    {
        if (!isInExplodedView)
        {
            childMeshRenderers = new List<SubMeshes>();

            foreach (var item in GetComponentsInChildren<MeshRenderer>())
            {
                SubMeshes mesh = new SubMeshes();
                mesh.meshRenderer = item;
                mesh.originalPosition = item.transform.position;
                // print(mesh.originalPosition);
                //mesh.explodedPosition = item.bounds.center * explosionDistance;
                mesh.explodedPosition = (item.bounds.center - transform.position) * explosionDistance + transform.position;//Added by Peri on 04-12-2020
                // print(mesh.explodedPosition);
                childMeshRenderers.Add(mesh);
            }
        }
    }

    private void Update()
    {
        if (isMoving)
        {
            if (childMeshRenderers.Count > 1)
            {
                if (isInExplodedView)
                {
                    foreach (var item in childMeshRenderers)
                    {
                        item.meshRenderer.transform.position = Vector3.Lerp(item.meshRenderer.transform.position, item.explodedPosition, explosionSpeed);

                        if (item.meshRenderer.gameObject.GetComponent<ObjectTransformComponent>())
                        {
                            ObjectTransformComponent objectTransformComponent = item.meshRenderer.gameObject.GetComponent<ObjectTransformComponent>();
                            objectTransformComponent.SetTransform(item.meshRenderer.gameObject.transform.position, item.meshRenderer.gameObject.transform.eulerAngles, item.meshRenderer.gameObject.transform.localScale);
                        }
                        //if (Vector3.Distance(item.meshRenderer.transform.position, item.originalPosition) < 0f)
                        if (Vector3.Distance(item.meshRenderer.transform.position, item.explodedPosition) < 0.0001f)
                        {
                            item.meshRenderer.transform.position = item.explodedPosition;
                            isMoving = false;
                        }
                    }
                }
                else
                {
                    foreach (var item in childMeshRenderers)
                    {
                        item.meshRenderer.transform.position = Vector3.Lerp(item.meshRenderer.transform.position, item.originalPosition, explosionSpeed);

                        if (item.meshRenderer.gameObject.GetComponent<ObjectTransformComponent>())
                        {
                            ObjectTransformComponent objectTransformComponent = item.meshRenderer.gameObject.GetComponent<ObjectTransformComponent>();
                            objectTransformComponent.SetTransform(item.meshRenderer.gameObject.transform.position, item.meshRenderer.gameObject.transform.eulerAngles, item.meshRenderer.gameObject.transform.localScale);
                        }
                        //if (Vector3.Distance(item.meshRenderer.transform.position, item.originalPosition) < 0f) 
                        if (Vector3.Distance(item.meshRenderer.transform.position, item.originalPosition) < 0.0001f)
                        {
                            item.meshRenderer.transform.position = item.originalPosition;
                            isMoving = false;
                        }
                    }
                }
            }
            else
            {
                isMoving = false;
            }
        }
    }
    #endregion

    #region CustomFunctions




    public void ToggleExplodedView()
    {
        if (isInExplodedView)
        {
            isInExplodedView = false;
            isMoving = true;
        }
        else
        {
            isInExplodedView = true;
            isMoving = true;
        }
    }

    void GetAnimationObj()
    {
        AnimationComp = this.transform.GetComponentInChildren<Animation>();
        if (!ScriptsManager.Instance.allAnimation.Contains(AnimationComp))
            ScriptsManager.Instance.allAnimation.Add(AnimationComp);

        AnimationComp.Stop();
        if (AnimationComp.GetClipCount() > 0)
        {
            if (AnimationComp != null)
            {
                HasAnimation = true;
            }
        }
    }

    public void GetTotalChildren()
    {
        AddChild();
        AddActiveObj();
    }

    public void AddChild()
    {
        childsName.Clear();
        childsActivateStatus.Clear();

        Transform[] childTransforms = transform.GetComponentsInChildren<Transform>(true);
        totalChildCount = childTransforms.Length - 1;

        foreach (Transform item in childTransforms)
        {
            if (!item.GetComponent<SkipThisObject>())
            {
                childsName.Add(item.name);
                childsActivateStatus.Add(item.gameObject.activeSelf);
            }
        }
    }

    public void AddActiveObj()
    {
        ScriptsManager._toolContent.activeObjects.Add(new ActiveObjects
        {
            parentFullPath = fullPath,
            activeObjectName = childsName,
            activeState = childsActivateStatus
        });
    }
    public class modelControlFlow
    {
        public Dictionary<string, bool> dict;//=new Dictionary<string, bool>();
        public List<string> str;//=new List<string>();
        public List<bool> strBool;//=new List<bool>();
        public modelControlFlow()
        {
            dict = new Dictionary<string, bool>();
            str = new List<string>();
            strBool = new List<bool>();
        }



    }
    public modelControlFlow GetTotalChildrenForControlFlowRef()
    {
        modelControlFlow mc = new modelControlFlow();
        objDetails.childName.Clear();
        objDetails.childActiveState.Clear();

        Transform[] childTransforms = transform.GetComponentsInChildren<Transform>(true);
        Dictionary<string, bool> tempDic = new Dictionary<string, bool>();

        foreach (Transform item in childTransforms)
        {
            if (!item.GetComponent<SkipThisObject>())
            {
                //   objDetails.childName.Add(item.name);
                mc.str.Add(item.name);
                //  objDetails.childActiveState.Add(item.gameObject.activeSelf);
                mc.strBool.Add(item.gameObject.activeSelf);
                //  tempDic.Add(item.name, item.gameObject.activeSelf);
                //mc.dict.Add(item.name, item.gameObject.activeSelf);
            }
        }
        //   mc.dict = tempDic;
        //  mc.str = objDetails.childName;
        //mc.strBool = objDetails.childActiveState;

        return mc;
    }

    public void CleanLabels()
    {
        BroadcastMessage("OnButtonListClose", SendMessageOptions.DontRequireReceiver);
    }


    public void BroadCast_DeleteLabels()
    {
        BroadcastMessage("OnButtonListClose", SendMessageOptions.DontRequireReceiver);
    }

    #endregion
}