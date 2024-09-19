using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 9f;
    public float downForce = 0.01f;
    private CharacterController controller;
    private Vector3 velocity;
    public float gravity = -9.81f;
    private bool isGrounded;

    public float maxStamina = 100f;          // Maximum stamina
    public float currentStamina;             // Current stamina level
    public float staminaDrainRate = 7f;      // Stamina drained per second
    public float staminaRegenRate = 12f;     // Stamina regenerated per second
    public bool isStaminaDepleting = true;   // Flag to control stamina depletion

    private float initialLightIntensity;
    private float initialSecondaryLightIntensity;

    public Light playerLight;            // Reference to the player's main light
    public Light secondaryLight;         // Reference to the secondary light (e.g., a light illuminating the area)

    private Transform cameraTransform;

    private float turnSmoothVelocity;
    public float turnSmoothTime = 0.1f;

    private Animation anim;  // Reference to the Animation component for legacy animations

    void Start()
    {
        controller = GetComponent<CharacterController>();
        currentStamina = maxStamina; // Start with full stamina

        // Get initial light intensities
        initialLightIntensity = playerLight.intensity;
        initialSecondaryLightIntensity = secondaryLight.intensity;

        // Get the main camera's transform
        cameraTransform = Camera.main.transform;

        // Get the Animation component for legacy animations
        anim = GetComponent<Animation>();

        // Start with the idle animation (replace "idle" with the name of your idle animation)
        anim.Play("Idle");
    }

    void Update()
    {
        // Ground check
        isGrounded = controller.isGrounded;

        // Get input axes
        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        float moveVertical = Input.GetAxisRaw("Vertical");

        // Calculate the direction relative to the camera
        Vector3 direction = new Vector3(moveHorizontal, 0f, moveVertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            // Calculate the target angle and smooth the rotation
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);

            // Rotate the player
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            // Move the player
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * speed * Time.deltaTime);

            // Play walking animation (replace "walk" with the name of your walk animation)
            if (!anim.IsPlaying("Walk"))
            {
                anim.CrossFade("Walk");
            }

            isStaminaDepleting = true;
        }
        else
        {
            isStaminaDepleting = false;

            // Play idle animation when not moving (replace "idle" with the name of your idle animation)
            if (!anim.IsPlaying("Idle"))
            {
                anim.CrossFade("Idle");
            }
        }

        // Jumping
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);

            // Play jump animation (replace "jump" with the name of your jump animation)
            anim.CrossFade("Jump");
        }
        else
        {
            velocity.y -= downForce;
        }

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // Stamina handling...
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
            currentStamina += staminaRegenRate * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        }

        // Update light intensity based on stamina
        UpdateLightIntensity();
    }

    void HandleStaminaDepletion()
    {
        // Turn off both lights when stamina depletes
        playerLight.enabled = false;
        secondaryLight.enabled = false;
    }

    void UpdateLightIntensity()
    {
        if (currentStamina > 0)
        {
            if (!playerLight.enabled)
                playerLight.enabled = true;

            if (!secondaryLight.enabled)
                secondaryLight.enabled = true;

            // Adjust both lights' intensity proportionally to stamina
            playerLight.intensity = (currentStamina / maxStamina) * initialLightIntensity;
            secondaryLight.intensity = (currentStamina / maxStamina) * initialSecondaryLightIntensity;
        }
    }
}
