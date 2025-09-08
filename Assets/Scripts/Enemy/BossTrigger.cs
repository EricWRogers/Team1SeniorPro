using UnityEngine;
using UnityEngine.Events;

public class BossTrigger : MonoBehaviour
{
    public UnityEvent bossTrigger;
    void OnTriggerEnter(Collider other)
    {
        bossTrigger.Invoke();
    }
}
