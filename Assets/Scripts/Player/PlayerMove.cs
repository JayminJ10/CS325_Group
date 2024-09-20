using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 7f;
    public float downForce = 0.1f;
    private CharacterController controller;
    private Vector3 velocity;
    public float gravity = -9.81f;
    public float mass = 2.5f;
    private bool isGrounded;

    // Dashing
    public float dashSpeed = 20f;
    public float dashDuration = 0.1f;
    public float dashCooldown = 1f;
    private float dashTimer;
    private float dashCooldownTimer;
    private bool isDashing = false;

    public float maxStamina = 100f;          // Maximum stamina
    public float currentStamina;             // Current stamina level
    public float staminaDrainRate = 4f;      // Stamina drained per second
    public float staminaRegenRate = 12f;     // Stamina regenerated per second
    public bool isStaminaDepleting = true;   // Flag to control stamina depletion

    private Transform cameraTransform;
    private float turnSmoothVelocity;
    public float turnSmoothTime = 0.1f;

    private Animation anim;  // Reference to the Animation component for legacy animations

    void Start()
    {
        controller = GetComponent<CharacterController>();
        currentStamina = maxStamina; // Start with full stamina

        // Get the main camera's transform
        cameraTransform = Camera.main.transform;

        // Get the Animation component for legacy animations
        anim = GetComponent<Animation>();

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

            float currentSpeed = isDashing ? dashSpeed : speed;
            controller.Move(moveDir.normalized * currentSpeed * Time.deltaTime);
            //controller.Move(moveDir.normalized * speed * Time.deltaTime);

            if (!anim.IsPlaying("Walk"))
            {
                anim.CrossFade("Walk");
            }

            isStaminaDepleting = true;
        }
        else
        {
            isStaminaDepleting = false;

            if (!anim.IsPlaying("Idle"))
            {
                anim.CrossFade("Idle");
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && dashCooldownTimer <= 0)
        {
            StartDash();
        }

        // Dash cooldown timer
        if (dashCooldownTimer > 0)
        {
            dashCooldownTimer -= Time.deltaTime;
        }

        // Handle dash duration
        if (isDashing)
        {
            dashTimer -= Time.deltaTime;
            if (dashTimer <= 0)
            {
                EndDash();
            }
        }

        // Jumping
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);

            anim.CrossFade("Jump");
        }
        else
        {
            velocity.y -= downForce + mass * Time.deltaTime;
        }

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // Stamina handling
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
    }

    private void StartDash()
    {
        currentStamina -= staminaDrainRate * 2;
        isDashing = true;
        dashTimer = dashDuration;
        dashCooldownTimer = dashCooldown;
    }

    private void EndDash()
    {
        isDashing = false;
    }

    void HandleStaminaDepletion()
    {
        // You can leave this empty since CandleMechanics will now handle turning off lights.
    }
}
