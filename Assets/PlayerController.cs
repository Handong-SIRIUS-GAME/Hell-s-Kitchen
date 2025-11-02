using System.Collections;
using System.Collections.Generic; // 1. List<> 를 사용하기 위해 추가합니다.
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

    [Header("Interaction Settings")] // 2. 아이템 관련 설정을 추가합니다.
    public int maxHeldItems = 3; // 2-1. 최대 소지 가능 아이템 개수를 3으로 설정 (인스펙터에서 변경 가능)

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

    // 3. GameObject 1개 대신, 여러 개를 담을 수 있는 List<GameObject>로 변경합니다.
    private List<GameObject> heldItems; // 획득하여 귀속된 아이템 리스트


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // 4. 아이템 리스트(가방)를 초기화합니다.
        heldItems = new List<GameObject>();
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
        // 5. 조건을 "아이템을 1개라도 들고 있으면" 에서 "아이템이 꽉 찼으면(maxHeldItems 이상)"으로 변경
        if (isHurting || !canInteract || heldItems.Count >= maxHeldItems)
        {
            if (heldItems.Count >= maxHeldItems)
            {
                Debug.Log("가방이 꽉 찼습니다! (최대 " + maxHeldItems + "개)");
            }
            return;
        }

        if (value.isPressed)
        {
            // 현재 몇 개를 들고 있는지 로그를 추가
            Debug.Log(interactableItem.name + " 아이템을 획득했다! (현재: " + (heldItems.Count + 1) + "개 / " + maxHeldItems + "개)");

            // 1. 획득한 아이템을 'heldItems' 리스트에 "추가" (귀속)
            heldItems.Add(interactableItem);

            // 2. 아이템의 부모를 플레이어(transform)로 설정
            // (변수 이름을 heldItem 대신 interactableItem으로 사용)
            interactableItem.transform.SetParent(transform);

            // 3. 아이템을 비활성화 (씬에서 숨김)
            interactableItem.SetActive(false);

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
        // 6. 조건을 "아이템을 안 들고 있을 때" 에서 "아이템이 꽉 차지 않았을 때"로 변경
        else if (other.CompareTag("Item") && heldItems.Count < maxHeldItems)
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
            // 만약 내가 상호작용 하려던 그 아이템(interactableItem)의 범위에서 나간 것이 맞다면
            if (other.gameObject == interactableItem)
            {
                Debug.Log("아이템 범위 이탈");
                canInteract = false;
                interactableItem = null;
            }
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


    // --- (추가) 아이템 관련 외부 제공 함수 ---
    // 이 함수들은 나중에 '부엌' 스크립트에서 플레이어의 아이템을 가져갈 때 유용하게 사용할 수 있습니다.

    /// <summary>
    /// 현재 들고 있는 아이템 개수를 반환합니다. (UI 표시에 사용 가능)
    /// </summary>
    public int GetHeldItemCount()
    {
        return heldItems.Count;
    }

    /// <summary>
    /// 플레이어가 들고 있는 모든 아이템을 사용(제거)하고, 그 아이템 리스트를 반환합니다.
    /// 부엌 스크립트에서 이 함수를 호출하여 요리를 시작할 수 있습니다.
    /// </summary>
    public List<GameObject> UseHeldItems()
    {
        if (heldItems.Count == 0)
        {
            Debug.Log("사용할 아이템이 없습니다.");
            return null;
        }

        Debug.Log(heldItems.Count + "개의 아이템을 사용합니다.");

        // 반환할 아이템 리스트를 복사해둡니다.
        List<GameObject> itemsToUse = new List<GameObject>(heldItems);

        // 플레이어의 가방(heldItems 리스트)을 비웁니다.
        heldItems.Clear();

        // 복사해둔 아이템 리스트를 부엌(호출한 스크립트)에 전달합니다.
        return itemsToUse;
    }
}
