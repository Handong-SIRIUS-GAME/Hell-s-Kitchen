using UnityEngine;

// 이 스크립트를 붙이면 "이 오브젝트는 플레이어가 주울 수 있는 대상"이라고 표시되는 것
public class InteractableItem : MonoBehaviour
{
    [Header("Item Info")]
    public string itemName = "Item";  // 디버그용 이름

    // 필요하면 여기서 아이콘, 타입, 인벤토리용 ID 등도 추가 가능
}
