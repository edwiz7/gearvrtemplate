using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public void Pressed()
    {
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        renderer.enabled = !renderer.enabled;
    }
}
