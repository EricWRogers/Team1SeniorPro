using UnityEngine;

public class NewPainter : MonoBehaviour
{
 public Camera playerCam;
    public Renderer targetRenderer;   // mesh renderer of window/wall
    public Texture2D brushTex;        // circular grayscale brush
    public Color brushColor = Color.red;
    public int textureSize = 1024;

    private RenderTexture paintRT;
    private Material paintMat;
    private Material blitMat;

    void Start()
    {
        // create blank paint RenderTexture
        paintRT = new RenderTexture(textureSize, textureSize, 0, RenderTextureFormat.ARGB32);
        paintRT.Create();

        // assign it to material


        // material to blit brushes
        blitMat = new Material(Shader.Find("Hidden/BrushStamp"));
        blitMat.SetTexture("_BrushTex", brushTex);
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Ray ray = playerCam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                targetRenderer = hit.collider.GetComponent<Renderer>();
                targetRenderer.material.SetTexture("_PaintTex", paintRT);
                Debug.Log($"Hit renderer: {targetRenderer.name}");
                    Vector2 uv = hit.textureCoord;
                    Stamp(uv, brushColor, 10f);

            }
        }
    }

    void Stamp(Vector2 uv, Color color, float scale)
    {
        blitMat.SetVector("_BrushColor", color);
        blitMat.SetVector("_BrushUV", new Vector4(uv.x, uv.y, scale, scale));

        Debug.Log($"Stamping at UV: {uv}, Color: {color}, Scale: {scale}");

        RenderTexture tmp = RenderTexture.GetTemporary(paintRT.width, paintRT.height, 0, paintRT.format);
        Graphics.Blit(paintRT, tmp); // copy old paint
        Graphics.Blit(tmp, paintRT, blitMat); // add new stroke
        RenderTexture.ReleaseTemporary(tmp);
    }
}
