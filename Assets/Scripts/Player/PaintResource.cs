using UnityEngine;
using System;
public class PaintResource : MonoBehaviour
{
    public float maxPaint = 100f;
    public float currentPaint = 100f;

    public event Action<float,float> OnPaintChanged; // current, max
    public event Action OnPaintDepleted;

    public float PaintPct => Mathf.Approximately(maxPaint, 0f) ? 0f : currentPaint / maxPaint;

    public void AddPaint(float amount)
    {
        currentPaint = Mathf.Clamp(currentPaint + amount, 0f, maxPaint);
        OnPaintChanged?.Invoke(currentPaint, maxPaint);
        if (currentPaint <= 0f) OnPaintDepleted?.Invoke();
    }

    public void Damage(float amount)
    {
        if (amount <= 0f) return;
        currentPaint = Mathf.Clamp(currentPaint - amount, 0f, maxPaint);
        OnPaintChanged?.Invoke(currentPaint, maxPaint);
        if (currentPaint <= 0f) OnPaintDepleted?.Invoke();
    }

    public bool TrySpend(float amount) // for the paint spraying later, call this
    {
        if (currentPaint < amount) return false;
        currentPaint -= amount;
        OnPaintChanged?.Invoke(currentPaint, maxPaint);
        if (currentPaint <= 0f) OnPaintDepleted?.Invoke();
        return true;
    }
}
