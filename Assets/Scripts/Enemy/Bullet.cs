using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage;
    public float speed;
    public float lifeTime;
    private Vector3 m_startPos;

    void Start()
    {
        m_startPos = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position += transform.forward * speed * Time.fixedDeltaTime;
        RaycastHit hit;
        if (Physics.Linecast(m_startPos, transform.position, out hit))
        {
            if (hit.transform.tag == "Player")
            {
                Debug.Log("hit player");
                hit.transform.GetComponent<PaintResource>()?.Damage(damage);
                Destroy(gameObject);
            }
            Destroy(gameObject);
        }
        m_startPos = transform.position;

        lifeTime -= Time.fixedDeltaTime;
        if (lifeTime <= 0)
        {
            Destroy(gameObject);
        }
    }
}
