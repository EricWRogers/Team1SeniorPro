using UnityEngine;

public class SimpleBeacon : MonoBehaviour
{
    public RoomAssembler assembler;

    void Reset()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!assembler) return;
        if (other.CompareTag("Player") || other.GetComponentInParent<Rigidbody>())
        {
            assembler.NextRoom();
        }
    }
}
