using CommandUndoRedo;
//using XrStudio.CommandStructure;
using RuntimeInspectorNamespace;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RuntimeGizmos
{
    //To be safe, if you are changing any transforms hierarchy, such as parenting an object to something,
    //you should call ClearTargets before doing so just to be sure nothing unexpected happens... as well as call UndoRedoManager.Clear()
    //For example, if you select an object that has children, move the children elsewhere, deselect the original object, then try to add those old children to the selection, I think it wont work.

    [RequireComponent(typeof(Camera))]
    public class TransformGizmo : MonoBehaviour
    {
        #region USERDEFINED
        public GameObject transformComponent = null;
        public List<Transform> childsParent = new List<Transform>();

        public Transform target = null;
        #endregion

        public TransformSpace space = TransformSpace.Global;
        public TransformType transformType = TransformType.Move;
        public TransformPivot pivot = TransformPivot.Pivot;
        public CenterType centerType = CenterType.All;
        public ScaleType scaleType = ScaleType.FromPoint;
        public CurrentObjectTransform curObjTrans;
        //These are the same as the unity editor hotkeys
        public KeyCode SetMoveType = KeyCode.W;
        public KeyCode SetRotateType = KeyCode.E;
        public KeyCode SetScaleType = KeyCode.R;
        //public KeyCode SetRectToolType = KeyCode.T;
        public KeyCode SetAllTransformType = KeyCode.Y;
        public KeyCode SetSpaceToggle = KeyCode.X;
        public KeyCode SetPivotModeToggle = KeyCode.Z;
        public KeyCode SetCenterTypeToggle = KeyCode.C;
        public KeyCode SetScaleTypeToggle = KeyCode.S;
        public KeyCode AddSelection = KeyCode.LeftShift;
        public KeyCode RemoveSelection = KeyCode.LeftControl;
        public KeyCode ActionKey = KeyCode.LeftShift; //Its set to shift instead of control so that while in the editor we dont accidentally undo editor changes =/
        public KeyCode UndoAction = KeyCode.Z;
        public KeyCode RedoAction = KeyCode.Y;

        public Color xColor = new Color(1, 0, 0, 0.8f);
        public Color yColor = new Color(0, 1, 0, 0.8f);
        public Color zColor = new Color(0, 0, 1, 0.8f);
        public Color allColor = new Color(.7f, .7f, .7f, 0.8f);
        public Color selectedColor = new Color(1, 1, 0, 0.8f);
        public Color hoverColor = new Color(1, .75f, 0, 0.8f);
        public float planesOpacity = .5f;
        //public Color rectPivotColor = new Color(0, 0, 1, 0.8f);
        //public Color rectCornerColor = new Color(0, 0, 1, 0.8f);
        //public Color rectAnchorColor = new Color(.7f, .7f, .7f, 0.8f);
        //public Color rectLineColor = new Color(.7f, .7f, .7f, 0.8f);

        public float handleLength = .25f;
        public float handleWidth = .003f;
        public float planeSize = .035f;
        public float triangleSize = .03f;
        public float boxSize = .03f;
        public int circleDetail = 40;
        public float allMoveHandleLengthMultiplier = 1f;
        public float allRotateHandleLengthMultiplier = 1.4f;
        public float allScaleHandleLengthMultiplier = 1.6f;
        public float minSelectedDistanceCheck = .01f;
        public float moveSpeedMultiplier = 1f;
        public float scaleSpeedMultiplier = 1f;
        public float rotateSpeedMultiplier = 1f;
        public float allRotateSpeedMultiplier = 20f;

        public bool useFirstSelectedAsMain = true;

        //If circularRotationMethod is true, when rotating you will need to move your mouse around the object as if turning a wheel.
        //If circularRotationMethod is false, when rotating you can just click and drag in a line to rotate.
        public bool circularRotationMethod;

        //Mainly for if you want the pivot point to update correctly if selected objects are moving outside the transformgizmo.
        //Might be poor on performance if lots of objects are selected...
        public bool forceUpdatePivotPointOnChange = true;

        public int maxUndoStored = 100;

        public bool manuallyHandleGizmo;

        public LayerMask selectionMask = Physics.DefaultRaycastLayers;

        public Action onCheckForSelectedAxis;
        public Action onDrawCustomGizmo;

        public Camera myCamera
        {
            get; private set;
        }

        public bool isTransforming
        {
            get; private set;
        }
        public float totalScaleAmount
        {
            get; private set;
        }
        public Quaternion totalRotationAmount
        {
            get; private set;
        }
        public Axis translatingAxis => nearAxis;
        public Axis translatingAxisPlane => planeAxis;
        public bool hasTranslatingAxisPlane => translatingAxisPlane != Axis.None && translatingAxisPlane != Axis.Any;
        public TransformType transformingType => translatingType;

        public Vector3 pivotPoint
        {
            get; private set;
        }

        private Vector3 totalCenterPivotPoint;

        public Transform mainTargetRoot => (targetRootsOrdered.Count > 0) ? (useFirstSelectedAsMain) ? targetRootsOrdered[0] : targetRootsOrdered[targetRootsOrdered.Count - 1] : null;

        private AxisInfo axisInfo;
        private Axis nearAxis = Axis.None;
        private Axis planeAxis = Axis.None;
        private TransformType translatingType;
        private AxisVectors handleLines = new AxisVectors();
        private AxisVectors handlePlanes = new AxisVectors();
        private AxisVectors handleTriangles = new AxisVectors();
        private AxisVectors handleSquares = new AxisVectors();
        private AxisVectors circlesLines = new AxisVectors();

        //We use a HashSet and a List for targetRoots so that we get fast lookup with the hashset while also keeping track of the order with the list.
        private List<Transform> targetRootsOrdered = new List<Transform>();
        private Dictionary<Transform, TargetInfo> targetRoots = new Dictionary<Transform, TargetInfo>();
        private HashSet<Renderer> highlightedRenderers = new HashSet<Renderer>();
        private HashSet<Transform> children = new HashSet<Transform>();
        private List<Transform> childrenBuffer = new List<Transform>();
        private List<Renderer> renderersBuffer = new List<Renderer>();
        private List<Material> materialsBuffer = new List<Material>();
        private WaitForEndOfFrame waitForEndOFFrame = new WaitForEndOfFrame();
        private Coroutine forceUpdatePivotCoroutine;
        private static Material lineMaterial;
        private static Material outlineMaterial;

        public bool isMovable, isRotateble, isScaleable;

        public Image move, rotate, scale;

        public List<Sprite> manipulateSprites = new List<Sprite>();
        public List<RaycastResult> results = new List<RaycastResult>();
        public GraphicRaycaster graphicsRaycaster;
        private PointerEventData m_PointerEventData;
        public RuntimeHierarchy runtimeHierarchy;

        private void Awake()
        {
            isMovable = isRotateble = isScaleable = false;
            myCamera = GetComponent<Camera>();
            SetMaterial();
        }

        private void OnEnable()
        {
            forceUpdatePivotCoroutine = StartCoroutine(ForceUpdatePivotPointAtEndOfFrame());
        }

        private void OnDisable()
        {
            ClearTargets(); //Just so things gets cleaned up, such as removing any materials we placed on objects.

            StopCoroutine(forceUpdatePivotCoroutine);
        }

        private void OnDestroy()
        {
            ClearAllHighlightedRenderers();
        }

        private void Update()
        {
            HandleUndoRedo();

            SetSpaceAndType();

            if (manuallyHandleGizmo)
            {
                if (onCheckForSelectedAxis != null)
                    onCheckForSelectedAxis();
            }
            else
            {
                SetNearAxis();
            }

            GetTarget();

            if (mainTargetRoot == null)
                return;

            TransformSelected();
        }

        private void LateUpdate()
        {
            if (mainTargetRoot == null)
                return;

            //We run this in lateupdate since coroutines run after update and we want our gizmos to have the updated target transform position after TransformSelected()
            SetAxisInfo();

            if (manuallyHandleGizmo)
            {
                if (onDrawCustomGizmo != null)
                    onDrawCustomGizmo();
            }
            else
            {
                SetLines();
            }
        }

        private void OnPostRender()
        {
            if (mainTargetRoot == null || manuallyHandleGizmo)
                return;

            lineMaterial.SetPass(0);

            Color xColor = (nearAxis == Axis.X) ? (isTransforming) ? selectedColor : hoverColor : this.xColor;
            Color yColor = (nearAxis == Axis.Y) ? (isTransforming) ? selectedColor : hoverColor : this.yColor;
            Color zColor = (nearAxis == Axis.Z) ? (isTransforming) ? selectedColor : hoverColor : this.zColor;
            Color allColor = (nearAxis == Axis.Any) ? (isTransforming) ? selectedColor : hoverColor : this.allColor;

            //Note: The order of drawing the axis decides what gets drawn over what.

            TransformType moveOrScaleType = (transformType == TransformType.Scale || (isTransforming && translatingType == TransformType.Scale)) ? TransformType.Scale : TransformType.Move;
            DrawQuads(handleLines.z, GetColor(moveOrScaleType, this.zColor, zColor, hasTranslatingAxisPlane));
            DrawQuads(handleLines.x, GetColor(moveOrScaleType, this.xColor, xColor, hasTranslatingAxisPlane));
            DrawQuads(handleLines.y, GetColor(moveOrScaleType, this.yColor, yColor, hasTranslatingAxisPlane));

            DrawTriangles(handleTriangles.x, GetColor(TransformType.Move, this.xColor, xColor, hasTranslatingAxisPlane));
            DrawTriangles(handleTriangles.y, GetColor(TransformType.Move, this.yColor, yColor, hasTranslatingAxisPlane));
            DrawTriangles(handleTriangles.z, GetColor(TransformType.Move, this.zColor, zColor, hasTranslatingAxisPlane));

            DrawQuads(handlePlanes.z, GetColor(TransformType.Move, this.zColor, zColor, planesOpacity, !hasTranslatingAxisPlane));
            DrawQuads(handlePlanes.x, GetColor(TransformType.Move, this.xColor, xColor, planesOpacity, !hasTranslatingAxisPlane));
            DrawQuads(handlePlanes.y, GetColor(TransformType.Move, this.yColor, yColor, planesOpacity, !hasTranslatingAxisPlane));

            DrawQuads(handleSquares.x, GetColor(TransformType.Scale, this.xColor, xColor));
            DrawQuads(handleSquares.y, GetColor(TransformType.Scale, this.yColor, yColor));
            DrawQuads(handleSquares.z, GetColor(TransformType.Scale, this.zColor, zColor));
            DrawQuads(handleSquares.all, GetColor(TransformType.Scale, this.allColor, allColor));

            DrawQuads(circlesLines.all, GetColor(TransformType.Rotate, this.allColor, allColor));
            DrawQuads(circlesLines.x, GetColor(TransformType.Rotate, this.xColor, xColor));
            DrawQuads(circlesLines.y, GetColor(TransformType.Rotate, this.yColor, yColor));
            DrawQuads(circlesLines.z, GetColor(TransformType.Rotate, this.zColor, zColor));
        }

        private Color GetColor(TransformType type, Color normalColor, Color nearColor, bool forceUseNormal = false)
        {
            return GetColor(type, normalColor, nearColor, false, 1, forceUseNormal);
        }

        private Color GetColor(TransformType type, Color normalColor, Color nearColor, float alpha, bool forceUseNormal = false)
        {
            return GetColor(type, normalColor, nearColor, true, alpha, forceUseNormal);
        }

        private Color GetColor(TransformType type, Color normalColor, Color nearColor, bool setAlpha, float alpha, bool forceUseNormal = false)
        {
            Color color;
            if (!forceUseNormal && TranslatingTypeContains(type, false))
            {
                color = nearColor;
            }
            else
            {
                color = normalColor;
            }

            if (setAlpha)
            {
                color.a = alpha;
            }

            return color;
        }

        private void HandleUndoRedo()
        {
            if (maxUndoStored != UndoRedoManager.maxUndoStored)
            {
                UndoRedoManager.maxUndoStored = maxUndoStored;
            }

            if (Input.GetKey(ActionKey))
            {
                if (Input.GetKeyDown(UndoAction))
                {
                    print("dddd");
                    UndoRedoManager.Undo();
                }
                else if (Input.GetKeyDown(RedoAction))
                {
                    UndoRedoManager.Redo();
                }
            }
        }

        //We only support scaling in local space.
        public TransformSpace GetProperTransformSpace()
        {
            return transformType == TransformType.Scale ? TransformSpace.Local : space;
        }

        public bool TransformTypeContains(TransformType type)
        {
            return TransformTypeContains(transformType, type);
        }

        public bool TranslatingTypeContains(TransformType type, bool checkIsTransforming = true)
        {
            TransformType transType = !checkIsTransforming || isTransforming ? translatingType : transformType;
            return TransformTypeContains(transType, type);
        }

        public bool TransformTypeContains(TransformType mainType, TransformType type)
        {
            return ExtTransformType.TransformTypeContains(mainType, type, GetProperTransformSpace());
        }

        public float GetHandleLength(TransformType type, Axis axis = Axis.None, bool multiplyDistanceMultiplier = true)
        {
            float length = handleLength;
            if (transformType == TransformType.All)
            {
                if (type == TransformType.Move)
                    length *= allMoveHandleLengthMultiplier;
                if (type == TransformType.Rotate)
                    length *= allRotateHandleLengthMultiplier;
                if (type == TransformType.Scale)
                    length *= allScaleHandleLengthMultiplier;
            }

            if (multiplyDistanceMultiplier)
                length *= GetDistanceMultiplier();

            if (type == TransformType.Scale && isTransforming && (translatingAxis == axis || translatingAxis == Axis.Any))
                length += totalScaleAmount;

            return length;
        }

        private void SetSpaceAndType()
        {
            if (Input.GetKey(ActionKey))
                return;


            if (Input.GetKeyDown(SetMoveType))
            {
                MoveObject();
            }
            else if (Input.GetKeyDown(SetRotateType))
            {
                RotateObject();
            }
            else if (Input.GetKeyDown(SetScaleType))
            {
                ScaleObject();
            }
            //else if(Input.GetKeyDown(SetRectToolType)) type = TransformType.RectTool;
            else if (Input.GetKeyDown(SetAllTransformType))
                transformType = TransformType.All;

            if (!isTransforming)
                translatingType = transformType;

            if (Input.GetKeyDown(SetPivotModeToggle))
            {
                if (pivot == TransformPivot.Pivot)
                    pivot = TransformPivot.Center;
                else if (pivot == TransformPivot.Center)
                    pivot = TransformPivot.Pivot;

                SetPivotPoint();
            }

            if (Input.GetKeyDown(SetCenterTypeToggle))
            {
                if (centerType == CenterType.All)
                    centerType = CenterType.Solo;
                else if (centerType == CenterType.Solo)
                    centerType = CenterType.All;

                SetPivotPoint();
            }

            if (Input.GetKeyDown(SetSpaceToggle))
            {
                if (space == TransformSpace.Global)
                    space = TransformSpace.Local;
                else if (space == TransformSpace.Local)
                    space = TransformSpace.Global;
            }

            if (Input.GetKeyDown(SetScaleTypeToggle))
            {
                if (scaleType == ScaleType.FromPoint)
                    scaleType = ScaleType.FromPointOffset;
                else if (scaleType == ScaleType.FromPointOffset)
                    scaleType = ScaleType.FromPoint;
            }

            if (transformType == TransformType.Scale)
            {
                if (pivot == TransformPivot.Pivot)
                    scaleType = ScaleType.FromPoint; //FromPointOffset can be inaccurate and should only really be used in Center mode if desired.
            }
        }

        private void TransformSelected()
        {
            if (mainTargetRoot != null)
            {
                if (nearAxis != Axis.None && Input.GetMouseButtonDown(0))
                {
                    StartCoroutine(TransformSelected(translatingType));
                }
            }
        }

        private IEnumerator TransformSelected(TransformType transType)
        {
            isTransforming = true;
            totalScaleAmount = 0;
            totalRotationAmount = Quaternion.identity;

            Vector3 originalPivot = pivotPoint;

            Vector3 axis = GetNearAxisDirection();
            Vector3 planeNormal = hasTranslatingAxisPlane ? axis : (transform.position - originalPivot).normalized;
            Vector3 projectedAxis = Vector3.ProjectOnPlane(axis, planeNormal).normalized;
            Vector3 previousMousePosition = Vector3.zero;

            // Debug.Log ($"{projectedAxis} Something");

            List<ICommand> transformCommands = new List<ICommand>();
            for (int i = 0; i < targetRootsOrdered.Count; i++)
            {
                transformCommands.Add(new TransformCommand(this, targetRootsOrdered[i]));
            }

            while (!Input.GetMouseButtonUp(0))
            {

                Ray mouseRay = myCamera.ScreenPointToRay(Input.mousePosition);
                Vector3 mousePosition = Geometry.LinePlaneIntersect(mouseRay.origin, mouseRay.direction, originalPivot, planeNormal);

                if (previousMousePosition != Vector3.zero && mousePosition != Vector3.zero)
                {
                    if (transType == TransformType.Move)
                    {
                        Vector3 movement = Vector3.zero;

                        if (hasTranslatingAxisPlane)
                        {
                            movement = mousePosition - previousMousePosition;
                        }
                        else
                        {
                            float moveAmount = ExtVector3.MagnitudeInDirection(mousePosition - previousMousePosition, projectedAxis) * moveSpeedMultiplier;
                            movement = axis * moveAmount;
                        }

                        for (int i = 0; i < targetRootsOrdered.Count; i++)
                        {
                            Transform target = targetRootsOrdered[i];
                            target.Translate(movement, Space.World);
                        }

                        SetPivotPointOffset(movement);
                    }
                    else if (transType == TransformType.Scale)
                    {
                        Vector3 projected = (nearAxis == Axis.Any) ? transform.right : projectedAxis;
                        float scaleAmount = ExtVector3.MagnitudeInDirection(mousePosition - previousMousePosition, projected) * scaleSpeedMultiplier;

                        //WARNING - There is a bug in unity 5.4 and 5.5 that causes InverseTransformDirection to be affected by scale which will break negative scaling. Not tested, but updating to 5.4.2 should fix it - https://issuetracker.unity3d.com/issues/transformdirection-and-inversetransformdirection-operations-are-affected-by-scale
                        Vector3 localAxis = (GetProperTransformSpace() == TransformSpace.Local && nearAxis != Axis.Any) ? mainTargetRoot.InverseTransformDirection(axis) : axis;

                        Vector3 targetScaleAmount = Vector3.one;
                        if (nearAxis == Axis.Any)
                            targetScaleAmount = (ExtVector3.Abs(mainTargetRoot.localScale.normalized) * scaleAmount);
                        else
                            targetScaleAmount = localAxis * scaleAmount;

                        for (int i = 0; i < targetRootsOrdered.Count; i++)
                        {
                            Transform target = targetRootsOrdered[i];
                            Vector3 targetScale = target.localScale + targetScaleAmount;

                            if (pivot == TransformPivot.Pivot)
                            {
                                target.localScale = targetScale;
                            }
                            else if (pivot == TransformPivot.Center)
                            {
                                if (scaleType == ScaleType.FromPoint)
                                {
                                    target.SetScaleFrom(originalPivot, targetScale);
                                }
                                else if (scaleType == ScaleType.FromPointOffset)
                                {
                                    target.SetScaleFromOffset(originalPivot, targetScale);
                                }
                            }
                        }

                        totalScaleAmount += scaleAmount;
                    }
                    else if (transType == TransformType.Rotate)
                    {
                        float rotateAmount = 0;
                        Vector3 rotationAxis = axis;

                        if (nearAxis == Axis.Any)
                        {
                            Vector3 rotation = transform.TransformDirection(new Vector3(Input.GetAxis("Mouse Y"), -Input.GetAxis("Mouse X"), 0));
                            Quaternion.Euler(rotation).ToAngleAxis(out rotateAmount, out rotationAxis);
                            rotateAmount *= allRotateSpeedMultiplier;
                        }
                        else
                        {
                            if (circularRotationMethod)
                            {
                                float angle = Vector3.SignedAngle(previousMousePosition - originalPivot, mousePosition - originalPivot, axis);
                                rotateAmount = angle * rotateSpeedMultiplier;
                            }
                            else
                            {
                                Vector3 projected = (nearAxis == Axis.Any || ExtVector3.IsParallel(axis, planeNormal)) ? planeNormal : Vector3.Cross(axis, planeNormal);
                                rotateAmount = (ExtVector3.MagnitudeInDirection(mousePosition - previousMousePosition, projected) * (rotateSpeedMultiplier * 100f)) / GetDistanceMultiplier();
                            }
                        }

                        for (int i = 0; i < targetRootsOrdered.Count; i++)
                        {
                            Transform target = targetRootsOrdered[i];
                            if (pivot == TransformPivot.Pivot)
                            {
                                target.Rotate(rotationAxis, rotateAmount, Space.World);
                            }
                            else if (pivot == TransformPivot.Center)
                            {
                                target.RotateAround(originalPivot, rotationAxis, rotateAmount);
                            }
                        }

                        totalRotationAmount *= Quaternion.Euler(rotationAxis * rotateAmount);
                    }
                }

                previousMousePosition = mousePosition;
                CurrentObjectTransform currentObj = ScriptsManager.Instance.GetComponent<CurrentObjectTransform>();
                currentObj.SetCurrentTransform();
                yield return null;
            }

            for (int i = 0; i < transformCommands.Count; i++)
            {
                ((TransformCommand)transformCommands[i]).StoreNewTransformValues();
            }
            CommandGroup commandGroup = new CommandGroup();
            commandGroup.Set(transformCommands);
            UndoRedoManager.Insert(commandGroup);

            totalRotationAmount = Quaternion.identity;
            totalScaleAmount = 0;
            isTransforming = false;
            SetTranslatingAxis(transformType, Axis.None);

            SetPivotPoint();
            // Debug.Log("-----------......." + CurrentObjectTransform.currentObject.name + " tr pos : " + CurrentObjectTransform.currentObject.position);
        }

        private Vector3 GetNearAxisDirection()
        {
            if (nearAxis != Axis.None)
            {
                if (nearAxis == Axis.X)
                    return axisInfo.xDirection;
                if (nearAxis == Axis.Y)
                    return axisInfo.yDirection;
                if (nearAxis == Axis.Z)
                    return axisInfo.zDirection;
                if (nearAxis == Axis.Any)
                    return Vector3.one;
            }
            return Vector3.zero;
        }

        public void MoveObject()
        {
            isMovable = !isMovable;
            //   isMovable = true;
            raycasthitting.enable_label_flag = false;
            ScriptsManager.Instance.featureScript.labels.sprite = ScriptsManager.Instance.featureScript.labelSprites[0];
            //ControlFlowManagerV2.Instance.labelButtObj.GetComponent<Image>().color = raycasthitting.enable_label_flag ? Color.blue : Color.white;

            Move(isMovable);
        }

        private void Move(bool enableMove = false)
        {
            switch (enableMove)
            {
                case true:
                    move.sprite = manipulateSprites[3];
                    if (runtimeHierarchy.CurrentSelection != null && !runtimeHierarchy.CurrentSelection.CompareTag("SceneProperty"))
                    {
                        ClearAndAddTarget(runtimeHierarchy.CurrentSelection);
                        AddTargetHighlightedRenderers(runtimeHierarchy.CurrentSelection);
                    }
                    if (runtimeHierarchy.CurrentSelection != null && runtimeHierarchy.CurrentSelection.GetComponent<NoTransform>())
                    {
                        if (runtimeHierarchy.CurrentSelection.GetComponent<NoTransform>().propertyType == TypesOfProperty.Video)
                        {
                            ClearAndAddTarget(runtimeHierarchy.CurrentSelection);
                            AddTargetHighlightedRenderers(runtimeHierarchy.CurrentSelection);
                        }
                    }
                    else if (runtimeHierarchy.CurrentSelection != null)
                    {
                        ClearAndAddTarget(runtimeHierarchy.CurrentSelection);
                        AddTargetHighlightedRenderers(runtimeHierarchy.CurrentSelection);
                    }
                    break;
                case false:
                    move.sprite = manipulateSprites[0];
                    ClearTargets();
                    break;
            }

            ColorPicker.Instance.Close();
            isRotateble = isScaleable = false;
            rotate.sprite = manipulateSprites[1];
            scale.sprite = manipulateSprites[2];
            transformType = TransformType.Move;
        }

        public void RotateObject()
        {
            isRotateble = !isRotateble;

            raycasthitting.enable_label_flag = false;
            ScriptsManager.Instance.featureScript.labels.sprite = ScriptsManager.Instance.featureScript.labelSprites[0];

            Rotate(isRotateble);
        }

        private void Rotate(bool enableRotate = false)
        {
            switch (isRotateble)
            {
                case true:
                    rotate.sprite = manipulateSprites[4];
                    if (runtimeHierarchy.CurrentSelection != null && !runtimeHierarchy.CurrentSelection.CompareTag("SceneProperty"))
                    {
                        ClearAndAddTarget(runtimeHierarchy.CurrentSelection);
                        AddTargetHighlightedRenderers(runtimeHierarchy.CurrentSelection);
                    }
                    if (runtimeHierarchy.CurrentSelection != null && runtimeHierarchy.CurrentSelection.GetComponent<NoTransform>())
                    {
                        if (runtimeHierarchy.CurrentSelection.GetComponent<NoTransform>().propertyType == TypesOfProperty.Video)
                        {
                            ClearAndAddTarget(runtimeHierarchy.CurrentSelection);
                            AddTargetHighlightedRenderers(runtimeHierarchy.CurrentSelection);
                        }
                    }
                    break;
                case false:
                    rotate.sprite = manipulateSprites[1];
                    ClearTargets();
                    break;
            }

            isMovable = isScaleable = false;
            move.sprite = manipulateSprites[0];
            scale.sprite = manipulateSprites[2];
            transformType = TransformType.Rotate;
            ColorPicker.Instance.Close();
        }

        public void ScaleObject()
        {
            isScaleable = !isScaleable;

            raycasthitting.enable_label_flag = false;
            ScriptsManager.Instance.featureScript.labels.sprite = ScriptsManager.Instance.featureScript.labelSprites[0];

            Scale(isScaleable);
        }

        private void Scale(bool enableScale = false)
        {
            switch (isScaleable)
            {
                case true:
                    scale.sprite = manipulateSprites[5];
                    if (runtimeHierarchy.CurrentSelection != null && !runtimeHierarchy.CurrentSelection.CompareTag("SceneProperty"))
                    {
                        ClearAndAddTarget(runtimeHierarchy.CurrentSelection);
                        AddTargetHighlightedRenderers(runtimeHierarchy.CurrentSelection);
                    }
                    if (runtimeHierarchy.CurrentSelection != null && runtimeHierarchy.CurrentSelection.GetComponent<NoTransform>())
                    {
                        if (runtimeHierarchy.CurrentSelection.GetComponent<NoTransform>().propertyType == TypesOfProperty.Video)
                        {
                            ClearAndAddTarget(runtimeHierarchy.CurrentSelection);
                            AddTargetHighlightedRenderers(runtimeHierarchy.CurrentSelection);
                        }
                    }
                    break;
                case false:
                    scale.sprite = manipulateSprites[2];
                    ClearTargets();
                    break;
            }

            isMovable = isRotateble = false;
            move.sprite = manipulateSprites[0];
            rotate.sprite = manipulateSprites[1];
            transformType = TransformType.Scale;
            ColorPicker.Instance.Close();
        }

        public void ManipulationControl()
        {
            if (isMovable)
            {
                MoveObject();
            }
            else if (isRotateble)
            {
                RotateObject();
            }
            else if (isScaleable)
            {
                ScaleObject();
            }
            else
            {
                ClearAllHighlightedRenderers();
                //  target = null;
            }
        }

        private void GetTarget()
        {
            if (nearAxis == Axis.None && Input.GetMouseButtonDown(0) && Battlehub.RTCommon.Pointer.isSceneWindowEnter && !ColorPicker.isActive)//!ScriptsManager.Instance.eventSystem.IsPointerOverGameObject())
            {
                if (isMovable || isRotateble || isScaleable)
                {
                    bool isAdding = Input.GetKey(AddSelection);
                    bool isRemoving = Input.GetKey(RemoveSelection);

                    if (Physics.Raycast(myCamera.ScreenPointToRay(Input.mousePosition), out RaycastHit hitInfo, Mathf.Infinity, selectionMask))
                    {
                        Transform tempTarget = hitInfo.transform;

                        if (tempTarget != null)
                        {
                            Debug.Log($"Target when on <color=green>{tempTarget}</color>");
                        }

                        if (target == tempTarget)
                        {
                            if (tempTarget.GetComponentInParent<ThreeDModelFunctions>())
                            {
                                ThreeDModelFunctions threeDFunction = tempTarget.GetComponentInParent<ThreeDModelFunctions>();
                                target = tempTarget = threeDFunction.transform;

                                SelectModelSceneObjectItem(threeDFunction);
                            }
                            else if (tempTarget.GetComponentInParent<TextContentCanvasHandler>())
                            {
                                TextContentCanvasHandler textContentCanvas = tempTarget.GetComponentInParent<TextContentCanvasHandler>();
                                target = tempTarget = textContentCanvas.transform;
                            }
                            else
                            {
                                target = tempTarget;
                            }
                        }
                        else
                        {
                            target = tempTarget;
                            if (target.GetComponentInParent<ThreeDModelFunctions>())
                            {
                                ThreeDModelFunctions threeDFunction = target.GetComponentInParent<ThreeDModelFunctions>();
                                SelectModelSceneObjectItem(threeDFunction);
                            }
                        }

                        ScriptsManager.Instance.currentObjectName.text = tempTarget.name;
                        runtimeHierarchy.Select(tempTarget);

                        if (isAdding)
                        {
                            AddTarget(tempTarget);
                            AddTargetHighlightedRenderers(tempTarget);
                        }
                        else if (isRemoving)
                        {
                            RemoveTarget(tempTarget);
                        }
                        else if (!isAdding && !isRemoving)
                        {
                            ClearAndAddTarget(tempTarget);
                            AddTargetHighlightedRenderers(tempTarget);
                        }
                    }
                    else
                    {
                        if (!isAdding && !isRemoving)
                        {
                            ClearTargets();
                            runtimeHierarchy.Deselect();
                            ScriptsManager.Instance.featureScript.Animation.sprite = ScriptsManager.Instance.featureScript.AnimationSprites[0];//: ScriptsManager.Instance.featureScript.browseSprites[0];
                            ScriptsManager.Instance.featureScript.browse.sprite = ScriptsManager.Instance.featureScript.browseSprites[0];//: ScriptsManager.Instance.featureScript.browseSprites[0];
                            //ScriptsManager.Instance.featureScript.lightImg.sprite = ScriptsManager.Instance.featureScript.lightSprites[0];
                            //ScriptsManager.Instance.featureScript.explosion.sprite = ScriptsManager.Instance.featureScript.explosionSprites[0];
                            //ScriptsManager.Instance.featureScript.skyBox.sprite = ScriptsManager.Instance.featureScript.skyBoxSprites[0];


                            if (ScriptsManager.Instance.enableDisableComponent.browserPrefab)
                                ScriptsManager.Instance.enableDisableComponent.browserPrefab.SetActive(false);

                            ScriptsManager.Instance.textPropertyObj.SetActive(false);

                            ScriptsManager.Instance.transformComponent.SetActive(false);
                            target = null;
                            ManipulationControl();
                        }

                        if (ScriptsManager.Instance.projectTypeDropdown.value != 1)
                        {
                            ScriptsManager.Instance.GetComponent<raycasthitting>().DisableTextPanels();
                            if (ScriptsManager.currentSelectedModelObjectLineItem != null)
                                ScriptsManager.currentSelectedModelObjectLineItem.DeSelect();
                        }
                    }
                }
                else if (!isMovable && !isRotateble && !isScaleable && !raycasthitting.enable_label_flag && !ScriptsManager.Instance.isBatchingEnabled)//&& !Battlehub.RTCommon.Pointer.isSceneWindowEnter)
                {
                    if (Physics.Raycast(myCamera.ScreenPointToRay(Input.mousePosition), out RaycastHit hitInfo, Mathf.Infinity, selectionMask))
                    {
                        Transform tempTarget = hitInfo.transform;

                        if (tempTarget != null)
                        {
                            Debug.Log($"Target when off <color=yellow>{tempTarget}</color>");
                        }

                        if (target == tempTarget)
                        {
                            if (tempTarget.GetComponentInParent<ThreeDModelFunctions>())
                            {
                                ThreeDModelFunctions threeDFunction = tempTarget.GetComponentInParent<ThreeDModelFunctions>();
                                target = tempTarget = threeDFunction.transform;

                                SelectModelSceneObjectItem(threeDFunction);
                            }
                            else if (tempTarget.GetComponentInParent<TextContentCanvasHandler>())
                            {
                                TextContentCanvasHandler textContentCanvas = tempTarget.GetComponentInParent<TextContentCanvasHandler>();
                                target = tempTarget = textContentCanvas.transform;
                            }
                            else
                            {
                                target = tempTarget;
                            }
                        }
                        else
                        {
                            target = tempTarget;
                            if (target.GetComponentInParent<ThreeDModelFunctions>())
                            {
                                ThreeDModelFunctions threeDFunction = target.GetComponentInParent<ThreeDModelFunctions>();
                                SelectModelSceneObjectItem(threeDFunction);
                            }
                        }

                        ScriptsManager.Instance.currentObjectName.text = tempTarget.name;
                        runtimeHierarchy.Select(tempTarget);
                        MoveObject();
                    }
                    else
                    {
                        ClearTargets();
                        runtimeHierarchy.Deselect();

                        if (ScriptsManager.Instance.projectTypeDropdown.value != 1)
                        {
                            ScriptsManager.Instance.DeactivateAllComponent();
                            ScriptsManager.Instance.GetComponent<raycasthitting>().DisableTextPanels();
                            if (ScriptsManager.currentSelectedModelObjectLineItem != null)
                                ScriptsManager.currentSelectedModelObjectLineItem.DeSelect();
                        }

                        ScriptsManager.Instance.GetComponent<Label_components>().ClearComp();
                        ScriptsManager.Instance.featureScript.Animation.sprite = ScriptsManager.Instance.featureScript.AnimationSprites[0];//: ScriptsManager.Instance.featureScript.browseSprites[0];
                        ScriptsManager.Instance.featureScript.browse.sprite = ScriptsManager.Instance.featureScript.browseSprites[0];
                        // ScriptsManager.Instance.featureScript.browse.sprite = ScriptsManager.Instance.featureScript.browseSprites[0];//: ScriptsManager.Instance.featureScript.browseSprites[0];
                        ScriptsManager.Instance.featureScript.lightImg.sprite = ScriptsManager.Instance.featureScript.lightSprites[0];
                        ScriptsManager.Instance.featureScript.explosion.sprite = ScriptsManager.Instance.featureScript.explosionSprites[0];
                        ScriptsManager.Instance.featureScript.skyBox.sprite = ScriptsManager.Instance.featureScript.skyBoxSprites[0];

                        if (ScriptsManager.Instance.enableDisableComponent.browserPrefab)
                            ScriptsManager.Instance.enableDisableComponent.browserPrefab.SetActive(false);

                        target = null;

                        ManipulationControl();
                    }
                }
                //else if (raycasthitting.enable_label_flag)
                //{
                //    //ScriptsManager.Instance.DeactivateAllComponent();
                //    ScriptsManager.Instance.GetComponent<Label_components>().ClearComp();
                //}

            }
        }

        private void SelectModelSceneObjectItem(ThreeDModelFunctions modelFunctions)
        {
            //control flow code ad hoc (saravanan)
            if (ScriptsManager.Instance.projectTypeDropdown.value == 1)
            {
                modelFunctions.thisSceneObjLineItem.Select();
            }
            else if (ScriptsManager.Instance.projectTypeDropdown.value == 0)
            {

                modelFunctions.thisModelLineItem.Select();
            }
        }

        public void AddTarget(Transform target, bool addCommand = true)
        {
            if (target != null)
            {
                if (targetRoots.ContainsKey(target))
                    return;
                if (children.Contains(target))
                    return;

                if (addCommand)
                {
                    UndoRedoManager.Insert(new AddTargetCommand(this, target, targetRootsOrdered));
                    //ICommand commandCurrent = new AddTargetCommand(this, target, targetRootsOrdered) as ICommand;
                    //GlobalCommandHolder.AddCommands(commandCurrent);
                }

                AddTargetRoot(target);

                SetPivotPoint();
            }
        }

        public void RemoveTarget(Transform target, bool addCommand = true)
        {
            if (target != null)
            {
                if (!targetRoots.ContainsKey(target))
                    return;

                if (addCommand)
                {
                    // ICommand command = new RemoveTargetCommand(this, target) as ICommand;
                    //GlobalCommandHolder.AddCommands(command);
                    UndoRedoManager.Insert(new RemoveTargetCommand(this, target));


                }
                RemoveTargetHighlightedRenderers(target);
                RemoveTargetRoot(target);

                SetPivotPoint();
            }
        }

        public void ClearTargets(bool addCommand = true)
        {
            if (addCommand)
            {
                UndoRedoManager.Insert(new ClearTargetsCommand(this, targetRootsOrdered));
                //ICommand command = new ClearTargetsCommand(this, targetRootsOrdered) as ICommand;
                //GlobalCommandHolder.AddCommands(command);
            }
            ClearAllHighlightedRenderers();
            targetRoots.Clear();
            targetRootsOrdered.Clear();
            children.Clear();
        }

        public void ClearAndAddTarget(Transform target)
        {
            UndoRedoManager.Insert(new ClearAndAddTargetCommand(this, target, targetRootsOrdered));
            // ICommand command = new ClearAndAddTargetCommand(this, target, targetRootsOrdered) as ICommand;
            // GlobalCommandHolder.AddCommands(command);

            ClearTargets(false);
            AddTarget(target, false);
        }

        public void AddTargetHighlightedRenderers(Transform target)
        {
            if (target != null)
            {
                GetTargetRenderers(target, renderersBuffer);

                for (int i = 0; i < renderersBuffer.Count; i++)
                {
                    Renderer render = renderersBuffer[i];

                    if (!highlightedRenderers.Contains(render))
                    {
                        materialsBuffer.Clear();
                        materialsBuffer.AddRange(render.sharedMaterials);

                        if (!materialsBuffer.Contains(outlineMaterial))
                        {
                            materialsBuffer.Add(outlineMaterial);
                            render.materials = materialsBuffer.ToArray();
                        }

                        highlightedRenderers.Add(render);
                    }
                }

                materialsBuffer.Clear();
            }
        }

        private void GetTargetRenderers(Transform target, List<Renderer> renderers)
        {
            renderers.Clear();
            if (target != null)
            {
                target.GetComponentsInChildren<Renderer>(true, renderers);
            }
        }

        public void ClearAllHighlightedRenderers()
        {
            foreach (KeyValuePair<Transform, TargetInfo> target in targetRoots)
            {
                RemoveTargetHighlightedRenderers(target.Key);
            }

            //In case any are still left, such as if they changed parents or what not when they were highlighted.
            renderersBuffer.Clear();
            renderersBuffer.AddRange(highlightedRenderers);
            RemoveHighlightedRenderers(renderersBuffer);
        }

        public void RemoveTargetHighlightedRenderers(Transform target)
        {
            GetTargetRenderers(target, renderersBuffer);

            RemoveHighlightedRenderers(renderersBuffer);
        }

        public void RemoveHighlightedRenderers(List<Renderer> renderers)
        {
            for (int i = 0; i < renderersBuffer.Count; i++)
            {
                Renderer render = renderersBuffer[i];
                if (render != null)
                {
                    materialsBuffer.Clear();
                    materialsBuffer.AddRange(render.sharedMaterials);

                    if (materialsBuffer.Contains(outlineMaterial))
                    {
                        materialsBuffer.Remove(outlineMaterial);
                        render.materials = materialsBuffer.ToArray();
                    }
                }

                highlightedRenderers.Remove(render);
            }

            renderersBuffer.Clear();
        }

        private void AddTargetRoot(Transform targetRoot)
        {
            targetRoots.Add(targetRoot, new TargetInfo());
            targetRootsOrdered.Add(targetRoot);

            AddAllChildren(targetRoot);
        }

        private void RemoveTargetRoot(Transform targetRoot)
        {
            if (targetRoots.Remove(targetRoot))
            {
                targetRootsOrdered.Remove(targetRoot);

                RemoveAllChildren(targetRoot);
            }
        }

        private void AddAllChildren(Transform target)
        {
            childrenBuffer.Clear();
            target.GetComponentsInChildren<Transform>(true, childrenBuffer);
            childrenBuffer.Remove(target);

            for (int i = 0; i < childrenBuffer.Count; i++)
            {
                Transform child = childrenBuffer[i];
                children.Add(child);
                RemoveTargetRoot(child); //We do this in case we selected child first and then the parent.
            }

            childrenBuffer.Clear();
        }

        private void RemoveAllChildren(Transform target)
        {
            childrenBuffer.Clear();
            target.GetComponentsInChildren<Transform>(true, childrenBuffer);
            childrenBuffer.Remove(target);

            for (int i = 0; i < childrenBuffer.Count; i++)
            {
                children.Remove(childrenBuffer[i]);
            }

            childrenBuffer.Clear();
        }

        public void SetPivotPoint()
        {
            if (mainTargetRoot != null)
            {
                CurrentObjectTransform.currentObject = mainTargetRoot;
                CurrentObjectTransform currentObj = ScriptsManager.Instance.GetComponent<CurrentObjectTransform>();
                currentObj.SetCurrentTransform();

                if (pivot == TransformPivot.Pivot)
                {
                    pivotPoint = mainTargetRoot.position;
                }
                else if (pivot == TransformPivot.Center)
                {
                    totalCenterPivotPoint = Vector3.zero;

                    Dictionary<Transform, TargetInfo>.Enumerator targetsEnumerator = targetRoots.GetEnumerator(); //We avoid foreach to avoid garbage.
                    while (targetsEnumerator.MoveNext())
                    {
                        Transform target = targetsEnumerator.Current.Key;
                        TargetInfo info = targetsEnumerator.Current.Value;
                        info.centerPivotPoint = target.GetCenter(centerType);

                        totalCenterPivotPoint += info.centerPivotPoint;
                    }

                    totalCenterPivotPoint /= targetRoots.Count;

                    if (centerType == CenterType.Solo)
                    {
                        pivotPoint = targetRoots[mainTargetRoot].centerPivotPoint;
                    }
                    else if (centerType == CenterType.All)
                    {
                        pivotPoint = totalCenterPivotPoint;
                    }
                }
            }
        }

        private void SetPivotPointOffset(Vector3 offset)
        {
            pivotPoint += offset;
            totalCenterPivotPoint += offset;
        }

        private IEnumerator ForceUpdatePivotPointAtEndOfFrame()
        {
            while (enabled)
            {
                ForceUpdatePivotPointOnChange();
                yield return waitForEndOFFrame;
            }
        }

        private void ForceUpdatePivotPointOnChange()
        {
            if (forceUpdatePivotPointOnChange)
            {
                if (mainTargetRoot != null && !isTransforming)
                {
                    bool hasSet = false;
                    Dictionary<Transform, TargetInfo>.Enumerator targets = targetRoots.GetEnumerator();
                    while (targets.MoveNext())
                    {
                        if (!hasSet)
                        {
                            if (targets.Current.Value.previousPosition != Vector3.zero && targets.Current.Key.position != targets.Current.Value.previousPosition)
                            {
                                SetPivotPoint();
                                hasSet = true;
                            }
                        }

                        targets.Current.Value.previousPosition = targets.Current.Key.position;
                    }
                }
            }
        }

        public void SetTranslatingAxis(TransformType type, Axis axis, Axis planeAxis = Axis.None)
        {
            translatingType = type;
            nearAxis = axis;
            this.planeAxis = planeAxis;
        }

        public AxisInfo GetAxisInfo()
        {
            AxisInfo currentAxisInfo = axisInfo;

            if (isTransforming && GetProperTransformSpace() == TransformSpace.Global && translatingType == TransformType.Rotate)
            {
                currentAxisInfo.xDirection = totalRotationAmount * Vector3.right;
                currentAxisInfo.yDirection = totalRotationAmount * Vector3.up;
                currentAxisInfo.zDirection = totalRotationAmount * Vector3.forward;
            }

            return currentAxisInfo;
        }

        private void SetNearAxis()
        {
            if (isTransforming)
                return;

            SetTranslatingAxis(transformType, Axis.None);

            if (mainTargetRoot == null)
                return;

            float distanceMultiplier = GetDistanceMultiplier();
            float handleMinSelectedDistanceCheck = (minSelectedDistanceCheck + handleWidth) * distanceMultiplier;

            if (nearAxis == Axis.None && (TransformTypeContains(TransformType.Move) || TransformTypeContains(TransformType.Scale)))
            {
                //Important to check scale lines before move lines since in TransformType.All the move planes would block the scales center scale all gizmo.
                if (nearAxis == Axis.None && TransformTypeContains(TransformType.Scale))
                {
                    float tipMinSelectedDistanceCheck = (minSelectedDistanceCheck + boxSize) * distanceMultiplier;
                    HandleNearestPlanes(TransformType.Scale, handleSquares, tipMinSelectedDistanceCheck);
                }

                if (nearAxis == Axis.None && TransformTypeContains(TransformType.Move))
                {
                    //Important to check the planes first before the handle tip since it makes selecting the planes easier.
                    float planeMinSelectedDistanceCheck = (minSelectedDistanceCheck + planeSize) * distanceMultiplier;
                    HandleNearestPlanes(TransformType.Move, handlePlanes, planeMinSelectedDistanceCheck);

                    if (nearAxis != Axis.None)
                    {
                        planeAxis = nearAxis;
                    }
                    else
                    {
                        float tipMinSelectedDistanceCheck = (minSelectedDistanceCheck + triangleSize) * distanceMultiplier;
                        HandleNearestLines(TransformType.Move, handleTriangles, tipMinSelectedDistanceCheck);
                    }
                }

                if (nearAxis == Axis.None)
                {
                    //Since Move and Scale share the same handle line, we give Move the priority.
                    TransformType transType = transformType == TransformType.All ? TransformType.Move : transformType;
                    HandleNearestLines(transType, handleLines, handleMinSelectedDistanceCheck);
                }
            }

            if (nearAxis == Axis.None && TransformTypeContains(TransformType.Rotate))
            {
                HandleNearestLines(TransformType.Rotate, circlesLines, handleMinSelectedDistanceCheck);
            }
        }

        private void HandleNearestLines(TransformType type, AxisVectors axisVectors, float minSelectedDistanceCheck)
        {
            float xClosestDistance = ClosestDistanceFromMouseToLines(axisVectors.x);
            float yClosestDistance = ClosestDistanceFromMouseToLines(axisVectors.y);
            float zClosestDistance = ClosestDistanceFromMouseToLines(axisVectors.z);
            float allClosestDistance = ClosestDistanceFromMouseToLines(axisVectors.all);

            HandleNearest(type, xClosestDistance, yClosestDistance, zClosestDistance, allClosestDistance, minSelectedDistanceCheck);
        }

        private void HandleNearestPlanes(TransformType type, AxisVectors axisVectors, float minSelectedDistanceCheck)
        {
            float xClosestDistance = ClosestDistanceFromMouseToPlanes(axisVectors.x);
            float yClosestDistance = ClosestDistanceFromMouseToPlanes(axisVectors.y);
            float zClosestDistance = ClosestDistanceFromMouseToPlanes(axisVectors.z);
            float allClosestDistance = ClosestDistanceFromMouseToPlanes(axisVectors.all);

            HandleNearest(type, xClosestDistance, yClosestDistance, zClosestDistance, allClosestDistance, minSelectedDistanceCheck);
        }

        private void HandleNearest(TransformType type, float xClosestDistance, float yClosestDistance, float zClosestDistance, float allClosestDistance, float minSelectedDistanceCheck)
        {
            if (type == TransformType.Scale && allClosestDistance <= minSelectedDistanceCheck)
                SetTranslatingAxis(type, Axis.Any);
            else if (xClosestDistance <= minSelectedDistanceCheck && xClosestDistance <= yClosestDistance && xClosestDistance <= zClosestDistance)
                SetTranslatingAxis(type, Axis.X);
            else if (yClosestDistance <= minSelectedDistanceCheck && yClosestDistance <= xClosestDistance && yClosestDistance <= zClosestDistance)
                SetTranslatingAxis(type, Axis.Y);
            else if (zClosestDistance <= minSelectedDistanceCheck && zClosestDistance <= xClosestDistance && zClosestDistance <= yClosestDistance)
                SetTranslatingAxis(type, Axis.Z);
            else if (type == TransformType.Rotate && mainTargetRoot != null)
            {
                Ray mouseRay = myCamera.ScreenPointToRay(Input.mousePosition);
                Vector3 mousePlaneHit = Geometry.LinePlaneIntersect(mouseRay.origin, mouseRay.direction, pivotPoint, (transform.position - pivotPoint).normalized);
                if ((pivotPoint - mousePlaneHit).sqrMagnitude <= (GetHandleLength(TransformType.Rotate)).Squared())
                    SetTranslatingAxis(type, Axis.Any);
            }
        }

        private float ClosestDistanceFromMouseToLines(List<Vector3> lines)
        {
            Ray mouseRay = myCamera.ScreenPointToRay(Input.mousePosition);

            float closestDistance = float.MaxValue;
            for (int i = 0; i + 1 < lines.Count; i++)
            {
                IntersectPoints points = Geometry.ClosestPointsOnSegmentToLine(lines[i], lines[i + 1], mouseRay.origin, mouseRay.direction);
                float distance = Vector3.Distance(points.first, points.second);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                }
            }
            return closestDistance;
        }

        private float ClosestDistanceFromMouseToPlanes(List<Vector3> planePoints)
        {
            float closestDistance = float.MaxValue;

            if (planePoints.Count >= 4)
            {
                Ray mouseRay = myCamera.ScreenPointToRay(Input.mousePosition);

                for (int i = 0; i < planePoints.Count; i += 4)
                {
                    Plane plane = new Plane(planePoints[i], planePoints[i + 1], planePoints[i + 2]);

                    if (plane.Raycast(mouseRay, out float distanceToPlane))
                    {
                        Vector3 pointOnPlane = mouseRay.origin + (mouseRay.direction * distanceToPlane);
                        Vector3 planeCenter = (planePoints[0] + planePoints[1] + planePoints[2] + planePoints[3]) / 4f;

                        float distance = Vector3.Distance(planeCenter, pointOnPlane);
                        if (distance < closestDistance)
                        {
                            closestDistance = distance;
                        }
                    }
                }
            }

            return closestDistance;
        }

        //float DistanceFromMouseToPlane(List<Vector3> planeLines)
        //{
        //	if(planeLines.Count >= 4)
        //	{
        //		Ray mouseRay = myCamera.ScreenPointToRay(Input.mousePosition);
        //		Plane plane = new Plane(planeLines[0], planeLines[1], planeLines[2]);

        //		float distanceToPlane;
        //		if(plane.Raycast(mouseRay, out distanceToPlane))
        //		{
        //			Vector3 pointOnPlane = mouseRay.origin + (mouseRay.direction * distanceToPlane);
        //			Vector3 planeCenter = (planeLines[0] + planeLines[1] + planeLines[2] + planeLines[3]) / 4f;

        //			return Vector3.Distance(planeCenter, pointOnPlane);
        //		}
        //	}

        //	return float.MaxValue;
        //}

        private void SetAxisInfo()
        {
            if (mainTargetRoot != null)
            {
                axisInfo.Set(mainTargetRoot, pivotPoint, GetProperTransformSpace());
            }
        }

        //This helps keep the size consistent no matter how far we are from it.
        public float GetDistanceMultiplier()
        {
            if (mainTargetRoot == null)
                return 0f;

            if (myCamera.orthographic)
                return Mathf.Max(.01f, myCamera.orthographicSize * 2f);
            return Mathf.Max(.01f, Mathf.Abs(ExtVector3.MagnitudeInDirection(pivotPoint - transform.position, myCamera.transform.forward)));
        }

        private void SetLines()
        {
            SetHandleLines();
            SetHandlePlanes();
            SetHandleTriangles();
            SetHandleSquares();
            SetCircles(GetAxisInfo(), circlesLines);
        }

        private void SetHandleLines()
        {
            handleLines.Clear();

            if (TranslatingTypeContains(TransformType.Move) || TranslatingTypeContains(TransformType.Scale))
            {
                float lineWidth = handleWidth * GetDistanceMultiplier();

                float xLineLength = 0;
                float yLineLength = 0;
                float zLineLength = 0;
                if (TranslatingTypeContains(TransformType.Move))
                {
                    xLineLength = yLineLength = zLineLength = GetHandleLength(TransformType.Move);
                }
                else if (TranslatingTypeContains(TransformType.Scale))
                {
                    xLineLength = GetHandleLength(TransformType.Scale, Axis.X);
                    yLineLength = GetHandleLength(TransformType.Scale, Axis.Y);
                    zLineLength = GetHandleLength(TransformType.Scale, Axis.Z);
                }

                AddQuads(pivotPoint, axisInfo.xDirection, axisInfo.yDirection, axisInfo.zDirection, xLineLength, lineWidth, handleLines.x);
                AddQuads(pivotPoint, axisInfo.yDirection, axisInfo.xDirection, axisInfo.zDirection, yLineLength, lineWidth, handleLines.y);
                AddQuads(pivotPoint, axisInfo.zDirection, axisInfo.xDirection, axisInfo.yDirection, zLineLength, lineWidth, handleLines.z);
            }
        }

        private int AxisDirectionMultiplier(Vector3 direction, Vector3 otherDirection)
        {
            return ExtVector3.IsInDirection(direction, otherDirection) ? 1 : -1;
        }

        private void SetHandlePlanes()
        {
            handlePlanes.Clear();

            if (TranslatingTypeContains(TransformType.Move))
            {
                Vector3 pivotToCamera = myCamera.transform.position - pivotPoint;
                float cameraXSign = Mathf.Sign(Vector3.Dot(axisInfo.xDirection, pivotToCamera));
                float cameraYSign = Mathf.Sign(Vector3.Dot(axisInfo.yDirection, pivotToCamera));
                float cameraZSign = Mathf.Sign(Vector3.Dot(axisInfo.zDirection, pivotToCamera));

                float planeSize = this.planeSize;
                if (transformType == TransformType.All)
                {
                    planeSize *= allMoveHandleLengthMultiplier;
                }
                planeSize *= GetDistanceMultiplier();

                Vector3 xDirection = (axisInfo.xDirection * planeSize) * cameraXSign;
                Vector3 yDirection = (axisInfo.yDirection * planeSize) * cameraYSign;
                Vector3 zDirection = (axisInfo.zDirection * planeSize) * cameraZSign;

                Vector3 xPlaneCenter = pivotPoint + (yDirection + zDirection);
                Vector3 yPlaneCenter = pivotPoint + (xDirection + zDirection);
                Vector3 zPlaneCenter = pivotPoint + (xDirection + yDirection);

                AddQuad(xPlaneCenter, axisInfo.yDirection, axisInfo.zDirection, planeSize, handlePlanes.x);
                AddQuad(yPlaneCenter, axisInfo.xDirection, axisInfo.zDirection, planeSize, handlePlanes.y);
                AddQuad(zPlaneCenter, axisInfo.xDirection, axisInfo.yDirection, planeSize, handlePlanes.z);
            }
        }

        private void SetHandleTriangles()
        {
            handleTriangles.Clear();

            if (TranslatingTypeContains(TransformType.Move))
            {
                float triangleLength = triangleSize * GetDistanceMultiplier();
                AddTriangles(axisInfo.GetXAxisEnd(GetHandleLength(TransformType.Move)), axisInfo.xDirection, axisInfo.yDirection, axisInfo.zDirection, triangleLength, handleTriangles.x);
                AddTriangles(axisInfo.GetYAxisEnd(GetHandleLength(TransformType.Move)), axisInfo.yDirection, axisInfo.xDirection, axisInfo.zDirection, triangleLength, handleTriangles.y);
                AddTriangles(axisInfo.GetZAxisEnd(GetHandleLength(TransformType.Move)), axisInfo.zDirection, axisInfo.yDirection, axisInfo.xDirection, triangleLength, handleTriangles.z);
            }
        }

        private void AddTriangles(Vector3 axisEnd, Vector3 axisDirection, Vector3 axisOtherDirection1, Vector3 axisOtherDirection2, float size, List<Vector3> resultsBuffer)
        {
            Vector3 endPoint = axisEnd + (axisDirection * (size * 2f));
            Square baseSquare = GetBaseSquare(axisEnd, axisOtherDirection1, axisOtherDirection2, size / 2f);

            resultsBuffer.Add(baseSquare.bottomLeft);
            resultsBuffer.Add(baseSquare.topLeft);
            resultsBuffer.Add(baseSquare.topRight);
            resultsBuffer.Add(baseSquare.topLeft);
            resultsBuffer.Add(baseSquare.bottomRight);
            resultsBuffer.Add(baseSquare.topRight);

            for (int i = 0; i < 4; i++)
            {
                resultsBuffer.Add(baseSquare[i]);
                resultsBuffer.Add(baseSquare[i + 1]);
                resultsBuffer.Add(endPoint);
            }
        }

        private void SetHandleSquares()
        {
            handleSquares.Clear();

            if (TranslatingTypeContains(TransformType.Scale))
            {
                float boxSize = this.boxSize * GetDistanceMultiplier();
                AddSquares(axisInfo.GetXAxisEnd(GetHandleLength(TransformType.Scale, Axis.X)), axisInfo.xDirection, axisInfo.yDirection, axisInfo.zDirection, boxSize, handleSquares.x);
                AddSquares(axisInfo.GetYAxisEnd(GetHandleLength(TransformType.Scale, Axis.Y)), axisInfo.yDirection, axisInfo.xDirection, axisInfo.zDirection, boxSize, handleSquares.y);
                AddSquares(axisInfo.GetZAxisEnd(GetHandleLength(TransformType.Scale, Axis.Z)), axisInfo.zDirection, axisInfo.xDirection, axisInfo.yDirection, boxSize, handleSquares.z);
                AddSquares(pivotPoint - (axisInfo.xDirection * (boxSize * .5f)), axisInfo.xDirection, axisInfo.yDirection, axisInfo.zDirection, boxSize, handleSquares.all);
            }
        }

        private void AddSquares(Vector3 axisStart, Vector3 axisDirection, Vector3 axisOtherDirection1, Vector3 axisOtherDirection2, float size, List<Vector3> resultsBuffer)
        {
            AddQuads(axisStart, axisDirection, axisOtherDirection1, axisOtherDirection2, size, size * .5f, resultsBuffer);
        }

        private void AddQuads(Vector3 axisStart, Vector3 axisDirection, Vector3 axisOtherDirection1, Vector3 axisOtherDirection2, float length, float width, List<Vector3> resultsBuffer)
        {
            Vector3 axisEnd = axisStart + (axisDirection * length);
            AddQuads(axisStart, axisEnd, axisOtherDirection1, axisOtherDirection2, width, resultsBuffer);
        }

        private void AddQuads(Vector3 axisStart, Vector3 axisEnd, Vector3 axisOtherDirection1, Vector3 axisOtherDirection2, float width, List<Vector3> resultsBuffer)
        {
            Square baseRectangle = GetBaseSquare(axisStart, axisOtherDirection1, axisOtherDirection2, width);
            Square baseRectangleEnd = GetBaseSquare(axisEnd, axisOtherDirection1, axisOtherDirection2, width);

            resultsBuffer.Add(baseRectangle.bottomLeft);
            resultsBuffer.Add(baseRectangle.topLeft);
            resultsBuffer.Add(baseRectangle.topRight);
            resultsBuffer.Add(baseRectangle.bottomRight);

            resultsBuffer.Add(baseRectangleEnd.bottomLeft);
            resultsBuffer.Add(baseRectangleEnd.topLeft);
            resultsBuffer.Add(baseRectangleEnd.topRight);
            resultsBuffer.Add(baseRectangleEnd.bottomRight);

            for (int i = 0; i < 4; i++)
            {
                resultsBuffer.Add(baseRectangle[i]);
                resultsBuffer.Add(baseRectangleEnd[i]);
                resultsBuffer.Add(baseRectangleEnd[i + 1]);
                resultsBuffer.Add(baseRectangle[i + 1]);
            }
        }

        private void AddQuad(Vector3 axisStart, Vector3 axisOtherDirection1, Vector3 axisOtherDirection2, float width, List<Vector3> resultsBuffer)
        {
            Square baseRectangle = GetBaseSquare(axisStart, axisOtherDirection1, axisOtherDirection2, width);

            resultsBuffer.Add(baseRectangle.bottomLeft);
            resultsBuffer.Add(baseRectangle.topLeft);
            resultsBuffer.Add(baseRectangle.topRight);
            resultsBuffer.Add(baseRectangle.bottomRight);
        }

        private Square GetBaseSquare(Vector3 axisEnd, Vector3 axisOtherDirection1, Vector3 axisOtherDirection2, float size)
        {
            Square square;
            Vector3 offsetUp = ((axisOtherDirection1 * size) + (axisOtherDirection2 * size));
            Vector3 offsetDown = ((axisOtherDirection1 * size) - (axisOtherDirection2 * size));
            //These might not really be the proper directions, as in the bottomLeft might not really be at the bottom left...
            square.bottomLeft = axisEnd + offsetDown;
            square.topLeft = axisEnd + offsetUp;
            square.bottomRight = axisEnd - offsetUp;
            square.topRight = axisEnd - offsetDown;
            return square;
        }

        private void SetCircles(AxisInfo axisInfo, AxisVectors axisVectors)
        {
            axisVectors.Clear();

            if (TranslatingTypeContains(TransformType.Rotate))
            {
                float circleLength = GetHandleLength(TransformType.Rotate);
                AddCircle(pivotPoint, axisInfo.xDirection, circleLength, axisVectors.x);
                AddCircle(pivotPoint, axisInfo.yDirection, circleLength, axisVectors.y);
                AddCircle(pivotPoint, axisInfo.zDirection, circleLength, axisVectors.z);
                AddCircle(pivotPoint, (pivotPoint - transform.position).normalized, circleLength, axisVectors.all, false);
            }
        }

        private void AddCircle(Vector3 origin, Vector3 axisDirection, float size, List<Vector3> resultsBuffer, bool depthTest = true)
        {
            Vector3 up = axisDirection.normalized * size;
            Vector3 forward = Vector3.Slerp(up, -up, .5f);
            Vector3 right = Vector3.Cross(up, forward).normalized * size;

            Matrix4x4 matrix = new Matrix4x4();

            matrix[0] = right.x;
            matrix[1] = right.y;
            matrix[2] = right.z;

            matrix[4] = up.x;
            matrix[5] = up.y;
            matrix[6] = up.z;

            matrix[8] = forward.x;
            matrix[9] = forward.y;
            matrix[10] = forward.z;

            Vector3 lastPoint = origin + matrix.MultiplyPoint3x4(new Vector3(Mathf.Cos(0), 0, Mathf.Sin(0)));
            Vector3 nextPoint = Vector3.zero;
            float multiplier = 360f / circleDetail;

            Plane plane = new Plane((transform.position - pivotPoint).normalized, pivotPoint);

            float circleHandleWidth = handleWidth * GetDistanceMultiplier();

            for (int i = 0; i < circleDetail + 1; i++)
            {
                nextPoint.x = Mathf.Cos((i * multiplier) * Mathf.Deg2Rad);
                nextPoint.z = Mathf.Sin((i * multiplier) * Mathf.Deg2Rad);
                nextPoint.y = 0;

                nextPoint = origin + matrix.MultiplyPoint3x4(nextPoint);

                if (!depthTest || plane.GetSide(lastPoint))
                {
                    Vector3 centerPoint = (lastPoint + nextPoint) * .5f;
                    Vector3 upDirection = (centerPoint - origin).normalized;
                    AddQuads(lastPoint, nextPoint, upDirection, axisDirection, circleHandleWidth, resultsBuffer);
                }

                lastPoint = nextPoint;
            }
        }

        private void DrawLines(List<Vector3> lines, Color color)
        {
            GL.Begin(GL.LINES);
            GL.Color(color);

            for (int i = 0; i < lines.Count; i += 2)
            {
                GL.Vertex(lines[i]);
                GL.Vertex(lines[i + 1]);
            }

            GL.End();
        }

        private void DrawTriangles(List<Vector3> lines, Color color)
        {
            GL.Begin(GL.TRIANGLES);
            GL.Color(color);

            for (int i = 0; i < lines.Count; i += 3)
            {
                GL.Vertex(lines[i]);
                GL.Vertex(lines[i + 1]);
                GL.Vertex(lines[i + 2]);
            }

            GL.End();
        }

        private void DrawQuads(List<Vector3> lines, Color color)
        {
            GL.Begin(GL.QUADS);
            GL.Color(color);

            for (int i = 0; i < lines.Count; i += 4)
            {
                GL.Vertex(lines[i]);
                GL.Vertex(lines[i + 1]);
                GL.Vertex(lines[i + 2]);
                GL.Vertex(lines[i + 3]);
            }

            GL.End();
        }

        private void DrawFilledCircle(List<Vector3> lines, Color color)
        {
            Vector3 center = Vector3.zero;
            for (int i = 0; i < lines.Count; i++)
            {
                center += lines[i];
            }
            center /= lines.Count;

            GL.Begin(GL.TRIANGLES);
            GL.Color(color);

            for (int i = 0; i + 1 < lines.Count; i++)
            {
                GL.Vertex(lines[i]);
                GL.Vertex(lines[i + 1]);
                GL.Vertex(center);
            }

            GL.End();
        }

        private void SetMaterial()
        {
            if (lineMaterial == null)
            {
                lineMaterial = new Material(Shader.Find("Custom/Lines"));
                outlineMaterial = new Material(Shader.Find("Custom/Outline"));
            }
        }
    }
}
