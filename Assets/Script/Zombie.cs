using UnityEngine;
using System.Collections;

public class Zombie : MonoBehaviour
{
    public Transform truck;  // 트럭의 위치를 저장
    public float moveSpeed = 8f;  // 좀비 이동 속도
    public float stopDistance = 2f;  // 트럭과의 거리가 2 이하일 때 공격 애니메이션 실행
    public float jumpForce = 10f;   // 점프 힘
    public float jumpCooldown = 1f; // 점프 쿨다운
    public LayerMask zombieLayerMask; // 같은 레이어 좀비 감지용
    public float zombieDetectionRange = 0.5f; // 좀비 감지 범위
    public float jumpTriggerDistance = 5f; // 트럭과의 거리가 이 값 이하일 때만 점프
    public float bounceForce = 5f; // 좀비가 충돌 시 튕겨내는 힘
    public float jumpKnockbackForce = 3f; // 점프 시 아래 좀비를 밀어내는 힘

    private Rigidbody2D rb;
    private Animator anim;
    private bool isJumping = false;
    private bool canJump = true;
    private float groundCheckDistance = 0.2f; // 바닥 체크 거리
    private bool reachedTruck = false; // 트럭에 도달했는지 여부
    private Vector2 lastPosition; // 정체 감지를 위한 마지막 위치
    private float stuckTime = 0f; // 좀비가 정체된 시간
    private bool gravityEnabled = false; // 중력이 활성화되었는지 여부

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        // 처음에는 중력 0으로 시작
        rb.gravityScale = 0f;
        gravityEnabled = false;

