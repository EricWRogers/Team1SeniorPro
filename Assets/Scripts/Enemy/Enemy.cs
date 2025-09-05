using UnityEngine;
using UnityEngine.Events;
using Pathfinding;
using UnityEditor;

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
    public float flashTime;

    private bool m_playerDetected = false;
    private bool m_inRange = false;
    private bool reachedEndOfPath;
    private int currentWaypoint;
    protected GameObject m_player;
    private Rigidbody m_rb;
    private Seeker m_seeker;
    private Path m_path;
    private bool m_canSeePlayer;
    private RaycastHit hit;
    protected Health m_health;
    private Color color;

    public void Awake()
    {
        m_health = GetComponent<Health>();
        m_health.maxHealth = health;
    }

    public void Start()
    {
        m_player = GameObject.FindGameObjectWithTag("Player");
        m_rb = GetComponent<Rigidbody>();
        m_seeker = GetComponent<Seeker>();
        InvokeRepeating("UpdatePath", 0f, .5f);
        color = GetComponent<SpriteRenderer>().color;
    }
    public void Update()
    {
        transform.LookAt(Camera.main.transform);
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
        Vector3 dir = (m_player.transform.position - transform.position).normalized;
        if (Physics.Raycast(transform.position, dir, out hit, attackRange))
        {
            Debug.Log(hit.transform.gameObject.name);
            if (hit.transform.tag == "Player")
            {
                m_canSeePlayer = true;
            }
            else m_canSeePlayer = false;
        }

        if (Vector3.Distance(transform.position, m_player.transform.position) < detectionRange)
        {
            m_playerDetected = true;
        }
        if (Vector3.Distance(transform.position, m_player.transform.position) < attackRange && m_canSeePlayer)
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
                Attack.Invoke();
            }
            else
            {
                m_rb.isKinematic = false;
            }

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
        Vector3 force = dir * speed * Time.deltaTime;
        m_rb.AddForce(force);
        float distance = Vector3.Distance(m_rb.position, m_path.vectorPath[currentWaypoint]);
        if (distance < nextWayPointDes)
        {
            currentWaypoint++;
        }
    }

    public void FlashRed()
    {
        Color color = GetComponent<SpriteRenderer>().color;
        GetComponent<SpriteRenderer>().color = Color.red;
        Invoke("ResetColor", flashTime);
    }
    public void ResetColor()
    {
        GetComponent<SpriteRenderer>().color = color;
    }
}
