using UnityEngine;
using UnityEngine.SceneManagement; // 씬 관리를 위해 필수!

/// <summary>
/// 게임 오버 씬의 '다시 시작' 버튼 기능을 관리합니다.
/// 
/// [중요!]
/// 이 스크립트의 파일 이름은 "GameOverSceneManager.cs" 여야 합니다.
/// public class "GameOverSceneManager" 와 이름이 같아야 합니다.
/// </summary>
public class GameOverSceneManager : MonoBehaviour
{
    [Header("씬 이름 설정")]
    [Tooltip("다시 시작할 게임 씬의 이름 (예: MainGameScene)")]
    public string mainGameSceneName = "MainGameScene"; // <- 본인 게임 씬 이름으로 변경하세요

    /// <summary>
    /// 게임을 다시 시작합니다. (UI 버튼의 OnClick() 이벤트에 연결)
    /// </summary>
    public void RestartGame()
    {
        // 씬 이름이 비어있지 않은지 확인
        if (string.IsNullOrEmpty(mainGameSceneName))
        {
            Debug.LogError("GameOverSceneManager: 'Main Game Scene Name'이 설정되지 않았습니다! Inspector 창에서 설정해주세요.");
            return;
        }

        Debug.Log(mainGameSceneName + " 씬에서 게임을 다시 시작합니다.");
        SceneManager.LoadScene(mainGameSceneName);
    }
}
