using UnityEngine;

/// <summary>
/// 플레이어의 '상호작용' (줍기/내려놓기) 로직을 전담하는 스크립트입니다.
/// 1. PlayerController로부터 'Interact' 입력(F키 등)을 받습니다.
/// 2. 트리거(Collider 2D)를 이용해 주울 수 있는 아이템(InteractableItem)과
///    내려놓을 수 있는 부엌(KitchenArea)을 감지합니다.
/// 3. 아이템을 줍거나(Pickup), 부엌에 내려놓는(TryDrop) 로직을 실행합니다.
/// </summary>
public class PlayerInteract : MonoBehaviour
{
    [Header("Interact Settings")]
    [SerializeField] private InteractableItem defaultInteractable; // (사용 안 함, 무시해도 됨)

    // --- 플레이어의 현재 상태 변수 ---
    private bool canInteract = false;          // (아이템 감지) 주울 수 있는 아이템 범위 안인가?
    private InteractableItem targetItem = null; // (아이템 감지) 현재 주울 수 있는 대상 아이템
    private GameObject heldItem = null;         // (아이템 소지) 현재 손에 들고 있는 아이템
    private bool isInKitchenArea = false;       // (부엌 감지) 부엌 범위 안인가?
    private KitchenArea currentKitchen = null; // (부엌 감지) 현재 감지된 부엌 스크립트

    [Header("Hold Position Settings")]
    [SerializeField] private Vector2 holdOffset = new Vector2(0f, 1.5f); 
    // X: 좌우, Y: 플레이어 기준 위쪽 거리


    /// <summary>
    /// PlayerController가 'Interact' 키(F키) 입력을 받으면 호출하는 메인 함수
    /// </summary>
    public void TryInteract()
    {
        // --- 1순위: 아이템을 들고 있는 경우 (내려놓기 시도) ---
        if (heldItem != null)
        {
            // 부엌 범위 안이고, 부엌 스크립트를 제대로 감지했다면
            if (isInKitchenArea && currentKitchen != null)
            {
                TryDrop(currentKitchen); // 부엌에 내려놓기 시도
            }
            else
            {
                Debug.Log("여기에 아이템을 둘 수 없습니다. 부엌으로 가세요.");
            }
        }
        // --- 2순위: 아이템을 안 들고 있는 경우 (줍기 시도) ---
        else
        {
            // (defaultInteractable 관련 로직은 무시)
            if (defaultInteractable != null)
            {
                Pickup(defaultInteractable.gameObject);
                return;
            }

            // 주울 수 있는 상태(canInteract)이고, 주울 대상(targetItem)이 있다면
            if (canInteract && targetItem != null)
            {
                Pickup(targetItem.gameObject); // 아이템 줍기
            }
        }
    }

    /// <summary>
    /// 아이템을 줍는 함수
    /// </summary>
    private void Pickup(GameObject itemObj)
    {
        Debug.Log(itemObj.name + " 아이템을 획득했다!");

        heldItem = itemObj;

        // InteractableItem 스크립트 처리
        InteractableItem itemScript = heldItem.GetComponent<InteractableItem>();
        if (itemScript != null)
            itemScript.OnPickedUp();

        // ➊ ItemMove 스크립트 비활성화
        ItemMove mover = heldItem.GetComponent<ItemMove>();
        if (mover != null)
            mover.enabled = false;

        // 부모를 플레이어로 설정
        heldItem.transform.SetParent(transform);

        // 머리 위 위치로 올리기 (네가 쓰던 위치값 적용)
        heldItem.transform.localPosition = new Vector3(holdOffset.x, holdOffset.y, 0f);

        // 혹시 비활성화되어 있으면 다시 활성화
        heldItem.SetActive(true);

        // 상태 초기화
        canInteract = false;
        targetItem = null;
    }



    /// <summary>
    /// 아이템을 부엌에 내려놓는(드롭하는) 함수
    /// </summary>
    private void TryDrop(KitchenArea kitchen)
    {
        // 1. 들고 있는 아이템(GameObject)에서 데이터(InteractableItem) 스크립트 가져오기
        InteractableItem itemData = heldItem.GetComponent<InteractableItem>();
        if (itemData == null) return; // 스크립트가 없으면 종료

        // 2. 부엌 스크립트에게 이 아이템 데이터를 넘겨주고 "추가해달라"고 요청
        bool success = kitchen.AddIngredient(itemData);

        // 3. 부엌이 아이템을 성공적으로 받았다면 (꽉 차지 않았다면)
        if (success)
        {
            Debug.Log(itemData.itemName + " 아이템을 부엌에 둠.");

            // 4. [중요] 플레이어가 들고 있던 원본 아이템(GameObject)을 파괴
            Destroy(heldItem);

            // 5. '들고 있는 아이템' 상태를 null로 비움 (다시 주울 수 있게 됨)
            heldItem = null;
        }
        else
        {
            // 부엌이 꽉 차서 추가에 실패한 경우
            Debug.Log("부엌이 꽉 차서 아이템을 둘 수 없음.");
            // (아이템을 파괴하지 않고 계속 들고 있음)
        }
    }


    // ───── 2D 트리거 감지 로직 ─────
    // [참고] 이 함수들은 플레이어에게 'Rigidbody 2D'와 'Collider 2D (Is Trigger=true)'가
    //        모두 있어야 작동합니다.

    /// <summary>
    /// 플레이어의 '감지 영역(Trigger)'에 다른 'Collider 2D'가 들어왔을 때 1번 호출됨
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 1. 부딪힌 대상이 'InteractableItem' 스크립트를 가졌는지 확인
        InteractableItem item = other.GetComponent<InteractableItem>();
        // (아이템 스크립트가 있고) && (내가 지금 손에 든 게 없다면)
        if (item != null && heldItem == null)
        {
            canInteract = true; // 주울 수 있는 상태로 변경
            targetItem = item;  // 주울 대상으로 저장
            Debug.Log("아이템 범위 진입: " + item.itemName);
        }

        // 2. 부딪힌 대상이 'KitchenArea' 스크립트를 가졌는지 확인
        KitchenArea kitchen = other.GetComponent<KitchenArea>();
        if (kitchen != null)
        {
            isInKitchenArea = true; // 부엌 범위 안으로 변경
            currentKitchen = kitchen; // 현재 부엌으로 저장
            Debug.LogWarning(">>> 부엌 범위 진입 성공! <<<");
        }
    }

    /// <summary>
    /// 플레이어의 '감지 영역'에서 다른 'Collider 2D'가 나갔을 때 1번 호출됨
    /// </summary>
    private void OnTriggerExit2D(Collider2D other)
    {
        // 1. 나간 대상이 'InteractableItem' 스크립트를 가졌는지 확인
        InteractableItem item = other.GetComponent<InteractableItem>();
        if (item != null)
        {
            canInteract = false; // 주울 수 없는 상태로 변경
            if (targetItem == item) // 나간 아이템이 내가 주우려던 그 아이템이 맞다면
                targetItem = null;  // 주울 대상에서 제외
            Debug.Log("아이템 범위 이탈");
        }

        // 2. 나간 대상이 'KitchenArea' 스크립트를 가졌는지 확인
        KitchenArea kitchen = other.GetComponent<KitchenArea>();
        if (kitchen != null)
        {
            isInKitchenArea = false; // 부엌 범위 밖으로 변경
            currentKitchen = null; // 현재 부엌 정보 초기화
            Debug.LogWarning("<<< 부엌 범위에서 이탈. <<<");
        }
    }
}