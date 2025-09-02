using UnityEngine;

public class PlayerGroundDamage : MonoBehaviour
{
    [Header("Damage")]
    public float dpsOnWhite = 10f;

    [Header("Detection")]
    public LayerMask hazardGroundMask;   // set to Ground
    public LayerMask safeGroundMask;     // set to SafeGround
    public float rayUp = 0.5f;
    public float rayDown = 2.0f;
    public float safeCheckRadius = 0.45f;

    PaintResource paint;

    void Awake() { paint = GetComponent<PaintResource>(); }

    void Update()
    {
        Vector3 from = transform.position + Vector3.up * rayUp;

        // Are we actually standing over "ground" that should hurt when white?
        bool onHazardGround = Physics.Raycast(from, Vector3.down, out RaycastHit _, rayDown + rayUp, hazardGroundMask);

        // Are we overlapping any Safe area (start pad or spawned discs)?
        bool inSafe = Physics.CheckSphere(transform.position + Vector3.up * 0.2f, safeCheckRadius, safeGroundMask, QueryTriggerInteraction.Collide);

        if (onHazardGround && !inSafe)
        {
            paint.Damage(dpsOnWhite * Time.deltaTime);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position + Vector3.up * 0.2f, safeCheckRadius);
    }
}
