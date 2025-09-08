using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager instance { get; private set; }

    [Header("Upgrade Amounts")]
    public float increaseHealth;
    public int healthCost;
    public float increaseDmg;
    public int dmgCost;
    public float increaseRadius;
    public int radiusCost;
    public float decreaseUsage;
    public int usageCost;

    [Header("Upgrades Total")]
    public float increaseHealthTotal;
    public float increaseDmgTotal;
    public float increaseRadiusTotal;
    public float decreaseUsageTotal;

    private GameObject m_player;

    public void Awake()
    {
        DontDestroyOnLoad(gameObject);
        // Make an instance
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
            return;
        }
        
        m_player = GameObject.FindGameObjectWithTag("Player");
    }

    public void HealthUpgrade()
    {
        increaseHealthTotal += increaseHealth;
        m_player.GetComponent<PlayerCurrency>().RemovePigment(healthCost);
        healthCost++;
    }
    public void DamageUpgrade()
    {
        increaseDmg += increaseDmgTotal;
        m_player.GetComponent<PlayerCurrency>().RemovePigment(dmgCost);
        dmgCost++;
    }
    public void RadiusUpgrade()
    {
        increaseRadius += increaseRadiusTotal;
        m_player.GetComponent<PlayerCurrency>().RemovePigment(radiusCost);
        radiusCost++;
    }
    public void UsageUpgrade()
    {
        decreaseUsage += decreaseUsageTotal;
        m_player.GetComponent<PlayerCurrency>().RemovePigment(usageCost);
        usageCost++;
    }
}
