
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public int maxEnemyCount;
    public int currentEnemyCount;
    private int currentSplice = 0;
    private int currentEnemyIndex = 0;

    void Update()
    {
        currentEnemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;

    }

    void FixedUpdate()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        if (enemies.Length > 0)
        {
            if (currentEnemyIndex >= enemies.Length)
            {
                currentEnemyIndex = 0;
            }
            enemies[currentEnemyIndex].GetComponent<Enemy>().CheckOnPaint();


            currentEnemyIndex++;
        }
    }

}
