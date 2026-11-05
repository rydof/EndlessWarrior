using UnityEngine;

public abstract class EnemyBaseState
{
    protected Enemy_Movement movement;
    protected Animator animator;

    public EnemyBaseState(Enemy_Movement movement)
    {
        this.movement = movement;
        this.animator = movement.animator;
    }

    public abstract void Enter();
    public abstract void Exit();
    public abstract void Update();
    public virtual void FixedUpdate() { }
}

public class EnemyIdleState : EnemyBaseState
{
    public EnemyIdleState(Enemy_Movement movement) : base(movement) { }

    public override void Enter()
    {
        animator.SetBool("isIdle", true);
        ResetMovementBools();
    }

    public override void Exit()
    {
        animator.SetBool("isIdle", false);
    }

    public override void Update()
    {
        movement.CheckMovementDirection();
    }

    private void ResetMovementBools()
    {
        animator.SetBool("isMovingUp", false);
        animator.SetBool("isMovingDown", false);
        animator.SetBool("isMovingSide", false);
    }
}

public class EnemyUpMovementState : EnemyBaseState
{
    public EnemyUpMovementState(Enemy_Movement movement) : base(movement) { }

    public override void Enter()
    {
        animator.SetBool("isMovingUp", true);
        animator.SetBool("isMovingDown", false);
        animator.SetBool("isMovingSide", false);
    }

    public override void Exit()
    {
        animator.SetBool("isMovingUp", false);
    }

    public override void Update()
    {
        movement.CheckMovementDirection();
    }
}

public class EnemyDownMovementState : EnemyBaseState
{
    public EnemyDownMovementState(Enemy_Movement movement) : base(movement) { }

    public override void Enter()
    {
        animator.SetBool("isMovingDown", true);
        animator.SetBool("isMovingUp", false);
        animator.SetBool("isMovingSide", false);
    }

    public override void Exit()
    {
        animator.SetBool("isMovingDown", false);
    }

    public override void Update()
    {
        movement.CheckMovementDirection();
    }
}

public class EnemySideMovementState : EnemyBaseState
{
    public EnemySideMovementState(Enemy_Movement movement) : base(movement) { }

    public override void Enter()
    {
        animator.SetBool("isMovingSide", true);
        animator.SetBool("isMovingUp", false);
        animator.SetBool("isMovingDown", false);
    }

    public override void Exit()
    {
        animator.SetBool("isMovingSide", false);
    }

    public override void Update()
    {
        movement.CheckMovementDirection();
    }
}

public class EnemyAttackState : EnemyBaseState
{
    private float attackDuration = 1f;
    private float attackTimer;

    public EnemyAttackState(Enemy_Movement movement) : base(movement) { }

    public override void Enter()
    {
        attackTimer = attackDuration;
        SetAttackDirection();
    }

    public override void Update()
    {
        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0)
            movement.SetState(new EnemyIdleState(movement));
    }

    public override void Exit()
    {
        animator.SetBool("isAttackingUp", false);
        animator.SetBool("isAttackingDown", false);
        animator.SetBool("isAttackingSide", false);
    }

    private void SetAttackDirection()
    {
        if (movement.player == null) return;

        Vector2 direction = ((Vector2)movement.player.position - (Vector2)movement.transform.position).normalized;
        
        if (Mathf.Abs(direction.y) > Mathf.Abs(direction.x))
        {
            animator.SetBool(direction.y > 0 ? "isAttackingUp" : "isAttackingDown", true);
        }
        else
        {
            animator.SetBool("isAttackingSide", true);
        }
    }
}