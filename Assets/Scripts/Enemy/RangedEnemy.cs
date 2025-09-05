using Unity.Mathematics;
using UnityEngine;

public class RangedEnemy : Enemy
{
    [Header("Ranged Settings")]
    public GameObject bulletPrefab;
    public float fireRate;
    public float bulletSpeed;
    public Transform firePoint;
    private float m_curFireRate;

    new void Start()
    {
        base.Start();

        m_curFireRate = bulletSpeed;
    }
    new void Update()
    {
        base.Update();

        firePoint.LookAt(m_player.transform);
        m_curFireRate -= Time.deltaTime;

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
}
