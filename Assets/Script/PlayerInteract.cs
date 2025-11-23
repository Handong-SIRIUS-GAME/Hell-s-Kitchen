using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [Header("Interact Settings")]
    [SerializeField] private InteractableItem defaultInteractable;

    // [추가됨] 아이템이 매달릴 위치 (플레이어 머리 위)
    [Tooltip("아이템을 들었을 때 위치할 플레이어의 자식 오브젝트 (머리 위)")]
    public Transform itemHoldPoint;

    private bool canInteract = false;
    private InteractableItem targetItem = null;
    private GameObject heldItem = null;

    private bool isInKitchenArea = false;
    private KitchenArea currentKitchen = null;

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

        // 1. 둥둥 떠다니는 효과 끄기 (InteractableItem 스크립트 기능)
        InteractableItem itemScript = heldItem.GetComponent<InteractableItem>();
        if (itemScript != null)
        {
            itemScript.OnPickedUp();
        }

        // 2. [수정됨] 물리 충돌 끄기 (중요: 머리 위에 있는 아이템이 플레이어를 밀면 안됨)
        Collider2D itemCol = heldItem.GetComponent<Collider2D>();
        if (itemCol != null)
        {
            itemCol.enabled = false;
        }

        // 3. [핵심] 아이템을 '머리 위 위치(itemHoldPoint)'의 자식으로 설정
        if (itemHoldPoint != null)
        {
            heldItem.transform.SetParent(itemHoldPoint);
            // 위치를 (0,0,0)으로 초기화하면 itemHoldPoint의 정확한 위치에 붙습니다.
            heldItem.transform.localPosition = Vector3.zero;
        }
        else
        {
            // 만약 itemHoldPoint를 안 만들었다면 그냥 플레이어 중심에 붙임
            heldItem.transform.SetParent(transform);
            heldItem.transform.localPosition = new Vector3(0, 1.5f, 0); // 대략 머리 위
        }

        // 4. [수정됨] 아이템을 숨기지 않음! (SetActive(false) 삭제)
        // heldItem.SetActive(false); <--- 이 코드를 지웠습니다.

        canInteract = false;
        targetItem = null;
    }

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