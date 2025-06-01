using UnityEngine;

public class CannonShooter2D : MonoBehaviour
{
    public GameObject cannonballPrefab;  // Prefab con Rigidbody2D y apariencia 3D
    public Transform shootPoint;
    public Transform player;  // Arrástralo desde la jerarquía
    public float shootForce = 700f;

    void Start()
    {
        InvokeRepeating(nameof(Shoot), 2f, 5f);
    }

    void Shoot()
    {
        GameObject cannonball = Instantiate(cannonballPrefab, shootPoint.position, Quaternion.identity);
        Rigidbody rb = cannonball.GetComponent<Rigidbody>();
        Vector3 direction = (player.position - shootPoint.position).normalized;
        rb.AddForce(direction * shootForce);

        Destroy(cannonball, 10f);
    }
}
