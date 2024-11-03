using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform target;             // The target the camera follows (the player)
    public Vector3 offset;               // Offset position from the target
    public float distance = 5.0f;        // Maximum distance from the target
    public float mouseSensitivity = 2.0f;   // Mouse sensitivity for rotation
    public float smoothTime = 0.1f;      // Smooth time for camera movement
    public LayerMask collisionMask;      // Layer mask for objects that can block the camera

    private float rotationY = 0.0f;      // Vertical rotation
    private float rotationX = 0.0f;      // Horizontal rotation

    private Vector3 currentVelocity;     // Used for smooth damping

    void Start()
    {
        // Initialize the rotations
        Vector3 angles = transform.eulerAngles;
        rotationX = angles.y;
        rotationY = angles.x;

        // Lock cursor to the game window (optional)
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (target)
        {
            // Get mouse inputs
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

            // Adjust the rotations
            rotationX += mouseX;
            rotationY -= mouseY;

            // Clamp the vertical rotation to prevent flipping
            rotationY = Mathf.Clamp(rotationY, -35f, 60f);

            // Calculate the new rotation
            Quaternion rotation = Quaternion.Euler(rotationY, rotationX, 0);

            // Calculate the desired position
            Vector3 desiredPosition = target.position - (rotation * Vector3.forward * distance) + offset;

            // Check for obstacles between the player and the camera
            RaycastHit hit;
            if (Physics.Linecast(target.position + offset, desiredPosition, out hit, collisionMask))
            {
                // Position the camera at the collision point, with a small offset to avoid clipping
                desiredPosition = hit.point + hit.normal * 0.2f;
            }

            // Smoothly move to the desired position
            transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref currentVelocity, smoothTime);

            // Look at the target
            transform.LookAt(target.position + offset);
        }
    }
}
