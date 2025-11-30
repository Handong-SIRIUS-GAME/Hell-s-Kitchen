using UnityEngine;

public class InteractableItem : MonoBehaviour
{
    public enum IngredientValue
    {
        Value0 = 0,
        Value1 = 1
    }

    [Header("Item Info")]
    public string itemName = "Item";

    [Header("Cooking Info")]
    [Tooltip("이 재료의 고유 맛 값 (0 또는 1)")]
    public IngredientValue value = IngredientValue.Value0;

    [Tooltip("부엌에 재료를 뒀을 때 UI에 표시될 이미지")]
    public Sprite itemSprite;

    // 내부적인 둥둥 떠다니기 효과 변수
    private float floatAmplitude = 0.2f;
    private float floatSpeed = 8f;
    private Vector3 startPos;
    private bool isFloating = true;

    void Start()
    {
        startPos = transform.position;

        if (itemSprite == null)
            Debug.LogWarning(itemName + "에 'Item Sprite'가 없습니다.");

        if (GetComponent<SpriteRenderer>() != null && GetComponent<SpriteRenderer>().sprite == null)
            Debug.LogWarning(itemName + " (원본)의 Sprite Renderer에 Sprite가 없습니다!");
    }

    void Update()
    {
        // 내부적인 둥둥 떠다니기 (ItemMove 스크립트가 없을 때 사용됨)
        if (isFloating)
        {
            float newY = startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
            transform.position = new Vector3(startPos.x, newY, startPos.z);
        }
    }

    /// <summary>
    /// 플레이어가 아이템을 주웠을 때 호출되는 함수
    /// </summary>
    public void OnPickedUp()
    {
        // 1. 내부 둥둥 떠다니기 끄기
        isFloating = false;

        // 2. 물리 충돌 끄기
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        // [추가됨] 3. ItemMove 스크립트가 붙어있다면 끄기 (외부 움직임 정지)
        ItemMove moveScript = GetComponent<ItemMove>();
        if (moveScript != null)
        {
            moveScript.enabled = false;
        }

        // [추가됨] 4. 회전 초기화 (움직이던 중 주우면 기울어져 있을 수 있으므로 똑바로 세움)
        transform.localRotation = Quaternion.identity;
    }
}