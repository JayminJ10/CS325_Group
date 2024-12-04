using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeZone : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] bool safe;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player")) {
            safe = true;
            player.GetComponent<PlayerStats>().safe = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player")) {
            safe = false;
            player.GetComponent<PlayerStats>().safe = false;
        }
    }
}
