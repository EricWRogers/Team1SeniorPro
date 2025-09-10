using UnityEngine;
using UnityEngine.Events;
using Pathfinding;
using System.Collections.Generic;
using System.Collections;
using System.Runtime.CompilerServices;

public class Enemy : MonoBehaviour
{
    [Header("General Enemy Settings")]
    public Vector3 target;
    public float speed;
    public float blueSpeedMult = 1.5f;
    private float m_speed = 40f;
    
    public float damage;
    public float health;
    public float attackRange;
    public float detectionRange;
    public UnityEvent Attack;
    public float nextWayPointDes;
    //public float flashTime;
    public List<Transform> wayPoints;

    private int m_curWayPoint = 0;
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
    private SpriteRenderer spriteRenderer;
    private bool isOnRed;


    [Header("Loot")]
    public List<InkBlotDrops> lootTable = new List<InkBlotDrops>();

    public void Awake()
    {
        m_health = GetComponent<Health>();
        m_health.maxHealth = health;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    public void Start()
    {
        m_player = GameObject.FindGameObjectWithTag("Player");
        m_rb = GetComponent<Rigidbody>();
        m_seeker = GetComponent<Seeker>();
        InvokeRepeating("UpdatePath", 0f, .5f);
    }
    public void FixedUpdate()
    {
        if (isOnRed)
        {
            m_health.Damage(5f * Time.fixedDeltaTime);
        }
        
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
        else if (wayPoints.Count != 0)
        {
            target = wayPoints[m_curWayPoint].position;
            Move();
            float distance = Vector3.Distance(m_rb.position, wayPoints[m_curWayPoint].position);
            if (distance < nextWayPointDes)
            {
                if (m_curWayPoint == wayPoints.Count - 1)
                {
                    m_curWayPoint = 0;
                }
                else
                    m_curWayPoint++;
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

    public void SpawnLoot()
    {
        foreach (InkBlotDrops inkBlotDrops in lootTable)
        {
            if (Random.Range(0f, 100f) <= inkBlotDrops.dropChance)
            {
                InstantiateLoot(inkBlotDrops.itemPrefab);
            }
            break;
        }
    }

    void InstantiateLoot(GameObject drops)
    {
        if (drops)
        {
            GameObject droppedInk = Instantiate(drops, transform.position, Quaternion.identity);

            MeshRenderer mesh = droppedInk.GetComponent<MeshRenderer>();
            if (mesh != null)
            {
                // This changes the material's color
                mesh.material.color = Color.red;
            }
        }
    }

    private IEnumerator FlashRed() //falsh red when taking damage
    {
        spriteRenderer.color = Color.red;

        yield return new WaitForSeconds(1.0f); // 0.5 seconds

        spriteRenderer.color = Color.white;
    }
    public void CheckOnPaint()
    {

        Ray ray = new Ray(transform.position, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 1f))
        {
            Renderer renderer = hit.collider.GetComponent<Renderer>();
            Texture texture = renderer.material.GetTexture("_Mask_Texture");

            if (texture is RenderTexture renderTexture)
            {
                RenderTexture.active = renderTexture;
                Texture2D readableTexture = new Texture2D(124, 124, TextureFormat.RGBAHalf, false);
                readableTexture.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);
                readableTexture.Apply();

                int pixelX = Mathf.FloorToInt(hit.textureCoord.x * readableTexture.width);
                int pixelY = Mathf.FloorToInt(hit.textureCoord.y * readableTexture.height);

                Color color = readableTexture.GetPixel(pixelX, pixelY);
                if (color.b > 0.8f && color.r < 0.3f && color.g < 0.3f)
                {
                    m_speed = speed * blueSpeedMult;
                }
                else
                {
                    m_speed = speed;
                }

                if (color.r > 0.8f && color.g < 0.3f && color.b < 0.2f)
                {
                    isOnRed = true;
                }
                else
                {
                    isOnRed = false;
                }


            }
            else
            {
                m_speed = speed;
                isOnRed = false;

            }


        }
        else
        {
            m_speed = speed; 
            isOnRed = false;
         
        }
       
    }
}
