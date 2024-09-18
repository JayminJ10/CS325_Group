using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 5f;
    private Rigidbody rb;
    private bool isGrounded;

    public float maxStamina = 100f;          // Maximum stamina
    public float currentStamina;             // Current stamina level
    public float staminaDrainRate = 7f;      // Stamina drained per second
    public float staminaRegenRate = 12f;     // Stamina regenerated per second
    public bool isStaminaDepleting = true;   // Flag to control stamina depletion

    private float initialLightIntensity;
    public Light playerLight; // Reference to the player's light

    private Transform cameraTransform;

    private float turnSmoothVelocity;
    public float turnSmoothTime = 0.1f;

    private bool jumpPressed; // New variable to detect if jump was pressed

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate; // Enable interpolation for smoother movement

        currentStamina = maxStamina; // Start with full stamina
        initialLightIntensity = playerLight.intensity;

        // Get the main camera's transform
        cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        // Handle movement inputs here
        HandleMovementInput();

        // Detect jump input here in Update()
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            jumpPressed = true; // Set this to true, so we handle the jump in FixedUpdate()
        }

        // Drain stamina over time
        if (isStaminaDepleting && currentStamina > 0)
        {
            currentStamina -= staminaDrainRate * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);

            if (currentStamina == 0)
            {
                HandleStaminaDepletion();
            }
        }
        else if (!isStaminaDepleting && currentStamina < maxStamina)
        {
            // Regenerate stamina when not depleting
            currentStamina += staminaRegenRate * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        }

        // Update light intensity based on stamina
        UpdateLightIntensity();
    }

    void FixedUpdate()
    {
        // Handle movement here
        MoveCharacter();

        // Handle jumping here in FixedUpdate
        if (jumpPressed && isGrounded)
        {
            Jump();
            jumpPressed = false; // Reset the jump input after applying force
        }
    }

    private void HandleMovementInput()
    {
        // Get input axes
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        // Calculate the direction relative to the camera
        Vector3 direction = new Vector3(moveHorizontal, 0f, moveVertical).normalized;

        // If there is input
        if (direction.magnitude >= 0.1f)
        {
            isStaminaDepleting = true;
        }
        else
        {
            isStaminaDepleting = false;
        }
    }

    private void MoveCharacter()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(moveHorizontal, 0f, moveVertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);

            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            rb.MovePosition(rb.position + moveDir.normalized * speed * Time.fixedDeltaTime);
        }
    }

    private void Jump()
    {
        // Apply force for jumping
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    void OnCollisionEnter(Collision collision)
    {
        // Check if the player is on the ground
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        // Check if the player leaves the ground
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    void HandleStaminaDepletion()
    {
        // Turn off the light when stamina depletes
        playerLight.enabled = false;
    }

    void UpdateLightIntensity()
    {
        if (currentStamina > 0)
        {
            if (!playerLight.enabled)
                playerLight.enabled = true;

            // Adjust light intensity proportionally to stamina
            playerLight.intensity = (currentStamina / maxStamina) * initialLightIntensity;
        }
    }
}
