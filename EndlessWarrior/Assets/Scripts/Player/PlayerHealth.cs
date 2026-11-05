using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int currentHealth;
    public int maxHealth;
    public Animator animator;

    public delegate void HealthChanged(int currentHealth, int maxHealth);
    public event HealthChanged OnHealthChanged;

    private PlayerMovement playerMovement;
    [SerializeField] private WaveManager waveManager;
    private Rigidbody2D rb;
    private Collider2D playerCollider2D;
    [SerializeField] private Vector3 respawnPosition = Vector3.zero;
    private bool isDead = false;

    [Header("Damage Invincibility")]
    [SerializeField] private float damageInvincibilityDuration = 0.5f;
    private float damageInvincibilityTimer = 0f;

    private void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody2D>();
        playerCollider2D = GetComponent<Collider2D>();

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        animator?.SetFloat("health", currentHealth);
    }

    private void Update()
    {
        if (damageInvincibilityTimer > 0f)
            damageInvincibilityTimer -= Time.deltaTime;
    }

    public void ChangeHealth(int amount)
    {
        if (amount < 0 && damageInvincibilityTimer > 0f)
            return;

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (amount < 0)
            damageInvincibilityTimer = damageInvincibilityDuration;

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        animator?.SetFloat("health", currentHealth);

        if (currentHealth <= 0)
            Die();
    }

    public void FullyHeal()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        animator?.SetFloat("health", currentHealth);
    }

    public void RespawnPlayer()
    {
        if (!isDead) return;

        transform.position = respawnPosition;
        currentHealth = maxHealth;
        playerMovement.enabled = true;
        rb.bodyType = RigidbodyType2D.Dynamic;
        playerCollider2D.enabled = true;
        isDead = false;

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        animator?.SetFloat("health", currentHealth);
    }

    private void Die()
    {
        isDead = true;

        waveManager?.HandlePlayerDeath();

        playerMovement.enabled = false;
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Static;
        playerCollider2D.enabled = false;
    }
}
