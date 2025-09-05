using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public List<GameObject> enemyPrefabs;
    public Vector2 spawnIntervalRange = new Vector2 (5, 15);
    public float m_currentInterval;
    private EnemyManager m_enemyManager;

    void Start()
    {
        m_enemyManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<EnemyManager>();
    }

    void Update()
    {
        m_currentInterval -= Time.deltaTime;
        if (m_currentInterval <= 0)
        {
            if (m_enemyManager.currentEnemyCount < m_enemyManager.maxEnemyCount)
            {
                SpawnEnemy();
                m_currentInterval = Random.Range(spawnIntervalRange.x, spawnIntervalRange.y);
            }
            
        }
    }
    public void SpawnEnemy()
    {
        int randIndex = Random.Range(0, enemyPrefabs.Count - 1);
        Instantiate(enemyPrefabs[randIndex], transform.position, Quaternion.identity);
    }
}
