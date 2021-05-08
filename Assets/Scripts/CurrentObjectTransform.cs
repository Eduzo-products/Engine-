using TMPro;
using UnityEngine;
public class CurrentObjectTransform : MonoBehaviour
{
    public static Transform currentObject = null;

    [Header("<------ Position ------>")]
    public TMP_InputField positionX;
    public TMP_InputField positionY, positionZ;

    [Header("<------ Rotation ------>")]
    public TMP_InputField rotationX;
    public TMP_InputField rotationY, rotationZ;

    [Header("<------ Scale ------>")]
    public TMP_InputField scaleX;
    public TMP_InputField scaleY, scaleZ;

    [Header("<------ Scale ------>")]
    public TMP_InputField colorR;
    public TMP_InputField colorG, colorB;

    [HideInInspector] public float posX, posY, posZ, rotX, rotY, rotZ, scaX, scaY, scaZ;
    public float mainRed, mainGreen, mainBlue;

    public void CloseApp()
    {
        Application.Quit();
    }

    private void Update()
    {
        //    if (currentObject != null)
        //    {
        //        if (Input.GetMouseButtonDown(0))
        //        {
        //            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //            if (Physics.Raycast(ray, out RaycastHit hit, 100.0f))
        //            {
        //               // if (hit.collider != null)
        //               // {
        //                    currentObject = hit.transform; Debug.Log("CurrentObject: " + currentObject.name);
        //               // }
        //            }
        //           // CurrentPositionValues();
        //           // CurrentRotationValues();
        //           // CurrentScaleValues();
        //        }
        //    }
    }

    public void SetCurrentTransform()
    {
        CurrentPositionValues();
        CurrentRotationValues();
        CurrentScaleValues();
    }

    public void SaveDetails()
    {
        //SaveData saveData = new SaveData
        //{
        //    tranX = posX,
        //    tranY = posY,
        //    tranZ = posZ,

        //    rotX = rotX,
        //    rotY = rotY,
        //    rotZ = rotZ,

        //    scaleX = scaX,
        //    scaleY = scaY,
        //    scaleZ = scaZ,

        //};

        //string fileToBeSaved = JsonUtility.ToJson(saveData);
        //PlayerPrefs.SetString("DataStored", ScriptsManager.Instance.mainPath);
    }

    public void CurrentPositionValues()
    {
        posX = currentObject.position.x;
        posY = currentObject.position.y;
        posZ = currentObject.position.z;
        positionX.text = posX.ToString();
        positionY.text = posY.ToString();
        positionZ.text = posZ.ToString();
    }

    public void CurrentRotationValues()
    {
        rotX = currentObject.rotation.eulerAngles.x;
        rotY = currentObject.rotation.eulerAngles.y;
        rotZ = currentObject.rotation.eulerAngles.z;
        rotationX.text = rotX.ToString();
        rotationY.text = rotY.ToString();
        rotationZ.text = rotZ.ToString();
    }

    public void CurrentScaleValues()
    {
        scaX = currentObject.localScale.x;
        scaY = currentObject.localScale.y;
        scaZ = currentObject.localScale.z;
        scaleX.text = scaX.ToString();
        scaleY.text = scaY.ToString();
        scaleZ.text = scaZ.ToString();
    }

    public void ChangeColor()
    {
        if (colorR.text.Length != 0)
        {
            float red = mainRed = float.Parse(colorR.text);
        }
        if (colorG.text.Length != 0)
        {
            float green = mainGreen = float.Parse(colorG.text);
        }
        if (colorB.text.Length != 0)
        {
            float blue = mainBlue = float.Parse(colorB.text);
        }

        MeshRenderer[] renderers = currentObject.GetComponentsInChildren<MeshRenderer>();

        for (int p = 0; p < renderers.Length; p++)
        {
            renderers[p].material.color = new Color(mainRed, mainGreen, mainBlue);
        }
    }
}
