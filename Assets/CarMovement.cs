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

    void Update()
    {
        if (targetWaypoint != null && !isStoppedAtLight)
        {
            MoveTowardsTarget();
            Debug.Log("moving");

        }else{
            Debug.Log("stopping");
            StopTheCar();
        }
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
    // Resume movement only if the green signal comes from the traffic light we're waiting at
    if (isStoppedAtLight && changedLight == waitingAtTrafficLight)
    {
        isStoppedAtLight = false;
        waitingAtTrafficLight = null; // Clear the reference since we're no longer waiting
        currentSpeed = speed;
    }
}

}
