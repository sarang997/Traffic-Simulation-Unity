using UnityEngine;

public class MultipleIntersectionsManager : MonoBehaviour
{
    public IntersectionGenerator intersectionPrefab; // Assign in the Inspector
    public int numberOfIntersections = 3; // Total number of intersections you want to generate
    public float distanceBetweenIntersections = 50f; // Distance between the centers of consecutive intersections

    // Keep track of the exit and entry points for drawing Gizmos
   // private GameObject previousExit = null;
   // private GameObject currentEntry = null;
/*    private GameObject currentExit = null;
    private GameObject previousEntry = null;*/

    private void Start()
    {
        GenerateAndConnectIntersectionsOnTheFly();
    }

    private void GenerateAndConnectIntersectionsOnTheFly()
    {
        IntersectionGenerator previousIntersection = null;

        for (int i = 0; i < numberOfIntersections; i++)
        {
            Vector3 newPosition = new Vector3(i * distanceBetweenIntersections, 0, 0);
            IntersectionGenerator newIntersection = Instantiate(intersectionPrefab, newPosition, Quaternion.identity, transform);

            newIntersection.Initialize(i + 1, "");

            if (previousIntersection != null)
            {
                // This is where you would update the references for previousExit, currentEntry, etc.
                // For demonstration, these are placeholders. Replace with actual logic to get the waypoints.
                GameObject currentEntry = newIntersection.road4.entryWaypoint; // Assuming road1 is the entry for the current intersection
                GameObject currentExit = newIntersection.road4.exitWaypoint; // Assuming road2 is the exit for the previous intersection

                GameObject previousExit = previousIntersection.road2.exitWaypoint; // Assuming road2 is the exit for the current intersection
                GameObject previousEntry = previousIntersection.road2.entryWaypoint; // Assuming road4 is the entry for the next intersection
                                                                         // Optionally, connect these waypoints as needed
                ExitWps previousExitWp = previousExit.GetComponent<ExitWps>();

                if (previousExitWp != null && currentEntry != null)
                {
                    // Add the current entry waypoint's transform to the previous exit's connected waypoints
                    previousExitWp.connectedWaypoints.Add(currentEntry.transform);
                }
                else
                {
                    Debug.LogError("Either previous exit or current entry waypoint is missing a required component.");
                }
                ExitWps currentExitWp = currentExit.GetComponent<ExitWps>();

                if (currentExitWp != null && currentExit != null)
                {
                    // Add the current entry waypoint's transform to the previous exit's connected waypoints
                    currentExitWp.connectedWaypoints.Add(previousEntry.transform);
                }
                else
                {
                    Debug.LogError("Either previous exit or current entry waypoint is missing a required component.");
                }

            }

            previousIntersection = newIntersection;
        }
    }

/*    // Draw Gizmos to visually represent the connections
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red; // Set the Gizmo color to red
        if (previousExit != null && currentEntry != null)
        {
            // Draw a line from the exit of the previous intersection to the entry of the current one
            Gizmos.DrawLine(previousExit.transform.position, currentEntry.transform.position);
        }

        if (currentExit != null && nextEntry != null)
        {
            // Draw a line from the exit of the current intersection to the entry of the next one
            Gizmos.DrawLine(currentExit.transform.position, nextEntry.transform.position);
        }
    }*/
}
