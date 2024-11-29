using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Xml.Schema;
using UnityEngine;
using UnityEngine.AI;

public class SpiderMovement : MonoBehaviour
{
    private GameObject player;           //Reference to player game object
    private SimpleState state;           //Current state
    private NavMeshAgent agent;          //Navmesh agent reference
    private bool isAlive;                //Alive status
    private float speed;                 //Movement speed
    private int damage;                  //Damage to player when attacks
    public Vector3 startPos;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        speed = player.GetComponent<PlayerMovement>().speed * 1.2f;
        damage = 1;
        startPos = transform.position;
        Debug.Log("Start pos: " + startPos);
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;
        agent.destination = startPos;
        isAlive = true;
    }

    // Update is called once per frame
    void Update()
    {
        state = GetComponent<SpiderState>().state;
        Vector3 playerPos = player.transform.position;

        if (!isAlive) {
            speed = 0;
            return;
        }

        switch (state)
        {

            case SimpleState.IDLE:
                agent.destination = RandomIdleWaypoint(startPos, transform.position, agent.destination);
                agent.speed = speed * 0.5f;
                
                break;
            case SimpleState.CHASE:
                agent.destination = playerPos;
                startPos = transform.position;
                agent.speed = speed;
                break;
            case SimpleState.ATTACK:
                agent.destination = playerPos;
                agent.speed = speed;
                break;
        }
        
        Debug.Log("Destination: " + agent.destination);
        Debug.Log("Dist to destination: " + Vector3.Distance(agent.destination, transform.position));



    }

    Vector3 RandomIdleWaypoint(Vector3 startPos, Vector3 currentPos, Vector3 destination)
    {
        Debug.Log("---------------RANDOMIDLEWAYPOINTENTER------------");
        Debug.Log("StartPos: " + startPos);
        Debug.Log("currentPos: " + currentPos);
        Debug.Log("Destination: " + destination);

        Vector3 waypoint = destination;
        float distance = Vector3.Distance(currentPos, destination);
        Debug.Log("Distance checking: " + distance);
        if (distance >= 2f)
        {
            return waypoint;
        }

        bool valid = false;
        while (!valid)
        {
            float newX = startPos.x + Random.Range(-5f, 5f);
            float newY = startPos.y;
            float newZ = startPos.z + Random.Range(-5f, 5f);
            waypoint = new Vector3(newX, newY, newZ);
            
            Ray ray = new Ray(waypoint, new Vector3(newX, -1, newZ));
            RaycastHit hit;
            bool rayBlocked = Physics.Raycast(ray, out hit);

            valid = true;
            
            //Check if dist to floor is too great
            if (hit.distance < 2f)
            {
                //Check if random point is outside of an object
                ray = new Ray(currentPos, waypoint);
                rayBlocked = Physics.Raycast(ray, out hit, 5f);
                if (!rayBlocked)
                {
                    valid = true;
                }

            }
            
        }
        return waypoint;
    }
}
