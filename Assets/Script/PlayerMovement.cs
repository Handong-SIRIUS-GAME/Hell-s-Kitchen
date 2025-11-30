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
    // [수정] 레이 길이를 1.1f -> 1.3f로 살짝 늘려서 확실하게 감지하게 함
    public float groundCheckDistance = 1.3f;
    public LayerMask groundLayer; // [중요] Inspector에서 바닥 레이어를 꼭 체크하세요!

    private Rigidbody2D rb;
    private Animator animator;
    private CapsuleCollider2D playerCollider;

    private float moveInputX;
    private bool isGrounded;
    private bool isFacingRight = true;

    // 현재 밟고 있는 바닥
    private GameObject currentGroundObject;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerCollider = GetComponent<CapsuleCollider2D>();
    }

    private void Update()
    {
        Flip();
        animator.SetFloat("Speed", Mathf.Abs(moveInputX));
        animator.SetBool("isJumping", !isGrounded);
    }

    private void FixedUpdate()
    {
        // Unity 6Preview가 아니라면 rb.velocity 사용
        // Unity 6라면 rb.linearVelocity 유지
        rb.linearVelocity = new Vector2(moveInputX * moveSpeed, rb.linearVelocity.y);

        CheckGrounded();
    }

    public void SetMoveInput(float x)
    {
        moveInputX = x;
    }

    public void TryJump()
    {
        // [디버그] 점프가 안 되면 이 로그가 뜨는지 확인
        if (!isGrounded)
        {
            // Debug.Log("점프 실패: 공중에 있음");
            return;
        }

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

    public void TryJumpDown()
    {
        // 바닥에 있고 바닥 오브젝트를 감지했다면
        if (isGrounded && currentGroundObject != null)
        {
            StartCoroutine(PassThroughFloor());
        }
        else
        {
            Debug.Log($"내려가기 실패: 바닥 감지({isGrounded}), 오브젝트({currentGroundObject})");
        }
    }

    private IEnumerator PassThroughFloor()
    {
        Collider2D groundCollider = currentGroundObject.GetComponent<Collider2D>();

        if (groundCollider != null)
        {
            Debug.Log("아래로 점프 수행!");
            // 1. 충돌 끄기
            Physics2D.IgnoreCollision(playerCollider, groundCollider, true);

            yield return new WaitForSeconds(0.5f);

            // 2. 충돌 켜기
            Physics2D.IgnoreCollision(playerCollider, groundCollider, false);
        }
    }

    private void CheckGrounded()
    {
        // 발밑으로 레이 쏘기 (위치, 방향, 길이, 레이어마스크)
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundLayer);

        if (hit.collider != null)
        {
            isGrounded = true;
            currentGroundObject = hit.collider.gameObject;
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
        // Scene 화면에서 빨간 선이 발바닥보다 아래로 내려오는지 확인하세요!
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundCheckDistance);
    }
}