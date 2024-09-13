using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public Transform player; // Reference to the player (capsule)
    public Vector3 offset;   // Offset between the player and the camera
    public float smoothSpeed = 0.125f; // Speed for smooth camera movement

    void LateUpdate()
    {
        // Desired position of the camera based on player position + offset
        Vector3 desiredPosition = player.position + offset;

        // Smooth the camera movement
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Update the camera's position
        transform.position = smoothedPosition;

        // Always look at the player
        transform.LookAt(player);
    }
}
