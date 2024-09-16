using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 5f;
    private Rigidbody rb;
    private bool isGrounded;

    public float maxStamina = 100f;          // Maximum stamina
    public float currentStamina;             // Current stamina level
    public float staminaDrainRate = 10f;     // Stamina drained per second
    public float staminaRegenRate = 5f;      // Stamina regenerated per second
    public bool isStaminaDepleting = true;   // Flag to control stamina depletion

    private float initialLightIntensity;
    public Light playerLight; // Reference to the player's light

    private Transform cameraTransform;

    private float turnSmoothVelocity;
    public float turnSmoothTime = 0.1f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentStamina = maxStamina; // Start with full stamina
        initialLightIntensity = playerLight.intensity;

        // Get the main camera's transform
        cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        // Get input axes
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        // Calculate the direction relative to the camera
        Vector3 direction = new Vector3(moveHorizontal, 0f, moveVertical).normalized;

        // If there is input
        if (direction.magnitude >= 0.1f)
        {
            // Calculate the target angle and smooth the rotation
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);

            // Rotate the player
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            // Move the player
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            rb.MovePosition(transform.position + moveDir.normalized * speed * Time.deltaTime);

            isStaminaDepleting = true;
        }
        else
        {
            isStaminaDepleting = false;
        }

        // Jumping
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
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
}
