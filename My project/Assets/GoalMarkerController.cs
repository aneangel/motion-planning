using UnityEngine;

public class GoalMarkerController : MonoBehaviour
{
    // Reference to the green marker prefab
    public GameObject goalMarkerPrefab;
    
    // Holds the reference to the current goal marker instance
    private GameObject currentGoalMarker;

    void Update()
    {
        // Check for left mouse click (index 0)
        if (Input.GetMouseButtonDown(0))
        {
            // Create a ray from the camera through the mouse click position
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // If the raycast hits a collider (like your plane)
            if (Physics.Raycast(ray, out hit))
            {
                // If no marker exists, instantiate it; otherwise, reposition it
                if (currentGoalMarker == null)
                {
                    currentGoalMarker = Instantiate(goalMarkerPrefab, hit.point, Quaternion.identity);
                }
                else
                {
                    currentGoalMarker.transform.position = hit.point;
                }
                
                // (Optional) You can now use currentGoalMarker.transform.position as the goal for any agent.
            }
        }
    }
}
