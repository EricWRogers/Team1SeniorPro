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

        Destroy(gameObject);
    }

    void CreatePaintPattern(GameObject obj, Vector3 hitPoint, Vector3 normal)
    {

        

        for (int i = 0; i < paintSpotsCount; i++)
        {
            float angle = i * (360f / paintSpotsCount);
            float randomRadius = Random.Range(0f, paintRadius);

            // Create points in a circle on the hit plane
            Vector3 right = Vector3.Cross(normal, Vector3.up).normalized;
            Vector3 forward = Vector3.Cross(right, normal);
            
            Vector3 circlePoint = hitPoint + 
                (right * Mathf.Cos(angle * Mathf.Deg2Rad) + 
                 forward * Mathf.Sin(angle * Mathf.Deg2Rad)) * randomRadius;

            var ray = Physics.RaycastAll(circlePoint + normal , -normal, 1f);
    
            foreach (var hit in ray)
            {
                SurfacePainter.instance.SetupTarget(hit.collider.gameObject);
                Vector2 paintUV = hit.textureCoord;
                Debug.Log($"Painting at UV: {paintUV} on {obj.name}");

                SurfacePainter.instance.PaintAtUV(paintUV);

            }
        }
    }
}
