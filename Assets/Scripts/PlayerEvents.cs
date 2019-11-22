using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerEvents : MonoBehaviour
{
    public static UnityAction OnTouchpadUp = null;
    public static UnityAction OnTouchpadDown = null;
    public static UnityAction<OVRInput.Controller, GameObject> OnControllerSource = null;


    public GameObject rightAnchor;
    public GameObject leftAnchor;
    public GameObject headAnchor;

    private Dictionary<OVRInput.Controller, GameObject> controllerSets = null;
    private OVRInput.Controller inputSource = OVRInput.Controller.None;
    private OVRInput.Controller controller = OVRInput.Controller.None;
    private bool inputActive = true;

    private void Awake()
    {
        OVRManager.HMDMounted += PlayerFound;
        OVRManager.HMDUnmounted += PlayerLost;

        controllerSets = CreateControllerSets();
    }

    private void OnDestroy()
    {
        OVRManager.HMDMounted -= PlayerFound;
        OVRManager.HMDUnmounted -= PlayerLost;
    }

    private void Update()
    {
        // Check active input
        if (!inputActive) return;

        // Check if a controller exists
        CheckForController();

        // Check input source
        CheckInputSource();

        // Check for actual input
        Input();
    }

    private void CheckForController()
    {
        OVRInput.Controller controllerCheck = controller;

        // Right remote
        if (OVRInput.IsControllerConnected(OVRInput.Controller.RTrackedRemote))
            controllerCheck = OVRInput.Controller.RTrackedRemote;

        // Left remote
        if (OVRInput.IsControllerConnected(OVRInput.Controller.LTrackedRemote))
            controllerCheck = OVRInput.Controller.LTrackedRemote;

        // Headset
        if (!OVRInput.IsControllerConnected(OVRInput.Controller.LTrackedRemote) && !OVRInput.IsControllerConnected(OVRInput.Controller.RTrackedRemote))
            controllerCheck = OVRInput.Controller.Touchpad;

        controller = UpdateSource(controllerCheck, controller);
    }

    private void CheckInputSource()
    {
        inputSource = UpdateSource(OVRInput.GetActiveController(), inputSource);
    }

    private void Input()
    {
        // Touchpad down
        if (OVRInput.GetDown(OVRInput.Button.PrimaryTouchpad))
        {
            if (OnTouchpadDown != null)
                OnTouchpadDown();
        }
        // Touchpad up
        if (OVRInput.GetUp(OVRInput.Button.PrimaryTouchpad))
        {
            if (OnTouchpadUp != null)
                OnTouchpadUp();
        }
    }

    private OVRInput.Controller UpdateSource(OVRInput.Controller check, OVRInput.Controller previous)
    {
        if (check == previous)
            return previous;

        GameObject controllerObject = null;
        controllerSets.TryGetValue(check, out controllerObject);

        if (controllerObject == null)
            controllerObject = headAnchor;

        if (OnControllerSource != null)
            OnControllerSource(check, controllerObject);

        return check;
    }

    private void PlayerFound()
    {
        inputActive = true;
    }
    private void PlayerLost()
    {
        inputActive = false;
    }

    private Dictionary<OVRInput.Controller, GameObject> CreateControllerSets()
    {
        var newSets = new Dictionary<OVRInput.Controller, GameObject>() {
            {OVRInput.Controller.LTrackedRemote, leftAnchor},
            {OVRInput.Controller.RTrackedRemote, rightAnchor},
            {OVRInput.Controller.Touchpad, headAnchor}
        };
        return newSets;
    }
}
