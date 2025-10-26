/*
 * 이 스크립트는 Unity의 New Input System을 사용하는 2D 플레이어 캐릭터를 제어합니다.
 * 이동, 점프, 피격 시 넉백, 그리고 아이템 상호작용 기능을 포함하고 있습니다.
 */

using System.Collections; // 코루틴(IEnumerator)을 사용하기 위해 필요합니다.
using UnityEngine;
using UnityEngine.InputSystem; // New Input System (InputValue)을 사용하기 위해 필요합니다.

public class PlayerController : MonoBehaviour
{
    // === 1. 인스펙터(Inspector) 설정 변수 ===
    // public으로 선언된 변수들은 유니티 에디터의 인스펙터 창에서 값을 쉽게 수정할 수 있습니다.

    [Header("Movement Settings")] // 인스펙터 창에서 구역을 나눠줍니다.
    public float moveSpeed = 5f;    // 플레이어의 초당 이동 속도
    public float jumpForce = 7f;    // 점프 시 가해지는 힘 (Velocity 방식에서는 y축 속도값)

    [Header("Knockback Settings")] // 피격 관련 설정
    public float knockbackForce = 5f;   // 넉백 시 뒤로 밀려나는 힘의 크기
    public float knockbackDuration = 0.5f; // 넉백이 지속되는 시간 (초)


    // === 2. 내부 상태 변수 ===
    // private 변수들은 이 스크립트 내부에서만 사용되며, 플레이어의 현재 '상태'를 저장합니다.

    // 'isHurting'은 이 스크립트에서 가장 중요한 "상태 머신" 변수입니다.
    // true가 되면, 플레이어는 이동, 점프, 추가 피격, 상호작용 등 대부분의 행동이 불가능해집니다.
    // 이는 넉백 애니메이션과 물리적 넉백이 끝날 때까지 다른 동작이 간섭하지 못하게 하기 위함입니다.
    private bool isHurting = false;

    // 자주 사용하는 컴포넌트들은 매번 찾는 것(GetComponent)보다, 
    // Awake()에서 한 번 찾아 변수에 저장해두는 것이 성능에 훨씬 좋습니다.
    private Rigidbody2D rb;         // 물리 효과를 위한 리지드바디 컴포넌트
    private Animator animator;     // 애니메이션 제어를 위한 애니메이터 컴포넌트

    private float moveInputX;      // 좌우 입력값 (-1.0f ~ 1.0f)을 저장
    private bool isGrounded;       // 플레이어가 땅에 닿아있는지 여부
    private bool isFacingRight = true; // 플레이어가 오른쪽을 바라보고 있는지 여부 (스프라이트 뒤집기용)

    // --- 상호작용 관련 상태 변수 ---
    private bool canInteract = false;     // 현재 상호작용 가능한 아이템 범위 안에 있는지 여부
    private GameObject interactableItem = null; // 현재 상호작용 가능한 아이템 (나중에 파괴하기 위해 저장)


    // === 3. Unity 생명주기 함수 ===

