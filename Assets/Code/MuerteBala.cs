using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CannonballCollision : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("¡Jugador alcanzado!");

            // Buscar el script de MovimientoPersonaje en el jugador
            MovimientoPersonaje mp = collision.gameObject.GetComponentInParent<MovimientoPersonaje>();

            if (mp != null)
            {
                mp.Muerte(); // Llama al método de muerte
            }

            Destroy(gameObject, 5f); // Destruye la bala tras impactar
        }
    }
}