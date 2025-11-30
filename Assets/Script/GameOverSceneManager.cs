using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverSceneManager : MonoBehaviour
{
    [Header("씬 이름 설정")]
    [Tooltip("다시 시작할 게임 씬의 이름")]
    // [수정] 기본값을 요청하신 "JisungRyu"로 변경해 두었습니다.
    public string mainGameSceneName = "JisungRyu";

    /// <summary>
    /// 게임을 다시 시작합니다. (UI 버튼의 OnClick() 이벤트에 연결)
    /// </summary>
    public void RestartGame()
    {
        if (string.IsNullOrEmpty(mainGameSceneName))
        {
            Debug.LogError("이동할 씬 이름이 설정되지 않았습니다!");
            return;
        }

        Debug.Log(mainGameSceneName + " 씬으로 돌아갑니다.");
        SceneManager.LoadScene(mainGameSceneName);
    }
}