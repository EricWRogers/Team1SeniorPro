using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager instance { get; private set; }
    public int PlayerCurrency;

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
    void Update()
    {
        PlayerCurrency = m_player.GetComponent<PlayerCurrency>().pigment;
    }

    public void HealthUpgrade()
    {
        if (m_player.GetComponent<PlayerCurrency>().pigment > healthCost)
        {
            increaseHealthTotal += increaseHealth;
            m_player.GetComponent<PlayerCurrency>().RemovePigment(healthCost);
            healthCost++;
        }
        
    }
    public void DamageUpgrade()
    {
        if (m_player.GetComponent<PlayerCurrency>().pigment > dmgCost)
        {
            increaseDmg += increaseDmgTotal;
            m_player.GetComponent<PlayerCurrency>().RemovePigment(dmgCost);
            dmgCost++;
        }
        
    }
    public void RadiusUpgrade()
    {
        if (m_player.GetComponent<PlayerCurrency>().pigment > radiusCost)
        {
            increaseRadius += increaseRadiusTotal;
            m_player.GetComponent<PlayerCurrency>().RemovePigment(radiusCost);
            radiusCost++;
        }
        
    }
    public void UsageUpgrade()
    {
        if (m_player.GetComponent<PlayerCurrency>().pigment > usageCost)
        {
            decreaseUsage += decreaseUsageTotal;
            m_player.GetComponent<PlayerCurrency>().RemovePigment(usageCost);
            usageCost++;
        }
        
    }
}
