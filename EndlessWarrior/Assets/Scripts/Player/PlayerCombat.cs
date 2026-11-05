using System.Collections;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Attack Settings")]
    public Transform attackPoint;
    public float weaponRange = 1f;
    public float weaponWidth = 0.5f;
    public LayerMask enemyLayer;

    [Header("Player Stats")]
    public int damage = 1;
    public float attackSpeed = 1f;

    [Header("References")]
    public Animator anim;
    private PlayerMovement playerMovement;

    public int Damage
    {
        get => damage;
        set
        {
            damage = value;
            OnDamageChanged?.Invoke(damage);
        }
    }

    public float AttackSpeed
    {
        get => attackSpeed;
        set
        {
            attackSpeed = value;
            OnAttackSpeedChanged?.Invoke(attackSpeed);
        }
    }

    public delegate void DamageChanged(int damage);
    public event DamageChanged OnDamageChanged;

    public delegate void AttackSpeedChanged(float attackSpeed);
    public event AttackSpeedChanged OnAttackSpeedChanged;

    private Vector2 attackDirection = Vector2.zero;
    private bool isAttacking = false;
    private float attackCooldown = 0f;

    private void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        OnDamageChanged?.Invoke(Damage);
        OnAttackSpeedChanged?.Invoke(AttackSpeed);
    }

    private void Update()
    {
        if (attackCooldown > 0f)
        {
            attackCooldown -= Time.deltaTime;
            anim.SetFloat("AttackCooldown", attackCooldown);
        }

        attackDirection = GetAttackDirection();

        if (attackDirection != Vector2.zero && attackCooldown <= 0f)
            TriggerAttack();
        else
            isAttacking = false;

        if (isAttacking)
            playerMovement.SetSlowerSpeed(0.5f);
        else
            playerMovement.SetOriginalMovementSpeed();
    }

    public Vector2 GetAttackDirection()
    {
        if (Input.GetKey(KeyCode.UpArrow)) return Vector2.up;
        if (Input.GetKey(KeyCode.DownArrow)) return Vector2.down;
        if (Input.GetKey(KeyCode.LeftArrow)) return Vector2.left;
        if (Input.GetKey(KeyCode.RightArrow)) return Vector2.right;
        return Vector2.zero;
    }

    public void TriggerAttack()
    {
        anim.SetTrigger("attack");
        isAttacking = true;
        attackCooldown = Mathf.Max(0.9f - attackSpeed / 5f, 0);
    }

    public void FinishAttack()
    {
        isAttacking = false;
    }

    public void PerformAttack()
    {
        anim.SetFloat("AttackSpeedMultiplier", attackSpeed);
        anim.SetFloat("attackDirectionX", attackDirection.x);
        anim.SetFloat("attackDirectionY", attackDirection.y);
        DealDamage();
    }

    private void DealDamage()
    {
        if (attackDirection == Vector2.zero) return;

        Vector2 boxSize;
        Vector2 attackPos;

        if (Mathf.Abs(attackDirection.x) > 0)
        {
            boxSize = new Vector2(weaponRange, weaponWidth);
            attackPos = (Vector2)attackPoint.position + attackDirection * (weaponRange / 2f);
        }
        else
        {
            boxSize = new Vector2(weaponWidth, attackDirection == Vector2.down ? weaponRange * 1.2f : weaponRange);
            attackPos = (Vector2)attackPoint.position + attackDirection * (attackDirection == Vector2.down ? weaponRange * 0.6f : weaponRange / 2f);
        }

        Collider2D[] enemies = Physics2D.OverlapBoxAll(attackPos, boxSize, 0f, enemyLayer);
        foreach (var enemy in enemies)
        {
            if (enemy == null) continue;

            if (enemy.TryGetComponent(out Enemy_Health enemyHealth))
            {
                enemyHealth.ChangeHealth(-Damage);
                ApplyKnockback(enemy);
                Debug.Log($"Hit {enemy.name}");
            }
        }
    }

    private void ApplyKnockback(Collider2D enemy)
    {
        if (enemy.TryGetComponent(out Rigidbody2D enemyRb))
        {
            float knockbackStrength = enemy.GetComponent<Enemy_Health>()?.knockbackStrength ?? 0f;
            Vector2 direction = (enemy.transform.position - attackPoint.position).normalized * knockbackStrength;
            StartCoroutine(KnockbackCoroutine(enemyRb, direction));
        }
    }

    private IEnumerator KnockbackCoroutine(Rigidbody2D enemyRb, Vector2 knockbackDirection)
    {
        if (enemyRb == null) yield break;

        float duration = 0.2f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            if (enemyRb == null) yield break;

            enemyRb.MovePosition(enemyRb.position + knockbackDirection * Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    private void OnDrawGizmos()
    {
        if (attackDirection == Vector2.zero) return;

        Vector2 boxSize;
        Vector2 attackPos;

        if (Mathf.Abs(attackDirection.x) > 0)
        {
            boxSize = new Vector2(weaponRange, weaponWidth);
            attackPos = (Vector2)attackPoint.position + attackDirection * (weaponRange / 2f);
        }
        else
        {
            boxSize = new Vector2(weaponWidth, attackDirection == Vector2.down ? weaponRange * 1.2f : weaponRange);
            attackPos = (Vector2)attackPoint.position + attackDirection * (attackDirection == Vector2.down ? weaponRange * 0.6f : weaponRange / 2f);
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(attackPos, boxSize);
    }
}