        // 현재 좀비의 레이어를 확인해서 좀비 레이어마스크 설정
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
        // 트럭이 지정되어 있을 경우만 동작
        if (truck != null)
        {
            // 트럭에 도달하지 않았거나 떨어졌다면 계속 이동
            if (!reachedTruck || !IsInContactWithTruck())
            {
                // 정체 감지
                CheckIfStuck();

                // 트럭과의 거리 계산 (y축을 제외한 x만 비교)
                float distanceToTruck = Mathf.Abs(truck.position.x - transform.position.x);

                // 트럭과 가까울 때만 앞에 같은 레이어의 좀비가 있는지 확인
                if (distanceToTruck <= jumpTriggerDistance)
                {
                    CheckForZombieInFront();
                }

                // 트럭 방향으로 이동
                MoveTowardsTruck();
            }

            // 바닥 체크 (이미 중력이 활성화된 경우만)
            if (gravityEnabled)
            {
                CheckGround();
            }
        }
    }

    void CheckIfStuck()
    {
        // 현재 위치와 마지막 위치의 차이 계산
        float movementDelta = Vector2.Distance(transform.position, lastPosition);

        // 거의 움직이지 않았다면 (정체 상태)
        if (movementDelta < 0.01f && !isJumping && !reachedTruck)
        {
            stuckTime += Time.deltaTime;

            // 일정 시간 이상 정체되면 점프 시도
            if (stuckTime > 0.5f && canJump)
            {
                EnableGravity();  // 중력 활성화
                Jump();
                stuckTime = 0f;
            }
        }
        else
        {
            stuckTime = 0f;
        }

        // 마지막 위치 업데이트
        lastPosition = transform.position;
    }

    void MoveTowardsTruck()
    {
        // 트럭으로 향하는 방향 벡터
        Vector3 direction = (truck.position - transform.position).normalized;

        // 트럭과의 거리 계산 (y축을 제외한 x만 비교)
        float distanceToTruck = Mathf.Abs(truck.position.x - transform.position.x);

        // 점프 중이 아니고 트럭과의 거리가 stopDistance보다 클 경우에만 이동
        if (distanceToTruck > stopDistance && !isJumping)
        {
            // Rigidbody2D의 속도를 사용하여 일정한 속도로 이동
            rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);
            reachedTruck = false;
        }
        else if (distanceToTruck <= stopDistance && !isJumping)
        {
            // 공격 애니메이션 실행
            if (anim != null)
            {
                anim.SetBool("IsAttacking", true);
            }

            // 이동을 멈추기 위해 x축 속도를 0으로 설정
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
    }

    void CheckForZombieInFront()
    {
        // 현재 방향에 따라 레이캐스트 방향 설정
        float direction = (truck.position.x > transform.position.x) ? 1f : -1f;

        // 좀비 앞에 다른 같은 레이어 좀비가 있는지 확인
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position + new Vector3(direction * 0.5f, 0, 0),
            new Vector2(direction, 0),
            zombieDetectionRange,
            zombieLayerMask
        );

        // 앞에 같은 레이어 좀비가 있고, 점프 가능하면 점프
        if (hit.collider != null && canJump && !isJumping)
        {
            EnableGravity();  // 중력 활성화
            Jump();
        }
    }

    void EnableGravity()
    {
        // 아직 중력이 활성화되지 않았다면
        if (!gravityEnabled)
        {
            rb.gravityScale = 1f;
            gravityEnabled = true;
        }
    }

    void Jump()
    {
        // 점프 실행
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);

        // 점프 상태 설정
        isJumping = true;
        canJump = false;

        // 바닥의 좀비 밀어내기 (아래에 있는 좀비 체크)
        PushZombiesBelow();

        // 점프 쿨다운 시작
        StartCoroutine(JumpCooldown());
    }

    void PushZombiesBelow()
    {
        // 아래에 있는 좀비들 감지
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
                // 아래 좀비의 Rigidbody2D 가져오기
                Rigidbody2D otherRb = hit.collider.GetComponent<Rigidbody2D>();
                if (otherRb != null)
                {
                    // 트럭 반대 방향으로 밀어내기
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
        // 바닥 체크 (좀비 아래에 뭔가 있는지)
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            Vector2.down,
            groundCheckDistance
        );

        // 바닥에 닿았고 이전에 점프 중이었다면
        if (hit.collider != null && isJumping)
        {
            isJumping = false;
        }
    }

    bool IsInContactWithTruck()
    {
        // 트럭과 접촉 중인지 확인하는 로직
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
        // 공격 로직 구현
    }

    // 트럭에 도달했을 때 (충돌 감지)
    void OnCollisionEnter2D(Collision2D collision)
    {
        // 트럭과 충돌했을 때
        if (collision.gameObject.CompareTag("Truck"))
        {
            // 트럭에 도달했음을 표시
            reachedTruck = true;

            // 충돌 시 중력 약간 조정 (좀비가 쌓이도록)
            rb.gravityScale = 0.2f;

            // 속도 줄이기
            rb.velocity = Vector2.zero;

            // 공격 애니메이션 시작
            if (anim != null)
            {
                anim.SetBool("IsAttacking", true);
            }
        }
        // 다른 좀비와 충돌했을 때 (점프 중일 경우에만 튕겨내기)
        else if (IsZombie(collision.gameObject) && isJumping)
        {
            // 충돌한 좀비의 Rigidbody2D 가져오기
            Rigidbody2D otherRb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (otherRb != null)
            {
                // 충돌 지점 구하기
                ContactPoint2D contact = collision.contacts[0];
                Vector2 bounceDirection = contact.normal;

                // 상대 좀비 튕겨내기
                otherRb.AddForce(bounceDirection * bounceForce, ForceMode2D.Impulse);

                // 자신은 앞으로 더 점프
                rb.AddForce(new Vector2((truck.position.x > transform.position.x) ? 1 : -1, 0.5f) * (bounceForce * 0.5f), ForceMode2D.Impulse);
            }
        }
    }

    bool IsZombie(GameObject obj)
    {
        // 현재 좀비와 같은 레이어인지 확인
        return obj.layer == gameObject.layer;
    }

    // 트럭에서 떨어졌을 때
    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Truck"))
        {
            // 트럭에서 떨어졌음을 표시
            reachedTruck = false;

            // 공격 애니메이션 중지
            if (anim != null)
            {
                anim.SetBool("IsAttacking", false);
            }
        }
    }
}