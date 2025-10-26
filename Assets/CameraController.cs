using UnityEngine;

public class CameraController : MonoBehaviour
{
    // 따라갈 대상을 Inspector 창에서 연결할 수 있도록 public으로 선언
    public Transform player;

    // 카메라의 부드러운 이동 속도
    public float smoothSpeed = 0.125f;

    // 플레이어로부터 떨어져 있을 거리 (카메라 위치 오프셋)
    public Vector3 offset;

    // LateUpdate는 모든 Update 함수가 호출된 후에 실행됩니다.
    // 플레이어가 움직인 후에 카메라가 따라가야 랙이나 떨림이 없기 때문에 LateUpdate를 사용합니다.
    void LateUpdate()
    {
        // 플레이어가 할당되지 않았다면 아무것도 하지 않음
        if (player == null)
        {
            return;
        }

        // 1. 목표 위치 계산: 플레이어의 현재 위치 + 오프셋
        Vector3 desiredPosition = player.position + offset;

        // 2. 부드러운 이동 계산 (Lerp 사용)
        // 현재 카메라 위치에서 목표 위치까지 smoothSpeed에 따라 부드럽게 이동한 위치를 계산
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // 3. 계산된 위치로 카메라 이동
        transform.position = smoothedPosition;
    }
}