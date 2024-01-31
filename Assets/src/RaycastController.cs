using UnityEngine;

public class RaycastController : MonoBehaviour
{
    public Camera playerCamera;
    public bool useCameraCenter = true; // Comută între utilizarea centrului camerei și crosshair UI
    public RectTransform crosshairRectTransform; // Doar dacă folosiți UI crosshair
    public float interactionDistance = 1000f;

    void Update()
    {
        Ray ray;

        if (useCameraCenter)
        {
            // Raycasting folosind centrul camerei
            ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        }
        else
        {
            // Raycasting folosind crosshair UI
            ray = playerCamera.ScreenPointToRay(crosshairRectTransform.position);
        }

        // Trasează ray-ul pentru debugging
        Debug.DrawRay(ray.origin, ray.direction * interactionDistance, Color.red);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            // Logică pentru când un obiect este lovit de raycast
            Debug.Log("Hit: " + hit.collider.name);
            // Adăugați aici orice alte interacțiuni dorite
        }
    }
}