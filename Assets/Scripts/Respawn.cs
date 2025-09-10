using UnityEngine;

public class Respawn : MonoBehaviour
{
    public Transform spawnPoint;
    public void RespawnPlayer()
    {
        spawnPoint = GameObject.FindGameObjectWithTag("Respawn").transform;
        GameObject.FindGameObjectWithTag("Player").transform.position = spawnPoint.position;
    }
}
