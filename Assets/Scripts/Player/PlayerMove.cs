using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 7f;
    public float gravity = -9.81f;
    public float mass = 2.5f;
    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private bool hasJumped = false;

    // Dashing
    public float dashSpeed = 20f;
    public float dashDuration = 0.1f;
    public float dashCooldown = 1f;
    private float dashTimer;
    private float dashCooldownTimer;
    private bool isDashing = false;

    public float maxStamina = 100f;
    public float currentStamina;
    public float staminaDrainRate = 4f;
    public float staminaRegenRate = 12f;
    public bool isStaminaDepleting = true;
    public Vector3 spawnPoint;
    public float fallThreshold = -10f;

    private Transform cameraTransform;
    private float turnSmoothVelocity;
    public float turnSmoothTime = 0.1f;

    private Animation anim;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        currentStamina = maxStamina;

        cameraTransform = Camera.main.transform;

        anim = GetComponent<Animation>();

        anim.Play("Idle");
    }

    void Update()
    {
        isGrounded = controller.isGrounded;

        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        float moveVertical = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(moveHorizontal, 0f, moveVertical).normalized;

        // Handle movement
        if (direction.magnitude >= 0.1f && !isDashing)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);

            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * speed * Time.deltaTime);

            // Only play Walk animation if grounded, not dashing, and not jumping
            if (isGrounded && !anim.IsPlaying("Walk") && !hasJumped)
            {
                anim.CrossFade("Walk");
            }
        }

        // Handle idle animation if grounded, no input, not dashing, and not jumping
        if (isGrounded && direction.magnitude < 0.1f && !isDashing && !hasJumped && !anim.IsPlaying("Idle"))
        {
            anim.CrossFade("Idle");
        }

        // Start dash if meets cooldown and stamina requirements
        if (Input.GetKeyDown(KeyCode.LeftShift) && dashCooldownTimer <= 0 && currentStamina > 0 && direction.magnitude >= 0.1f)
        {
            StartDash();
        }

        // Dash cooldown timer
        if (dashCooldownTimer > 0)
        {
            dashCooldownTimer -= Time.deltaTime;
        }

        // Handle dashing
        if (isDashing)
        {
            controller.Move(transform.forward * dashSpeed * Time.deltaTime); // Move forward during dash
            dashTimer -= Time.deltaTime;
            if (dashTimer <= 0)
            {
                EndDash();
            }
        }

        // Handle jump start
        if (isGrounded && Input.GetKeyDown(KeyCode.Space) && !hasJumped)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
            anim.CrossFade("Jump");
            hasJumped = true; // Set hasJumped to true so the Jump animation plays only once
        }

        // Apply gravity while in the air
        if (!isGrounded)
        {
            velocity.y += gravity * Time.deltaTime * mass;
        }

        // Apply the velocity to the CharacterController
        controller.Move(velocity * Time.deltaTime);

        // Reset vertical velocity and handle landing
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
            hasJumped = false; // Reset jump state when landed

            // If landed, go back to Walk or Idle animation based on movement input if not dashing
            if (!isDashing)
            {
                if (direction.magnitude >= 0.1f)
                {
                    anim.CrossFade("Walk");
                }
                else
                {
                    anim.CrossFade("Idle");
                }
            }
        }

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

        if (transform.position.y < fallThreshold)
        {
            RespawnPlayer();
        }
    }

    private void StartDash()
    {
        currentStamina -= staminaDrainRate * 2;
        isDashing = true;
        dashTimer = dashDuration;
        dashCooldownTimer = dashCooldown;

        // Play the dash animation
        anim.CrossFade("Dash");
    }

    private void EndDash()
    {
        isDashing = false;

        // After dash ends, go back to Walk or Idle based on movement
        if (isGrounded)
        {
            if (controller.velocity.magnitude > 0.1f)
            {
                anim.CrossFade("Walk");
            }
            else
            {
                anim.CrossFade("Idle");
            }
        }
    }

    void HandleStaminaDepletion()
    {
        // Optional logic when stamina depletes
    }

    void RespawnPlayer()
    {
        transform.position = spawnPoint;
        velocity.y = 0f;
    }

    public void SetSpawnPoint(Vector3 newSpawnPoint)
    {
        spawnPoint = newSpawnPoint;
        Debug.Log("New spawn point set at: " + spawnPoint);
    }
}
