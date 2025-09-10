
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
        

        for (int i = 0; i < enemies.Length; i++)
        {
            if (i % 5 == currentSplice)
            {
                enemies[i].GetComponent<Enemy>().CheckOnPaint();
            }
        }
        currentSplice = (currentSplice + 1) % 5;
       /*
        if (enemies.Length > 0)
        {
            if (currentEnemyIndex >= enemies.Length)
            {
                currentEnemyIndex = 0;
            }
            enemies[currentEnemyIndex].GetComponent<Enemy>().CheckOnPaint();


            currentEnemyIndex++;
        }
        */
    }

}
