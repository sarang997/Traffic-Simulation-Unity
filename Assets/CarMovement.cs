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
        public float decelerationRate = 1f; // Rate at which the car decelerates.
public float accelerationRate = 0.2f; // Rate at which the car accelerates.
private bool isStoppedForObstacle = false;

    public float detectionDistance = 1.0f; // Distance ahead of the car to detect obstacles


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
    CastAndVisualizeRay();

    if (targetWaypoint != null)
    {
        if (!isStoppedAtLight && !isStoppedForObstacle && !isAccelerating)
        {
            MoveTowardsTarget();
            Debug.Log("Moving");
        }
        else if (isStoppedAtLight || isStoppedForObstacle)
        {
            Debug.Log("Stopping");
            StopTheCar(); // Adjust this method if needed to smoothly decelerate
        }
        else if (isAccelerating)
        {
            Debug.Log("Accelerating");
            Accelerate();
        }
    }
}
void CastAndVisualizeRay()
{
    // Define a vertical offset
    float verticalOffset = 2.0f; // Adjust this value as needed
    float forwardOffset = 3f; // How far forward from the center to start the ray

    // Adjust the ray's origin vertically and forward
    Vector3 rayOrigin = transform.position + Vector3.up * verticalOffset + transform.forward * forwardOffset;
    Vector3 rayDirection = transform.forward;


    // Cast the ray
    RaycastHit hit;
    bool isHit = Physics.Raycast(rayOrigin, rayDirection, out hit, detectionDistance);

    // Visualization and detection logic remains the same
    if (isHit && hit.collider.CompareTag("Car"))
    {
        // Draw a red line if the ray hits an object tagged as "Car"
        Debug.DrawRay(rayOrigin, rayDirection * detectionDistance, Color.red);
        Debug.Log("Ray hit a car: " + hit.collider.gameObject.name);
    isStoppedForObstacle = true; // Use the new flag here
    }
    else
    {
        // Draw a green line if the ray doesn't hit a "Car" tagged object
        Debug.DrawRay(rayOrigin, rayDirection * detectionDistance, Color.green);
            isStoppedForObstacle = false; // Use the new flag here

    }
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

void StopTheCar()
{
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
        transform.position = Vector3.MoveTowards(transform.position, targetWaypoint.position, speed * Time.deltaTime);

        Vector3 targetDirection = targetWaypoint.position - transform.position;
        targetDirection.y = 0; // Ensure rotation only on the y-axis
        if (targetDirection != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(targetDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }

        if (Vector3.Distance(transform.position, targetWaypoint.position) < 0.1f)
        {
            ChooseNextTarget();
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
        if (other.CompareTag("TrafficLight"))
        {
            TrafficLightController trafficLightController = other.GetComponentInParent<TrafficLightController>();
            if (trafficLightController != null && trafficLightController.currentState == TrafficLightController.LightState.Red &&  targetWaypoint.GetComponent<EntryWps>() !=null)
            {
                isStoppedAtLight = true;
                waitingAtTrafficLight = trafficLightController; // Remember this traffic light

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
