using UnityEngine;
using System.Collections.Generic;

public class IntersectionGenerator : MonoBehaviour
{
    public float roadLength = 20f;
    public GameObject waypointPrefab; // Prefab for visualizing waypoints
    private List<GameObject> entryObjects = new List<GameObject>();
    private List<GameObject> exitObjects = new List<GameObject>();
    private int intersectionID; // Added to store the intersection identifier
    public GameObject redLightPrefab; // Assign this in the Unity Editor
    public GameObject trafficLightManagerPrefab; // Assign your TrafficLightManager prefab in Inspector


    // Method to set the intersection ID
    public void SetIntersectionID(int id)


    {
        intersectionID = id;
    }
    public Road road1, road2, road3, road4;
    /*   void Awake()
       {
       GenerateIntersection();
       AssignConnectedWaypoints();
       }*/
   public void Initialize(int id, string position)
    {
        this.intersectionID = id;
        GenerateIntersection(position);
        AssignConnectedWaypoints();
    }


    void GenerateIntersection(string position)
    {
        // Clear previous entry and exit objects to handle re-initialization
        entryObjects.Clear();
        exitObjects.Clear();
        // Debug.Log($"position: {position}");
        for (int i = 0; i < 4; i++) // Iterate over potential four roads
        {
            // Skip road creation based on position
            if (position == "top-left" && (i == 2 || i == 3)) continue; // Skip road3 and road4 for top-left
            if (position == "top-right" && (i == 1 || i == 2)) continue; // Example for top-right, adjust as needed
            if (position == "top-edge" && (i == 2)) continue; // Example for top-right, adjust as needed
            if (position == "left-edge" && (i == 3)) continue;
            if (position == "right-edge" && (i == 1)) continue;
            if (position == "bottom-edge" && (i == 0)) continue;

            if (position == "bottom-left" && (i == 0 || i == 3)) continue;
            if (position == "bottom-right" && (i == 0 || i == 1)) continue;
/*            if (position == "inside") continue;
*/

            // Debug.Log($"intersectionID: {intersectionID}");
            Vector3 roadDirection = Quaternion.Euler(0, i * 90, 0) * Vector3.forward;
            Vector3 roadEnd = transform.position + (roadDirection * roadLength);

            // Entry waypoint
            Vector3 entryPoint = roadEnd + (Quaternion.Euler(0, 90, 0) * roadDirection * 4);
            GameObject entry = Instantiate(waypointPrefab, entryPoint, Quaternion.identity, transform);
            entry.name = $"Intersection_{intersectionID}_Entry_{i + 1}";
            entry.AddComponent<EntryWps>(); // Attach EntryWps script
            entryObjects.Add(entry);

            // Exit waypoint
            Vector3 exitPoint = roadEnd + (Quaternion.Euler(0, -90, 0) * roadDirection * 4);
            GameObject exit = Instantiate(waypointPrefab, exitPoint, Quaternion.identity, transform);
            exit.name = $"Intersection_{intersectionID}_Exit_{i + 1}";
            exit.AddComponent<ExitWps>(); // Attach ExitWps script
            exitObjects.Add(exit);
        }
        // Only assign roads that have been instantiated
        if (position == "top-left")
        {
            road1 = new Road(entryObjects[0], exitObjects[0]);
            road2 = new Road(entryObjects[1], exitObjects[1]);
            // Instantiate red lights for road1 and road2
            Road[] roadsToLight = new Road[] { road1, road2 };

            // Now pass the array of roads to the modified method
            InstantiateRedLights(roadsToLight);
        }
        if (position == "top-edge")
        {
            road1 = new Road(entryObjects[0], exitObjects[0]);
            road2 = new Road(entryObjects[1], exitObjects[1]);
            road4 = new Road(entryObjects[2], exitObjects[2]);
            Road[] roadsToLight = new Road[] { road1, road2, road4 };

            // Now pass the array of roads to the modified method
            InstantiateRedLights(roadsToLight);


        }
        if (position == "top-right")
        {
            road1 = new Road(entryObjects[0], exitObjects[0]);
            road4 = new Road(entryObjects[1], exitObjects[1]);
            Road[] roadsToLight = new Road[] { road1, road4 };

            // Now pass the array of roads to the modified method
            InstantiateRedLights(roadsToLight);

        }
        if (position == "left-edge")
        {
            road1 = new Road(entryObjects[0], exitObjects[0]);
            road2 = new Road(entryObjects[1], exitObjects[1]);
            road3 = new Road(entryObjects[2], exitObjects[2]);
            Road[] roadsToLight = new Road[] { road1, road2, road3 };

            // Now pass the array of roads to the modified method
            InstantiateRedLights(roadsToLight);

        }
        if (position == "right-edge")
        {
            road1 = new Road(entryObjects[0], exitObjects[0]);
            road3 = new Road(entryObjects[1], exitObjects[1]);
            road4 = new Road(entryObjects[2], exitObjects[2]);
            Road[] roadsToLight = new Road[] { road1, road3, road4 };

            // Now pass the array of roads to the modified method
            InstantiateRedLights(roadsToLight);

        }
        if (position == "bottom-edge")
        {
            road2 = new Road(entryObjects[0], exitObjects[0]);
            road3 = new Road(entryObjects[1], exitObjects[1]);
            road4 = new Road(entryObjects[2], exitObjects[2]);
            Road[] roadsToLight = new Road[] { road2, road3, road4 };

            // Now pass the array of roads to the modified method
            InstantiateRedLights(roadsToLight);
        }
        if (position == "bottom-left")
        {
            road2 = new Road(entryObjects[0], exitObjects[0]);
            road3 = new Road(entryObjects[1], exitObjects[1]);
            Road[] roadsToLight = new Road[] { road2, road3 };

            // Now pass the array of roads to the modified method
            InstantiateRedLights(roadsToLight);

        }
        if (position == "bottom-right")
        {
            road3 = new Road(entryObjects[0], exitObjects[0]);
            road4 = new Road(entryObjects[1], exitObjects[1]);
            Road[] roadsToLight = new Road[] { road3, road4 };

            // Now pass the array of roads to the modified method
            InstantiateRedLights(roadsToLight);

        }
        if (position == "inside")
        {
            road1 = new Road(entryObjects[0], exitObjects[0]);
            road2 = new Road(entryObjects[1], exitObjects[1]);
            road3 = new Road(entryObjects[2], exitObjects[2]);
            road4 = new Road(entryObjects[3], exitObjects[3]);
            Road[] roadsToLight = new Road[] { road1, road2, road3, road4 };

            // Now pass the array of roads to the modified method
            InstantiateRedLights(roadsToLight);


        }
        // Debug.Log($"position: {position}");

        // Debug.Log($"road1: {road1.entryWaypoint}");
        // Debug.Log($"road2: { road2.entryWaypoint}");
        // Debug.Log($"road3: {road3.entryWaypoint}");
        // Debug.Log($"road4: {road4.entryWaypoint}");



        // Note: This approach assumes the roads are ordered and added based on their logical position.
        // You may need to adjust the logic based on the specific requirements for corners and edges.
    }

private void InstantiateRedLights(Road[] roads)
{
    if (redLightPrefab == null) return; // Safety check

    foreach (Road road in roads)
    {
        if (road == null) continue; // Skip if the road is null

        Vector3 entryPosition = road.entryWaypoint.transform.position;
        Vector3 exitPosition = road.exitWaypoint.transform.position;
        Vector3 midpoint = (entryPosition + exitPosition) / 2;

        // Calculate direction from entry to exit
        Vector3 direction = (exitPosition - entryPosition).normalized;

        // Create a rotation that looks in the direction of the road
        Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);

        // Instantiate the red light prefab at the midpoint with the calculated rotation
        GameObject redLightGO = Instantiate(redLightPrefab, midpoint, rotation, transform);
                TrafficLightController lightController = redLightGO.GetComponent<TrafficLightController>();
       
        TrafficLightManager trafficLightManager = GetComponent<TrafficLightManager>();

       if (lightController != null)
        {
            trafficLightManager.RegisterTrafficLight(lightController);
        }
        else
        {
            Debug.LogError("Instantiated Red Light does not have a TrafficLightController component.");
        }
    }
}



    void AssignConnectedWaypoints()
    {
        foreach (GameObject entry in entryObjects)
        {
            EntryWps entryScript = entry.GetComponent<EntryWps>();
            foreach (GameObject exit in exitObjects)
            {

                // Ensure the entry waypoint doesn't connect to its corresponding exit
                if (entry.name.Substring(entry.name.Length - 1) != exit.name.Substring(exit.name.Length - 1))
                {
                    entryScript.connectedExits.Add(exit.transform);
                }
            }
        }
    }

    // Optional: OnDrawGizmos for visualization
    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        for (int i = 0; i < 4; i++)
        {
            Vector3 direction = Quaternion.Euler(0, i * 90, 0) * Vector3.forward;
            Vector3 end = transform.position + (direction * roadLength);

            // Draw lines for roads
            Gizmos.DrawLine(transform.position, end);

            // Draw dots for entry and exit points
            Vector3 entryPoint = end + (Quaternion.Euler(0, 90, 0) * direction * 2);
            Gizmos.DrawSphere(entryPoint, 0.5f);

            Vector3 exitPoint = end + (Quaternion.Euler(0, -90, 0) * direction * 2);
            Gizmos.DrawSphere(exitPoint, 0.5f);
        }
    }

}

[System.Serializable] // Makes it visible in the Unity Editor, if needed
public class Road
{
    public GameObject entryWaypoint;
    public GameObject exitWaypoint;

    // Constructor to easily create a Road with entry and exit waypoints
    public Road(GameObject entry, GameObject exit)
    {
        entryWaypoint = entry;
        exitWaypoint = exit;
    }
}