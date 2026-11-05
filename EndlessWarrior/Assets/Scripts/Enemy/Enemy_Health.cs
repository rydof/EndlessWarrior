using System.Collections;
using UnityEngine;

public class Enemy_Health : MonoBehaviour
{
    [Header("Health Settings")]
    public int currentHealth;
    public int maxHealth;
    public float knockbackStrength = 2f;

    [Header("Coin Drop Settings")]
    public GameObject coinPrefab;
    public int coinsDropped = 5;

    [Header("Visual Settings")]
    public Collider2D playableAreaCollider;
    public float colorChangeDuration = 0.2f;

    [Header("Upgrade Settings")]
    public float sizeIncreasePerUpgrade = 0.2f;
    public int additionalHealthPerUpgrade = 10;

    private Animator anim;
    private bool isDead = false;
    private bool isDamaged = false;
    private SpriteRenderer[] spriteRenderers;
    private Color[] originalColors;
    private Vector3 originalScale;
    private int originalMaxHealth;

    private void Awake()
    {
        originalScale = transform.localScale;
        if (originalScale == Vector3.zero)
        {
            Debug.LogWarning("Original scale was zero in Awake. Using fallback Vector3.one.");
            originalScale = Vector3.one;
        }
    }

    private void Start()
    {
        InitializePlayableArea();
        InitializeSpriteRenderers();
        InitializeHealth();
        anim = GetComponent<Animator>();
    }

    private void InitializePlayableArea()
    {
        if (playableAreaCollider == null)
        {
            GameObject playableAreaObj = GameObject.FindWithTag("PlayableArea");
            if (playableAreaObj != null)
            {
                playableAreaCollider = playableAreaObj.GetComponent<Collider2D>();
            }
        }
    }

    private void InitializeSpriteRenderers()
    {
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        if (spriteRenderers == null || spriteRenderers.Length == 0)
        {
            Debug.LogError("Enemy_Health: No SpriteRenderer found!");
            return;
        }

        originalColors = new Color[spriteRenderers.Length];
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            Color col = spriteRenderers[i].color;
            col.a = 1f;
            spriteRenderers[i].color = col;
            originalColors[i] = col;
            spriteRenderers[i].sortingOrder = 10;
        }
    }

    private void InitializeHealth()
    {
        originalMaxHealth = maxHealth;
        currentHealth = maxHealth;
    }

    public void ChangeHealth(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (spriteRenderers != null && currentHealth > 0 && !isDamaged)
        {
            StartCoroutine(FlashGrey());
        }

        if (currentHealth <= 0 && !isDead)
        {
            StartDeathSequence();
        }
    }

    private IEnumerator FlashGrey()
    {
        isDamaged = true;
        SetSpriteColors(Color.grey);
        yield return new WaitForSeconds(colorChangeDuration);
        ResetSpriteColors();
        isDamaged = false;
    }

    private void SetSpriteColors(Color color)
    {
        foreach (var sr in spriteRenderers)
        {
            sr.color = color;
        }
    }

    private void ResetSpriteColors()
    {
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            spriteRenderers[i].color = originalColors[i];
        }
    }

    private void StartDeathSequence()
    {
        isDead = true;
        DisableCollider();
        TriggerDeathAnimation();
        StartCoroutine(HandleDeath());
    }

    private void DisableCollider()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = false;
        }
    }

    private void TriggerDeathAnimation()
    {
        if (anim != null)
        {
            anim.SetBool("isDead", true);
        }
    }

    private IEnumerator HandleDeath()
    {
        float animationDuration = anim != null ? anim.GetCurrentAnimatorStateInfo(0).length : 1f;
        yield return new WaitForSeconds(animationDuration);
        DropCoins();
        Destroy(gameObject);
    }

    private void DropCoins()
    {
        int randomCoins = Random.Range(1, coinsDropped + 1);
        for (int i = 0; i < randomCoins; i++)
        {
            Vector3 spawnPosition = GetValidSpawnPosition();
            Instantiate(coinPrefab, spawnPosition, Quaternion.identity);
        }
    }

    private Vector3 GetValidSpawnPosition()
    {
        Vector3 spawnPosition = transform.position;
        int attempts = 0;
        const int maxAttempts = 10;
        
        do
        {
            spawnPosition = transform.position + new Vector3(
                Random.Range(-0.5f, 0.5f),
                Random.Range(-0.5f, 0.5f),
                0
            );
            attempts++;
        }
        while (!IsPositionInsidePlayableArea(spawnPosition) && attempts < maxAttempts);
        
        return spawnPosition;
    }

    private bool IsPositionInsidePlayableArea(Vector3 position)
    {
        return playableAreaCollider == null || playableAreaCollider.OverlapPoint(position);
    }

    public void ApplyUpgrades(int upgradeLevel)
    {
        int clampedUpgrade = Mathf.Clamp(upgradeLevel, 1, 5);
        ApplySizeUpgrade(clampedUpgrade);
        ApplyHealthUpgrade(clampedUpgrade);
        
        Debug.Log($"Enemy upgraded - Level: {clampedUpgrade}, " +
                 $"Size: {transform.localScale}, Health: {maxHealth}");
    }

    private void ApplySizeUpgrade(int upgradeLevel)
    {
        float scaleFactor = 1 + sizeIncreasePerUpgrade * upgradeLevel;
        transform.localScale = originalScale * scaleFactor;
    }

    private void ApplyHealthUpgrade(int upgradeLevel)
    {
        maxHealth = originalMaxHealth + additionalHealthPerUpgrade * upgradeLevel;
        currentHealth = maxHealth;
    }
}