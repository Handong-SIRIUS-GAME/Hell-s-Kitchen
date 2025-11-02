using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // 이동 관련 설정 변수
    [Header("Movement Settings")]
    public float moveSpeed = 5f;    // 좌우 이동 속도
    public float jumpForce = 7f;    // 점프 힘 (위로 가하는 속도)

    // 내부 상태 변수
    private Rigidbody2D rb;             // 물리 연산용 Rigidbody2D
    private Animator animator;          // 애니메이터 제어
    private float moveInputX;           // 좌우 입력값 (-1 ~ 1)
    private bool isGrounded;            // 지면 접촉 여부
    private bool isFacingRight = true;  // 오른쪽을 보고 있는지 여부

    // 상호작용 관련 변수
    private bool canInteract = false;           // 상호작용 가능한 범위에 있는가?
    public GameObject interactableItem = null; // 현재 상호작용 가능한 아이템
    private GameObject heldItem = null;         // 플레이어가 소유한(들고 있는) 아이템

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        Flip();     // 이동 방향에 따라 캐릭터 좌우 반전
        animator.SetFloat("Speed", Mathf.Abs(moveInputX));  // 이동 속도(절댓값) 전달
        animator.SetBool("isJumping", !isGrounded);         // 공중 여부에 따라 점프 애니메이션 제어
    }

    void FixedUpdate()
    {
        // Rigidbody의 X축 속도를 moveInputX에 맞게 설정
        rb.linearVelocity = new Vector2(moveInputX * moveSpeed, rb.linearVelocity.y);
        // 항상 지면 체크
        CheckGrounded();
    }

    // --- Input System 이벤트 핸들러 ---
    public void OnMove(InputValue value)
    {
        // -1(왼쪽), 0(중립), 1(오른쪽)
        moveInputX = value.Get<float>();
    }

    // 점프 입력 처리
    public void OnJump(InputValue value)
    {
        // 공중이면 점프 불가
        if (!isGrounded) return;

        if (value.isPressed)
        {
            // 위 방향으로 jumpForce만큼 속도를 주어 점프
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    // 상호작용 입력 처리
    public void OnInteract(InputValue value)
    {
        // 상호작용 범위가 아니거나, 이미 아이템을 들고 있으면(heldItem != null) 종료
        if (!canInteract || heldItem != null) return;

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
        // 아이템 감지 (단, 내가 아이템을 들고 있지 않을 때만)
        if (other.CompareTag("Item") && heldItem == null)
        {
            Debug.Log("아이템 범위 진입");
            canInteract = true; // 상호작용 가능
            interactableItem = other.gameObject;    // 현재 아이템 저장
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

    // --- 기타 유틸리티 함수 ---

    // 캐릭터 좌우 반전 처리
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

    // 바닥 감지 (Raycast)
    private void CheckGrounded()
    {
         // 캐릭터 중심에서 아래로 1.1 유닛 거리로 레이캐스트를 쏴서 'Ground' 레이어와 충돌 확인
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1.1f, LayerMask.GetMask("Ground"));
        isGrounded = (hit.collider != null);    // 충돌이 있으면 바닥에 닿아 있음
    }
}