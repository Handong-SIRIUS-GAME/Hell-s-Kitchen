using UnityEngine;

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
        // 1. 아이템을 들고 있을 때 -> 내려놓기(Drop) 시도
        if (heldItem != null)
        {
            if (isInKitchenArea && currentKitchen != null)
            {
                TryDrop(currentKitchen);
            }
            else
            {
                Debug.Log("여기에 아이템을 둘 수 없습니다. 부엌으로 가세요.");
            }
        }
        // 2. 아이템을 안 들고 있을 때 -> 줍기(Pickup) 시도
        else
        {
            if (defaultInteractable != null)
            {
                Pickup(defaultInteractable.gameObject);
                return;
            }
            if (canInteract && targetItem != null)
            {
                Pickup(targetItem.gameObject);
            }
        }
    }

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
        InteractableItem itemData = heldItem.GetComponent<InteractableItem>();
        if (itemData == null) return;

        bool success = kitchen.AddIngredient(itemData);
        if (success)
        {
            Debug.Log(itemData.itemName + " 아이템을 부엌에 둠.");

            // [핵심] 여기서 아이템을 파괴하므로 머리 위에서 사라짐
            Destroy(heldItem);

            heldItem = null;
        }
        else
        {
            Debug.Log("부엌이 꽉 차서 아이템을 둘 수 없음.");
        }
    }

    // ───── 트리거 감지 로직 (이전과 동일) ─────
    private void OnTriggerEnter2D(Collider2D other)
    {
        InteractableItem item = other.GetComponent<InteractableItem>();
        if (item != null && heldItem == null)
        {
            canInteract = true;
            targetItem = item;
        }

        KitchenArea kitchen = other.GetComponent<KitchenArea>();
        if (kitchen != null)
        {
            isInKitchenArea = true;
            currentKitchen = kitchen;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        InteractableItem item = other.GetComponent<InteractableItem>();
        if (item != null)
        {
            canInteract = false;
            if (targetItem == item) targetItem = null;
        }

        KitchenArea kitchen = other.GetComponent<KitchenArea>();
        if (kitchen != null)
        {
            isInKitchenArea = false;
            currentKitchen = null;
        }
    }
}