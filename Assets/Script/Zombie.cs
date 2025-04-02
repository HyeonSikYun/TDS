using UnityEngine;
using System.Collections;

public class Zombie : MonoBehaviour
{
    [Header("���� ����")]
    public Transform truck;  // Ʈ���� ��ġ�� ����
    public float moveSpeed = 8f;  // ���� �̵� �ӵ�
    public float stopDistance = 2f;  // Ʈ������ �Ÿ��� 2 ������ �� 
    public float jumpForce = 10f;   // ���� ��
    public float jumpCooldown = 1f; // ���� ��ٿ�
    public float zombieDetectionRange = 0.5f; // ���� ���� ����
    public float jumpTriggerDistance = 5f; // Ʈ������ �Ÿ��� �� �� ������ ���� ����
    public float bounceForce = 5f; // ���� �浹 �� ƨ�ܳ��� ��
    public float jumpKnockbackForce = 3f; // ���� �� �Ʒ� ���� �о�� ��

    private Rigidbody2D rb;
    private Animator anim;
    private bool isJumping = false;
    private bool canJump = true;
    private float groundCheckDistance = 0.2f; // �ٴ� üũ �Ÿ�
    private bool reachedTruck = false; // Ʈ���� �����ߴ��� ����
    private Vector2 lastPosition; // ��ü ������ ���� ������ ��ġ
    private float stuckTime = 0f; // ���� ��ü�� �ð�
    private bool gravityEnabled = false; // �߷��� Ȱ��ȭ�Ǿ����� ����

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        rb.gravityScale = 0f;
        gravityEnabled = false;

        lastPosition = transform.position;
    }

    void Update()
    {
        if (truck != null)
        {
            if (!reachedTruck || !IsInContactWithTruck())
            {
                CheckIfStuck();

                float distanceToTruck = Mathf.Abs(truck.position.x - transform.position.x);

                if (distanceToTruck <= jumpTriggerDistance)
                {
                    CheckForZombieInFront();
                }

                MoveTowardsTruck();
            }

            if (gravityEnabled)
            {
                CheckGround();
            }
        }
    }

    void CheckIfStuck()
    {
        float movementDelta = Vector2.Distance(transform.position, lastPosition);

        if (movementDelta < 0.01f && !isJumping && !reachedTruck)
        {
            stuckTime += Time.deltaTime;

            if (stuckTime > 0.5f && canJump)
            {
                EnableGravity();
                Jump();
                stuckTime = 0f;
            }
        }
        else
        {
            stuckTime = 0f;
        }

        lastPosition = transform.position;
    }

    void MoveTowardsTruck()
    {
        Vector3 direction = (truck.position - transform.position).normalized;

        float distanceToTruck = Mathf.Abs(truck.position.x - transform.position.x);

        if (distanceToTruck > stopDistance && !isJumping)
        {
            rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);
            reachedTruck = false;
        }
        else if (distanceToTruck <= stopDistance && !isJumping)
        {
            if (anim != null)
            {
                anim.SetBool("IsAttacking", true);
            }

            rb.velocity = new Vector2(0, rb.velocity.y);
        }
    }

    void CheckForZombieInFront()
    {
        float direction = (truck.position.x > transform.position.x) ? 1f : -1f;

        RaycastHit2D hit = Physics2D.Raycast(
            transform.position + new Vector3(direction * 0.5f, 0, 0),
            new Vector2(direction, 0),
            zombieDetectionRange
        );

        if (hit.collider != null && canJump && !isJumping)
        {
            EnableGravity();
            Jump();
        }
    }

    void EnableGravity()
    {
        if (!gravityEnabled)
        {
            rb.gravityScale = 1f;
            gravityEnabled = true;
        }
    }

    void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);

        isJumping = true;
        canJump = false;

        PushZombiesBelow();

        StartCoroutine(JumpCooldown());
    }

    void PushZombiesBelow()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(
            transform.position,
            Vector2.down,
            1.0f
        );

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null && hit.collider.gameObject != gameObject)
            {
                Rigidbody2D otherRb = hit.collider.GetComponent<Rigidbody2D>();
                if (otherRb != null)
                {
                    float direction = (truck.position.x > transform.position.x) ? -1f : 1f;
                    otherRb.AddForce(new Vector2(direction * jumpKnockbackForce, 0), ForceMode2D.Impulse);
                }
            }
        }
    }

    IEnumerator JumpCooldown()
    {
        yield return new WaitForSeconds(jumpCooldown);
        canJump = true;
    }

    void CheckGround()
    {
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            Vector2.down,
            groundCheckDistance
        );

        if (hit.collider != null && isJumping)
        {
            isJumping = false;
        }
    }

    bool IsInContactWithTruck()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.5f);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Truck"))
            {
                return true;
            }
        }
        return false;
    }

    void OnAttack()
    {
        
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Truck"))
        {
            reachedTruck = true;
            rb.gravityScale = 0.2f;
            rb.velocity = Vector2.zero;
            if (anim != null)
            {
                anim.SetBool("IsAttacking", true);
            }
        }

        else if (IsZombie(collision.gameObject) && isJumping)
        {
            
            Rigidbody2D otherRb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (otherRb != null)
            {
                
                ContactPoint2D contact = collision.contacts[0];
                Vector2 bounceDirection = contact.normal;

                
                otherRb.AddForce(bounceDirection * bounceForce, ForceMode2D.Impulse);

                rb.AddForce(new Vector2((truck.position.x > transform.position.x) ? 1 : -1, 0.5f) * (bounceForce * 0.5f), ForceMode2D.Impulse);
            }
        }
    }

    bool IsZombie(GameObject obj)
    {
        return obj.layer == gameObject.layer;
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Truck"))
        {
            reachedTruck = false;

            if (anim != null)
            {
                anim.SetBool("IsAttacking", false);
            }
        }
    }
}