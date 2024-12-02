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

    [Header("Visual Settings")]
    public Material defaultEyeMaterial; // Default material for the eyes
    public Material glowingRedMaterial; // Glowing red material for the eyes
    public List<Renderer> eyeRenderers; // List of Renderers for the enemy's eyes


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

        // Make the enemy face the direction they are moving
        FaceMovementDirection();
    }

    private void FaceMovementDirection()
    {
        // Get the velocity of the agent
        Vector3 velocity = agent.velocity;

        // Ignore small movements
        if (velocity.sqrMagnitude > 0.1f)
        {
            // Calculate the direction the agent is moving
            Vector3 direction = velocity.normalized;

            // Update the rotation to face the direction of movement
            Quaternion lookRotation = Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f); // Smooth rotation
        }
    }

    private void SetEyeMaterial(Material material)
    {
        foreach (Renderer renderer in eyeRenderers)
        {
            renderer.material = material;
        }
    }


    private void FindNextTarget()
    {
        // Skip if the enemy is already targeting or extinguishing a candle
        if (isExtinguishing) return;

        bool anyLitCandle = false; // Track if any candle is lit
        float closestDistance = float.MaxValue;
        targetCandle = null;

        foreach (CandleToNextLevel candle in candles)
        {
            if (candle.IsLit)
            {
                anyLitCandle = true; // A lit candle was found
                float distance = Vector3.Distance(transform.position, candle.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    targetCandle = candle;
                }
            }
        }

        // Change eye material based on candle states
        if (anyLitCandle)
        {
            StartCoroutine(SmoothEyeMaterialTransition(glowingRedMaterial, 0.5f)); // For glowing
        }
        else
        {
            StartCoroutine(SmoothEyeMaterialTransition(defaultEyeMaterial, 0.5f)); // For reverting
        }

        // If no lit candles are found, keep patrolling
        if (targetCandle == null)
        {
            PatrolRandomly();
        }
    }

    private IEnumerator SmoothEyeMaterialTransition(Material targetMaterial, float duration)
    {
        // Get the current material of the first eye (assuming all eyes have the same material)
        Material currentMaterial = eyeRenderers[0].material;
        float elapsed = 0f;

        // Uniformly transition all eyes
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            // Lerp between the current material and the target material
            Material lerpedMaterial = new Material(currentMaterial);
            lerpedMaterial.Lerp(currentMaterial, targetMaterial, elapsed / duration);

            // Apply the same lerped material to all eyes
            foreach (Renderer renderer in eyeRenderers)
            {
                renderer.material = lerpedMaterial;
            }

            yield return null;
        }

        // Ensure all eyes have the final material after the transition
        foreach (Renderer renderer in eyeRenderers)
        {
            renderer.material = targetMaterial;
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
