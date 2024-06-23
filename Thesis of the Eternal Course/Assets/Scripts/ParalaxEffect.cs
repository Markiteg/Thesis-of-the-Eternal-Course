using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParalaxEffect : MonoBehaviour
{
    private Transform cameraTransform;
    private Vector3 previousCameraPosition;

    // Коэффициенты параллакса для каждого слоя
    public Vector2 parallaxMultiplier;

    void Start()
    {
        cameraTransform = Camera.main.transform;
        previousCameraPosition = cameraTransform.position;
    }

    void Update()
    {
        Vector3 deltaMovement = cameraTransform.position - previousCameraPosition;
        transform.position += new Vector3(deltaMovement.x * parallaxMultiplier.x, deltaMovement.y * parallaxMultiplier.y);
        previousCameraPosition = cameraTransform.position;
    }
}