    // Update()보다 먼저, 스크립트가 활성화될 때 딱 한 번 호출됩니다.
    // 주로 컴포넌트를 찾아와 변수에 할당하는 초기화 작업을 합니다.
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();       // 이 오브젝트의 Rigidbody 2D 컴포넌트를 찾아서 rb 변수에 할당
        animator = GetComponent<Animator>();  // 이 오브젝트의 Animator 컴포넌트를 찾아서 animator 변수에 할당
    }

    // 매 프레임마다 호출됩니다. (컴퓨터 사양에 따라 호출 간격이 다름)
    // 주로 애니메이션, 입력 상태 확인 등 시각적인 요소를 처리합니다.
    void Update()
    {
        // 'isHurting' 상태(즉, 넉백 중)가 아닐 때만 정상적인 로직을 수행합니다.
        if (!isHurting)
        {
            Flip(); // 1. 입력 값에 따라 캐릭터 방향을 뒤집습니다.

            // 2. 애니메이터의 파라미터 값을 업데이트합니다.
            // "Speed" 파라미터에 현재 이동 입력의 절대값(방향과 상관없이 0 또는 1)을 전달합니다.
            animator.SetFloat("Speed", Mathf.Abs(moveInputX));
            // "isJumping" 파라미터에 현재 점프 상태(!isGrounded)를 전달합니다.
            animator.SetBool("isJumping", !isGrounded);
        }
    }

    // 고정된 시간 간격(기본 0.02초)마다 호출됩니다. (Update와 호출 주기가 다름)
    // 물리 계산(Rigidbody)은 프레임 속도에 영향을 받지 않도록 FixedUpdate에서 처리하는 것이 정석입니다.
    void FixedUpdate()
    {
        // 넉백 중이 아닐 때만 플레이어의 입력을 받아 이동시킵니다.
        if (!isHurting)
        {
            // Rigidbody의 속도(linearVelocity)를 직접 제어합니다.
            // 이렇게 하면 AddForce를 쓰는 것보다 더 즉각적이고 미끄러짐 없는(less "icy") 조작감을 만듭니다.
            // X축 속도: 입력값 * 스피드 / Y축 속도: 현재 Y축 속도(중력/점프)를 그대로 유지
            rb.linearVelocity = new Vector2(moveInputX * moveSpeed, rb.linearVelocity.y);
        }

        // 매 물리 프레임마다 바닥에 닿았는지 체크합니다.
        CheckGrounded();
    }


    // === 4. Input System 이벤트 핸들러 ===
    // 'Player Input' 컴포넌트가 'PlayerControls' 에셋의 액션(Move, Jump, Interact)이 
    // 발동할 때마다 이 함수들을 호출합니다. (Behavior 방식에 따라 자동 또는 수동 연결)

    // 'Move' 액션(1D Axis)이 호출될 때 실행됩니다.
    public void OnMove(InputValue value)
    {
        if (!isHurting)
        {
            // InputValue에서 1D Axis 값을 float 타입(-1.0f ~ 1.0f)으로 가져옵니다.
            moveInputX = value.Get<float>();
        }
        else
        {
            // 만약 'isHurting' 상태가 되는 순간에 키를 누르고 있었다면, 
            // 그 값을 0으로 강제 초기화하여 넉백 후에도 계속 움직이는 것을 방지합니다.
            moveInputX = 0f;
        }
    }

    // 'Jump' 액션(Button)이 호출될 때 실행됩니다.
    public void OnJump(InputValue value)
    {
        // [방어 코드] 넉백 중이거나, 이미 공중에 떠있다면(땅이 아니라면) 점프를 실행하지 않습니다.
        if (isHurting || !isGrounded) return;

        // 'value.isPressed'는 버튼이 "눌리는 순간"에만 true가 됩니다. (뗐을 때는 false)
        if (value.isPressed)
        {
            // Y축 속도(Velocity)를 'jumpForce' 값으로 즉시 설정하여 점프시킵니다.
            // X축 속도는 현재 값을 유지합니다. (점프 중에도 좌우 이동 속도가 유지됨)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    // 'Interact' 액션(Button)이 호출될 때 실행됩니다.
    public void OnInteract(InputValue value)
    {
        // [방어 코드] 넉백 중이거나, 상호작용 범위 밖이라면('canInteract'가 false) 실행하지 않습니다.
        if (isHurting || !canInteract) return;

        // 'E' 키가 "눌리는 순간"에만 true
        if (value.isPressed)
        {
            // 콘솔 창에 로그를 찍어 기능이 작동했는지 확인합니다.
            Debug.Log("아이템을 얻었다!");

            // (선택 사항) 아이템 획득 로직
            if (interactableItem != null) // 혹시 모를 오류 방지를 위해 null 체크
            {
                // 저장해둔 아이템 게임 오브젝트를 씬에서 파괴(제거)합니다.
                Destroy(interactableItem);

                /*
                 * [매우 중요] 
                 * 아이템을 Destroy() 시키면, 그 아이템의 콜라이더도 즉시 사라집니다.
                 * 콜라이더가 사라지면 'OnTriggerExit2D' 함수가 호출되지 않습니다.
                 * 따라서, 'canInteract'와 'interactableItem' 변수를 여기서 
                 * 수동으로 초기화하지 않으면, 'canInteract'가 영원히 true로 남아
                 * 허공에 E키를 눌러도 '아이템을 얻었다!'가 계속 호출되는 버그가 발생합니다.
                 */
                canInteract = false;
                interactableItem = null;
            }
        }
    }


    // === 5. 물리 충돌 및 트리거 감지 ===

    // 이 오브젝트의 콜라이더가 다른 'Is Trigger'가 체크된 콜라이더(Trigger)에 
    // "진입하는 순간"에 1회 호출됩니다.
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 1. "Spike"(가시) 태그 감지
        // 'other.tag == "Spike"' 보다 'CompareTag'를 쓰는 것이 성능상 더 효율적입니다.
        // '&& !isHurting' : 이미 넉백 중이라면 다시 넉백되지 않도록 방지합니다. (무적 시간)
        if (other.CompareTag("Spike") && !isHurting)
        {
            // 넉백/피격 처리는 여러 프레임에 걸쳐 진행되므로 코루틴으로 실행합니다.
            StartCoroutine(HurtRoutine());
        }
        // 2. "Item" 태그 감지 (가시가 아니라면, 아이템인지 확인)
        else if (other.CompareTag("Item"))
        {
            Debug.Log("아이템 범위 진입"); // 확인용 로그
            canInteract = true; // 상호작용 가능 상태로 변경
            interactableItem = other.gameObject; // 어떤 아이템인지 저장 (OnInteract에서 써야 하므로)
        }
    }

    // 트리거(Trigger) 범위에서 "벗어나는 순간"에 1회 호출됩니다.
    private void OnTriggerExit2D(Collider2D other)
    {
        // "Item" 태그를 가진 트리거에서 벗어났는지 확인
        if (other.CompareTag("Item"))
        {
            Debug.Log("아이템 범위 이탈"); // 확인용 로그
            canInteract = false; // 상호작용 불가능 상태로 변경
            interactableItem = null; // 저장했던 아이템 정보 초기화
        }
    }


    // === 6. 코루틴(Coroutine) ===

    // 피격 및 넉백처럼 "일정 시간 동안" 지속되는 로직을 처리하는 특별한 함수입니다.
    private IEnumerator HurtRoutine()
    {
        // 1. 넉백 시작: "isHurting" 상태로 즉시 전환
        // 이 코드가 실행되는 즉시 Update, FixedUpdate, OnMove 등의 로직이 차단됩니다.
        isHurting = true;

        // 2. 현재 입력 및 속도 초기화
        // 넉백이 플레이어의 기존 움직임에 방해받지 않도록 현재 입력을 0으로, 속도를 0으로 만듭니다.
        moveInputX = 0f;
        rb.linearVelocity = Vector2.zero;

        // 3. 애니메이션 재생
        // 'animator.SetTrigger("Hurt")' 대신 'Play'를 쓰는 이유:
        // 'Play'는 현재 애니메이션 상태를 강제로 "Hurt"로 전환시킵니다.
        // 'SetTrigger'는 트랜지션(전환) 조건에 의존하므로, 점프나 다른 동작 중에 씹힐 수 있습니다.
        // 피격처럼 즉각적이고 중요한 반응은 'Play'가 더 확실합니다.
        animator.Play("Hurt");

        // 4. 넉백 방향 결정
        // (isFacingRight ? -1 : 1) -> 3항 연산자.
        // 만약 오른쪽을 보고 있었다면(true) -1 (왼쪽)을,
        // 왼쪽을 보고 있었다면(false) 1 (오른쪽)을 knockbackDirection에 저장합니다.
        float knockbackDirection = isFacingRight ? -1 : 1;

        // 5. 넉백 힘 적용
        // AddForce에 ForceMode2D.Impulse 옵션을 사용하면,
        // 질량(mass)을 무시하고 순간적인 "폭발" 같은 힘을 가합니다. 넉백에 가장 적합합니다.
        // (X방향: 넉백방향 * 힘, Y방향: 위로 살짝 띄우기 위해 힘의 80% 적용)
        rb.AddForce(new Vector2(knockbackDirection * knockbackForce, knockbackForce * 0.8f), ForceMode2D.Impulse);

        // 6. 넉백 시간만큼 "대기"
        // 'yield return'은 코루틴의 핵심입니다.
        // 이 함수(HurtRoutine)의 실행을 'knockbackDuration' (예: 0.5초) 동안 "일시 정지"시킵니다.
        // 그동안 유니티의 다른 Update, FixedUpdate 등은 정상적으로 실행됩니다.
        yield return new WaitForSeconds(knockbackDuration);

        // 7. 넉백 종료: 0.5초가 지난 후, 이 코드가 다시 실행됩니다.
        // 'isHurting' 상태를 false로 되돌려 플레이어가 다시 조작할 수 있도록 풀어줍니다.
        isHurting = false;

        // 8. (선택적) Idle 애니메이션으로 강제 복귀
        // animator.Play("Idle"); 
        // 넉백 애니메이션이 끝난 후 자동으로 Idle로 돌아가지 않는다면 이 코드를 추가할 수 있습니다.
    }


    // === 7. 기타 유틸리티 함수 ===

    // 캐릭터 스프라이트 방향 전환
    private void Flip()
    {
        // (오른쪽을 보는데 왼쪽 입력이 들어왔거나) 또는 (왼쪽을 보는데 오른쪽 입력이 들어왔을 때)
        if ((isFacingRight && moveInputX < 0f) || (!isFacingRight && moveInputX > 0f))
        {
            isFacingRight = !isFacingRight; // 바라보는 방향 상태를 반전시킴 (true <-> false)

            // 오브젝트의 Transform 컴포넌트의 Local Scale(크기) 값을 가져옵니다.
            Vector3 localScale = transform.localScale;
            // X축 스케일 값에 -1을 곱하여 방향을 뒤집습니다. (1 -> -1 또는 -1 -> 1)
            localScale.x *= -1f;
            // 변경된 localScale 값을 다시 적용합니다.
            transform.localScale = localScale;
        }
    }

    // 바닥 감지 (Raycast 방식)
    private void CheckGrounded()
    {
        // Raycast(광선)를 쏴서 바닥이 있는지 확인합니다.
        // Physics2D.Raycast(시작 위치, 방향, 길이, 감지할 레이어);

        // 1. 시작 위치: transform.position (플레이어의 (0,0) 위치. 보통 발목)
        // 2. 방향: Vector2.down (아래쪽)
        // 3. 길이: 1.1f (플레이어 발보다 조금 더 긴 길이. 이 값은 public 변수로 빼서 조절하는 것이 좋습니다)
        // 4. 감지할 레이어: "Ground" 레이어만 감지하도록 마스크를 설정합니다.
        //    (이렇게 해야 아이템이나 적을 "땅"으로 인식하지 않습니다.)
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1.1f, LayerMask.GetMask("Ground"));

        // 'hit' 변수는 Raycast의 결과입니다.
        // 'hit.collider'가 null이 아니라는 것은, 레이캐스트가 "Ground" 레이어의 무언가와 부딪혔다는 뜻입니다.
        isGrounded = (hit.collider != null);
    }
}