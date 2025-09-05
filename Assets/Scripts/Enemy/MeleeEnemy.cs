using UnityEngine;

public class MeleeEnemy : Enemy
{
    public void AttackPlayer()
    {
        m_player.GetComponent<PaintResource>().Damage(damage / 2);
    }
}
