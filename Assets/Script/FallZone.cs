using UnityEngine;
using UnityEngine.SceneManagement;

// [중요] 여기 'FallZone'이 파일 이름과 똑같아야 합니다!
public class FallZone : MonoBehaviour
{
    [Header("설정")]
    [Tooltip("플레이어가 떨어졌을 때 이동할 씬 이름 (예: GameOverScene)")]
    public string gameOverSceneName = "GameOverScene";

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 플레이어 태그를 가진 오브젝트가 닿으면
        if (other.CompareTag("Player"))
        {
            Debug.Log("플레이어 추락! 게임 오버.");

            if (!string.IsNullOrEmpty(gameOverSceneName))
            {
                SceneManager.LoadScene(gameOverSceneName);
            }
            else
            {
                Debug.LogError("FallZone: 이동할 씬 이름이 비어있습니다!");
            }
        }
    }
}