using UnityEngine;

public class GizmosTest : MonoBehaviour
{
    void OnDrawGizmos()
    {
        // Set the color of the Gizmos
        Gizmos.color = Color.red;

        // Draw a wire sphere at the GameObject's position with a radius of 1 unit
        Gizmos.DrawWireSphere(transform.position, 1f);
    }
}
