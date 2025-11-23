using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 7f;

    [Header("Ground Check")]
    public float groundCheckDistance = 1.1f;
    public LayerMask groundLayer; // 바닥 레이어 (Inspector에서 Ground 체크 필수!)

    private Rigidbody2D rb;
    private Animator animator;
    private CapsuleCollider2D playerCollider; // 플레이어의 몸

    private float moveInputX;
    private bool isGrounded;
    private bool isFacingRight = true;

    // 현재 밟고 있는 바닥 (S키 누르면 이걸 뚫음)
    private GameObject currentGroundObject;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        // 플레이어의 물리 콜라이더 (Trigger 아님!)
        playerCollider = GetComponent<CapsuleCollider2D>();
    }

    private void Update()
    {
        // 1. 방향 전환
        Flip();

        // 2. 애니메이션
        animator.SetFloat("Speed", Mathf.Abs(moveInputX));
        animator.SetBool("isJumping", !isGrounded);
    }

    private void FixedUpdate()
    {
        // 3. 물리 이동
        rb.linearVelocity = new Vector2(moveInputX * moveSpeed, rb.linearVelocity.y);

        // 4. 바닥 체크
        CheckGrounded();
    }

    // --- 외부 호출 함수 ---

    public void SetMoveInput(float x)
    {
        moveInputX = x;
    }

    public void TryJump()
    {
        if (isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    // [핵심] S키를 눌렀을 때 호출됨
    public void TryJumpDown()
    {
        // 바닥에 서 있다면, 무조건 뚫고 내려가기 시도
        if (isGrounded && currentGroundObject != null)
        {
            StartCoroutine(PassThroughFloor());
        }
        else
        {
            Debug.Log("공중이거나 밟고 있는 바닥이 없어서 내려갈 수 없습니다.");
        }
    }

    // 바닥 뚫기 코루틴
    private IEnumerator PassThroughFloor()
    {
        Collider2D groundCollider = currentGroundObject.GetComponent<Collider2D>();

        if (groundCollider != null)
        {
            // 0.5초 동안 플레이어와 바닥의 충돌을 끈다 (유령처럼 통과)
            Physics2D.IgnoreCollision(playerCollider, groundCollider, true);

            yield return new WaitForSeconds(0.5f);

            // 다시 충돌을 켠다
            Physics2D.IgnoreCollision(playerCollider, groundCollider, false);
        }
    }

    // --- 내부 유틸 ---

    private void CheckGrounded()
    {
        // 발밑으로 레이를 쏴서 닿는 게 있는지 확인
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundLayer);

        if (hit.collider != null)
        {
            isGrounded = true;
            currentGroundObject = hit.collider.gameObject; // 밟고 있는 바닥 저장
        }
        else
        {
            isGrounded = false;
            currentGroundObject = null;
        }
    }

    private void Flip()
    {
        if ((isFacingRight && moveInputX < 0f) || (!isFacingRight && moveInputX > 0f))
        {
            isFacingRight = !isFacingRight;
            Vector3 scale = transform.localScale;
            scale.x *= -1f;
            transform.localScale = scale;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundCheckDistance);
    }
}