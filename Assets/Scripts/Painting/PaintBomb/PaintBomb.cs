using UnityEngine;

public class PaintBomb : MonoBehaviour
{
    [Header("Paint Settings")]
    public float paintRadius = 2f;
    public int paintSpotsCount = 12;
    public ParticleSystem splashEffect;
    public SurfacePainter surfacePainter;

    [Header("Enemy Settings")]
    public float enemyDamage = 20f;
    public LayerMask enemyLayer;

private void OnCollisionEnter(Collision collision)
    {
         if (collision.gameObject.CompareTag("Enemy"))
        {
         
            var enemy = collision.gameObject.GetComponent<Health>(); 
            if (enemy != null)
            {
                enemy.Damage(enemyDamage);
                Debug.Log($"Paint bomb hit enemy: {collision.gameObject.name}");
            }

    
            Collider[] nearbyEnemies = Physics.OverlapSphere(transform.position, paintRadius, enemyLayer);
            foreach (var enemyCollider in nearbyEnemies)
            {
                if (enemyCollider.gameObject != collision.gameObject) // Don't hit the same enemy twice
                {
                    var splashEnemy = enemyCollider.GetComponent<Health>();
                    if (splashEnemy != null)
                    {
                        float distance = Vector3.Distance(transform.position, enemyCollider.transform.position);
                    
                        splashEnemy.Damage(enemyDamage);
                    }
                }
            }
        }
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
    int ringsCount = 3;
    float maxRadius = paintRadius;
    
    for (int ring = 0; ring < ringsCount; ring++)
    {
        float ringRadius = maxRadius * ((float)(ring + 1) / ringsCount);
        int spotsInRing = paintSpotsCount - (ring * 4); // Fewer spots in outer rings
        
        for (int i = 0; i < spotsInRing; i++)
        {
            // Add randomness to angle and radius
            float baseAngle = i * (360f / spotsInRing);
            float randomAngleOffset = Random.Range(-15f, 15f);
            float angle = baseAngle + randomAngleOffset;
            
            // More variation in radius for outer rings
            float radiusVariation = ringRadius * 0.3f * (ring + 1);
            float randomRadius = ringRadius + Random.Range(-radiusVariation, radiusVariation);
            
            // Create points in a circle on the hit plane
            Vector3 right = Vector3.Cross(normal, Vector3.up).normalized;
            Vector3 forward = Vector3.Cross(right, normal);
            
            // Add some random offset to create more chaos
            float chaos = Random.Range(0f, 0.5f) * ring; // More chaos in outer rings
            Vector3 randomOffset = (Random.onUnitSphere * chaos);
            randomOffset = Vector3.ProjectOnPlane(randomOffset, normal); // Keep offset on surface
            
            Vector3 circlePoint = hitPoint + 
                (right * Mathf.Cos(angle * Mathf.Deg2Rad) + 
                 forward * Mathf.Sin(angle * Mathf.Deg2Rad)) * randomRadius +
                randomOffset;

            // Adjust ray length based on distance from center
            float rayLength = 1f + (ring * 0.5f); // Longer rays for outer rings
            var ray = Physics.RaycastAll(circlePoint + normal, -normal, rayLength);
    
            foreach (var hit in ray)
            {
                Vector2 paintUV = hit.textureCoord;
                SurfacePainterMulti.instance.ActiveTarget(hit);
            }
        }
    }

    // Add some random splatter points
    int randomSplatterCount = paintSpotsCount / 2;
    for (int i = 0; i < randomSplatterCount; i++)
    {
        Vector3 randomDir = Random.onUnitSphere;
        randomDir = Vector3.ProjectOnPlane(randomDir, normal).normalized;
        float randomDist = Random.Range(0f, paintRadius * 1.2f);
        
        Vector3 splatterPoint = hitPoint + (randomDir * randomDist);
        var ray = Physics.RaycastAll(splatterPoint + normal, -normal, 1f);
        
        foreach (var hit in ray)
        {
            Vector2 paintUV = hit.textureCoord;
            SurfacePainterMulti.instance.ActiveTarget(hit);
        }
    }


    }
}
