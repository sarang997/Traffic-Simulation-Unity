using UnityEngine;

public class TestMultipleIntersectionsManager : MonoBehaviour
{
    public GameObject carObject; // Assign this in the Inspector
    public GameObject[] carPrefabs; // Assign this in the Inspector with your 4 vehicle types

    public IntersectionGenerator intersectionPrefab; // Assign in the Inspector
    public int numberOfIntersections = 1; // Total number of intersections you want to generate

    // Keep track of the exit and entry points for drawing Gizmos
    // private GameObject previousExit = null;
    // private GameObject currentEntry = null;
    /*    private GameObject currentExit = null;
        private GameObject previousEntry = null;*/

    private void Start()
    {
        GenerateAndConnectIntersectionsGrid();
        AssignInitialTargetWaypoint();
    }
    private void AssignInitialTargetWaypoint()
    {
        if (carObject != null)
        {
            CarMovement carMovement = carObject.GetComponent<CarMovement>();
            if (carMovement != null)
            {
                // Use GameObject.Find to locate the waypoint by name
                GameObject initialWaypointObject = GameObject.Find("Intersection_1_Exit_1");

                if (initialWaypointObject != null)
                {
                    Transform initialWaypoint = initialWaypointObject.transform;
                    carMovement.initialTargetWaypoint = initialWaypoint;
/*                    carMovement.targetWaypoint = initialWaypoint*//**//*; // Also set targetWaypoint if needed
*/                    Debug.Log("Initial waypoint assigned to the car by name.");
                }
                else
                {
                    Debug.LogError("Initial waypoint not found by name.");
                }
            }
            else
            {
                Debug.LogError("CarMovement script not found on the car object.");
            }
        }
        else
        {
            Debug.LogError("Car object not assigned in TestMultipleIntersectionsManager.");
        }
    }


    private void GenerateAndConnectIntersectionsGrid()
{
    int numberOfRows = 5; // Specify the number of rows
    int numberOfColumns = 5; // Specify the number of columns
    float distanceBetweenIntersections = 60f; // Distance between the centers of consecutive intersections

    IntersectionGenerator[,] grid = new IntersectionGenerator[numberOfRows, numberOfColumns];

    for (int row = 0; row < numberOfRows; row++)
    {
        for (int col = 0; col < numberOfColumns; col++)
        {
            Vector3 newPosition = new Vector3(col * distanceBetweenIntersections, 0, row * distanceBetweenIntersections);
            IntersectionGenerator newIntersection = Instantiate(intersectionPrefab, newPosition, Quaternion.identity, transform);
            string position = DetermineIntersectionPosition(row, col, numberOfRows, numberOfColumns);

            newIntersection.Initialize(row * numberOfColumns + col + 1, position);
            grid[row, col] = newIntersection;

            // Instantiate and configure a car at each intersection
            SpawnCarAtIntersection(newIntersection);

            // Connect to left neighbor
            if (col > 0)
            {
                ConnectIntersections(grid[row, col - 1], newIntersection, road2: true, road4: true);
            }

            // Connect to top neighbor
            if (row > 0)
            {
                ConnectIntersections(grid[row - 1, col], newIntersection, road1: true, road3: true);
            }
        }
    }
}

private void SpawnCarAtIntersection(IntersectionGenerator intersection)
{
    if (carPrefabs.Length > 0 && intersection != null)
    {
        // Specify the number of cars you want to spawn at each intersection
        int carsToSpawn = 3; // For example, spawning 3 cars at each intersection

        for (int i = 0; i < carsToSpawn; i++)
        {
            // Randomly select a car prefab
            int prefabIndex = Random.Range(0, carPrefabs.Length);
            GameObject carPrefab = carPrefabs[prefabIndex];

            // Optional: Adjust the spawn position for each car to avoid overlapping
            Vector3 spawnPosition = intersection.transform.position + new Vector3(i * 2, 0, 0); // Example adjustment

            // Assuming the car's initial target waypoint is one of the exit waypoints of the intersection
            GameObject initialWaypoint = intersection.road1.exitWaypoint; // You might want to choose different waypoints for each car

            if (initialWaypoint != null)
            {
                // Instantiate the car at the adjusted position of the intersection
                GameObject carInstance = Instantiate(carPrefab, spawnPosition, Quaternion.identity);

                // Assign the initial target waypoint to the car
                CarMovement carMovement = carInstance.GetComponent<CarMovement>();
                if (carMovement != null)
                {
                    carMovement.initialTargetWaypoint = initialWaypoint.transform;
                    Debug.Log("Car spawned at intersection with initial waypoint assigned.");
                }
                else
                {
                    Debug.LogError("CarMovement script not found on the car instance.");
                }
            }
            else
            {
                Debug.LogError("Initial waypoint not found within the intersection.");
            }
        }
    }
    else
    {
        Debug.LogError("Car prefabs array is empty or intersection is null.");
    }
}



    private string DetermineIntersectionPosition(int row, int col, int totalRows, int totalColumns)
    {
        // Corners
        if (row == 0 && col == 0) return "top-left";
        if (row == 0 && col == totalColumns - 1) return "top-right";
        if (row == totalRows - 1 && col == 0) return "bottom-left";
        if (row == totalRows - 1 && col == totalColumns - 1) return "bottom-right";

        // Edges
        if (row == 0) return "top-edge";
        if (row == totalRows - 1) return "bottom-edge";
        if (col == 0) return "left-edge";
        if (col == totalColumns - 1) return "right-edge";

        // Inside
        return "inside";
    }

    private void ConnectIntersections(IntersectionGenerator previousIntersection, IntersectionGenerator currentIntersection, bool road2 = false, bool road4 = false, bool road1 = false, bool road3 = false)
    {
        Debug.Log($"intersections: {currentIntersection.road4.entryWaypoint}, {previousIntersection.road2.exitWaypoint}");
        if (road2 && road4)
        {
            Debug.Log("connect 2 4 ");

            // Connect road4 of the current to road2 of the previous

            GameObject currentEntry = currentIntersection.road4.entryWaypoint;
            GameObject previousExit = previousIntersection.road2.exitWaypoint;
            Debug.Log($"connect 2 4:{currentEntry}, {previousExit} ");

            ConnectWaypoints(previousExit, currentEntry);

            // Connect road2 of the current to road4 of the previous (new logic)
            GameObject currentExit = currentIntersection.road4.exitWaypoint;
            GameObject previousEntry = previousIntersection.road2.entryWaypoint;
            ConnectWaypoints(currentExit, previousEntry);
        }

        if (road1 && road3)
        {
            Debug.Log("connect 1 3");

            // Connect road1 of the current to road3 of the previous
            GameObject currentEntry = currentIntersection.road3.entryWaypoint;
            GameObject previousExit = previousIntersection.road1.exitWaypoint;
            ConnectWaypoints(previousExit, currentEntry);

            GameObject currentExit = currentIntersection.road3.exitWaypoint;
            GameObject previousEntry = previousIntersection.road1.entryWaypoint;
            ConnectWaypoints(currentExit, previousEntry);

        }
    }

    private void ConnectWaypoints(GameObject exitWaypoint, GameObject entryWaypoint)
    {
        Debug.Log("inside connect waypoints");
        ExitWps exitWps = exitWaypoint.GetComponent<ExitWps>();
        Debug.Log("after connect waypoints");

        if (exitWps != null && entryWaypoint != null)
        {
            exitWps.connectedWaypoints.Add(entryWaypoint.transform);
        }
        else
        {
            Debug.LogError("Either exit or entry waypoint is missing a required component.");
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
