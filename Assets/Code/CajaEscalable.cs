using UnityEngine;

public class CajaEscalable : MonoBehaviour
{
    public float escalaZ = 0f; // valor en eje Z simulado
    public float minZ = -2f;
    public float maxZ = 2f;
    public float velocidadMovimiento = 3f;

    private bool siendoManipulada = false;
    private MovimientoPersonaje movimientoJugador;
    private Transform jugadorTransform;

    private Vector3 escalaOriginal; // <-- esta línea es la que faltaba


    void Start()
    {
        GameObject jugador = GameObject.FindGameObjectWithTag("Player");
        if (jugador != null)
        {
            jugadorTransform = jugador.transform;
            movimientoJugador = jugador.GetComponent<MovimientoPersonaje>();
        }
        escalaOriginal = transform.localScale;
    }

    void Update()
    {
        if (!siendoManipulada) return;

        // Movimiento con teclas
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 direccion = new Vector3(h, v, 0).normalized;
        transform.position += direccion * velocidadMovimiento * Time.deltaTime;

        // Scroll ajusta solo la posición Z (profundidad, para simular perspectiva)
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            escalaZ = Mathf.Clamp(escalaZ + scroll * 5f, minZ, maxZ);
            Vector3 pos = transform.position;
            transform.position = new Vector3(pos.x, pos.y, escalaZ);
        }

        // Salir del modo manipulación
        if (Input.GetKeyDown(KeyCode.E))
        {
            FinalizarManipulacion();
        }
    }

    public void IniciarManipulacion()
    {
        escalaOriginal = transform.localScale;
        siendoManipulada = true;
        if (movimientoJugador != null)
            movimientoJugador.enabled = false;
    }

    private void FinalizarManipulacion()
    {
        siendoManipulada = false;

        // Guardar z para cálculo
        float zFactor = 1f - (escalaZ / 8f);  // Puedes ajustar el divisor para sensibilidad

        // Clamp por seguridad (para evitar invertir escala o exagerar)
        zFactor = Mathf.Clamp(zFactor, 0.5f, 1.5f);

        // Aplicar nueva escala basada en escalaOriginal y zFactor
        transform.localScale = escalaOriginal * zFactor;

        // Volver a z = 0 (para colisiones y lógica)
        transform.position = new Vector3(transform.position.x, transform.position.y, 0f);

        if (movimientoJugador != null)
            movimientoJugador.enabled = true;

        enabled = false;
    }
}
