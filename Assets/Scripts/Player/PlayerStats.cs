using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField]
    private TitleCardManager titleCardManager;  //Reference to TitleCardManager
    private bool alive;                         //Player life
    public bool safe;                           //If player can be detected/attacked by enemy
    // Start is called before the first frame update
    void Start()
    {
        alive = true;
        safe = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!alive)
        {
            titleCardManager.ShowDeathCard();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Safe Zone"))
        {
            safe = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Safe Zone"))
        {
            safe = false;
        }
    }

    public void IsHit()
    {
        if (!safe)
        {
            alive = false;
        }
    }
}
