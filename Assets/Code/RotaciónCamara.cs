using UnityEngine;

public class CameraRotationFEZ : MonoBehaviour
{
    public Transform cameraPivot;  // El objeto vacío que rota
    public float rotationSpeed = 300f;
    private bool isRotating = false;
    private Quaternion targetRotation;

    void Update()
    {
        if (isRotating)
        {
            cameraPivot.rotation = Quaternion.RotateTowards(cameraPivot.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            if (Quaternion.Angle(cameraPivot.rotation, targetRotation) < 0.1f)
            {
                cameraPivot.rotation = targetRotation;
                isRotating = false;
            }
            return;
        }

        if (Input.GetKeyDown(KeyCode.Q))
            RotateCamera(-90);
        if (Input.GetKeyDown(KeyCode.E))
            RotateCamera(90);
    }

    void RotateCamera(float angle)
    {
        if (isRotating) return;

        targetRotation = cameraPivot.rotation * Quaternion.Euler(0, angle, 0);
        isRotating = true;
    }
}
