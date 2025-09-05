using UnityEngine;
using UnityEngine.Events;

public class Enemy : MonoBehaviour
{
    [Header("General Enemy Settings")]
    public float speed;
    public float damage;
    public float attackRange;
    public float detectionRange;
    public UnityEvent Attack;

    private bool m_playerDetected = false;
    private GameObject m_player;


    void Start()
    {
        m_player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        if (m_playerDetected)
        {
            //move to player
            //if in range invoke attack

        }
        else
        {
            //move to random point in area
        }
    }

}
