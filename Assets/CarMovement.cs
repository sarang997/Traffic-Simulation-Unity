using UnityEngine;
using System.Collections.Generic;

public class CarMovement : MonoBehaviour
{
    public float speed = 5f;
    public Transform initialTargetWaypoint;
    private Transform targetWaypoint;
    private bool isStoppedAtLight = false;
    private TrafficLightController waitingAtTrafficLight;
    private float currentSpeed; // The current dynamic speed of the car.
public float accelerationRate = 0.2f; // Rate at which the car accelerates.
private bool isStoppedForObstacle = false;
    float detectionDistance = 20f;
    float forwardOffset = -3f;
    float verticalOffset = 1f;



    private void OnEnable()
    {
        TrafficLightController.OnGreenLightChanged += HandleGreenLight; // Subscribe to the green light event
    }

    private void OnDisable()
    {
        TrafficLightController.OnGreenLightChanged -= HandleGreenLight; // Unsubscribe from the green light event
    }

    void Start()
    {
        targetWaypoint = initialTargetWaypoint;
        currentSpeed = speed; // Start moving at cruising speed initially.

    }
private bool isAccelerating = false;

void Update()
{
    // CastAndVisualizeRay();

    if (targetWaypoint != null)
    {
        if (!isStoppedAtLight && !isStoppedForObstacle && !isAccelerating)
        {
            MoveTowardsTarget();
            // Debug.Log("Moving");
        }
        else if (isStoppedAtLight)
        {
            Debug.Log("Stopping");
            StopTheCar(); // Adjust this method if needed to smoothly decelerate
        }else if (isStoppedForObstacle)
        {
            Debug.Log("Stopping behind obstacle");
            StopTheCarBehindObstactle(); // Adjust this method if needed to smoothly decelerate
        }
        else if (isAccelerating )
        {
            // Debug.Log("Accelerating");
            Accelerate();
        }
    }
}
void CastAndVisualizeRay()
{
    Vector3 rayOrigin = transform.position + Vector3.up * verticalOffset + transform.forward * forwardOffset;
    float sphereRadius = 0.5f; // Adjust the radius based on the size of your vehicles and desired detection width
    Vector3 rayDirection = transform.forward; // The direction of the SphereCast, typically forward from the car

    RaycastHit hit;
    // Perform the SphereCast
    bool isHit = Physics.SphereCast(rayOrigin, sphereRadius, rayDirection, out hit, detectionDistance);
    if (isHit && hit.collider.CompareTag("Car"))
    {
        // If the SphereCast hits a car, draw a red line from the origin to the hit point and a sphere at the hit point
        Debug.DrawRay(rayOrigin, rayDirection * hit.distance, Color.red);
        // Also, visualize the sphere at the point of contact
        Debug.DrawLine(rayOrigin, hit.point, Color.red); // Optional: Draw a line to the exact hit point
        Gizmos.color = Color.red; // Set Gizmos color to red to draw the sphere
        Gizmos.DrawWireSphere(rayOrigin + rayDirection * hit.distance, sphereRadius); // Draw the sphere at the point of hit
        Debug.Log("SphereCast hit a car: " + hit.collider.gameObject.name);
    }
    else
    {
        // If no car is hit, draw a green line to indicate the SphereCast's path
        Debug.DrawRay(rayOrigin, rayDirection * detectionDistance, Color.green);
    }
}

void OnDrawGizmos()
{
    // You might want to visualize the SphereCast's path even when not hitting anything
    // This code should be outside of the if-statement if you always want to see the sphere in the scene
    Gizmos.color = Color.green; // Set Gizmos color to green to draw the sphere path
    Vector3 rayOrigin = transform.position + Vector3.up * verticalOffset + transform.forward * forwardOffset;
    Vector3 rayDirection = transform.forward; // Assuming the forward direction for visualization
    Gizmos.DrawWireSphere(rayOrigin + rayDirection * detectionDistance, 0.5f); // Adjust the radius as needed
}




void Accelerate()
{
    currentSpeed += accelerationRate * Time.deltaTime;
    currentSpeed = Mathf.Min(currentSpeed, speed); // Ensure we don't exceed the max speed.
    
    if (currentSpeed >= speed)
    {
        isAccelerating = false; // Stop accelerating once we reach cruising speed.
    }
    
    // Continue moving towards the target while accelerating.
    MoveTowardsTarget();
}
void StopTheCarBehindObstactle()
{
    float decelarationRateBehindObstacle = 30f;
    // Decelerate the car smoothly
    if (currentSpeed > 0)
    {
        currentSpeed -= decelarationRateBehindObstacle * Time.deltaTime;
    }
    else
    {
        currentSpeed = 0; // Ensure the car doesn't go backward
    }

    // // Even when stopping, we want the car to face the direction of the target waypoint
    // if (targetWaypoint != null)
    // {
    //     // Calculate direction to the target waypoint
    //     Vector3 targetDirection = targetWaypoint.position - transform.position;
    //     targetDirection.y = 0; // Ensure rotation only on the y-axis
        
    //     // Rotate towards the target waypoint
    //     if (targetDirection != Vector3.zero)
    //     {
    //         Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
    //         transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
    //     }
    // }

    // Apply the deceleration to actually stop the car
    transform.position += transform.forward * currentSpeed * Time.deltaTime;
}
void StopTheCar()
{
    float decelerationRate = 5f; // Rate at which the car decelerates.

    // Decelerate the car smoothly
    if (currentSpeed > 0)
    {
        currentSpeed -= decelerationRate * Time.deltaTime;
    }
    else
    {
        currentSpeed = 0; // Ensure the car doesn't go backward
    }

    // // Even when stopping, we want the car to face the direction of the target waypoint
    // if (targetWaypoint != null)
    // {
    //     // Calculate direction to the target waypoint
    //     Vector3 targetDirection = targetWaypoint.position - transform.position;
    //     targetDirection.y = 0; // Ensure rotation only on the y-axis
        
    //     // Rotate towards the target waypoint
    //     if (targetDirection != Vector3.zero)
    //     {
    //         Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
    //         transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
    //     }
    // }

    // Apply the deceleration to actually stop the car
    transform.position += transform.forward * currentSpeed * Time.deltaTime;
}

void MoveTowardsTarget()
{
    if (targetWaypoint == null) return;

    // Move the car towards the target waypoint
    transform.position = Vector3.MoveTowards(transform.position, targetWaypoint.position, currentSpeed * Time.deltaTime);

    // Determine the direction to the next waypoint
    Vector3 targetDirection = targetWaypoint.position - transform.position;
    targetDirection.y = 0; // Ensure rotation only on the y-axis

    // Check if the car is very close to the waypoint
    if (Vector3.Distance(transform.position, targetWaypoint.position) < 0.1f)
    {
        ChooseNextTarget(); // Update target waypoint to the next one

        // Immediately adjust orientation towards the new target waypoint, if available
        if (targetWaypoint != null)
        {
            targetDirection = targetWaypoint.position - transform.position;
            targetDirection.y = 0; // Ensure rotation only on the y-axis
            transform.rotation = Quaternion.LookRotation(targetDirection);
        }
    }
    else if (targetDirection != Vector3.zero)
    {
        // Smoothly turn towards the target waypoint if not yet close
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
    }
}




