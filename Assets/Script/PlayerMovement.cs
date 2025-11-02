using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;   // 좌우 이동 속도
    public float jumpForce = 7f;   // 점프 힘

    [Header("Ground Check")]
    public float groundCheckDistance = 1.1f;  // 바닥 레이 길이
    public LayerMask groundLayer;             // 바닥 레이어

    private Rigidbody2D rb;
    private Animator animator;

    private float moveInputX;     // 입력 받은 X값
    private bool isGrounded;      // 바닥인지
    private bool isFacingRight = true;   // 현재 바라보는 방향

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // 좌우 반전
        Flip();

        // 애니메이션 파라미터 업데이트
        animator.SetFloat("Speed", Mathf.Abs(moveInputX));
        animator.SetBool("isJumping", !isGrounded);
    }

    private void FixedUpdate()
    {
        // 실제 이동
        rb.linearVelocity = new Vector2(moveInputX * moveSpeed, rb.linearVelocity.y);

        // 바닥 체크
        CheckGrounded();
    }

    // ───────── 외부에서 호출하는 메서드들 ─────────

    // PlayerController에서 이동 입력 줄 때 호출
    public void SetMoveInput(float x)
    {
        moveInputX = x;
    }

    // PlayerController에서 점프 입력 줄 때 호출
    public void TryJump()
    {
        if (!isGrounded) return;

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

    // ───────── 내부 유틸 ─────────

    private void Flip()
    {
        // 오른쪽 보고 있는데 왼쪽 입력 들어오거나,
        // 왼쪽 보고 있는데 오른쪽 입력 들어오면 뒤집기
        if ((isFacingRight && moveInputX < 0f) || (!isFacingRight && moveInputX > 0f))
        {
            isFacingRight = !isFacingRight;
            Vector3 scale = transform.localScale;
            scale.x *= -1f;
            transform.localScale = scale;
        }
    }

    private void CheckGrounded()
    {
        // 캐릭터 발밑으로 레이 하나 쏴서 바닥인지 확인
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down,
                                             groundCheckDistance, groundLayer);
        isGrounded = (hit.collider != null);
    }

    // 디버그용 레이 보이게
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position,
                        transform.position + Vector3.down * groundCheckDistance);
    }
}
