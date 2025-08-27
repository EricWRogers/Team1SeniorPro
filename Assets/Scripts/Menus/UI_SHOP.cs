using UnityEngine;
public class UI_SHOP : MonoBehaviour
{
    private Transform container;
    private Transform shopItemTemplate;
    private PlayerCurrency playerCurrency;

    public GameObject UPG;
    public GameObject UPG2;
    public GameObject UPGMAX;
    private void Awake()
    {
        container = transform.Find("container");
        shopItemTemplate = container.Find("shopUpgTemplate");
        shopItemTemplate.gameObject.SetActive(true);

        playerCurrency = Object.FindFirstObjectByType<PlayerCurrency>();
    }

    public void Upgrade()
    {
        if (playerCurrency == null) return;

        if (playerCurrency.pigment >= 10)
        {
            UPG.SetActive(true);
        }

        if (playerCurrency.pigment >= 50)
        {
            UPG2.SetActive(true);
        }

        if (playerCurrency.pigment >= 100)
        {
            UPGMAX.SetActive(true);
        }
        
    }
}