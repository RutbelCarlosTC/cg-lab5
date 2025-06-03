using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    public float moveSpeed = 5.0f;
    public float mouseSensitivity = 2.0f;
    public float zoomSpeed = 2.0f;
    
    [Header("Límites de Rotación")]
    public float minVerticalAngle = -60f;
    public float maxVerticalAngle = 60f;
    
    [Header("Configuración de Zoom")]
    public float minZoom = 20f;
    public float maxZoom = 80f;
    
    private Camera playerCamera;
    private float rotationX = 0f;
    private float rotationY = 0f;
    private bool isMouseLookEnabled = true;
    
    void Start()
    {
        playerCamera = GetComponent<Camera>();
        if (playerCamera == null)
            playerCamera = Camera.main;
            
        // Configurar cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        // Inicializar rotación
        Vector3 rotation = transform.rotation.eulerAngles;
        rotationY = rotation.y;
        rotationX = rotation.x;
    }
    
    void Update()
    {
        HandleMovement();
        HandleMouseLook();
        HandleZoom();
        HandleCursorToggle();
    }
    
    void HandleMovement()
    {
        // Movimiento WASD
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        
        Vector3 direction = new Vector3(horizontal, 0, vertical);
        direction = transform.TransformDirection(direction);
        direction.y = 0; // Mantener movimiento horizontal
        
        // Movimiento vertical con Q y E
        if (Input.GetKey(KeyCode.Space))
            direction.y = 1f;
        if (Input.GetKey(KeyCode.LeftShift))
            direction.y = -1f;
            
        transform.Translate(direction * moveSpeed * Time.deltaTime, Space.World);
    }
    
    void HandleMouseLook()
    {
        if (!isMouseLookEnabled)
            return;
            
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        
        rotationY += mouseX;
        rotationX -= mouseY;
        
        rotationX = Mathf.Clamp(rotationX, minVerticalAngle, maxVerticalAngle);
        
        transform.rotation = Quaternion.Euler(rotationX, rotationY, 0);
    }
    
    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0f && playerCamera != null)
        {
            float newFOV = playerCamera.fieldOfView - scroll * zoomSpeed * 10f;
            playerCamera.fieldOfView = Mathf.Clamp(newFOV, minZoom, maxZoom);
        }
    }
    
    void HandleCursorToggle()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleCursor();
        }
    }
    
    void ToggleCursor()
    {
        isMouseLookEnabled = !isMouseLookEnabled;
        
        if (isMouseLookEnabled)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
    
    // Método para obtener la distancia a un objeto
    public float GetDistanceTo(Transform target)
    {
        if (target != null)
            return Vector3.Distance(transform.position, target.position);
        return 0f;
    }
}