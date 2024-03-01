using UnityEngine;

public class DragAndDropInstantiate : MonoBehaviour
{
    public GameObject prefabToInstantiate;

    void OnMouseDrag()
    {
        if (prefabToInstantiate != null)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0f; // Set the z-coordinate to avoid depth issues
            Instantiate(prefabToInstantiate, mousePosition, Quaternion.identity);
        }
    }
}
