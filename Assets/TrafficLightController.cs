using UnityEngine;
using System.Collections;
using System; // Ensure this using directive is present


public class TrafficLightController : MonoBehaviour
{
    public enum LightState { Red, Green }
    private LightState _currentState = LightState.Red; // Default state to red
public static event Action<TrafficLightController> OnGreenLightChanged;

    public LightState currentState
    {
        get => _currentState;
        set
        {
            _currentState = value;
            UpdateLightColor();
            if (_currentState == LightState.Green)
            {
                OnGreenLightChanged?.Invoke(this); // Raise the event when the light turns green
            }
        }
    }

    public Material redLightMaterial;
    public Material greenLightMaterial;
    private Renderer lightRenderer;

    public float redDuration = 10.0f; // Duration the light stays red
    public float greenDuration = 10.0f; // Duration the light stays green

    private void Awake()
    {
        lightRenderer = GetComponentInChildren<Renderer>();
    }

    void Start()
    {
        currentState = LightState.Red; // Initialize state to red
        // StartCoroutine(TrafficLightCycle());
    }

    void UpdateLightColor()
    {
        // Update the material of the light based on the current state
        lightRenderer.material = currentState == LightState.Red ? redLightMaterial : greenLightMaterial;
    }

    IEnumerator TrafficLightCycle()
    {
        while (true) // Infinite loop to continuously cycle the traffic light
        {
            // Change the state from red to green and vice versa, then wait for the specified duration
            currentState = LightState.Red;
            UpdateLightColor();
            yield return new WaitForSeconds(redDuration); // Wait for redDuration seconds

            currentState = LightState.Green;
            UpdateLightColor();
            yield return new WaitForSeconds(greenDuration); // Wait for greenDuration seconds
        }
    }

    private void OnValidate()
    {
        // This method is called when a value is changed in the inspector
        // Ensure renderer is found in case it's null (helpful for initial setup and updates in Editor)
        if (!lightRenderer)
        {
            lightRenderer = GetComponentInChildren<Renderer>();
        }
        UpdateLightColor(); // Update the color in the Editor for immediate feedback
    }
}
