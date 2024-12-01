using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyCandleExtinguisher : MonoBehaviour
{
    [Header("Settings")]
    public List<CandleToNextLevel> candles;  // List of all candles in the level
    public float interactionRange = 2f;     // Distance within which the enemy can extinguish a candle
    public float extinguishDelay = 1f;      // Time spent extinguishing a candle
    public float patrolSpeed = 3f;          // Movement speed of the enemy
    public float checkInterval = 2f;        // How often to check for lit candles

    private NavMeshAgent agent;             // NavMeshAgent for movement
    private CandleToNextLevel targetCandle; // Current candle the enemy is targeting
    private bool isExtinguishing = false;   // Whether the enemy is currently extinguishing a candle

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = patrolSpeed;

        // Start periodic checks for lit candles
        InvokeRepeating(nameof(FindNextTarget), 0, checkInterval);
    }

    void Update()
    {
        // If the enemy is extinguishing or there's no target, don't update movement
        if (isExtinguishing) return;

        if (targetCandle != null)
        {
            // Move towards the target candle
            agent.SetDestination(targetCandle.transform.position);

            // Check if the enemy is within range of the target candle
            float distanceToCandle = Vector3.Distance(transform.position, targetCandle.transform.position);
            if (distanceToCandle <= interactionRange)
            {
                StartCoroutine(ExtinguishCandle(targetCandle));
            }
        }
        else
        {
            // If no candle is targeted, patrol randomly
            PatrolRandomly();
        }
    }

    private void FindNextTarget()
    {
        // Skip if the enemy is already targeting or extinguishing a candle
        if (isExtinguishing) return;

        // Find the nearest lit candle
        float closestDistance = float.MaxValue;
        targetCandle = null;

        foreach (CandleToNextLevel candle in candles)
        {
            if (candle.IsLit)
            {
                float distance = Vector3.Distance(transform.position, candle.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    targetCandle = candle;
                }
            }
        }

        // If no lit candles are found, keep patrolling
        if (targetCandle == null)
        {
            PatrolRandomly();
        }
    }

    private IEnumerator ExtinguishCandle(CandleToNextLevel candle)
    {
        isExtinguishing = true;
        agent.isStopped = true;

        Debug.Log($"Extinguishing candle: {candle.name}");
        yield return new WaitForSeconds(extinguishDelay);

        // Turn off the candle
        if (candle.IsLit)
        {
            candle.TurnOffCandle();
        }

        isExtinguishing = false;
        agent.isStopped = false;

        // Find the next candle to target
        FindNextTarget();
    }

    private void PatrolRandomly()
    {
        if (agent.isStopped) agent.isStopped = false;

        // Move to a random point in the level
        Vector3 randomPoint = transform.position + Random.insideUnitSphere * 10f;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 10f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
        else
        {
            Debug.Log("Failed to find a valid random patrol point.");
        }
    }

    private void OnDrawGizmos()
    {
        // Visualize the interaction range in the Editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}
