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
        // Get all renderers in the hierarchy
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0) return;

        // Calculate base vectors
        Vector3 right = Vector3.Cross(normal, Vector3.up).normalized;
        Vector3 forward = Vector3.Cross(right, normal);

        int ringsCount = 3;
        float maxRadius = paintRadius;

        for (int ring = 0; ring < ringsCount; ring++)
        {
            float ringRadius = maxRadius * ((float)(ring + 1) / ringsCount);
            int spotsInRing = paintSpotsCount - (ring * 4);

            for (int i = 0; i < spotsInRing; i++)
            {
                float baseAngle = i * (360f / spotsInRing);
                float randomAngleOffset = Random.Range(-15f, 15f);
                float angle = baseAngle + randomAngleOffset;

                float radiusVariation = ringRadius * 0.3f * (ring + 1);
                float randomRadius = ringRadius + Random.Range(-radiusVariation, radiusVariation);

                float chaos = Random.Range(0f, 0.5f) * ring;
                Vector3 randomOffset = (Random.onUnitSphere * chaos);
                randomOffset = Vector3.ProjectOnPlane(randomOffset, normal);

                Vector3 circlePoint = hitPoint + 
                    (right * Mathf.Cos(angle * Mathf.Deg2Rad) + 
                     forward * Mathf.Sin(angle * Mathf.Deg2Rad)) * randomRadius +
                    randomOffset;

                // Cast ray against all colliders
                RaycastHit[] hits = Physics.RaycastAll(
                    circlePoint + normal * 0.5f, 
                    -normal,
                    1f
                );

                // Try to paint each hit
                foreach (var hit in hits)
                {
                    
                        SurfacePainterMulti.instance.ActiveTarget(hit);
                    
                }
            }
        }

        // random splatter
        int randomSplatterCount = paintSpotsCount / 2;
        for (int i = 0; i < randomSplatterCount; i++)
        {
            Vector3 randomDir = Random.onUnitSphere;
            randomDir = Vector3.ProjectOnPlane(randomDir, normal).normalized;
            float randomDist = Random.Range(0f, paintRadius * 1.2f);

            Vector3 splatterPoint = hitPoint + (randomDir * randomDist);
            RaycastHit[] hits = Physics.RaycastAll(splatterPoint + normal * 0.5f, -normal, 1f);

            foreach (var hit in hits)
            {
                if (hit.collider.transform.IsChildOf(obj.transform))
                {
                    SurfacePainterMulti.instance.ActiveTarget(hit);
                }
            }
        }
    }
}
