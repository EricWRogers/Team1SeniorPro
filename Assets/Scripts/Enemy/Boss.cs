using UnityEngine;

public class Boss : Enemy
{
    [Header("Boss Settings")]
    public GameObject bulletPrefab;
    public float fireRate;
    public float bulletSpeed;
    public Transform firePoint;
    public float meleeRange;
    private float m_curFireRate;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    new void Start()
    {
        base.Start();
        m_curFireRate = bulletSpeed;
    }

    // Update is called once per frame
    new void FixedUpdate()
    {
        base.FixedUpdate();
        firePoint.LookAt(m_player.transform);
        m_curFireRate -= Time.deltaTime;
        Shoot();
        if (Vector3.Distance(transform.position, m_player.transform.position) < meleeRange)
        {
            AttackPlayer();
        }

    }
    public void Shoot()
    {
        if (m_curFireRate <= 0)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            bullet.GetComponent<Bullet>().damage = damage;
            m_curFireRate = fireRate;
        }
    }
    public void AttackPlayer()
    {
        m_player.GetComponent<PaintResource>().Damage(damage / 2);
    }
}
