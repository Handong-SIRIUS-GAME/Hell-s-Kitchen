using UnityEngine;
using UnityEngine.InputSystem; // New Input System 사용

public class DoorEvent : MonoBehaviour
{
    [Header("연결할 오브젝트들")]
    [Tooltip("사라질 문 오브젝트를 넣으세요")]
    public GameObject doorObject;

    [Tooltip("나타날 마왕 오브젝트를 넣으세요")]
    public GameObject demonKingObject;

    private bool isPlayerIn = false; // 플레이어가 문 앞에 있는지

    void Start()
    {
        // 게임 시작 시 마왕은 안 보이게 숨겨둠
        if (demonKingObject != null)
        {
            demonKingObject.SetActive(false);
        }
    }

    // 트리거 감지
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerIn = true;
            Debug.Log("문 앞 도착! (W 또는 E 키로 입장)");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerIn = false;
        }
    }

    void Update()
    {
        // 플레이어가 문 앞에 있고 + 키보드가 연결되어 있을 때
        if (isPlayerIn && Keyboard.current != null)
        {
            // W 키 또는 E 키가 눌렸는지 확인
            if (Keyboard.current.wKey.wasPressedThisFrame || Keyboard.current.eKey.wasPressedThisFrame)
            {
                StartBossEvent();
            }
        }
    }

    void StartBossEvent()
    {
        Debug.Log("마왕 소환 이벤트 시작!");

        // 1. 문 사라지게 하기
        if (doorObject != null)
            doorObject.SetActive(false); // 또는 Destroy(doorObject);

        // 2. 마왕 나타나게 하기 (SetActive true가 되면 마왕 스크립트가 켜짐)
        if (demonKingObject != null)
            demonKingObject.SetActive(true);

        // 3. 이 이벤트 트리거 자체도 끄기 (재사용 방지)
        gameObject.SetActive(false);
    }
}