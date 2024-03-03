using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrafficLightManager : MonoBehaviour
{
    [SerializeField]
    private List<TrafficLightController> trafficLights;

    public float greenLightDuration = 10.0f; // Duration for a light to stay green
    public float allRedDuration = 2.0f; // Short pause when all lights are red between changes

    private void Start()
    {
        StartCoroutine(ControlTrafficLights());
    }
       public void RegisterTrafficLight(TrafficLightController lightController)
    {
        // Check if the lightController is not already in the list to avoid duplicates
        if (!trafficLights.Contains(lightController))
        {
            trafficLights.Add(lightController);
        }
    }
    private IEnumerator ControlTrafficLights()
    {
        while (true)
        {
            foreach (TrafficLightController light in trafficLights)
            {
                // Set all lights to red initially
                light.currentState = TrafficLightController.LightState.Red;
            }
            yield return new WaitForSeconds(allRedDuration); // Pause with all lights red

            foreach (TrafficLightController light in trafficLights)
            {
                // Turn one light green at a time
                light.currentState = TrafficLightController.LightState.Green;
                // Debug.Log($"Turning {light.gameObject.name} green.");
                yield return new WaitForSeconds(greenLightDuration);

                // After green phase, ensure this light turns back to red
                light.currentState = TrafficLightController.LightState.Red;
                // Debug.Log($"Turning {light.gameObject.name} back to red.");
            }
        }
    }
}
