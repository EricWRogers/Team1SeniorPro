using UnityEngine;

public class PaintBomb : MonoBehaviour
{
    [Header("Paint Settings")]
    public float paintRadius = 2f;
    public int paintSpotsCount = 12;
    public ParticleSystem splashEffect;
    public SurfacePainter surfacePainter;

private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Cleanable"))
        {
            // Use the first contact point as the center of our paint pattern
            ContactPoint contact = collision.GetContact(0);
            Debug.Log($"PaintBomb hit: {collision.gameObject.name} at {contact.point}");
            CreatePaintPattern(collision.gameObject, contact.point, contact.normal);
        }

        // Spawn effect and destroy bomb
        if (splashEffect)
        {
            Instantiate(splashEffect, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }

    void CreatePaintPattern(GameObject obj, Vector3 hitPoint, Vector3 normal)
    {
        // Create a plane at the hit point
        Plane hitPlane = new Plane(normal, hitPoint);
        
        SurfacePainter.instance.SetupTarget(obj);

        // Create a circular pattern of paint spots
        for (int i = 0; i < paintSpotsCount; i++)
        {
            float angle = i * (360f / paintSpotsCount);
            float randomRadius = Random.Range(0f, paintRadius);

            // Create points in a circle on the hit plane
            Vector3 right = Vector3.Cross(normal, Vector3.up).normalized;
            Vector3 forward = Vector3.Cross(right, normal);
            
            // Calculate point on the circle
            Vector3 circlePoint = hitPoint + 
                (right * Mathf.Cos(angle * Mathf.Deg2Rad) + 
                 forward * Mathf.Sin(angle * Mathf.Deg2Rad)) * randomRadius;

            // Cast a ray from slightly above the circle point towards the surface
            Ray ray = new Ray(circlePoint + normal * 0.1f, -normal);
            if (Physics.Raycast(ray, out RaycastHit hit, 0.2f))
            {
                if (hit.collider.gameObject == obj)
                {
                    Vector2 paintUV = hit.textureCoord;
                    paintUV.x = Mathf.Clamp01(paintUV.x);
                    paintUV.y = Mathf.Clamp01(paintUV.y);

                    SurfacePainter.instance.PaintAtUV(paintUV);
                }
            }
        }
    }
}
