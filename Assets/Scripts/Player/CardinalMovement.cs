using UnityEngine;

public class CardinalMovement : MonoBehaviour 
{
    [Header("Move")]
    public float moveSpeed = 5f;
    public float acceleration = 20f;   //how long to reach target speed
    public float maxSlopeAngle = 45f;

    [Header("Camera")]
    public Transform cameraTransform;  //plug in cam

    Rigidbody rb;
    Vector3 velocity;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.constraints = RigidbodyConstraints.FreezeRotation; //keep Y pos free
    }

    void FixedUpdate()
    {
        // Read input
        float h = Input.GetAxisRaw("Horizontal"); // A/D or Left/Right
        float v = Input.GetAxisRaw("Vertical");   // W/S or Up/Down
        Vector2 input = new Vector2(h, v);
        input = Vector2.ClampMagnitude(input, 1f);

        
        Vector3 fwd = Vector3.forward;
        Vector3 right = Vector3.right;

        if (cameraTransform != null)
        {
            Vector3 camFwd = cameraTransform.forward;
            camFwd.y = 0f;
            camFwd.Normalize();

            Vector3 camRight = cameraTransform.right;
            camRight.y = 0f;
            camRight.Normalize();

            fwd = camFwd;
            right = camRight;
        }

        Vector3 desiredVel = (right * input.x + fwd * input.y) * moveSpeed;

        //smooth towards the speed you want
        velocity = Vector3.MoveTowards(rb.linearVelocity, desiredVel, acceleration * Time.fixedDeltaTime);

        //stay on plane, no vertical drift
        velocity.y = rb.linearVelocity.y;
        rb.linearVelocity = velocity;
    }
}
