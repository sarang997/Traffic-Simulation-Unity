using UnityEngine;
using System.Collections.Generic;

public class CarMovement : MonoBehaviour
{
    public float speed = 5f;
    public Transform initialTargetWaypoint;
    private Transform targetWaypoint;
    private bool isStoppedAtLight = false;

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
    }

    void Update()
    {
        if (targetWaypoint != null && !isStoppedAtLight)
        {
            MoveTowardsTarget();
        }
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
            if (trafficLightController != null && trafficLightController.currentState == TrafficLightController.LightState.Red)
            {
                isStoppedAtLight = true;
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

    void HandleGreenLight()
    {
        isStoppedAtLight = false; // Resume movement when the traffic light turns green
    }
}
