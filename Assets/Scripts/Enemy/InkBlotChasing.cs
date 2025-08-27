using UnityEngine;

public class InkBlotChasing : MonoBehaviour
{
    public Transform target;            // assign Player or maybe auto find
    public float speed = 2.0f;
    public float contactDamagePerSec = 12f;

    Rigidbody rb;
    PaintResource targetPaint;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    void Start()
    {
        if (!target) target = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (target) targetPaint = target.GetComponent<PaintResource>();
    }

    void Update()
    {
        if (!target) return;
        Vector3 to = target.position - transform.position; to.y = 0f;
        transform.position += to.normalized * speed * Time.deltaTime;
        // TO Do later: avoid pits / obstacles
    }

    void OnTriggerStay(Collider other)
    {
        if (targetPaint != null && other.transform == target)
        {
            targetPaint.Damage(contactDamagePerSec * Time.deltaTime);
        }
    }
}
