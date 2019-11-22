using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Pointer : MonoBehaviour
{
    public float distance = 10f;
    public LineRenderer lineRenderer = null;
    public LayerMask everythingMask = 0;
    public LayerMask interactableMask = 0;
    public UnityAction<Vector3, GameObject> OnPointerUpdate = null;

    private Transform currentOrigin = null;
    private GameObject currentObject = null;

    private void Start()
    {
        SetLineColor();
    }

    private void Awake()
    {
        PlayerEvents.OnControllerSource += UpdateOrigin;
        PlayerEvents.OnTouchpadDown += ProcessTouchpadDown;
    }

    private void OnDestroy()
    {
        PlayerEvents.OnControllerSource -= UpdateOrigin;
        PlayerEvents.OnTouchpadDown -= ProcessTouchpadDown;
    }

    private void Update()
    {
        Vector3 hitPoint = UpdateLine();

        currentObject = UpdatePointerStatus();

        if (OnPointerUpdate != null)
            OnPointerUpdate(hitPoint, currentObject);
    }

    private Vector3 UpdateLine()
    {
        RaycastHit hit = CreateRaycast(everythingMask);

        Vector3 endPosition = currentOrigin.position + (currentOrigin.forward * distance);

        if (hit.collider != null)
            endPosition = hit.point;
        
        lineRenderer.SetPosition(0, currentOrigin.position);
        lineRenderer.SetPosition(1, endPosition);
        
        return endPosition;
    }

    private void UpdateOrigin(OVRInput.Controller controller, GameObject controllerObject)
    {
        currentOrigin = controllerObject.transform;

        lineRenderer.enabled = controller == OVRInput.Controller.Touchpad ? false : true;
    }

    private GameObject UpdatePointerStatus()
    {
        RaycastHit hit = CreateRaycast(interactableMask);

        if (hit.collider)
            return hit.collider.gameObject;

        return null;
    }

    private RaycastHit CreateRaycast(int layer)
    {
        RaycastHit hit;
        Ray ray = new Ray(currentOrigin.position, currentOrigin.forward);
        Physics.Raycast(ray, out hit, distance, layer);

        return hit;
    }

    private void SetLineColor()
    {
        if (!lineRenderer) return;

        Color endColor = Color.white;
        endColor.a = 0f;

        lineRenderer.endColor = endColor;
    }

    private void ProcessTouchpadDown()
    {
        if (!currentObject)
        return;

        Interactable interactable = currentObject.GetComponent<Interactable>();
        interactable.Pressed();
    }
}
