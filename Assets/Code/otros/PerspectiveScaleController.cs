using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerspectiveScaleController : MonoBehaviour
{
    [Header("Configuración")]
    public float perspectiveStrength = 2f; // Intensidad del efecto de perspectiva
    public float dragSmoothness = 10f;
    public LayerMask interactableLayer = -1;
    
    // Variables privadas
    private bool isDragging = false;
    private GameObject targetObject;
    private Vector3 screenSpace;
    private Vector3 offset;
    private Camera mainCamera;
    private float originalZPosition;
    private float zOffset = 0f; // Desplazamiento actual en Z
    
    void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
            mainCamera = FindObjectOfType<Camera>();
    }
    
    void Update()
    {
        HandleInput();
        UpdateDragging();
    }
    
    void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartDragging();
        }
        
        if (Input.GetMouseButtonUp(0))
        {
            StopDragging();
        }
        
        // Controlar perspectiva mientras se arrastra
        if (isDragging && targetObject != null)
        {
            HandlePerspectiveInput();
        }
    }
    
    void StartDragging()
    {
        RaycastHit hitInfo;
        GameObject clickedObject = GetClickedObject(out hitInfo);
        
        if (clickedObject != null)
        {
            targetObject = clickedObject;
            isDragging = true;
            zOffset = 0f;
            
            // Guardar posición Z original
            originalZPosition = targetObject.transform.position.z;
            
            // Calcular offset para el arrastre
            screenSpace = mainCamera.WorldToScreenPoint(targetObject.transform.position);
            offset = targetObject.transform.position - 
                    mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenSpace.z));
            
            Debug.Log($"Arrastrando: {targetObject.name}");
        }
    }
    
    void StopDragging()
    {
        if (isDragging)
        {
            isDragging = false;
            Debug.Log($"Objeto soltado - Z final: {targetObject.transform.position.z:F2}");
            targetObject = null;
        }
    }
    
    void HandlePerspectiveInput()
    {
        float perspectiveChange = 0f;
        
        // Rueda del mouse
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            perspectiveChange = scroll * perspectiveStrength;
        }
        
        // Teclas alternativas
        if (Input.GetKey(KeyCode.Q))
            perspectiveChange = -Time.deltaTime * perspectiveStrength;
        if (Input.GetKey(KeyCode.E))
            perspectiveChange = Time.deltaTime * perspectiveStrength;
        
        // Aplicar cambio en Z
        if (perspectiveChange != 0f)
        {
            zOffset += perspectiveChange;
            
            Vector3 pos = targetObject.transform.position;
            pos.z = originalZPosition + zOffset;
            targetObject.transform.position = pos;
            
            string direction = perspectiveChange > 0 ? "ACERCANDO" : "ALEJANDO";
            Debug.Log($"{direction} - Z: {pos.z:F2}");
        }
    }
    
    void UpdateDragging()
    {
        if (isDragging && targetObject != null)
        {
            // Calcular nueva posición basada en el mouse
            Vector3 curScreenSpace = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenSpace.z);
            Vector3 curPosition = mainCamera.ScreenToWorldPoint(curScreenSpace) + offset;
            
            // Mantener el desplazamiento Z de perspectiva
            curPosition.z = originalZPosition + zOffset;
            
            // Aplicar movimiento suave
            targetObject.transform.position = Vector3.Lerp(
                targetObject.transform.position, 
                curPosition, 
                Time.deltaTime * dragSmoothness
            );
        }
    }
    
    GameObject GetClickedObject(out RaycastHit hit)
    {
        GameObject target = null;
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, interactableLayer))
        {
            target = hit.collider.gameObject;
        }
        
        return target;
    }
    
    void OnGUI()
    {
        GUI.Box(new Rect(10, 10, 280, 100), "Controles de Perspectiva:");
        GUI.Label(new Rect(20, 35, 260, 20), "• Click y arrastra para mover objetos");
        GUI.Label(new Rect(20, 55, 260, 20), "• Rueda ADELANTE: mas grande");
        GUI.Label(new Rect(20, 75, 260, 20), "• Rueda ATRÁS: más pequeño");
    }
}