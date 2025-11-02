using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem; // 1. 'New Input System'을 사용하기 위해 추가!

/// <summary>
/// 플레이어가 범위 내에서 특정 키를 누르면 지정된 씬으로 이동시키는 임시 트리거입니다.
/// (New Input System 호환 버전)
/// </summary>
public class SceneTrigger : MonoBehaviour
{
    [Header("설정")]
    [Tooltip("이동할 씬의 이름을 정확하게 입력하세요 (예: GameOverScene)")]
    public string sceneToLoad;

    [Tooltip("상호작용에 사용할 임시 키 (New Input System)")]
    // 2. KeyCode (Old) -> Key (New) 로 변경
    public Key interactKey = Key.F;

    // --- 내부 상태 ---
    private bool isPlayerInRange = false;

    void Awake()
    {
        // Collider2D가 있는지 확인
        Collider2D col = GetComponent<Collider2D>();
        if (col == null)
        {
            Debug.LogError("SceneTrigger: Collider2D가 없습니다! 이 오브젝트에 Box Collider 2D 등을 추가해주세요.");
            return;
        }

        // Is Trigger가 체크되어 있는지 확인
        if (!col.isTrigger)
        {
            Debug.LogWarning("SceneTrigger: Collider2D의 'Is Trigger'가 체크되어 있지 않습니다. 자동으로 체크합니다.");
            col.isTrigger = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            Debug.Log("플레이어가 씬 트리거 범위에 진입");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            Debug.Log("플레이어가 씬 트리거 범위에서 이탈");
        }
    }

    private void Update()
    {
        // 3. 'Old' 방식 대신 'New' 방식으로 키 입력 감지
        // (Keyboard.current != null) 는 키보드가 연결되어 있는지 확인하는 안전장치
        // [interactKey] 는 인스펙터에서 설정한 키(F)를 의미
        // .wasPressedThisFrame 는 '방금 이 프레임에 눌렸는가?' (GetKeyDown과 동일)
        if (isPlayerInRange && Keyboard.current != null && Keyboard.current[interactKey].wasPressedThisFrame)
        {
            if (string.IsNullOrEmpty(sceneToLoad))
            {
                Debug.LogWarning("SceneTrigger: 'Scene To Load' 이름이 설정되지 않았습니다!");
                return;
            }

            Debug.Log(sceneToLoad + " 씬으로 이동합니다.");
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}

