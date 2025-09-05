using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth = 100f;
    public UnityEvent OnDamaged;
    public UnityEvent OnHeal;

    
    void Start()
    {
        currentHealth = maxHealth;
    }
    public void Heal(float amount)
    {
        if (amount <= 0f) return;
        currentHealth = Mathf.Clamp(currentHealth + amount, 0f, maxHealth);
        OnHeal.Invoke();
    }

    public void Damage(float amount)
    {
        if (amount <= 0f) return;
        currentHealth = Mathf.Clamp(currentHealth - amount, 0f, maxHealth);
        OnDamaged.Invoke();
        if (currentHealth <= 0f)
        {
            Destroy(gameObject);
        }
    }
}
