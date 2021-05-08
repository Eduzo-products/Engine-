using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    #region CAMERA_PROPERTIES
    public Transform cameraTransform;
    public float cameraTurnSpeed = 3.5f,
        cameraFOV, cameraMinFOV = 30.0f,
        cameraMaxFOV = 90.0f, cameraZoomSensitivity = 1.0f;
    public Texture2D cursorEye;
    public Button resetView;

    private Camera mainCamera;
    #endregion

    private float directionX, directionY, directionZ = 0.0f;

    private void Awake()
    {
        cameraTransform = transform;
        mainCamera = cameraTransform.GetComponent<Camera>();
    }

    

    private void CameraRotate()
    {
        if (Input.GetKey(KeyCode.LeftAlt) && Input.GetMouseButton(0))
        {
            Cursor.SetCursor(cursorEye, Vector2.zero, CursorMode.Auto);
            cameraTransform.Rotate(new Vector3(Input.GetAxis("Mouse Y") * cameraTurnSpeed, -Input.GetAxis("Mouse X") * cameraTurnSpeed, 0.0f));

            directionX = cameraTransform.rotation.eulerAngles.x;
            directionY = cameraTransform.rotation.eulerAngles.y;
            cameraTransform.rotation = Quaternion.Euler(directionX, directionY, directionZ);
        }
        else
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }

        if (!cameraTransform.rotation.eulerAngles.Equals(Vector3.zero))
        {
            resetView.transform.parent.gameObject.SetActive(true);
        }
        else
        {
            resetView.transform.parent.gameObject.SetActive(false);
        }
    }

}