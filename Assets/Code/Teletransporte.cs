using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitalTrigger : MonoBehaviour
{
    private int currentAngle = 0;
    public CameraRotator cameraController;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            currentAngle += 180;
            cameraController.RotateToNextZone();
        }
        
    }
}
