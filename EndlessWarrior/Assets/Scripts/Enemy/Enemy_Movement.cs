using UnityEngine;

public class Enemy_Movement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 2f;
    public int facingDirection = 1;

    [Header("References")]
    public Transform player;
    public Animator animator;

    private Rigidbody2D rb;
    private EnemyBaseState currentState;
    private Enemy_Combat combat;

    private void Start()
    {
        InitializeComponents();
        SetInitialState();
        EnsureCorrectFacing();
    }

    private void Update()
    {
        if (!(currentState is EnemyAttackState))
        {
            combat.Update();
        }
        currentState.Update();
    }

    private void InitializeComponents()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        combat = GetComponent<Enemy_Combat>();
        player = GameObject.FindWithTag("Player")?.transform;
        
        if (player == null)
        {
            Debug.LogError("Player not found! Ensure the Player GameObject is tagged 'Player'.");
        }
    }

    private void SetInitialState()
    {
        SetState(new EnemyIdleState(this));
    }

    private void EnsureCorrectFacing()
    {
        if ((facingDirection == -1 && transform.localScale.x > 0) ||
            (facingDirection == 1 && transform.localScale.x < 0))
        {
            Flip();
        }
    }

    public void SetState(EnemyBaseState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }

    public void CheckMovementDirection()
    {
        if (player == null) return;

        Vector2 direction = ((Vector2)player.position - rb.position).normalized;
        HandleMovementState(direction);
        MoveTowardsPlayer(direction);
        UpdateFacingDirection(direction);
    }

    private void HandleMovementState(Vector2 direction)
    {
        if (Mathf.Abs(direction.y) > Mathf.Abs(direction.x))
        {
            SetState(direction.y > 0 ? 
                new EnemyUpMovementState(this) : 
                new EnemyDownMovementState(this));
        }
        else
        {
            SetState(new EnemySideMovementState(this));
        }
    }

    private void MoveTowardsPlayer(Vector2 direction)
    {
        Vector2 targetPosition = rb.position + direction * speed * Time.fixedDeltaTime;
        rb.MovePosition(targetPosition);
    }

    private void UpdateFacingDirection(Vector2 direction)
    {
        if ((direction.x > 0 && transform.localScale.x < 0) || 
            (direction.x < 0 && transform.localScale.x > 0))
        {
            Flip();
        }
    }

    private void Flip()
    {
        facingDirection *= -1;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}