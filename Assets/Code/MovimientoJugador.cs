using UnityEngine;

public class FezPlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Transform cameraPivot;
    private Vector3 moveDirection;

    void Start()
    {
        cameraPivot = GameObject.Find("CameraPivot").transform;
    }

    void Update()
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Vector3 camForward = cameraPivot.forward;
        Vector3 camRight = cameraPivot.right;

        // Proyectamos en plano XZ
        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        moveDirection = (camRight * input.x + camForward * input.y).normalized;
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);
    }
}