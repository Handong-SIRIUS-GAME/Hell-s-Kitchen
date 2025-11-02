using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [Header("Interact Settings")]
    [SerializeField] private InteractableItem defaultInteractable; 
    // ⬆ 인스펙터에서 수동으로 넣어둘 수 있는 아이템
    // (없으면 자동으로 트리거에서 찾음)

    private bool canInteract = false;              // 지금 상호작용 가능한 상태인가
    private InteractableItem targetItem = null;    // 트리거로 들어온 아이템
    private GameObject heldItem = null;            // 이미 들고 있는 아이템

    // PlayerController가 호출
    public void TryInteract()
    {
        // 1) 우선 인스펙터에서 지정한 거 있으면 그걸 먼저 사용
        if (heldItem == null && defaultInteractable != null)
        {
            Pickup(defaultInteractable.gameObject);
            return;
        }

        // 2) 그게 아니면 트리거로 감지된 거 사용
        if (!canInteract || heldItem != null) return;
        if (targetItem == null) return;

        Pickup(targetItem.gameObject);
    }

    private void Pickup(GameObject itemObj)
    {
        Debug.Log(itemObj.name + " 아이템을 획득했다!");

        // 들고 있는 아이템으로 등록
        heldItem = itemObj;

        // 플레이어 자식으로 붙이기
        heldItem.transform.SetParent(transform);

        // 화면에서 숨기기
        heldItem.SetActive(false);

        // 상태 초기화
        canInteract = false;
        targetItem = null;
    }

    // ───── 트리거 진입/이탈로 자동 감지 ─────
    private void OnTriggerEnter2D(Collider2D other)
    {
        // InteractableItem 컴포넌트를 가진 애만 받기
        InteractableItem item = other.GetComponent<InteractableItem>();
        if (item != null && heldItem == null)
        {
            canInteract = true;
            targetItem = item;
            Debug.Log("아이템 범위 진입: " + item.itemName);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        InteractableItem item = other.GetComponent<InteractableItem>();
        if (item != null)
        {
            canInteract = false;
            if (targetItem == item)
                targetItem = null;

            Debug.Log("아이템 범위 이탈");
        }
    }
}
