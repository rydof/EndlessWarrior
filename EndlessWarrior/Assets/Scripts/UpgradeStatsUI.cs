using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeStatsUI : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button healthUpgradeButton;
    [SerializeField] private Button damageUpgradeButton;
    [SerializeField] private Button attackSpeedUpgradeButton;
    [SerializeField] private Button continueButton;

    [Header("UI Texts")]
    [SerializeField] private TMP_Text healthDescriptionText;
    [SerializeField] private TMP_Text damageDescriptionText;
    [SerializeField] private TMP_Text attackSpeedDescriptionText;
    [SerializeField] private TMP_Text coinCountText;

    [Header("UI Panels")]
    [SerializeField] private GameObject playerUI;
    [SerializeField] private GameObject waveText;

    [Header("Player References")]
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private PlayerCombat playerCombat;
    [SerializeField] private PlayerCoins playerCoins;

    [Header("Wave Manager")]
    [SerializeField] private WaveManager waveManager;

    private int healthUpgradeLevel = 0;
    private int damageUpgradeLevel = 0;
    private int attackSpeedUpgradeLevel = 0;

    private const int baseCost = 10;
    private const float costMultiplier = 1.5f;

    private void Start()
    {
        UpdateUI();

        if (healthUpgradeButton != null)
            healthUpgradeButton.onClick.AddListener(UpgradeHealth);
        if (damageUpgradeButton != null)
            damageUpgradeButton.onClick.AddListener(UpgradeDamage);
        if (attackSpeedUpgradeButton != null)
            attackSpeedUpgradeButton.onClick.AddListener(UpgradeAttackSpeed);
        if (continueButton != null)
            continueButton.onClick.AddListener(CloseUpgradeUI);

        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (healthUpgradeButton != null)
            healthUpgradeButton.onClick.RemoveAllListeners();
        if (damageUpgradeButton != null)
            damageUpgradeButton.onClick.RemoveAllListeners();
        if (attackSpeedUpgradeButton != null)
            attackSpeedUpgradeButton.onClick.RemoveAllListeners();
        if (continueButton != null)
            continueButton.onClick.RemoveAllListeners();
    }

    public void OpenUpgradeUI()
    {
        gameObject.SetActive(true);
        playerUI?.SetActive(false);
        waveText?.SetActive(false);
        if (coinCountText != null)
            coinCountText.text = $"Balanță: {playerCoins.Money}";
        Time.timeScale = 0;
    }

    private void CloseUpgradeUI()
    {
        gameObject.SetActive(false);
        playerUI?.SetActive(true);
        waveText?.SetActive(true);
        Time.timeScale = 1;
        if (waveManager != null)
            waveManager.BeginNextWave();
    }

    private int CalculateUpgradeCost(int level)
    {
        return Mathf.CeilToInt(baseCost * Mathf.Pow(costMultiplier, level));
    }

    private void UpgradeHealth()
    {
        int cost = CalculateUpgradeCost(healthUpgradeLevel);
        if (playerCoins.Money >= cost)
        {
            playerCoins.Money -= cost;
            playerHealth.maxHealth += 2;
            playerHealth.FullyHeal();
            healthUpgradeLevel++;
            UpdateUI();
        }
    }

    private void UpgradeDamage()
    {
        int cost = CalculateUpgradeCost(damageUpgradeLevel);
        if (playerCoins.Money >= cost)
        {
            playerCoins.Money -= cost;
            playerCombat.Damage += 1;
            damageUpgradeLevel++;
            UpdateUI();
        }
    }

    private void UpgradeAttackSpeed()
    {
        int cost = CalculateUpgradeCost(attackSpeedUpgradeLevel);
        if (playerCoins.Money >= cost)
        {
            playerCoins.Money -= cost;
            playerCombat.AttackSpeed += 0.2f;
            attackSpeedUpgradeLevel++;
            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        if (coinCountText != null)
            coinCountText.text = $"Balanță: {playerCoins.Money}";

        if (healthDescriptionText != null)
        {
            healthDescriptionText.text = $"Crește viața maximă: {playerHealth.maxHealth} -> {playerHealth.maxHealth + 2}";
            TMP_Text btnText = healthUpgradeButton.GetComponentInChildren<TMP_Text>();
            if (btnText != null)
                btnText.text = $"Cost: {CalculateUpgradeCost(healthUpgradeLevel)}¢";
        }

        if (damageDescriptionText != null)
        {
            damageDescriptionText.text = $"Crește puterea atacului: {playerCombat.Damage} -> {playerCombat.Damage + 1}";
            TMP_Text btnText = damageUpgradeButton.GetComponentInChildren<TMP_Text>();
            if (btnText != null)
                btnText.text = $"Cost: {CalculateUpgradeCost(damageUpgradeLevel)}¢";
        }

        if (attackSpeedDescriptionText != null)
        {
            attackSpeedDescriptionText.text = $"Crește viteza atacului: {playerCombat.AttackSpeed:F1} -> {(playerCombat.AttackSpeed + 0.2f):F1}";
            TMP_Text btnText = attackSpeedUpgradeButton.GetComponentInChildren<TMP_Text>();
            if (btnText != null)
                btnText.text = $"Cost: {CalculateUpgradeCost(attackSpeedUpgradeLevel)}¢";
        }
    }
}
