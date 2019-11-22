using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reticle : MonoBehaviour
{
    public Pointer pointer;
    public SpriteRenderer circleRenderer;

    public Sprite openSprite;
    public Sprite closedSprite;

    private Camera cam = null;

    private void Awake()
    {
        pointer.OnPointerUpdate += UpdateSprite;

        cam = Camera.main;
    }

    private void Update()
    {
        transform.LookAt(cam.gameObject.transform);
    }

    private void OnDestroy()
    {
        pointer.OnPointerUpdate -= UpdateSprite;
    }

    private void UpdateSprite(Vector3 point, GameObject hitObject)
    {
        transform.position = point;

        if(hitObject)
        {
            circleRenderer.sprite = closedSprite;
        }
        else 
        {
            circleRenderer.sprite = openSprite;
        }
    }
}
