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



    void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentStamina = maxStamina; // Start with full stamina
        initialLightIntensity = playerLight.intensity;
    }

    void Update()
    {
        // Move the player on the horizontal plane (X and Z axes)
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
        rb.MovePosition(transform.position + movement * speed * Time.deltaTime);

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
