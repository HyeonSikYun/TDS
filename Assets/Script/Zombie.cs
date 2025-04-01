using UnityEngine;
using System.Collections;

public class Zombie : MonoBehaviour
{
    public Transform truck;  // Ʈ���� ��ġ�� ����
    public float moveSpeed = 8f;  // ���� �̵� �ӵ�
    public float stopDistance = 2f;  // Ʈ������ �Ÿ��� 2 ������ �� ���� �ִϸ��̼� ����
    public float jumpForce = 10f;   // ���� ��
    public float jumpCooldown = 1f; // ���� ��ٿ�
    public LayerMask zombieLayerMask; // ���� ���̾� ���� ������
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

        // ó������ �߷� 0���� ����
        rb.gravityScale = 0f;
        gravityEnabled = false;

        // ���� ������ ���̾ Ȯ���ؼ� ���� ���̾��ũ ����
        if (gameObject.layer == LayerMask.NameToLayer("Zombie"))
        {
            zombieLayerMask = 1 << LayerMask.NameToLayer("Zombie");
        }
        else if (gameObject.layer == LayerMask.NameToLayer("Enemies"))
        {
            zombieLayerMask = 1 << LayerMask.NameToLayer("Enemies");
        }

        lastPosition = transform.position;
    }

    void Update()
    {
        // Ʈ���� �����Ǿ� ���� ��츸 ����
        if (truck != null)
        {
            // Ʈ���� �������� �ʾҰų� �������ٸ� ��� �̵�
            if (!reachedTruck || !IsInContactWithTruck())
            {
                // ��ü ����
                CheckIfStuck();

                // Ʈ������ �Ÿ� ��� (y���� ������ x�� ��)
                float distanceToTruck = Mathf.Abs(truck.position.x - transform.position.x);

                // Ʈ���� ����� ���� �տ� ���� ���̾��� ���� �ִ��� Ȯ��
                if (distanceToTruck <= jumpTriggerDistance)
                {
                    CheckForZombieInFront();
                }

                // Ʈ�� �������� �̵�
                MoveTowardsTruck();
            }

            // �ٴ� üũ (�̹� �߷��� Ȱ��ȭ�� ��츸)
            if (gravityEnabled)
            {
                CheckGround();
            }
        }
    }

    void CheckIfStuck()
    {
        // ���� ��ġ�� ������ ��ġ�� ���� ���
        float movementDelta = Vector2.Distance(transform.position, lastPosition);

        // ���� �������� �ʾҴٸ� (��ü ����)
        if (movementDelta < 0.01f && !isJumping && !reachedTruck)
        {
            stuckTime += Time.deltaTime;

            // ���� �ð� �̻� ��ü�Ǹ� ���� �õ�
            if (stuckTime > 0.5f && canJump)
            {
                EnableGravity();  // �߷� Ȱ��ȭ
                Jump();
                stuckTime = 0f;
            }
        }
        else
        {
            stuckTime = 0f;
        }

        // ������ ��ġ ������Ʈ
        lastPosition = transform.position;
    }

    void MoveTowardsTruck()
    {
        // Ʈ������ ���ϴ� ���� ����
        Vector3 direction = (truck.position - transform.position).normalized;

        // Ʈ������ �Ÿ� ��� (y���� ������ x�� ��)
        float distanceToTruck = Mathf.Abs(truck.position.x - transform.position.x);

        // ���� ���� �ƴϰ� Ʈ������ �Ÿ��� stopDistance���� Ŭ ��쿡�� �̵�
        if (distanceToTruck > stopDistance && !isJumping)
        {
            // Rigidbody2D�� �ӵ��� ����Ͽ� ������ �ӵ��� �̵�
            rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);
            reachedTruck = false;
        }
        else if (distanceToTruck <= stopDistance && !isJumping)
        {
            // ���� �ִϸ��̼� ����
            if (anim != null)
            {
                anim.SetBool("IsAttacking", true);
            }

            // �̵��� ���߱� ���� x�� �ӵ��� 0���� ����
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
    }

    void CheckForZombieInFront()
    {
        // ���� ���⿡ ���� ����ĳ��Ʈ ���� ����
        float direction = (truck.position.x > transform.position.x) ? 1f : -1f;

        // ���� �տ� �ٸ� ���� ���̾� ���� �ִ��� Ȯ��
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position + new Vector3(direction * 0.5f, 0, 0),
            new Vector2(direction, 0),
            zombieDetectionRange,
            zombieLayerMask
        );

        // �տ� ���� ���̾� ���� �ְ�, ���� �����ϸ� ����
        if (hit.collider != null && canJump && !isJumping)
        {
            EnableGravity();  // �߷� Ȱ��ȭ
            Jump();
        }
    }

    void EnableGravity()
    {
        // ���� �߷��� Ȱ��ȭ���� �ʾҴٸ�
        if (!gravityEnabled)
        {
            rb.gravityScale = 1f;
            gravityEnabled = true;
        }
    }

    void Jump()
    {
        // ���� ����
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);

        // ���� ���� ����
        isJumping = true;
        canJump = false;

        // �ٴ��� ���� �о�� (�Ʒ��� �ִ� ���� üũ)
        PushZombiesBelow();

        // ���� ��ٿ� ����
        StartCoroutine(JumpCooldown());
    }

    void PushZombiesBelow()
    {
        // �Ʒ��� �ִ� ����� ����
        RaycastHit2D[] hits = Physics2D.RaycastAll(
            transform.position,
            Vector2.down,
            1.0f,
            zombieLayerMask
        );

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null && hit.collider.gameObject != gameObject)
            {
                // �Ʒ� ������ Rigidbody2D ��������
                Rigidbody2D otherRb = hit.collider.GetComponent<Rigidbody2D>();
                if (otherRb != null)
                {
                    // Ʈ�� �ݴ� �������� �о��
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
        // �ٴ� üũ (���� �Ʒ��� ���� �ִ���)
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            Vector2.down,
            groundCheckDistance
        );

        // �ٴڿ� ��Ұ� ������ ���� ���̾��ٸ�
        if (hit.collider != null && isJumping)
        {
            isJumping = false;
        }
    }

    bool IsInContactWithTruck()
    {
        // Ʈ���� ���� ������ Ȯ���ϴ� ����
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
        // ���� ���� ����
    }

    // Ʈ���� �������� �� (�浹 ����)
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Ʈ���� �浹���� ��
        if (collision.gameObject.CompareTag("Truck"))
        {
            // Ʈ���� ���������� ǥ��
            reachedTruck = true;

            // �浹 �� �߷� �ణ ���� (���� ���̵���)
            rb.gravityScale = 0.2f;

            // �ӵ� ���̱�
            rb.velocity = Vector2.zero;

            // ���� �ִϸ��̼� ����
            if (anim != null)
            {
                anim.SetBool("IsAttacking", true);
            }
        }
        // �ٸ� ����� �浹���� �� (���� ���� ��쿡�� ƨ�ܳ���)
        else if (IsZombie(collision.gameObject) && isJumping)
        {
            // �浹�� ������ Rigidbody2D ��������
            Rigidbody2D otherRb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (otherRb != null)
            {
                // �浹 ���� ���ϱ�
                ContactPoint2D contact = collision.contacts[0];
                Vector2 bounceDirection = contact.normal;

                // ��� ���� ƨ�ܳ���
                otherRb.AddForce(bounceDirection * bounceForce, ForceMode2D.Impulse);

                // �ڽ��� ������ �� ����
                rb.AddForce(new Vector2((truck.position.x > transform.position.x) ? 1 : -1, 0.5f) * (bounceForce * 0.5f), ForceMode2D.Impulse);
            }
        }
    }

    bool IsZombie(GameObject obj)
    {
        // ���� ����� ���� ���̾����� Ȯ��
        return obj.layer == gameObject.layer;
    }

    // Ʈ������ �������� ��
    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Truck"))
        {
            // Ʈ������ ���������� ǥ��
            reachedTruck = false;

            // ���� �ִϸ��̼� ����
            if (anim != null)
            {
                anim.SetBool("IsAttacking", false);
            }
        }
    }
}