using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum State //All possible states
{
    WANDER,     //Move along on predefined path
    SEEK,       //Same as WANDER, but at increased speed
    SEARCH,     //Stay in place, looking animation
    CHASE,      //Follow player
    ATTACK      //Attack player
}

public class EnemyState : MonoBehaviour
{
    static public float searchCooldownMax = 100f;      //Max value of search cooldown
    
    public GameObject player;                          //Store player reference
    public State state;                                //Current enemy state
    public float attackRangeMax = 10f;                 //Attack range radius
    public float chaseRangeMax = 20f;                  //Chase range radius
    public float searchCooldown = searchCooldownMax;   //Search cooldown
    public bool playerLightOn;                         //Status of the player light

    private float distToPlayer;                        //Store distance to player
    private Ray ray;                                   //Ray to player location

    void Start()
    {
        state = State.WANDER;
    }

    //Check if any changes needed for enemy state
    void Update()
    {
        //Update player information
        Light light = player.GetComponent<PlayerMovement>().playerLight;
        playerLightOn = light.intensity > 0.2f && light.isActiveAndEnabled;
        Vector3 playerPos = player.GetComponent<Transform>().position;
        distToPlayer = playerPos.magnitude;
        RaycastHit hit;        
        
        //Attack state, overrules all other conditions
        if (state == State.ATTACK)
        {
            //WARNING: CURRENTLY ONCE STATE IS ENTERED, CANNOT BE CHANGED (9/19)
            //Handle all attack functionalities
            //Do not change state unless attack animation finished
            //If player dead, return to WANDER
            //Else, search
        }

        //Chase state, overrules all other conditions
        else if (state == State.CHASE)
        {
            if (distToPlayer <= attackRangeMax) { state = State.ATTACK; }
        }

        //Player is within chase range with any state
        else if (distToPlayer <= chaseRangeMax)
        {
            //Cast ray toward player and log possible hit
            ray = new Ray(transform.position, playerPos);
            bool rayBlocked = Physics.Raycast(ray, out hit, distToPlayer);

            //Wall is between player and enemy
            //Currently does nothing, I just need it to override other conditions
            //and can't be bothered to rewrite
            if (rayBlocked && hit.collider.CompareTag("Wall")) { } 

            //Attack state
            else if (distToPlayer <= attackRangeMax) { state = State.ATTACK; }

            //Chase state
            else if (playerLightOn) { state = State.CHASE; }

            //Search state
            else if (searchCooldown <= 0)
            {
                state = State.SEARCH;
                searchCooldown = searchCooldownMax;
            }
            //Default to WANDER
            else { state = State.WANDER; }
        }

        //Player outside chase range
        else
        {
            //Player is outside chase range, seek or wander states
            if (playerLightOn) { state = State.SEEK; }
            else { state = State.WANDER; }
        }

        //Reduce search cooldown
        searchCooldown -= 1 *Time.deltaTime;
    }
}
