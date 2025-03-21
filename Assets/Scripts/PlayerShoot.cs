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
        // Lock the mouse cursor for clean aiming
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Make sure our FreeLook camera is using "Mouse X" & "Mouse Y"
        thirdPersonCamera.m_XAxis.m_InputAxisName = "Mouse X";
        thirdPersonCamera.m_YAxis.m_InputAxisName = "Mouse Y";
    }

    void Update()
    {
        HandleAiming();
        HandleCameraRotation();
        // ^ If you prefer Cinemachine to handle all rotation automatically,
        //   you can comment this out. 
    }

    void HandleAiming()
    {
        if (Input.GetMouseButtonDown(1)) // Right mouse button toggles ADS
        {
            isAiming = !isAiming;

            if (isAiming)
            {
                // Switch to ADS camera
                thirdPersonCamera.Priority = 0;
                adsCamera.Priority = 20;

                // ** Do NOT disable FreeLook input axes here ** 
                // thirdPersonCamera.m_XAxis.m_InputAxisName = "";
                // thirdPersonCamera.m_YAxis.m_InputAxisName = "";
            }
            else
            {
                // Go back to third-person camera
                thirdPersonCamera.Priority = 10;
                adsCamera.Priority = 0;

                // Again, do NOT worry about restoring the input axis; 
                // we never removed them this time.
            }
        }

        // If you still want the player to auto-rotate to the camera direction while aiming...
        if (isAiming)
        {
            RotatePlayerToCamera();
        }
        // ...otherwise, comment the above out if you’d rather keep player orientation independent.
    }

    void RotatePlayerToCamera()
    {
        // Rotate the player so they face the camera's forward direction
        Vector3 aimDirection = cameraTransform.forward;
        aimDirection.y = 0f; // Flatten to avoid tilting

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
        // Simple manual camera control (if you’re mixing Cinemachine with a custom approach)
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Rotate camera horizontally
        cameraTransform.Rotate(Vector3.up * mouseX);

        // Clamp vertical rotation
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -70f, 80f);
        cameraTransform.localRotation = Quaternion.Euler(xRotation, cameraTransform.localEulerAngles.y, 0f);
    }
}
