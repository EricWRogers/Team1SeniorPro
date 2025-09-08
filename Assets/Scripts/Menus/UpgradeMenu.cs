using UnityEngine;
using UnityEngine.SceneManagement;
public class UpgradeMenu : MonoBehaviour
{
    private UpgradeManager m_UM;
    private GameObject m_player;

    void Start()
    {
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<LoseScreen>().gameOverUI = gameObject.transform.GetChild(0).gameObject;
        m_player = GameObject.FindGameObjectWithTag("Player");
        m_UM = GameObject.FindGameObjectWithTag("GameManager").GetComponent<UpgradeManager>();
    }
    public void HealthUpgrade()
    {
        if (m_player.GetComponent<PlayerCurrency>().pigment >= m_UM.healthCost)
        {
            m_UM.increaseHealthTotal += m_UM.increaseHealth;
            m_player.GetComponent<PlayerCurrency>().RemovePigment(m_UM.healthCost);
            m_UM.healthCost++;
        }

    }
    public void DamageUpgrade()
    {
        if (m_player.GetComponent<PlayerCurrency>().pigment >= m_UM.dmgCost)
        {
            m_UM.increaseDmgTotal += m_UM.increaseDmg;
            m_player.GetComponent<PlayerCurrency>().RemovePigment(m_UM.dmgCost);
            m_UM.dmgCost++;
        }

    }
    public void RadiusUpgrade()
    {
        if (m_player.GetComponent<PlayerCurrency>().pigment >= m_UM.radiusCost)
        {
            m_UM.increaseRadiusTotal += m_UM.increaseRadius;
            m_player.GetComponent<PlayerCurrency>().RemovePigment(m_UM.radiusCost);
            m_UM.radiusCost++;
        }

    }
    public void UsageUpgrade()
    {
        if (m_player.GetComponent<PlayerCurrency>().pigment >= m_UM.usageCost)
        {
            m_UM.decreaseUsageTotal += m_UM.decreaseUsage;
            m_player.GetComponent<PlayerCurrency>().RemovePigment(m_UM.usageCost);
            m_UM.usageCost++;
        }

    }
    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void Quit()
    {
        Application.Quit();
    }
}
