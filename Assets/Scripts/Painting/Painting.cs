using UnityEngine;

// Your script is named "Painting", so the class name must match the file name.
public class Painting : MonoBehaviour
{
    public Transform nozzleTransform; 
    public Texture2D brushTexture; 
    public float brushSize = 1.0f;
    public float washDistance = 5f;

    private GameObject lastHitObject; 
    private RenderTexture maskRenderTexture; 
    private Material objectMaterial;

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Ray ray = new Ray(nozzleTransform.position, nozzleTransform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, washDistance))
            {
                // --- SOLUTION ---
                // Check if the object we hit has the "Cleaning" tag.
                if (hit.collider.CompareTag("Cleaning"))
                {
                    // If it's a new object, set it up.
                    if (lastHitObject != hit.collider.gameObject)
                    {
                        lastHitObject = hit.collider.gameObject;
                        SetupCleanableObject(lastHitObject);
                    }

                    // If setup was successful, paint on its mask.
                    if (maskRenderTexture != null)
                    {
                        Vector2 uv = hit.textureCoord;
                        PaintOnMask(uv);
                    }
                }
                else
                {
                    // If we hit something not cleanable, reset our references.
                    lastHitObject = null;
                    maskRenderTexture = null;
                }
            }
        }
    }

    void SetupCleanableObject(GameObject obj)
    {
        MeshRenderer renderer = obj.GetComponent<MeshRenderer>();
        if (renderer == null) return;

        objectMaterial = renderer.material;

        // This safety check also prevents errors if the wrong material is on a cleanable object.
        if (!objectMaterial.HasProperty("_Mask_Texture"))
        {
            maskRenderTexture = null;
            return;
        }

        Texture originalMask = objectMaterial.GetTexture("_Mask_Texture");
        if (originalMask == null) return;

        maskRenderTexture = new RenderTexture(originalMask.width, originalMask.height, 0, RenderTextureFormat.ARGB32);
        Graphics.Blit(originalMask, maskRenderTexture);
        objectMaterial.SetTexture("_Mask_Texture", maskRenderTexture);
    }

    void PaintOnMask(Vector2 uv)
    {
        RenderTexture previousActive = RenderTexture.active;
        RenderTexture.active = maskRenderTexture;
        GL.PushMatrix();
        GL.LoadPixelMatrix(0, maskRenderTexture.width, maskRenderTexture.height, 0);
        float brushSizePixels = maskRenderTexture.width * (brushSize / 100.0f);
        float x = (uv.x * maskRenderTexture.width) - (brushSizePixels / 2.0f);
        float y = ((1 - uv.y) * maskRenderTexture.height) - (brushSizePixels / 2.0f);
        Rect brushRect = new Rect(x, y, brushSizePixels, brushSizePixels);
        Graphics.DrawTexture(brushRect, brushTexture);
        GL.PopMatrix();
        RenderTexture.active = previousActive;
    }
}