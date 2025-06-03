using UnityEngine;

public class CajaTrigger : MonoBehaviour
{
    private bool jugadorCerca = false;
    private CajaEscalable cajaEscalable;

    void Start()
    {
        cajaEscalable = GetComponent<CajaEscalable>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            jugadorCerca = true;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            jugadorCerca = false;
        }
    }

    void Update()
    {
        if (jugadorCerca && Input.GetKeyDown(KeyCode.E))
        {
            if (cajaEscalable != null)
            {
                cajaEscalable.enabled = true;
                cajaEscalable.IniciarManipulacion();
            }
        }
    }
}
