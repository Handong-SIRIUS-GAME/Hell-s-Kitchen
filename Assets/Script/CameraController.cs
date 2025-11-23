using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player;
    public float followSpeed = 5f;
    public Vector3 offset = new Vector3(0, 2, -10);

    private void LateUpdate()
    {
        if (player == null) return;

        Vector3 targetPos = player.position + offset;

        // 부드럽게 따라가기
        // t = 1 - exp(-k * dt) 형태라 프레임에 덜 민감함
        float t = 1f - Mathf.Exp(-followSpeed * Time.deltaTime);
        transform.position = Vector3.Lerp(transform.position, targetPos, t);
    }
}
