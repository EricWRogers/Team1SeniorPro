using UnityEngine;

public class PlayerGroundDamage : MonoBehaviour
{
    public float dpsOnWhite = 10f;
    public LayerMask safeGroundMask;     // set to SafeGround layer
    public float safeCheckRadius = 0.4f; // feet radius

    PaintResource paint;

    void Awake() { paint = GetComponent<PaintResource>(); }

    void Update()
    {
        Vector3 feet = transform.position + Vector3.up * 0.2f;
        bool onSafe = Physics.CheckSphere(feet, safeCheckRadius, safeGroundMask);
        if (!onSafe)
            paint.Damage(dpsOnWhite * Time.deltaTime);
    }
}
