using System.Collections;
using UnityEngine;

public class CameraRotator : MonoBehaviour
{
    public Transform cameraPivot;      // El objeto padre de la cámara (CameraPivot)
    public Transform player;           // Referencia al jugador
    public Transform[] spawnPoints;    // 0 = Area_0, 1 = Area_90, etc.
    public GameObject[] areas;         // Tilemaps de cada zona
    public float rotationDuration = 0.3f;

    private int currentZone = 0;
    public int currentAngle = 0;
    private bool rotating = false;

    void Start()
    {
        // Inicializar
        player.position = spawnPoints[currentZone].position;

        for (int i = 0; i < areas.Length; i++)
            areas[i].SetActive(i == currentZone);

        cameraPivot.rotation = Quaternion.Euler(0, currentZone * 180, 0);
    }
    public void RotateToNextZone()
    {
        if (!rotating)
        {
            int newZone = 1 - currentZone; // Alternar entre 0 y 1
            int targetAngle = newZone * 180; // 0 o 180
            StartCoroutine(RotateAndTeleport(targetAngle, newZone));
        }
    }

    IEnumerator RotateAndTeleport(int angle, int zoneIndex)
    {
        rotating = true;

        Quaternion startRot = cameraPivot.rotation;
        Quaternion endRot = Quaternion.Euler(0, angle, 0);

        float elapsed = 0f;
        while (elapsed < rotationDuration)
        {
            cameraPivot.rotation = Quaternion.Slerp(startRot, endRot, elapsed / rotationDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        cameraPivot.rotation = endRot;

        // Teletransportar jugador
        player.position = spawnPoints[zoneIndex].position;

        // Activar solo la zona correspondiente
        for (int i = 0; i < areas.Length; i++)
        {
            areas[i].SetActive(i == zoneIndex);
        }

        currentZone = zoneIndex;
        rotating = false;
    }
}
