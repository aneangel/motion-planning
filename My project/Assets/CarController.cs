using UnityEngine; 
using UnityEngine.AI;

public class CarController : MonoBehaviour
{
    [Header("Goal Settings")]
    // Optionally assign the goal marker via inspector.
    public Transform goalMarker;

    [Header("Path Update Settings")]
    // How often (in seconds) to recalc the path.
    public float pathUpdateInterval = 0.5f;
    private float pathUpdateTimer;

    private NavMeshAgent agent;
    private LineRenderer lineRenderer;

    void Start()
    {
        // Get required components.
        agent = GetComponent<NavMeshAgent>();
        lineRenderer = GetComponent<LineRenderer>();

        // Initially, don't move until a goal marker is set.
        agent.isStopped = true;
    }

    void Update()
    {
        // Ensure the agent is on a NavMesh before proceeding.
        if (!agent.isOnNavMesh)
        {
            Debug.LogWarning("NavMeshAgent is not on a NavMesh yet.");
            return;
        }

        // If no goal marker is set, try to find one in the scene.
        if (goalMarker == null)
        {
            GameObject gm = GameObject.FindGameObjectWithTag("GoalMarker");
            if (gm != null)
            {
                goalMarker = gm.transform;
                // Resume movement when a new goal is found.
                agent.isStopped = false;
            }
            else
            {
                // No goal marker found – clear the path line and do nothing.
                lineRenderer.positionCount = 0;
                return;
            }
        }

        // Periodically update the path to the current goal.
        pathUpdateTimer += Time.deltaTime;
        if (pathUpdateTimer >= pathUpdateInterval)
        {
            pathUpdateTimer = 0f;
            UpdatePath();
        }

        // Draw the dynamic path if one exists.
        if (agent.hasPath)
        {
            DrawPath(agent.path);
        }
        else
        {
            lineRenderer.positionCount = 0;
        }

        // Check if the agent has reached its destination.
        if (!agent.pathPending && agent.hasPath && agent.remainingDistance <= agent.stoppingDistance)
        {
            // Stop the agent and clear the visual path.
            agent.isStopped = true;
            lineRenderer.positionCount = 0;
            // Optionally, remove the reached goal marker.
            Destroy(goalMarker.gameObject);
            goalMarker = null;
        }
    }

    /// <summary>
    /// Sets the agent’s destination to the goal marker’s position.
    /// </summary>
    void UpdatePath()
    {
        if (goalMarker != null)
        {
            agent.SetDestination(goalMarker.position);
        }
    }

    /// <summary>
    /// Draws a line from the car’s current position through all the NavMesh path corners.
    /// </summary>
    /// <param name="path">The computed NavMeshPath.</param>
    void DrawPath(NavMeshPath path)
    {
        if (lineRenderer == null)
            return;

        // Create an array that starts at the car's position followed by the path's corners.
        Vector3[] positions = new Vector3[path.corners.Length + 1];
        positions[0] = transform.position;
        for (int i = 0; i < path.corners.Length; i++)
        {
            positions[i + 1] = path.corners[i];
        }
        lineRenderer.positionCount = positions.Length;
        lineRenderer.SetPositions(positions);
    }

    // Optional: Detect collisions with obstacles to force a re-route.
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            // Stop, update the path, then resume movement.
            agent.isStopped = true;
            UpdatePath();
            Invoke("ResumeMovement", 0.5f);
        }
    }

    void ResumeMovement()
    {
        agent.isStopped = false;
    }
}
