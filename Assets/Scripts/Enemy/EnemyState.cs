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
    static public float searchTimerMax = 5f;          //Max value of search timer
    static public float preAttackTimerMax = 3f;       //Max value of attack timer

    public GameObject player;                          //Store player reference
    public State state;                                //Current enemy state
    public float attackRangeMax = 8f;                  //Attack range radius
    public float chaseRangeMax = 30f;                  //Chase range radius
    public float searchCooldown = searchCooldownMax;   //Search cooldown
    public float searchTimer = searchTimerMax;         //Duration of search state
    public bool playerLightOn;                         //Status of the player light

    public float preAttackTimer = preAttackTimerMax;   //Time until attack hits
    public float attackTimer = preAttackTimerMax-2;    //Time attack lasts

    private float distToPlayer;                        //Store distance to player
    private Ray ray;                                   //Ray to player location

    private bool playerSafe;                           //Ref to player safe state

    private Collider attackCol;                        //Store reference to attack collider

    void Start()
    {
        state = State.WANDER;
        attackCol = GetComponent<BoxCollider>();
        attackCol.enabled = false;
        preAttackTimer = preAttackTimerMax;
    }

    //Check if any changes needed for enemy state
    void Update()
    {
        //Update player information
        Light light = player.GetComponent<CandleMechanics>().playerLight;
        playerLightOn = light.intensity > 0.2f && light.isActiveAndEnabled;
        Vector3 playerPos = player.GetComponent<Transform>().position;
        distToPlayer = Vector3.Distance(transform.position, playerPos);
        playerSafe = player.GetComponent<PlayerStats>().safe;

        //Cast ray to player
        RaycastHit hit;
        ray = new Ray(transform.position, (playerPos - transform.position));
        bool rayBlocked = Physics.Raycast(ray, out hit, distToPlayer);

        //Attack state, overrules all other conditions
        if (state == State.ATTACK)
        {
            Debug.DrawRay(transform.position, (playerPos - transform.position), Color.red);
            preAttackTimer = (preAttackTimer <= 0) ? 0 : preAttackTimer - 1 * Time.deltaTime;
            if (preAttackTimer <= 0)
            {
                //Enable collider
                attackCol.enabled = true;
                attackTimer = (attackTimer <= 0) ? 0 : attackTimer - 1 * Time.deltaTime;
                if (attackTimer <= 0)
                {
                    Debug.Log("Attacked");
                    preAttackTimer = preAttackTimerMax;
                    attackTimer = preAttackTimerMax - 2;
                    //Start searching
                    attackCol.enabled = false;
                    state = State.SEARCH;
                    searchCooldown = 0;
                    searchTimer = 0;
                    
                }
                
            }
        }

        //If player safe, ignore them, stop pursuit
        else if (playerSafe) { state = State.WANDER; }
        
        //Chase state, overrules all other conditions
        else if (state == State.CHASE)
        {
            if (distToPlayer <= attackRangeMax && !(rayBlocked && hit.collider.CompareTag("Wall"))) { state = State.ATTACK; }
            Debug.DrawRay(transform.position, (playerPos - transform.position), Color.yellow);

        }

        //Player is within chase range and on ground level with any state
        else if (distToPlayer <= chaseRangeMax && !(player.transform.position.y > transform.position.y))
        {
            //Cast ray toward player and log possible hit
            Debug.DrawRay(transform.position, (playerPos - transform.position), Color.white);

            //Object is between player and enemy
            //Currently does nothing, I just need it to override other conditions
            //and can't be bothered to rewrite
            if (rayBlocked && !hit.collider.CompareTag("Player")) { } 

            //Attack state
            //IMPORTANT: Attack will trigger even if light is off,
            //so player can't just run up to enemy
            else if (distToPlayer <= attackRangeMax) { state = State.ATTACK; }

            //If player safe, do not pursue
            else if (playerSafe) { state = State.WANDER;  }

            //Chase state
            else if (playerLightOn) { state = State.CHASE; }

            //Search state
            //Starts if countdown at zero, and stays until search timer is complete
            else if (searchCooldown <= 0 || searchTimer > 0)
            {
                state = State.SEARCH;
                searchCooldown = searchCooldownMax;
                if (searchTimer <= 0) { searchTimer = searchTimerMax; }

                //REMOVE WHEN ATTACK IS ANIMATED
                attackCol.enabled = false;
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

        //Reduce all timers
        searchCooldown = (searchCooldown <= 0) ? 0 : searchCooldown - 1 * Time.deltaTime;
        searchTimer = (searchTimer <= 0) ? 0 : searchTimer - 1 * Time.deltaTime;
    }

    /*
     * Detect when player is colliding with enemy/attack hitbox
     */
    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject == player)
        {
            Debug.Log("Player collision: True");
            if (attackCol.enabled)
            {
                //TODO: indicate to player somehow that they have been hit and are dead
                Debug.Log("Player has been hit.");
                player.GetComponent<PlayerStats>().IsHit();
            }

        }
    }
}
