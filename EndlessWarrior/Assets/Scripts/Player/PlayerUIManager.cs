using UnityEngine;
using TMPro;

public class PlayerUIManager : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private PlayerCombat playerCombat;
    [SerializeField] private PlayerCoins playerCoins;

    [SerializeField] private TMP_Text healthText;
    [SerializeField] private TMP_Text attackSpeedText;
    [SerializeField] private TMP_Text attackText;
    [SerializeField] private TMP_Text coinText;

    private void OnEnable()
    {
        SubscribeToEvents();
        InitializeUI();
    }

    private void OnDisable()
    {
        UnsubscribeFromEvents();
    }

    private void Start()
    {
        VerifyReferences();
    }

    private void SubscribeToEvents()
    {
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged += UpdateHealthUI;
        }

        if (playerCombat != null)
        {
            playerCombat.OnDamageChanged += UpdateAttackUI;
            playerCombat.OnAttackSpeedChanged += UpdateAttackSpeedUI;
        }

        if (playerCoins != null)
        {
            playerCoins.OnMoneyChanged += UpdateMoneyUI;
        }
    }

    private void UnsubscribeFromEvents()
    {
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged -= UpdateHealthUI;
        }

        if (playerCombat != null)
        {
            playerCombat.OnDamageChanged -= UpdateAttackUI;
            playerCombat.OnAttackSpeedChanged -= UpdateAttackSpeedUI;
        }

        if (playerCoins != null)
        {
            playerCoins.OnMoneyChanged -= UpdateMoneyUI;
        }
    }

    private void InitializeUI()
    {
        if (playerHealth != null)
        {
            UpdateHealthUI(playerHealth.currentHealth, playerHealth.maxHealth);
        }

        if (playerCombat != null)
        {
            UpdateAttackUI(playerCombat.Damage);
            UpdateAttackSpeedUI(playerCombat.AttackSpeed);
        }

        if (playerCoins != null)
        {
            UpdateMoneyUI(playerCoins.Money);
        }
    }

    private void VerifyReferences()
    {
        if (playerHealth == null) Debug.LogError("PlayerHealth reference is missing!");
        if (playerCombat == null) Debug.LogError("PlayerCombat reference is missing!");
        if (playerCoins == null) Debug.LogError("PlayerCoins reference is missing!");
    }

    public void UpdateHealthUI(int currentHealth, int maxHealth)
    {
        if (healthText != null)
        {
            healthText.text = $": {currentHealth} / {maxHealth}";
        }
    }

    private void UpdateAttackUI(int attackDamage)
    {
        if (attackText != null)
        {
            attackText.text = $": {attackDamage}";
        }
    }

    private void UpdateAttackSpeedUI(float currentAttackSpeed)
    {
        if (attackSpeedText != null)
        {
            attackSpeedText.text = $": {currentAttackSpeed:F1}";
        }
    }

    private void UpdateMoneyUI(int currentCoinCount)
    {
        if (coinText != null)
        {
            coinText.text = $": {currentCoinCount}";
        }
    }
}