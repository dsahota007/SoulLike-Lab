using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float maximumSpeed = 5f;       // Normal movement speed
    public float sprintSpeed = 8f;        // Sprint speed when holding Shift
    public float rotationSpeed = 700f;    // Rotation speed when turning
    public float jumpForce = 5f;          // Force applied when jumping
    public Transform cameraTransform;     // Reference to the camera for direction

    private Animator animator;
    private Rigidbody rb;
    private bool isGrounded;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // Prevent physics-based rotation
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous; // Prevent falling through ground
    }

    void Update()
    {
        // Handle movement and sprinting only if grounded
        if (isGrounded)
        {
            // Get input values
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");
            bool isSprinting = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

            // Set Animator parameter for sprinting
            animator.SetBool("IsSprinting", isSprinting);

            // Determine speed based on whether sprinting
            float currentSpeed = isSprinting ? sprintSpeed : maximumSpeed;

            // Calculate movement direction based on camera orientation
            Vector3 movementDirection = new Vector3(horizontalInput, 0, verticalInput);
            movementDirection = Quaternion.Euler(0, cameraTransform.eulerAngles.y, 0) * movementDirection;
            movementDirection.Normalize();

            // Set Input Magnitude for Animator
            float inputMagnitude = Mathf.Clamp01(movementDirection.magnitude);
            animator.SetFloat("Input Magnitude", inputMagnitude);

            // Calculate velocity and maintain vertical velocity
            Vector3 velocity = movementDirection * inputMagnitude * currentSpeed;
            velocity.y = rb.velocity.y;

            // Apply movement
            rb.velocity = velocity;

            // Handle rotation
            if (movementDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(movementDirection);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }

        // Handle jump input
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
        }
    }

    void Jump()
    {
        if (isGrounded) // Only jump if grounded
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse); // Apply jump force
            isGrounded = false; // Set grounded to false immediately after jumping
            animator.SetTrigger("Jump"); // Trigger the Jump animation
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Check for grounding
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            animator.ResetTrigger("Jump"); // Reset the Jump trigger when grounded
        }
    }

    void OnCollisionExit(Collision collision)
    {
        // Lost grounding
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    private void OnApplicationFocus(bool focus)
    {
        // Lock cursor when focused
        Cursor.lockState = focus ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !focus;
    }
}
