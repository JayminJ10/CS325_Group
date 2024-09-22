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
    static public float searchCooldownMax = 20f;      //Max value of search cooldown
    static public float searchTimerMax = 10f;         //Max value of search timer
    
    public GameObject player;                          //Store player reference
    public State state;                                //Current enemy state
    public float attackRangeMax = 10f;                 //Attack range radius
    public float chaseRangeMax = 40f;                  //Chase range radius
    public float searchCooldown = searchCooldownMax;   //Search cooldown
    public float searchTimer = searchTimerMax;         //Duration of search state
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
        Light light = player.GetComponent<CandleMechanics>().playerLight;
        playerLightOn = light.intensity > 0.2f && light.isActiveAndEnabled;
        Vector3 playerPos = player.GetComponent<Transform>().position;
        distToPlayer = Vector3.Distance(transform.position, playerPos);
        
        RaycastHit hit;        
        
        //Attack state, overrules all other conditions
        if (state == State.ATTACK)
        {
            //With no enemy animations or player death scenarios, 
            //all this does is return to search mode
            Debug.DrawRay(transform.position, (playerPos - transform.position), Color.red);
            state = State.SEARCH;
            searchCooldown = 0;
            searchTimer = 0;
        }

        //Chase state, overrules all other conditions
        else if (state == State.CHASE)
        {
            if (distToPlayer <= attackRangeMax) { state = State.ATTACK; }
            Debug.DrawRay(transform.position, (playerPos - transform.position), Color.yellow);

        }

        //Player is within chase range with any state
        else if (distToPlayer <= chaseRangeMax)
        {
            //Cast ray toward player and log possible hit
            ray = new Ray(transform.position, (playerPos - transform.position));
            bool rayBlocked = Physics.Raycast(ray, out hit, distToPlayer);
            Debug.DrawRay(transform.position, (playerPos - transform.position), Color.white);

            //Wall is between player and enemy
            //Currently does nothing, I just need it to override other conditions
            //and can't be bothered to rewrite
            if (rayBlocked && hit.collider.CompareTag("Wall")) { } 

            //Attack state
            //IMPORTANT: Attack will trigger even if light is off,
            //so player can't just run up to enemy
            else if (distToPlayer <= attackRangeMax) { state = State.ATTACK; }

            //Chase state
            else if (playerLightOn) { state = State.CHASE; }

            //Search state
            //Starts if countdown at zero, and stays until search timer is complete
            else if (searchCooldown <= 0 || searchTimer > 0)
            {
                state = State.SEARCH;
                searchCooldown = searchCooldownMax;
                if (searchTimer <= 0) { searchTimer = searchTimerMax; }
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
            Debug.DrawRay(transform.position, (playerPos - transform.position), Color.blue);
        }

        //Reduce search cooldown
        searchCooldown = (searchCooldown <= 0) ? 0 : searchCooldown - 1 * Time.deltaTime;
        searchTimer = (searchTimer <= 0) ? 0 : searchTimer - 1 * Time.deltaTime;
    }
}
