using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [Header("Interact Settings")]
    [SerializeField] private InteractableItem defaultInteractable;

    [Tooltip("아이템을 들었을 때 위치할 플레이어의 자식 오브젝트 (머리 위)")]
    public Transform itemHoldPoint;

    private bool canInteract = false;
    private InteractableItem targetItem = null;
    private GameObject heldItem = null;

    private bool isInKitchenArea = false;
    private KitchenArea currentKitchen = null;

    // [추가됨] 애니메이션 제어를 위한 컴포넌트
    private Animator animator;

    private void Awake()
    {
        // 플레이어에게 붙어있는 Animator 컴포넌트 가져오기
        animator = GetComponent<Animator>();
    }

    public void TryInteract()
    {
        // 1. 내려놓기
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
        // 2. 줍기
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
        // [수정 1] 아이템 이름 예쁘게 출력하기
        // InteractableItem 스크립트 정보를 먼저 가져옵니다.
        InteractableItem itemScript = itemObj.GetComponent<InteractableItem>();

        string displayName = itemObj.name; // 기본값은 오브젝트 이름
        if (itemScript != null)
        {
            displayName = itemScript.itemName; // 설정된 이름("양상추" 등)이 있으면 덮어쓰기
            itemScript.OnPickedUp(); // 둥둥 떠다니기 멈춤
        }

        Debug.Log($"<color=yellow>{displayName}</color>을(를) 획득했습니다!");

        heldItem = itemObj;

        // 물리 충돌 끄기
        Collider2D itemCol = heldItem.GetComponent<Collider2D>();
        if (itemCol != null) itemCol.enabled = false;

        // 머리 위로 이동
        if (itemHoldPoint != null)
        {
            heldItem.transform.SetParent(itemHoldPoint);
            heldItem.transform.localPosition = Vector3.zero;
        }
        else
        {
            heldItem.transform.SetParent(transform);
            heldItem.transform.localPosition = new Vector3(0, 1.5f, 0);
        }

        // [수정 2] 애니메이션 상태 변경 (들고 있음!)
        if (animator != null)
        {
            animator.SetBool("isHolding", true);
        }

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
            Debug.Log($"{itemData.itemName}을(를) 부엌에 제출했습니다.");

            Destroy(heldItem);
            heldItem = null;

            // [수정 2] 애니메이션 상태 복귀 (내려놨음!)
            if (animator != null)
            {
                animator.SetBool("isHolding", false);
            }
        }
        else
        {
            Debug.Log("부엌이 꽉 차서 아이템을 둘 수 없음.");
        }
    }

    // ───── 트리거 감지 로직 ─────
    private void OnTriggerEnter2D(Collider2D other)
    {
        InteractableItem item = other.GetComponent<InteractableItem>();
        if (item != null && heldItem == null)
        {
            canInteract = true;
            targetItem = item;
            // 여기서도 이름을 예쁘게 출력
            Debug.Log("아이템 발견: " + item.itemName);
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