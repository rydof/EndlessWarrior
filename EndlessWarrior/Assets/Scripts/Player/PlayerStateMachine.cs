using UnityEngine;

public abstract class PlayerBaseState
{
    protected PlayerMovement playerMovement;
    protected Animator animator;

    public PlayerBaseState(PlayerMovement playerMovement)
    {
        this.playerMovement = playerMovement;
        this.animator = playerMovement.animator;
    }

    public abstract void Enter();
    public abstract void Exit();
    public abstract void Update();
    public virtual void FixedUpdate() { }

    protected void HandleAttackInput()
    {
        Vector2 attackDirection = playerMovement.GetComponent<PlayerCombat>().GetAttackDirection();
        if (attackDirection != Vector2.zero)
            playerMovement.SetState(new PlayerAttackingState(playerMovement, attackDirection));
    }

    protected void UpdateAnimatorMovement(Vector2 movement)
    {
        animator.SetFloat("horizontal", movement.x);
        animator.SetFloat("vertical", movement.y);
        animator.SetFloat("speed", movement.magnitude);
    }
}

public class PlayerIdleState : PlayerBaseState
{
    public PlayerIdleState(PlayerMovement playerMovement) : base(playerMovement) { }

    public override void Enter()
    {
        Vector2 lastDirection = new Vector2(playerMovement.GetLastHorizontal(), playerMovement.GetLastVertical());
        UpdateAnimatorMovement(lastDirection);
        animator.SetFloat("speed", 0f);
    }

    public override void Update()
    {
        if (Mathf.Abs(playerMovement.GetLastHorizontal()) > 0.1f || 
            Mathf.Abs(playerMovement.GetLastVertical()) > 0.1f)
        {
            playerMovement.SetState(new PlayerMovingState(playerMovement));
        }
        HandleAttackInput();
    }

    public override void Exit() { }
}

public class PlayerMovingState : PlayerBaseState
{
    public PlayerMovingState(PlayerMovement playerMovement) : base(playerMovement) { }

    public override void Enter() { }

    public override void Update()
    {
        if (playerMovement.GetHorizontal() == 0 && playerMovement.GetVertical() == 0)
            playerMovement.SetState(new PlayerIdleState(playerMovement));
        
        HandleAttackInput();
    }

    public override void FixedUpdate()
    {
        Vector2 movement = new Vector2(playerMovement.GetHorizontal(), playerMovement.GetVertical()).normalized;
        playerMovement.Move(movement);
        UpdateAnimatorMovement(movement);
    }

    public override void Exit() { }
}

public class PlayerAttackingState : PlayerBaseState
{
    private float attackTimer;
    private Vector2 attackDirection;

    public PlayerAttackingState(PlayerMovement playerMovement, Vector2 direction) : base(playerMovement)
    {
        attackDirection = direction;
    }

    public override void Enter()
    {
        animator.SetTrigger("attack");
        animator.SetFloat("attackDirectionX", attackDirection.x);
        animator.SetFloat("attackDirectionY", attackDirection.y);
        
        attackTimer = animator.GetFloat("AttackCooldown");
        if (attackTimer <= 0)
            playerMovement.GetComponent<PlayerCombat>().PerformAttack();
    }

    public override void Exit()
    {
        animator.ResetTrigger("attack");
    }

    public override void Update()
    {
        if (Mathf.Abs(playerMovement.GetHorizontal()) > 0.1f || 
            Mathf.Abs(playerMovement.GetVertical()) > 0.1f)
        {
            playerMovement.SetState(new PlayerMovingState(playerMovement));
        }
        else
        {
            playerMovement.SetState(new PlayerIdleState(playerMovement));
        }
        
        HandleAttackInput();
    }

    public override void FixedUpdate()
    {
        Vector2 movement = new Vector2(playerMovement.GetHorizontal(), playerMovement.GetVertical()).normalized;
        playerMovement.Move(movement);
    }
}