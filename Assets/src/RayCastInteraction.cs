using UnityEngine;

public class CameraRaycaster : MonoBehaviour
{
    public Camera playerCamera;
    public float interactionDistance = 2f;
    public float interactionAngle = 30f; // Unghiul maxim de interacțiune
    public Transform handTransform; // Transform pentru atașarea obiectului

    private GameObject currentTarget;

    void Update()
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        float rayLength = 1000f; // De exemplu, lungimea ray-ului
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionDistance))
        {

            Debug.DrawRay(ray.origin, ray.direction * rayLength, Color.green);



            GameObject hitObject = hit.collider.gameObject;
            Vector3 directionToObject = (hitObject.transform.position - playerCamera.transform.position).normalized;

            // Verificăm unghiul
            if (Vector3.Angle(playerCamera.transform.forward, directionToObject) <= interactionAngle)
            {
                if (hitObject != currentTarget)
                {
                    ResetHighlight();
                    currentTarget = hitObject;
                    HighlightObject(currentTarget, true);
                }

                if (Input.GetKeyDown(KeyCode.E))
                {
                    PickupObject(currentTarget);
                }
            }
            else
            {
                ResetHighlight();
                currentTarget = null;
            }
        }
        else
        {
            ResetHighlight();
            currentTarget = null;
        }
    }

    void HighlightObject(GameObject obj, bool highlight)
    {
        if (obj == null) return;
        var renderer = obj.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = highlight ? Color.yellow : Color.white;
        }
    }

    void ResetHighlight()
    {
        if (currentTarget != null)
        {
            HighlightObject(currentTarget, false);
        }
    }

    void PickupObject(GameObject obj)
    {
        if (obj == null) return;
        obj.transform.SetParent(handTransform);
        obj.transform.localPosition = Vector3.zero;
        // Opțional: Dezactivează fizica obiectului
    }
}