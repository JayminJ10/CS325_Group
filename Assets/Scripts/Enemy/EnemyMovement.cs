using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    private float speedMax;                   //Maximum speed
    private State currentState;               //Current state
    private Vector3 target;                   //Target to move toward
    private float speed;                      //Current speed
    private Vector3 startPos;                //Starting position
    private int currentWaypoint;             //Waypoint currently navigating to

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
        speedMax = player.GetComponent<PlayerMovement>().speed;
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
                    case State.WANDER: speed = speedMax;
                        break;
                    case State.SEEK: speed = speedMax * 2f;
                        break;
                    case State.SEARCH: speed = 0;
                        break;
                }

                //Update target to waypoint
                UpdateCurrentWaypoint();
                target = waypoints[currentWaypoint].position;
                break;

            //Free movement cases
            case State.CHASE:
                //TODO: Make better system to account for walls
                //Currently, stops when gets to player position when triggered
                //Should be fixed when nav map is implemented
                target = player.transform.position;
                speed = speedMax * 2f;
                break;

            case State.ATTACK:
                break;
        }

        //Move to target
        var step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, target, step);
    }


    private void UpdateCurrentWaypoint()
    {
        //Check if approximately at waypoint
        if (Vector3.Distance(transform.position, target) < 0.5f)
        {
            //Go to next waypoint
            currentWaypoint++; 
        }

        //Last waypoint on list, return to start
        if (currentWaypoint == waypoints.Count) { currentWaypoint = 0; }
    }
}
