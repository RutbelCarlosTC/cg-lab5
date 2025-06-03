using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragAndDrop : MonoBehaviour
{
    [Header("Configuración de Perspectiva")]
    public float perspectiveStrength = 1.8f; // Intensidad del efecto de perspectiva
    
    private bool _mouseState;
    private GameObject target;
    public Vector3 screenSpace;
    public Vector3 offset;
    private float originalZPosition; // Posición Z original del objeto
    private float zOffset = 0f; // Desplazamiento actual en Z

    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hitInfo;
            target = GetClickedObject(out hitInfo);
            if (target != null)
            {
                _mouseState = true;
                
                // Guardar posición Z original
                originalZPosition = target.transform.position.z;
                zOffset = 0f;
                
                screenSpace = Camera.main.WorldToScreenPoint(target.transform.position);
                offset = target.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenSpace.z));
            }
        }
        
        if (Input.GetMouseButtonUp(0))
        {
            _mouseState = false;
        }
        
        if (_mouseState && target != null)
        {
            // Controlar perspectiva con rueda del mouse
            HandlePerspectiveInput();
            
            // Mover objeto
            var curScreenSpace = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenSpace.z);
            var curPosition = Camera.main.ScreenToWorldPoint(curScreenSpace) + offset;
            
            // Mantener el desplazamiento Z de perspectiva
            curPosition.z = originalZPosition + zOffset;
            
            target.transform.position = curPosition;
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
            
            Vector3 pos = target.transform.position;
            pos.z = originalZPosition + zOffset;
            target.transform.position = pos;
        }
    }

    GameObject GetClickedObject(out RaycastHit hit)
    {
        GameObject target = null;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray.origin, ray.direction * 10, out hit))
        {
            target = hit.collider.gameObject;
        }

        return target;
    }
}