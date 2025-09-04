using UnityEngine;

public class PaintBombThrower : MonoBehaviour
{
[Header("References")]
    public Camera cam;
    public GameObject paintBombPrefab;
    public Transform throwOrigin;

    [Header("Throwing")]
    public float arcHeight = 2f;
    public float throwSpeed = 10f;
    public float cooldownTime = 1f;
    
    private float nextThrowTime;

    void Reset()
    {
        if (!cam) cam = Camera.main;
    }

    void Update()
    {
        // Right click to throw paint bomb
        if (Input.GetMouseButtonDown(1) && Time.time >= nextThrowTime)
        {
            Ray mouseRay = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(mouseRay, out RaycastHit hit))
            {
                ThrowPaintBomb(hit.point);
                nextThrowTime = Time.time + cooldownTime;
            }
        }
    }

    void ThrowPaintBomb(Vector3 targetPos)
    {
        if (!paintBombPrefab || !throwOrigin) return;

        GameObject bomb = Instantiate(paintBombPrefab, throwOrigin.position, Quaternion.identity);
        Rigidbody rb = bomb.GetComponent<Rigidbody>();
        
        if (rb)
        {
            // Calculate velocity needed to reach target in an arc
            Vector3 toTarget = targetPos - throwOrigin.position;
            Vector3 directionXZ = new Vector3(toTarget.x, 0, toTarget.z);
            
            float distance = directionXZ.magnitude;
            directionXZ.Normalize();
            
            // Add upward arc
            Vector3 throwVelocity = directionXZ * throwSpeed;
            throwVelocity.y = arcHeight;
            
            rb.linearVelocity = throwVelocity;
        }
    }
}
