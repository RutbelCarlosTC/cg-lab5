using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovimientoPersonaje : MonoBehaviour
{
    public float velocidad = 5f;
    private Rigidbody2D rb;
    public Animator animator;
    private float direccion = 1f;
    private float escalaBase = 1f;
    public float longitud = 0.5f;
    public bool enSuelo;
    public bool muerte = false;
    public LayerMask capaSuelo;
    // Start is called before the first frame update

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!muerte)
        {
            float velocidadX = Input.GetAxis("Horizontal") * Time.deltaTime * velocidad;

            animator.SetFloat("Movimiento", velocidadX * velocidad);

            if (velocidadX < 0)
            {
                direccion = -1f;
            }
            else if (velocidadX > 0)
            {
                direccion = 1f;
            }
            transform.localScale = new Vector3(direccion * escalaBase, escalaBase, 1f);

            Vector3 posicion = transform.position;

            transform.position = new Vector3(velocidadX + posicion.x, posicion.y, posicion.z);
        }
        float escalaRaycast = escalaBase;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, longitud * escalaRaycast, capaSuelo);
        enSuelo = hit.collider != null;

        if (enSuelo && Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(new Vector2(0f, 10f), ForceMode2D.Impulse);
        }

        animator.SetBool("Suelo", enSuelo);

    }
    public void Muerte()
    {        
        muerte = true;
        animator.SetBool("Daño", muerte);

    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * longitud);
    }
}
