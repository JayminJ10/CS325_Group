using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class EnemyMovement : MonoBehaviour
{
    private float speedMax;                   //Maximum speed
    private State currentState;               //Current state
    private Vector3 startPos;                 //Starting position
    private int currentWaypoint;              //Waypoint currently navigating to
    private NavMeshAgent agent;               //Nav mesh controller

    [SerializeField]
    private List<Transform> waypoints;        //Waypoints to navigate to
    
    [SerializeField]
    private GameObject player;                //Reference to player

    // Start is called before the first frame update
    void Start()
    {
        currentState = GetComponent<EnemyState>().state;
        startPos = transform.position;
        //Ensure speed is relative to player speed
        speedMax = player.GetComponent<PlayerMovement>().speed * 0.75f;
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        currentState = GetComponent<EnemyState>().state;
        //Change movement depending on current state
        switch (currentState) {
            //Follow waypoint cases
            case State.WANDER:
            case State.SEEK:
            case State.SEARCH:
                //Change speed depending on state
                switch(currentState){
                    case State.WANDER:
                        agent.speed = speedMax * 0.75f;
                        break;
                    case State.SEEK:
                        //agent.speed = speedMax * 1.5f;
                        agent.speed = speedMax * 0.75f;
                        break;
                    case State.SEARCH:
                        agent.speed = 0;
                        break;
                }

                //Update target to waypoint
                UpdateCurrentWaypoint();
                agent.destination = waypoints[currentWaypoint].position;
                break;

            //Free movement cases
            case State.CHASE:
                agent.speed = speedMax * 1.8f;
                agent.destination = player.transform.position;
                FindNearestWaypoint();
                break;

            case State.ATTACK:
                //Stop moving, slowly turn to face player while winding up attack
                agent.speed = 0;
                if (!GetComponent<BoxCollider>().enabled)
                {
                    Vector3 relativePos = player.transform.position - transform.position;
                    Quaternion LookAtRotation = Quaternion.LookRotation(relativePos);
                    Quaternion LookAtRotationOnly_Y = Quaternion.Euler(transform.rotation.eulerAngles.x, LookAtRotation.eulerAngles.y, transform.rotation.eulerAngles.z);
                    transform.rotation = Quaternion.Lerp(transform.rotation, LookAtRotationOnly_Y, 1.75f * Time.deltaTime);

                }
                FindNearestWaypoint();
                break;
        }
    }

    /*
     * Uses current position of the enemy to determine which waypoint
     * to navigate to and update currentWaypoint accordingly
     */
    private void UpdateCurrentWaypoint()
    {
        //Check if approximately at waypoint
        if (Vector3.Distance(transform.position, waypoints[currentWaypoint].position) < 1f)
        {
            //Go to next waypoint
            currentWaypoint++;
        }

        //Last waypoint on list, return to start
        if (currentWaypoint == waypoints.Count) { currentWaypoint = 0; }
    }

    /*
     * Finds the waypoint closest to the enemy and update currentWaypoint
     * accordingly.
     */
    private void FindNearestWaypoint()
    {
        int closest = 0;
        float closestDist = Vector3.Distance(transform.position, waypoints[0].transform.position);
        for (int i = 1; i < waypoints.Count; i++)
        {
            float newDist = Vector3.Distance(transform.position, (waypoints[i].transform.position));
            if (newDist < closestDist)
            {
                closest = i;
                closestDist = newDist;
            }
        }

        currentWaypoint = closest;
    }
}
