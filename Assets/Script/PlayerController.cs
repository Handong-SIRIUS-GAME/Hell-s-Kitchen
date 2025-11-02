using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 7f;

    [Header("Knockback Settings")]
    public float knockbackForce = 5f;
    public float knockbackDuration = 0.5f;

    // --- 내부 상태 변수 ---
    private bool isHurting = false;
    private Rigidbody2D rb;
    private Animator animator;
    private float moveInputX;
    private bool isGrounded;
    private bool isFacingRight = true;

    // --- 상호작용 관련 변수 ---
    private bool canInteract = false;
    private GameObject interactableItem = null;
    private GameObject heldItem = null; // 획득하여 귀속된 아이템


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (!isHurting)
        {
            Flip();
            animator.SetFloat("Speed", Mathf.Abs(moveInputX));
            animator.SetBool("isJumping", !isGrounded);
        }
    }

    void FixedUpdate()
    {
        if (!isHurting)
        {
            rb.linearVelocity = new Vector2(moveInputX * moveSpeed, rb.linearVelocity.y);
        }
        CheckGrounded();
    }

    // --- Input System 이벤트 핸들러 ---

    public void OnMove(InputValue value)
    {
        if (!isHurting)
        {
            moveInputX = value.Get<float>();
        }
        else
        {
            moveInputX = 0f;
        }
    }

    public void OnJump(InputValue value)
    {
        if (isHurting || !isGrounded) return;

        if (value.isPressed)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    public void OnInteract(InputValue value)
    {
        // 다치고 있거나, 상호작용 범위가 아니거나, 이미 아이템을 들고 있으면(heldItem != null) 종료
        if (isHurting || !canInteract || heldItem != null) return;

        if (value.isPressed)
        {
            Debug.Log(interactableItem.name + " 아이템을 획득했다!");

            // 1. 획득한 아이템을 'heldItem' 변수에 저장 (귀속)
            heldItem = interactableItem;

            // 2. 아이템의 부모를 플레이어(transform)로 설정
            heldItem.transform.SetParent(transform);

            // 3. 아이템을 비활성화 (씬에서 숨김)
            heldItem.SetActive(false);

            // 4. 상태 초기화 (OnTriggerExit2D가 호출되지 않으므로 수동 리셋)
            canInteract = false;
            interactableItem = null;
        }
    }

    // --- 물리 충돌 및 트리거 감지 ---

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 1. 스파이크 감지
        if (other.CompareTag("Spike") && !isHurting)
        {
            StartCoroutine(HurtRoutine());
        }
        // 2. 아이템 감지 (단, 내가 아이템을 들고 있지 않을 때만)
        else if (other.CompareTag("Item") && heldItem == null)
        {
            Debug.Log("아이템 범위 진입");
            canInteract = true;
            interactableItem = other.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // 아이템 범위에서 나갔는지 확인
        if (other.CompareTag("Item"))
        {
            Debug.Log("아이템 범위 이탈");
            canInteract = false;
            interactableItem = null;
        }
    }

    // --- 코루틴 ---

    private IEnumerator HurtRoutine()
    {
        isHurting = true;
        moveInputX = 0f;
        rb.linearVelocity = Vector2.zero;
        animator.Play("Hurt");

        float knockbackDirection = isFacingRight ? -1 : 1;
        rb.AddForce(new Vector2(knockbackDirection * knockbackForce, knockbackForce * 0.8f), ForceMode2D.Impulse);

        yield return new WaitForSeconds(knockbackDuration);

        isHurting = false;
    }

    // --- 기타 유틸리티 함수 ---

    private void Flip()
    {
        if ((isFacingRight && moveInputX < 0f) || (!isFacingRight && moveInputX > 0f))
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    private void CheckGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1.1f, LayerMask.GetMask("Ground"));
        isGrounded = (hit.collider != null);
    }
}