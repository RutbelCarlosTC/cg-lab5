using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamaraControlador : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform objetivo;
    public float velocidadCamara = 0.025f;
    public Vector3 desplazamiento;

    // Update is called once per frame
    public void LateUpdate()
    {
        Vector3 PosicionDeseada = objetivo.position + desplazamiento;

        Vector3 PosicionSuavizada = Vector3.Lerp(transform.position, PosicionDeseada, velocidadCamara);

        transform.position = PosicionSuavizada;
    }
}
