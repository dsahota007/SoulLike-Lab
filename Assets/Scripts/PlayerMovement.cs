using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    public float maximumSpeed = 5f;       // Normal movement speed
    public float sprintSpeed = 8f;        // Sprint speed when holding Shift
    public float rotationSpeed = 700f;    // Rotation speed when turning
    public float jumpForce = 5f;          // Force applied when jumping
    public Transform cameraTransform;     // Reference to the camera for direction

    public float rollSpeed = 8f;         // Roll speed
    public float rollDuration = 0.5f;     // How long the roll lasts

    private Animator animator;
    private Rigidbody rb;
    private bool isGrounded;
    private bool isRolling;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // Prevent physics-based rotation
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous; // Prevent falling through ground
        animator.applyRootMotion = false; // ✅ Turn OFF root motion since animation is stationary
    }

    void Update()
    {
        if (!isRolling) // ✅ Allow movement only when not rolling
        {
            HandleMovement();
            HandleJump();
        }

        HandleRoll();
    }

    void HandleMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        bool isSprinting = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        animator.SetBool("IsSprinting", isSprinting);

        float currentSpeed = isSprinting ? sprintSpeed : maximumSpeed;

        // Calculate movement direction based on camera orientation
        Vector3 movementDirection = new Vector3(horizontalInput, 0, verticalInput);
        movementDirection = Quaternion.Euler(0, cameraTransform.eulerAngles.y, 0) * movementDirection;
        movementDirection.Normalize();

        float inputMagnitude = Mathf.Clamp01(movementDirection.magnitude);
        animator.SetFloat("Input Magnitude", inputMagnitude);

        // Apply movement velocity and preserve vertical velocity
        Vector3 velocity = movementDirection * inputMagnitude * currentSpeed;
        velocity.y = rb.velocity.y;
        rb.velocity = velocity;

        // Smooth rotation toward movement direction
        if (movementDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movementDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
        }
    }

    void Jump()
    {
        if (isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
            animator.SetTrigger("Jump");
        }
    }

    void HandleRoll()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl) && isGrounded && !isRolling)
        {
            StartCoroutine(PerformRoll());
        }
    }

    private IEnumerator PerformRoll()
    {
        isRolling = true;
        animator.SetTrigger("Roll");

        float timer = 0;
        Vector3 rollDirection = transform.forward; // ✅ Roll in the direction the player is facing

        // ✅ Manually apply forward movement during the roll
        while (timer < rollDuration)
        {
            rb.MovePosition(transform.position + rollDirection * rollSpeed * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }

        isRolling = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            animator.ResetTrigger("Jump");
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    private void OnApplicationFocus(bool focus)
    {
        Cursor.lockState = focus ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !focus;
    }

    // ✅ Helper function to check if the player is moving
    public bool IsMoving()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        return horizontalInput != 0 || verticalInput != 0;
    }
}
