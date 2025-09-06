using UnityEngine;

public class PaintBombThrower : MonoBehaviour
{
[Header("References")]
    public Camera cam;
    public GameObject paintBombPrefab;
    public Transform throwOrigin;

    [Header("Throwing")]
    public float arc_Height = 2f;
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
        Vector3 toTarget = targetPos - throwOrigin.position;
        
        // Calculate horizontal and vertical components separately
        float gravity = Physics.gravity.magnitude;
        float time = Mathf.Sqrt(2 * arc_Height / gravity);
        
        // Horizontal velocity needed to reach target
        Vector3 horizontalVelocity = toTarget / (time * 2);
        horizontalVelocity.y = 0;
        
        // Vertical velocity for desired arc height
        float verticalVelocity = Mathf.Sqrt(2 * gravity * arc_Height);
        
        // Combine velocities
        Vector3 finalVelocity = horizontalVelocity + (Vector3.up * verticalVelocity);
        
        // Apply the velocity
        rb.linearVelocity = finalVelocity;
    }
    }

    private float CalculateVelocity(Vector3 _start, Vector3 _end, float _height)
    {
        float gravity = Physics.gravity.magnitude;
        float displacementY = _end.y - _start.y;
        Vector3 displacementXZ = new Vector3(_end.x - _start.x, 0f, _end.z - _start.z);
        float time = Mathf.Sqrt(-2 * _height / gravity) + Mathf.Sqrt(2 * (displacementY - _height) / gravity);
        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * _height);
        Vector3 velocityXZ = displacementXZ / time;

        return velocityXZ.magnitude + velocityY.magnitude;
    }
}
