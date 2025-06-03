using UnityEngine;

public class SceneGenerator : MonoBehaviour
{
    [Header("Configuración de Objetos")]
    public int numberOfObjects = 10;
    public float spawnRadius = 15f;
    public float minObjectScale = 0.5f;
    public float maxObjectScale = 2.5f;
    
    [Header("Materiales")]
    public Material[] objectMaterials;
    
    [Header("Configuración del Suelo")]
    public bool createFloor = true;
    public Material floorMaterial;
    public Vector3 floorSize = new Vector3(30, 1, 30);
    
    [Header("Iluminación")]
    public bool createLights = true;
    public Color lightColor = Color.white;
    public float lightIntensity = 1.0f;
    
    private GameObject[] spawnedObjects;
    
    void Start()
    {
        GenerateScene();
    }
    
    void GenerateScene()
    {
        CreateFloor();
        CreateEnvironmentalObjects();
        CreateLighting();
        SetupMainInteractiveObject();
    }
    
    void CreateFloor()
    {
        if (!createFloor) return;
        
        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        floor.name = "Floor";
        floor.transform.position = new Vector3(0, -floorSize.y/2, 0);
        floor.transform.localScale = floorSize;
        
        if (floorMaterial != null)
        {
            floor.GetComponent<Renderer>().material = floorMaterial;
        }
        else
        {
            // Crear material básico para el suelo
            Material floor_mat = new Material(Shader.Find("Standard"));
            floor_mat.color = new Color(0.3f, 0.5f, 0.3f);
            floor.GetComponent<Renderer>().material = floor_mat;
        }
    }
    
    void CreateEnvironmentalObjects()
    {
        spawnedObjects = new GameObject[numberOfObjects];
        
        for (int i = 0; i < numberOfObjects; i++)
        {
            GameObject obj = CreateRandomObject(i);
            PositionObject(obj);
            spawnedObjects[i] = obj;
        }
    }
    
    GameObject CreateRandomObject(int index)
    {
        // Diferentes tipos de primitivos
        PrimitiveType[] primitives = { 
            PrimitiveType.Cube, 
            PrimitiveType.Sphere, 
            PrimitiveType.Cylinder, 
            PrimitiveType.Capsule 
        };
        
        PrimitiveType selectedPrimitive = primitives[Random.Range(0, primitives.Length)];
        GameObject obj = GameObject.CreatePrimitive(selectedPrimitive);
        obj.name = $"EnvObject_{index}_{selectedPrimitive}";
        
        // Escala aleatoria
        float scale = Random.Range(minObjectScale, maxObjectScale);
        obj.transform.localScale = Vector3.one * scale;
        
        // Material aleatorio
        ApplyRandomMaterial(obj);
        
        return obj;
    }
    
    void ApplyRandomMaterial(GameObject obj)
    {
        Renderer renderer = obj.GetComponent<Renderer>();
        
        if (objectMaterials != null && objectMaterials.Length > 0)
        {
            Material selectedMaterial = objectMaterials[Random.Range(0, objectMaterials.Length)];
            renderer.material = selectedMaterial;
        }
        else
        {
            // Crear material con color aleatorio
            Material randomMat = new Material(Shader.Find("Standard"));
            randomMat.color = new Color(
                Random.Range(0.2f, 1f),
                Random.Range(0.2f, 1f),
                Random.Range(0.2f, 1f),
                1f
            );
            
            // Propiedades aleatorias del material
            //randomMat.metallic = Random.Range(0f, 0.8f);
            //randomMat.smoothness = Random.Range(0.2f, 0.9f);
            
            renderer.material = randomMat;
        }
    }
    
    void PositionObject(GameObject obj)
    {
        Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
        Vector3 position = new Vector3(randomCircle.x, 0, randomCircle.y);
        
        // Ajustar altura según el tamaño del objeto
        float objectHeight = obj.transform.localScale.y;
        position.y = objectHeight / 2f;
        
        obj.transform.position = position;
        
        // Rotación aleatoria
        obj.transform.rotation = Quaternion.Euler(
            Random.Range(0, 360),
            Random.Range(0, 360),
            Random.Range(0, 360)
        );
    }
    
