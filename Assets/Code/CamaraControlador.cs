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
        Vector3 PosiciónDeseada = objetivo.position + desplazamiento;

        Vector3 PosiciónSuavizada = Vector3.Lerp(transform.position, PosiciónDeseada, velocidadCamara);

        transform.position = PosiciónSuavizada;
    }
}
