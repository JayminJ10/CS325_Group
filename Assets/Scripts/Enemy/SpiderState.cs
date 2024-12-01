using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SimpleState
{
    IDLE,
    CHASE,
    ATTACK
}

public class SpiderState : MonoBehaviour
{
    private float attackCoownMax = 2f;      //Maximum cooldown time

    public GameObject player;               //Reference to player object
    public float distToPlayer;              //Distance to player from current position
    private float maxChaseDist;             //Maximum chase range
    private float maxAttackDist;            //Maximum attack distance
    private float attackCooldown;           //Time between attacks
    public SimpleState state;              //Current state
    

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        state = SimpleState.IDLE;
        maxChaseDist = 20f;
        maxAttackDist = 3.6f;
        attackCooldown = attackCoownMax;
    }

    // Update is called once per frame
    void Update()
    {
        if (!GetComponent<SpiderMovement>().IsAlive())
        {
            return;
        }
        Vector3 playerPos = player.GetComponent<Transform>().position;
        distToPlayer = Vector3.Distance(transform.position, playerPos);


        RaycastHit hit;
        Ray ray = new Ray(transform.position, (playerPos - transform.position));
        bool rayBlocked = Physics.Raycast(ray, out hit, distToPlayer);

        if (state == SimpleState.ATTACK)
        {
            Debug.Log("Attacked");
            state = SimpleState.IDLE;
        }

        //Attack state
        if (distToPlayer <= maxAttackDist && attackCooldown <= 0)
        {
            if(hit.collider.CompareTag("Player"))
            {
                state = SimpleState.ATTACK;
                attackCooldown = attackCoownMax;
            }
            
        }

        //Chase state
        else if (distToPlayer <= maxChaseDist)
        {
            if (hit.collider.CompareTag("Player"))
            {
                state = SimpleState.CHASE;
            }
        }

        //Idle state
        else
        {
            state = SimpleState.IDLE;
        }

        //reduce any timers
        attackCooldown = (attackCooldown <= 0) ? 0 : attackCooldown - 1 * Time.deltaTime;
    }
}
