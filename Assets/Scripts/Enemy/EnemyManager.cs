using System.Linq;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public int maxEnemyCount;
    public int currentEnemyCount;

    void Update()
    {
        currentEnemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;
    }
}