    void CreateLighting()
    {
        if (!createLights) return;
        
        // Luz direccional principal (como el sol)
        GameObject mainLight = new GameObject("Main Directional Light");
        Light dirLight = mainLight.AddComponent<Light>();
        dirLight.type = LightType.Directional;
        dirLight.color = lightColor;
        dirLight.intensity = lightIntensity;
        mainLight.transform.rotation = Quaternion.Euler(45f, 45f, 0f);
        
        // Luces puntuales para iluminación ambiental
        for (int i = 0; i < 3; i++)
        {
            GameObject pointLight = new GameObject($"Point Light {i + 1}");
            Light pLight = pointLight.AddComponent<Light>();
            pLight.type = LightType.Point;
            pLight.color = Color.Lerp(lightColor, Random.ColorHSV(), 0.3f);
            pLight.intensity = lightIntensity * 0.5f;
            pLight.range = 10f;
            
            // Posicionar las luces alrededor del escenario
            float angle = (360f / 3f) * i;
            Vector3 lightPos = new Vector3(
                Mathf.Sin(angle * Mathf.Deg2Rad) * spawnRadius * 0.7f,
                5f,
                Mathf.Cos(angle * Mathf.Deg2Rad) * spawnRadius * 0.7f
            );
            pointLight.transform.position = lightPos;
        }
    }
    
    void SetupMainInteractiveObject()
    {
        // Crear el objeto principal que se puede manipular
        GameObject mainObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        mainObject.name = "Interactive Object";
        mainObject.transform.position = new Vector3(0, 2f, 5f);
        mainObject.transform.localScale = Vector3.one * 1.5f;
        
        // Material especial para el objeto interactivo
        Material interactiveMat = new Material(Shader.Find("Standard"));
        interactiveMat.color = Color.cyan;
        //interactiveMat.metallic = 0.3f;
        //interactiveMat.smoothness = 0.8f;
        interactiveMat.EnableKeyword("_EMISSION");
        interactiveMat.SetColor("_EmissionColor", Color.cyan * 0.2f);
        mainObject.GetComponent<Renderer>().material = interactiveMat;
        
        // Agregar el script de control de perspectiva
        PerspectiveController perspectiveController = mainObject.AddComponent<PerspectiveController>();
        perspectiveController.minScale = 0.3f;
        perspectiveController.maxScale = 4.0f;
        perspectiveController.scaleSpeed = 1.5f;
        perspectiveController.highlightColor = Color.yellow;
        perspectiveController.normalColor = Color.cyan;
        
        // Configurar la cámara si existe
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            perspectiveController.playerCamera = mainCamera;
            perspectiveController.minDistance = 2f;
            perspectiveController.maxDistance = 12f;
            
            // Agregar control de cámara si no existe
            if (mainCamera.GetComponent<CameraController>() == null)
            {
                mainCamera.gameObject.AddComponent<CameraController>();
            }
        }
    }
    
    // Método para regenerar la escena
    [ContextMenu("Regenerate Scene")]
    public void RegenerateScene()
    {
        // Limpiar objetos existentes
        if (spawnedObjects != null)
        {
            for (int i = 0; i < spawnedObjects.Length; i++)
            {
                if (spawnedObjects[i] != null)
                    DestroyImmediate(spawnedObjects[i]);
            }
        }
        
        // Limpiar otros objetos generados
        GameObject[] objectsToClean = GameObject.FindGameObjectsWithTag("Untagged");
        foreach (GameObject obj in objectsToClean)
        {
            if (obj.name.Contains("EnvObject_") || 
                obj.name.Contains("Light") || 
                obj.name == "Floor")
            {
                DestroyImmediate(obj);
            }
        }
        
        // Regenerar
        GenerateScene();
    }
}