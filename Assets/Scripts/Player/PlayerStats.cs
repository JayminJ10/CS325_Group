using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField]
    private TitleCardManager titleCardManager;
    private bool alive;
    // Start is called before the first frame update
    void Start()
    {
        alive = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!alive)
        {
            titleCardManager.ShowDeathCard();
        }
    }

    public void IsHit()
    {
        alive = false;
    }
}
