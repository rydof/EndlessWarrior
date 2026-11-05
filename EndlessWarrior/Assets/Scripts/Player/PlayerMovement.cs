using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public Rigidbody2D rb;
    public Animator animator;

    private float horizontal;
    private float vertical;
    private float lastHorizontal;
    private float lastVertical;
    private float originalSpeed;
    private PlayerBaseState currentState;

    private void Awake()
    {
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    private void Start()
    {
        originalSpeed = speed;
        SetState(new PlayerIdleState(this));
    }

    private void Update()
    {
        GetMovementInput();
        currentState.Update();
    }

    private void FixedUpdate()
    {
        currentState.FixedUpdate();
    }

    public void SetState(PlayerBaseState newState)
    {
        if (currentState != null)
        {
            currentState.Exit();
        }
        currentState = newState;
        currentState.Enter();
    }

    public void GetMovementInput()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        if (Mathf.Abs(horizontal) > 0.1f || Mathf.Abs(vertical) > 0.1f)
        {
            lastHorizontal = horizontal;
            lastVertical = vertical;
        }
        else
        {
            lastHorizontal = 0;
            lastVertical = 0;
        }
    }

    public void SetSlowerSpeed(float percentage)
    {
        speed = originalSpeed * percentage;
    }

    public void SetOriginalMovementSpeed()
    {
        speed = originalSpeed;
    }

    public void Move(Vector2 movement)
    {
        if (Mathf.Abs(horizontal) < 0.1f && Mathf.Abs(vertical) < 0.1f)
        {
            movement = Vector2.zero;
        }
        rb.MovePosition(rb.position + movement * speed * Time.fixedDeltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Wall"))
        {
            Debug.Log("Collision with wall verified.");
        }
    }

    public float GetHorizontal() { return horizontal; }
    public float GetVertical() { return vertical; }
    public float GetLastHorizontal() { return lastHorizontal; }
    public float GetLastVertical() { return lastVertical; }
}