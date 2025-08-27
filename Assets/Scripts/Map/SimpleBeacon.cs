using UnityEngine;

public class SimpleBeacon : MonoBehaviour
{
    public RoomAssembler assembler;

    void Reset() { GetComponent<Collider>().isTrigger = true; }

    void OnTriggerEnter(Collider other)
    {
        if (!assembler) return;
        if (other.CompareTag("Player") || other.GetComponentInParent<PaintResource>())
        {
            assembler.NextRoom(); // should internally GenerateRoom again
        }
    }
}
