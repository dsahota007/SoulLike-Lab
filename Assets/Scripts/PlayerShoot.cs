using UnityEngine;
using Cinemachine;

public class ThirdPersonShooterController : MonoBehaviour
{
    public CinemachineFreeLook thirdPersonCamera;
    public CinemachineVirtualCamera adsCamera;
    public Transform cameraTransform;

    public float aimRotationSpeed = 15f;
    public float mouseSensitivity = 150f;

    private bool isAiming = false;
    private float xRotation = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        thirdPersonCamera.m_XAxis.m_InputAxisName = "Mouse X";
        thirdPersonCamera.m_YAxis.m_InputAxisName = "Mouse Y";
    }

    void Update()
    {
        HandleAiming();
        HandleCameraRotation();
    }

    void HandleAiming()
    {
        if (Input.GetMouseButtonDown(1)) // Right mouse button toggles ADS
        {
            isAiming = !isAiming;

            if (isAiming)
            {
                // Snap the ADS camera to the current FreeLook camera position and rotation
                adsCamera.transform.position = thirdPersonCamera.transform.position;
                adsCamera.transform.rotation = thirdPersonCamera.transform.rotation;

                // Switch to ADS camera
                thirdPersonCamera.Priority = 0;
                adsCamera.Priority = 20;

                // Remove input from FreeLook
                thirdPersonCamera.m_XAxis.m_InputAxisName = "";
                thirdPersonCamera.m_YAxis.m_InputAxisName = "";

                // Immediately align player to camera direction
                RotatePlayerToCamera();
            }
            else
            {
                // Go back to third-person camera
                thirdPersonCamera.Priority = 10;
                adsCamera.Priority = 0;

                // Restore FreeLook input
                thirdPersonCamera.m_XAxis.m_InputAxisName = "Mouse X";
                thirdPersonCamera.m_YAxis.m_InputAxisName = "Mouse Y";
            }
        }

        // While aiming, align player to the camera's forward direction
        if (isAiming)
        {
            RotatePlayerToCamera();
        }
    }

    void RotatePlayerToCamera()
    {
        Vector3 aimDirection = cameraTransform.forward;
        aimDirection.y = 0f;

        if (aimDirection.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(aimDirection);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                Time.deltaTime * aimRotationSpeed
            );
        }
    }

    void HandleCameraRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        if (!isAiming)
        {
            // Rotate camera + player in FreeLook mode
            transform.Rotate(Vector3.up * mouseX);

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -70f, 80f);
            cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }
        else
        {
            // While aiming, control the ADS camera directly
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -70f, 80f);

            adsCamera.transform.rotation = Quaternion.Euler(xRotation, adsCamera.transform.eulerAngles.y + mouseX, 0f);
        }
    }
}
