using UnityEngine;

/// <summary>
/// 플레이어가 주울 수 있는 '재료 아이템'을 정의하는 스크립트입니다.
/// 1. 요리 맛 계산을 위한 고유 값(0 또는 1)을 가집니다.
/// 2. 부엌 UI에 표시될 스프라이트 이미지를 가집니다.
/// 3. 맵에 놓여있을 때 위아래로 둥둥 떠다니는 효과를 가집니다.
/// 4. 플레이어가 주웠을 때 둥둥 떠다니는 효과를 멈추는 기능('머리 위 사각형' 버그 수정)을 가집니다.
/// </summary>
public class InteractableItem : MonoBehaviour
{
    // enum (열거형): 값의 범위를 Value0, Value1로 명확하게 제한합니다.
    public enum IngredientValue
    {
        Value0 = 0,
        Value1 = 1
    }

    [Header("Item Info")]
    public string itemName = "Item"; // 아이템 이름 (디버깅용)

    [Header("Cooking Info")]
    [Tooltip("이 재료의 고유 맛 값 (0 또는 1)")]
    public IngredientValue value = IngredientValue.Value0; // 요리 맛 계산에 쓰일 값 (0 또는 1)

    [Tooltip("부엌에 재료를 뒀을 때 UI에 표시될 이미지")]
    public Sprite itemSprite; // 부엌(KitchenArea)의 UI 슬롯에 표시될 스프라이트 에셋

    void Start()
    {

        // [오류 방지] Inspector 창에서 itemSprite를 할당했는지 확인
        if (itemSprite == null)
        {
            Debug.LogWarning(itemName + "에 'Item Sprite'가 할당되지 않았습니다. 부엌 UI에 이미지가 표시되지 않습니다.");
        }

        // [버그 방지] 씬에 배치된 이 오브젝트의 Sprite Renderer에 스프라이트가 없으면 경고
        if (GetComponent<SpriteRenderer>() != null && GetComponent<SpriteRenderer>().sprite == null)
        {
            Debug.LogWarning(itemName + " (원본)의 Sprite Renderer에 Sprite가 없습니다!");
        }
    }

    void Update()
    {

    }

    /// <summary>
    /// PlayerInteract 스크립트가 아이템을 주웠을 때 이 함수를 호출합니다.
    /// </summary>
    public void OnPickedUp()
    {
        // 2. 콜라이더를 비활성화
        // (플레이어가 들고 있는 동안, 다른 아이템이나 부엌 영역에 감지되면 안 되기 때문)
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;
    }
}