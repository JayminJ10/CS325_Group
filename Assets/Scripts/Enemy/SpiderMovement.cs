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
    private float maxAttackDist;         //Distance to attack
    public Vector3 startPos;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        speed = player.GetComponent<PlayerMovement>().speed * 1.2f;
        damage = 10;
        maxAttackDist = 3.6f;
        startPos = transform.position;
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;
        agent.destination = startPos;
        isAlive = true;
    }

    // Update is called once per frame
    void Update()
    {
        state = GetComponent<SpiderState>().state;
        Vector3 playerPos = Vector3.zero;
        if (player != null)
        {
            playerPos = player.transform.position;
        }
        

        if (!isAlive) {
            speed = 0;
            StartCoroutine(Death());
            return;
        }

        switch (state)
        {

            case SimpleState.IDLE:
                agent.destination = RandomIdleWaypoint(startPos, transform.position, agent.destination);
                agent.speed = speed * 0.5f;
                break;
            case SimpleState.CHASE:
                isAlive = !player.GetComponent<CandleMechanics>().IsShiningBrighter();
                LookAtPlayer();
                agent.destination = playerPos;
                startPos = transform.position;
                agent.speed = SetSpeed();
                break;
            case SimpleState.ATTACK:
                isAlive = !player.GetComponent<CandleMechanics>().IsShiningBrighter();
                LookAtPlayer();
                agent.destination = playerPos;
                agent.speed = SetSpeed();
                Attack();
                break;
        }
    }

    Vector3 RandomIdleWaypoint(Vector3 startPos, Vector3 currentPos, Vector3 destination)
    {
        Vector3 waypoint = destination;
        float distance = Vector3.Distance(currentPos, destination);
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

    private void Attack()
    {
        if (Vector3.Distance(player.transform.position, transform.position) <= maxAttackDist)
        {
            player.GetComponent<PlayerStats>().IsHit("Spider", damage);
        }
    }

    private float SetSpeed()
    {
        if (Vector3.Distance(player.transform.position, transform.position) <= maxAttackDist)
        {
            return 0f;
        }
        return speed;
    }

    public bool IsAlive()
    {
        return isAlive;
    }

    IEnumerator Death()
    {
        yield return new WaitForSeconds(5);
        GameObject.Destroy(gameObject);
    }

    private void LookAtPlayer()
    {
        Vector3 relativePos = player.transform.position - transform.position;
        Quaternion LookAtRotation = Quaternion.LookRotation(relativePos);
        Quaternion LookAtRotationOnly_Y = Quaternion.Euler(transform.rotation.eulerAngles.x, LookAtRotation.eulerAngles.y, transform.rotation.eulerAngles.z);
        transform.rotation = Quaternion.Lerp(transform.rotation, LookAtRotationOnly_Y, 1.75f * Time.deltaTime);
    }
}
