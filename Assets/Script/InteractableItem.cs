using UnityEngine;

// 이 스크립트를 붙이면 "플레이어가 주울 수 있는 대상" + "위아래로 둥둥 뜨는 효과"가 생김
public class InteractableItem : MonoBehaviour
{
    [Header("Item Info")]
    public string itemName = "Item";  // 디버그용 이름

    [Header("Floating Motion Settings")]
    private float floatAmplitude = 0.2f; // 위아래 이동 범위
    private float floatSpeed = 8f;       // 위아래 이동 속도

    private Vector3 startPos; // 시작 위치 저장

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        // 사인파를 이용해 부드럽게 위아래 이동
        float newY = startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
        transform.position = new Vector3(startPos.x, newY, startPos.z);
    }
}
