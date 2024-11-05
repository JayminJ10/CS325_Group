using UnityEngine;

public class Billboard : MonoBehaviour
{
    public Transform player;  // Reference to the player's transform

    void Start()
    {
        // Find the player in the scene by tag (make sure your player is tagged "Player")
        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                player = playerObject.transform;
            }
        }
    }

    void Update()
    {
        if (player != null)
        {
            // Make the object face the player
            transform.LookAt(player);
            // Optionally, make it only rotate around the y-axis for a flat look
            transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
        }
    }
}
