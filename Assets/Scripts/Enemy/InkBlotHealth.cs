using UnityEngine;

public class InkBlotHealth : MonoBehaviour
{
    public float maxHP = 30f;
    float hp;

    void Awake() { hp = maxHP; }
    public void TakeSpray(float amount)
    {
        hp -= amount;
        if (hp <= 0f) Die();
    }
    void Die()
    {
        // TODO: drop pigment / paint cans here
        Destroy(gameObject);
    }
}
