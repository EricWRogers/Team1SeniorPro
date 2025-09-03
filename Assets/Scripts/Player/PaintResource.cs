using UnityEngine;
using System;
using System.Collections;
public class PaintResource : MonoBehaviour
{
    public float maxPaint = 100f;
    public float currentPaint = 100f;

    public event System.Action<float,float> OnPaintChanged;
    public event System.Action OnPaintDepleted;

    bool _depletedRaised;

    void RaiseChanged() => OnPaintChanged?.Invoke(currentPaint, maxPaint);

    public void AddPaint(float amount)
    {
        currentPaint = Mathf.Clamp(currentPaint + amount, 0f, maxPaint);
        if (currentPaint > 0f) _depletedRaised = false;
        RaiseChanged();
    }

    public void Damage(float amount)
    {
        if (amount <= 0f) return;
        currentPaint = Mathf.Clamp(currentPaint - amount, 0f, maxPaint);
        RaiseChanged();
        if (currentPaint <= 0f && !_depletedRaised)
        {
            _depletedRaised = true;
            OnPaintDepleted?.Invoke();
        }
    }

    public bool TrySpend(float amount)
    {
        if (currentPaint < amount) { Damage(currentPaint); return false; } // drop to zero -> trigger
        currentPaint -= amount;
        RaiseChanged();
        if (currentPaint <= 0f && !_depletedRaised)
        {
            _depletedRaised = true;
            OnPaintDepleted?.Invoke();
        }
        return true;
    }
}
