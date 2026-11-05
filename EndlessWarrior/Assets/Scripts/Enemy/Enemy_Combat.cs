using System.Collections;
using UnityEngine;

public class Enemy_Combat : MonoBehaviour
{
    [Header("Attack Settings")]
    public float attackRange = 2f;
    public float attackCooldown = 1f;
    public int attackDamage = 10;

    [Header("Collision Damage Settings")]
    public float collisionDamageCooldown = 1f;
    public bool useCollisionDamage = true;

    [Header("Visual Feedback")]
    public float colorChangeDuration = 0.5f;
    public float invincibilityDuration = 0.5f;

    [Header("Upgrade Settings")]
    private int originalAttackDamage;
    public int additionalAttackDamagePerUpgrade = 5;

    private Transform player;
    private PlayerHealth playerHealth;
    private SpriteRenderer playerSprite;
    private Enemy_Movement enemyMovement;
    private Color originalColor;

    private float attackTimer;
    private float collisionDamageTimer;
    private bool isInContact;
    private bool canDealCollisionDamage = true;
    private Coroutine colorChangeCoroutine;

    private void Start()
    {
        InitializeReferences();
        originalAttackDamage = attackDamage;
    }

    private void OnDestroy()
    {
        if (playerHealth != null)
            playerHealth.OnHealthChanged -= HandlePlayerHealthChanged;
        RestorePlayerColorImmediately();
    }

    public void Update()
    {
        HandleAttackCooldown();
        HandleCollisionDamage();
    }

    private void InitializeReferences()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            playerSprite = playerObj.GetComponent<SpriteRenderer>();
            playerHealth = playerObj.GetComponent<PlayerHealth>();
            
            if (playerHealth != null)
                playerHealth.OnHealthChanged += HandlePlayerHealthChanged;
            
            if (playerSprite != null)
                originalColor = playerSprite.color;
        }
        else
        {
            Debug.LogError("Enemy_Combat: Player not found!");
        }

        enemyMovement = GetComponent<Enemy_Movement>();
    }

    private void HandleAttackCooldown()
    {
        attackTimer = Mathf.Max(attackTimer - Time.deltaTime, 0f);

        if (!useCollisionDamage && IsPlayerInRange() && attackTimer <= 0f)
        {
            attackTimer = attackCooldown;
            TriggerAttack();
        }
    }

    private void HandleCollisionDamage()
    {
        if (!canDealCollisionDamage)
        {
            collisionDamageTimer -= Time.deltaTime;
            if (collisionDamageTimer <= 0f)
                canDealCollisionDamage = true;
        }

        if (useCollisionDamage && isInContact && canDealCollisionDamage && 
            playerHealth != null && playerHealth.currentHealth > 0)
        {
            DealCollisionDamage();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && playerHealth != null && playerHealth.currentHealth > 0)
        {
            isInContact = true;
            if (useCollisionDamage && canDealCollisionDamage)
                DealCollisionDamage();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            isInContact = false;
    }

    private void DealCollisionDamage()
    {
        if (!canDealCollisionDamage || playerHealth == null) return;

        playerHealth.ChangeHealth(-attackDamage);
        canDealCollisionDamage = false;
        collisionDamageTimer = collisionDamageCooldown;

        if (playerHealth.currentHealth > 0)
            ChangePlayerColor();
    }

    private void ChangePlayerColor()
    {
        if (playerSprite == null || playerHealth == null || playerHealth.currentHealth <= 0) return;

        if (colorChangeCoroutine != null)
            StopCoroutine(colorChangeCoroutine);

        playerSprite.color = Color.red;
        colorChangeCoroutine = StartCoroutine(RestorePlayerColorRoutine());
    }

    private IEnumerator RestorePlayerColorRoutine()
    {
        float elapsedTime = 0f;
        while (elapsedTime < colorChangeDuration)
        {
            if (playerHealth != null && playerHealth.currentHealth <= 0)
                break;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        if (playerSprite != null)
            playerSprite.color = originalColor;
        colorChangeCoroutine = null;
    }

    private void RestorePlayerColorImmediately()
    {
        if (colorChangeCoroutine != null)
            StopCoroutine(colorChangeCoroutine);
        
        if (playerSprite != null)
            playerSprite.color = originalColor;
        
        colorChangeCoroutine = null;
    }

    private bool IsPlayerInRange()
    {
        if (player == null) return false;
        float sqrDistance = (player.position - transform.position).sqrMagnitude;
        return sqrDistance <= attackRange * attackRange;
    }

    private void TriggerAttack()
    {
        if (enemyMovement != null)
            enemyMovement.SetState(new EnemyAttackState(enemyMovement));
    }

    public void ApplyDamage()
    {
        if (IsPlayerInRange() && playerHealth != null && playerHealth.currentHealth > 0)
        {
            playerHealth.ChangeHealth(-attackDamage);
            if (playerHealth.currentHealth > 0)
                ChangePlayerColor();
        }
    }

    public void ApplyUpgrades(int upgradeLevel)
    {
        int clampedUpgrade = Mathf.Clamp(upgradeLevel, 1, 5);
        attackDamage = originalAttackDamage + additionalAttackDamagePerUpgrade * clampedUpgrade;
        Debug.Log($"Enemy upgraded: Level {clampedUpgrade}, Damage {attackDamage}");
    }

    private void HandlePlayerHealthChanged(int currentHealth, int maxHealth)
    {
        if (currentHealth <= 0)
            RestorePlayerColorImmediately();
    }
}