    void ChooseNextTarget()
    {
        EntryWps entryWps = targetWaypoint.GetComponent<EntryWps>();
        if (entryWps != null)
        {
            ChooseExitRandomly(entryWps);
        }
        else
        {
            ExitWps exitWps = targetWaypoint.GetComponent<ExitWps>();
            if (exitWps != null)
            {
                ChooseNextEntryRandomly(exitWps);
            }
        }
        isStoppedAtLight = false; // Reset the flag upon reaching a waypoint
    }

    void OnTriggerEnter(Collider other)
    {
 if (other.CompareTag("Car")) // Make sure the other object has a Car tag
    {
            Debug.Log("Another car is too close in front! Decelerating...");

        Vector3 directionToOther = other.transform.position - transform.position;
        float angleBetween = Vector3.Angle(transform.forward, directionToOther);

        // Check if the other car is within a certain angle in front of this car
        if (angleBetween < 40) // Adjust this angle as needed
        {
            isStoppedForObstacle = true;
        }
    }

        if (other.CompareTag("TrafficLight"))
        {   
            Debug.Log("Traffic light");
            TrafficLightController trafficLightController = other.GetComponentInParent<TrafficLightController>();
            if (trafficLightController != null && trafficLightController.currentState == TrafficLightController.LightState.Red &&  targetWaypoint.GetComponent<EntryWps>() !=null)
            {
                isStoppedAtLight = true;

                waitingAtTrafficLight = trafficLightController; // Remember this traffic light

            }
        }
    }
    void OnTriggerExit(Collider other)
{
    if (other.CompareTag("Car")) // Make sure the other object has a Car tag
    {
        Vector3 directionToOther = other.transform.position - transform.position;
        float angleBetween = Vector3.Angle(transform.forward, directionToOther);
        // Check if the other car moves out of the specified angle in front of this car
        if (angleBetween < 90) // Adjust this angle as needed
        {
            Debug.Log("Obstacle has moved out of range. Resuming movement...");
            isStoppedForObstacle = false;
        isAccelerating = true;
            // Accelerate();

        }
    }
}

    // Method adjustments for new waypoint classes
    void ChooseExitRandomly(EntryWps entryWps)
    {
        int index = Random.Range(0, entryWps.connectedExits.Count);
        targetWaypoint = entryWps.connectedExits[index];
    }

    void ChooseNextEntryRandomly(ExitWps exitWps)
    {
        int index = Random.Range(0, exitWps.connectedWaypoints.Count);
        targetWaypoint = exitWps.connectedWaypoints[index];
    }

void HandleGreenLight(TrafficLightController changedLight)
{
    if (changedLight == waitingAtTrafficLight)
    {
        isStoppedAtLight = false;
        if (!isStoppedForObstacle) // Only start accelerating if there's no obstacle
        {
            isAccelerating = true;
        }
        waitingAtTrafficLight = null;
    }
}


}
