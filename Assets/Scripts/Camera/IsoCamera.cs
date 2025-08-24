using UnityEngine;

public class IsoCamera : MonoBehaviour
{
    public Transform target;           
    [Range(10f, 80f)] public float pitch = 33f;   // tilt down
    [Range(0f, 360f)] public float yaw = 0f;     // rotate
    public float distance = 12f;                  // how far the camera sits form player
    public Vector3 targetOffset = new Vector3(0f, 0f, 0f); 
    public float followDamp = 10f;                

        public bool clampToBounds = false;
    public Vector2 xzMin = new Vector2(-100, -100);
    public Vector2 xzMax = new Vector2( 100,  100);

    Vector3 currentVel;

    void LateUpdate()
    {
        if (!target) return;

        
        Quaternion rot = Quaternion.Euler(pitch, yaw, 0f);

        
        Vector3 focus = target.position + targetOffset;
        Vector3 desiredPos = focus - (rot * Vector3.forward) * distance;

        // Smooth position
        transform.position = Vector3.Lerp(transform.position, desiredPos, 1f - Mathf.Exp(-followDamp * Time.deltaTime));

        
        if (clampToBounds)
        {
            Vector3 p = transform.position;
            p.x = Mathf.Clamp(p.x, xzMin.x, xzMax.x);
            p.z = Mathf.Clamp(p.z, xzMin.y, xzMax.y);
            transform.position = p;
        }

        
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation((focus - transform.position).normalized, Vector3.up), 1f - Mathf.Exp(-followDamp * Time.deltaTime));
    }
}
