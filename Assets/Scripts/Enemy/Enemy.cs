using UnityEngine;
using UnityEngine.Events;
using Pathfinding;

public class Enemy : MonoBehaviour
{
    [Header("General Enemy Settings")]
    public Vector3 target;
    public float speed;
    public float damage;
    public float health;
    public float attackRange;
    public float detectionRange;
    public UnityEvent Attack;
    public float nextWayPointDes;
    public LayerMask layerMask;

    private bool m_playerDetected = false;
    private bool m_inRange = false;
    private bool reachedEndOfPath;
    private int currentWaypoint;
    private GameObject m_player;
    private Rigidbody m_rb;
    private Seeker m_seeker;
    private Path m_path;
    private float tempSpeed;


    void Start()
    {
        tempSpeed = speed;
        m_player = GameObject.FindGameObjectWithTag("Player");
        m_rb = GetComponent<Rigidbody>();
        m_seeker = GetComponent<Seeker>();
        InvokeRepeating("UpdatePath", 0f, .5f);
    }

    void FixedUpdate()
    {
        if (m_path == null)
        {
            return;
        }
        if (currentWaypoint >= m_path.vectorPath.Count)
        {
            reachedEndOfPath = true;
            return;
        }
        else
        {
            reachedEndOfPath = false;
        }

        if (Vector3.Distance(transform.position, m_player.transform.position) < detectionRange)
        {
            m_playerDetected = true;
        }
        else m_playerDetected = false;
        if (Vector3.Distance(transform.position, m_player.transform.position) < attackRange)
        {
            m_inRange = true;
        }
        else m_inRange = false;


        if (m_playerDetected)
        {
            target = GameObject.FindGameObjectWithTag("Player").transform.position;
            Move();
            if (m_inRange)
            {

                m_rb.isKinematic = true;
            }
            else
            {
                m_rb.isKinematic = false;
            }

        }
        else
        {

        }
    }

    void UpdatePath()
    {
        if (m_seeker.IsDone())
        {
            m_seeker.StartPath(m_rb.position, target, OnPathComplete);
        }
    }
    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            m_path = p;
            currentWaypoint = 0;
        }
    }

    public void Move()
    {
        Vector3 dir = (Vector3)m_path.vectorPath[currentWaypoint] - m_rb.position;
        Vector3 force = dir * speed * Time.fixedDeltaTime;
        m_rb.AddForce(force);
        float distance = Vector3.Distance(m_rb.position, m_path.vectorPath[currentWaypoint]);
        if (distance < nextWayPointDes)
        {
            currentWaypoint++;
        }
    }

    private void OnDrawGizmos()
    {
        Vector3 dir = (transform.position - m_player.transform.position).normalized;
        Gizmos.DrawRay(transform.position, dir * -attackRange);
    }

}
