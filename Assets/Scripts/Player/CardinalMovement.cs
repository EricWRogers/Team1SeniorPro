using UnityEngine;

public class CardinalMovement : MonoBehaviour
{
    [Header("Move")]
    public float moveSpeed = 5f;
    public float onBlueSpeed = 6f;
    private float m_speed = 3f;

    [Header("Camera")]
    public Transform cameraTransform;

    private Rigidbody rb;
    private Animator anim;
    private SpriteRenderer spriteRenderer;
    private PaintResource paint;
    private bool checkOnPaint = true;
    private Texture2D readableTexture;
    RenderTexture activeMask;



    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.constraints = RigidbodyConstraints.FreezeRotation; // lock rotation
        readableTexture = new Texture2D(1024, 1024, TextureFormat.ARGB32, false);
        
    }
    void Start()
    {
        paint = gameObject.GetComponent<PaintResource>();
    }

    void FixedUpdate()
    {
        if (checkOnPaint)
        {
            CheckOnPaint();
            checkOnPaint = false;
        }
        else
        {
            checkOnPaint = true;
        }
        transform.LookAt(Camera.main.transform);
        // Input
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector2 input = Vector2.ClampMagnitude(new Vector2(h, v), 1f);


        Vector3 fwd = Vector3.forward;
        Vector3 right = Vector3.right;

        if (cameraTransform != null)
        {
            Vector3 camFwd = cameraTransform.forward; camFwd.y = 0f; camFwd.Normalize();
            Vector3 camRight = cameraTransform.right; camRight.y = 0f; camRight.Normalize();
            fwd = camFwd; right = camRight;
        }


        Vector3 desiredPlanarVel = (right * input.x + fwd * input.y) * m_speed;


        Vector3 vel = rb.linearVelocity;
        vel.x = desiredPlanarVel.x;
        vel.z = desiredPlanarVel.z;

        // stop tiny drift when no input
        if (input.sqrMagnitude < 0.0001f)
        {
            vel.x = 0f;
            vel.z = 0f;
        }

        rb.linearVelocity = vel;
        anim.SetBool("IsWalking", input.sqrMagnitude > 0.0001f);

        if (h > 0.1f)
        {
            spriteRenderer.flipX = false; // Facing Right
        }
        else if (h < -0.1f)
        {
            spriteRenderer.flipX = true; // Facing Left
        }
    }


    public void CheckOnPaint()
    {

        Ray ray = new Ray(transform.position, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 1f))
        {
             Renderer renderer = hit.collider.GetComponent<Renderer>();

            PaintableGroup temp = hit.transform.gameObject.GetComponent<PaintableGroup>();
            if (temp == null) return;
            activeMask = null;
            
            temp.TryGetMask(renderer, out activeMask);
            
            Texture texture = renderer.material.GetTexture("_Mask_Texture");
            //Debug.Log(hit.textureCoord);

            if (activeMask)
            {
                RenderTexture prev = RenderTexture.active;
                RenderTexture.active = activeMask;
                readableTexture.ReadPixels(new Rect(0, 0, activeMask.width, activeMask.height), 0, 0);
        
                readableTexture.Apply();
                //Debug.Log($"Width {texture.width}, Height {texture.height}");

                int pixelX = Mathf.FloorToInt(hit.textureCoord.x * activeMask.width);
                int pixelY = Mathf.FloorToInt(hit.textureCoord.y * activeMask.height);


                Color color = readableTexture.GetPixel(pixelX, pixelY);
                Debug.Log($"the player color {color}");
                if (color.b > 0.5f)
                {

                    m_speed = onBlueSpeed;
                }
                else
                {
                    m_speed = moveSpeed;
                }
                
                if (color.r > 0.5f && color.b < .5)
                {
                   //paint.Damage(1f * Time.fixedDeltaTime);
                }


            }
            else
            {
                m_speed = moveSpeed;

            }
            

        }
        else
        {
            m_speed = moveSpeed;
         
        }
       
    }
}