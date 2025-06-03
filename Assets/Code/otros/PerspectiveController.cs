using UnityEngine;

public class PerspectiveController : MonoBehaviour
{
    [Header("Configuración del Objeto")]
    public Transform targetObject;
    public float minScale = 0.5f;
    public float maxScale = 3.0f;
    public float scaleSpeed = 2.0f;
    
    [Header("Configuración de Cámara")]
    public Camera playerCamera;
    public float minDistance = 2.0f;
    public float maxDistance = 10.0f;
    
    [Header("Control de Interacción")]
    public KeyCode increaseKey = KeyCode.E;
    public KeyCode decreaseKey = KeyCode.Q;
    public bool useMouseControl = true;
    public bool useCameraDistance = true;
    
    [Header("Configuración Visual")]
    public Color highlightColor = Color.yellow;
    public Color normalColor = Color.white;
    
    private Vector3 originalScale;
    private Vector3 currentTargetScale;
    private bool isInteracting = false;
    private Renderer objectRenderer;
    private Color originalColor;
    private float currentScaleFactor = 1.0f;
    
    // Variables para suavizar el escalado
    private Vector3 velocityScale = Vector3.zero;
    private float smoothTime = 0.1f;
    
    void Start()
    {
        InitializeComponents();
        SetupInitialValues();
    }
    
    void InitializeComponents()
    {
        if (targetObject == null)
            targetObject = transform;
            
        if (playerCamera == null)
            playerCamera = Camera.main;
            
        objectRenderer = targetObject.GetComponent<Renderer>();
        if (objectRenderer != null)
            originalColor = objectRenderer.material.color;
    }
    
    void SetupInitialValues()
    {
        originalScale = targetObject.localScale;
        currentTargetScale = originalScale;
        currentScaleFactor = 1.0f;
    }
    
    void Update()
    {
        HandleInput();
        UpdateScale();
        UpdateVisualFeedback();
    }
    
    void HandleInput()
    {
        bool wasInteracting = isInteracting;
        isInteracting = false;
        
        // Control por teclado
        if (Input.GetKey(increaseKey))
        {
            ModifyScale(scaleSpeed * Time.deltaTime);
            isInteracting = true;
        }
        
        if (Input.GetKey(decreaseKey))
        {
            ModifyScale(-scaleSpeed * Time.deltaTime);
            isInteracting = true;
        }
        
        // Control por mouse (scroll wheel)
        if (useMouseControl)
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(scroll) > 0.01f)
            {
                ModifyScale(scroll * scaleSpeed * 10);
                isInteracting = true;
            }
        }
        
        // Control por distancia de cámara
        if (useCameraDistance && playerCamera != null)
        {
            float distance = Vector3.Distance(playerCamera.transform.position, targetObject.position);
            float normalizedDistance = Mathf.InverseLerp(minDistance, maxDistance, distance);
            float targetScaleFactor = Mathf.Lerp(maxScale, minScale, normalizedDistance);
            
            // Solo aplicar si hay una diferencia significativa para evitar fluctuaciones
            if (Mathf.Abs(targetScaleFactor - currentScaleFactor) > 0.1f)
            {
                currentScaleFactor = targetScaleFactor;
                isInteracting = true;
            }
        }
        
        // Detectar inicio y fin de interacción
        if (isInteracting && !wasInteracting)
        {
            OnInteractionStart();
        }
        else if (!isInteracting && wasInteracting)
        {
            OnInteractionEnd();
        }
    }
    
    void ModifyScale(float delta)
    {
        currentScaleFactor = Mathf.Clamp(currentScaleFactor + delta, minScale, maxScale);
        currentTargetScale = originalScale * currentScaleFactor;
    }
    
    void UpdateScale()
    {
        // Suavizar la transición de escala
        targetObject.localScale = Vector3.SmoothDamp(
            targetObject.localScale, 
            currentTargetScale, 
            ref velocityScale, 
            smoothTime
        );
    }
    
    void UpdateVisualFeedback()
    {
        if (objectRenderer != null)
        {
            Color targetColor = isInteracting ? highlightColor : originalColor;
            objectRenderer.material.color = Color.Lerp(
                objectRenderer.material.color, 
                targetColor, 
                Time.deltaTime * 5f
            );
        }
    }
    
    void OnInteractionStart()
    {
        Debug.Log($"Iniciando interacción - Escala actual: {currentScaleFactor:F2}");
    }
    
    void OnInteractionEnd()
    {
        Debug.Log($"Finalizando interacción - Escala final: {currentScaleFactor:F2}");
        
        // Aquí es donde el usuario "suelta" el objeto y puede notar el cambio
        ShowScaleChange();
    }
    
    void ShowScaleChange()
    {
        // Efecto visual para mostrar el cambio de tamaño
        StartCoroutine(ScaleRevealEffect());
    }
    
    System.Collections.IEnumerator ScaleRevealEffect()
    {
        // Pequeño efecto de "rebote" para hacer evidente el cambio
        Vector3 targetScale = currentTargetScale;
        Vector3 bounceScale = targetScale * 1.1f;
        
        float duration = 0.3f;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            Vector3 scale;
            if (t < 0.5f)
            {
                // Expandir
                scale = Vector3.Lerp(targetScale, bounceScale, t * 2f);
            }
            else
            {
                // Contraer de vuelta
                scale = Vector3.Lerp(bounceScale, targetScale, (t - 0.5f) * 2f);
            }
            
            targetObject.localScale = scale;
            yield return null;
        }
        
        targetObject.localScale = targetScale;
    }
    
    // Métodos públicos para control externo
    public void SetScale(float newScale)
    {
        currentScaleFactor = Mathf.Clamp(newScale, minScale, maxScale);
        currentTargetScale = originalScale * currentScaleFactor;
    }
    
    public float GetCurrentScale()
    {
        return currentScaleFactor;
    }
    
    public void ResetScale()
    {
        currentScaleFactor = 1.0f;
        currentTargetScale = originalScale;
    }
    
    // Visualización en el editor
    void OnDrawGizmosSelected()
    {
        if (targetObject != null && playerCamera != null)
        {
            // Dibujar las distancias mínima y máxima
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(targetObject.position, minDistance);
            
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(targetObject.position, maxDistance);
            
            // Línea hacia la cámara
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(targetObject.position, playerCamera.transform.position);
        }
    }
}