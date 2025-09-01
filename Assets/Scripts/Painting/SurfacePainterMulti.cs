using UnityEngine;

public class SurfacePainterMulti : MonoBehaviour
{
    public Camera cam;
    public Transform nozzle;
    public Texture2D brushTexture;        // black-circle brush with alpha
    [Range(0.1f, 50f)] public float brushSizePercent = 4f;
    public float maxSprayDistance = 15f;
    public LayerMask paintMask = ~0;      // set to Cleanable layer if you make one

    Renderer activeRenderer;
    RenderTexture activeMask;
    PaintableGroup activeGroup;

    void Reset() { if (!cam) cam = Camera.main; }

    void Update()
    {
        if (!Input.GetMouseButton(0)) return;

        // Aim from camera to mouse to get a direction
        Ray mouseRay = cam.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(mouseRay, out RaycastHit aimHit, 100f, paintMask)) return;
        Vector3 dir = (aimHit.point - nozzle.position).normalized;

        // Now raycast from the nozzle and prefer MeshCollider hits
        var hits = Physics.RaycastAll(nozzle.position, dir, maxSprayDistance, paintMask);
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        foreach (var h in hits)
        {
            var mc = h.collider as MeshCollider;
            var rend = h.collider.GetComponent<Renderer>();
            if (!mc || !rend) continue; // skip non-mesh hits

            if (rend != activeRenderer)
            {
                activeRenderer = rend;
                activeGroup = rend.GetComponentInParent<PaintableGroup>();
                activeMask = null;
                if (activeGroup != null) activeGroup.TryGetMask(rend, out activeMask);
            }

            if (activeMask)
            {
                PaintAtUV(activeMask, h.textureCoord);
            }
            return; // paint only the nearest valid mesh
        }
    }

    void PaintAtUV(RenderTexture maskRT, Vector2 uv)
    {
        if (!brushTexture) return;
        RenderTexture prev = RenderTexture.active;
        RenderTexture.active = maskRT;
        GL.PushMatrix();
        GL.LoadPixelMatrix(0, maskRT.width, maskRT.height, 0);

        float px = maskRT.width  * uv.x;
        float py = maskRT.height * (1f - uv.y);
        float brushPx = Mathf.Max(2f, maskRT.width * (brushSizePercent / 100f));
        Rect rect = new Rect(px - brushPx * 0.5f, py - brushPx * 0.5f, brushPx, brushPx);

        Graphics.DrawTexture(rect, brushTexture);

        GL.PopMatrix();
        RenderTexture.active = prev;
    }
}