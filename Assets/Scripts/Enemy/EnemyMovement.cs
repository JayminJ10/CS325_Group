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
    
    //TODO: REMOVE WHEN ANIMATION IMPLEMENTED
    private Material attackVisualMat;         //Reference to child attack range obj material

    [SerializeField]
    private List<Transform> waypoints;        //Waypoints to navigate to
    
    [SerializeField]
    private GameObject player;                //Reference to player

    // Start is called before the first frame update
    void Start()
    {
        //TODO: REMOVE WHEN ANIMATION IMPLEMENTED
        attackVisualMat = transform.Find("Attack Hitbox Visual").gameObject.GetComponent<Renderer>().material;
        
        currentState = GetComponent<EnemyState>().state;
        startPos = transform.position;
        //Ensure speed is relative to player speed
        speedMax = player.GetComponent<PlayerMovement>().speed;
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        //TODO: REMOVE WHEN ANIMATION IMPLEMENTED
        Color color;

        currentState = GetComponent<EnemyState>().state;
        //Change movement depending on current state
        switch (currentState) {
            //Follow waypoint cases
            case State.WANDER:
            case State.SEEK:
            case State.SEARCH:
                //Change material to default
                //TODO: REMOVE WHEN ANIMATION IMPLEMENTED
                color = new Color(0, 255, 0, 0.8f);
                attackVisualMat.color = color;

                //Change speed depending on state
                switch(currentState){
                    case State.WANDER:
                        agent.speed = speedMax;
                        break;
                    case State.SEEK: 
                        agent.speed = speedMax * 2f;
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
                //Change attackVis material
                //TODO: REMOVE WHEN ANIMATION IMPLEMENTED
                color = new Color(255, 120, 0, 0.8f);
                attackVisualMat.color = color;

                agent.speed = speedMax * 2f;
                agent.destination = player.transform.position;
                break;

            case State.ATTACK:
                //Change attackVis material
                //TODO: REMOVE WHEN ANIMATION IMPLEMENTED
                color = new Color(255, 0, 0, 0.8f);
                attackVisualMat.color = color;

                //Stop moving, slowly turn to face player while winding up attack
                agent.speed = 0;
                if (!GetComponent<BoxCollider>().enabled)
                {
                    Vector3 relativePos = player.transform.position - transform.position;
                    Quaternion LookAtRotation = Quaternion.LookRotation(relativePos);
                    Quaternion LookAtRotationOnly_Y = Quaternion.Euler(transform.rotation.eulerAngles.x, LookAtRotation.eulerAngles.y, transform.rotation.eulerAngles.z);
                    transform.rotation = Quaternion.Lerp(transform.rotation, LookAtRotationOnly_Y, 1f * Time.deltaTime);

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
