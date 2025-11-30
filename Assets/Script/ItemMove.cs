using UnityEngine;

public class ItemMove : MonoBehaviour
{
    public enum MoveMode
    {
        Horizontal = 1, // 좌우 이동
        Vertical = 2,   // 위아래 이동
        Circle = 3,   // 원 운동
        Teleport = 4    // 순간이동
    }

    [Header("기본 설정")]
    public MoveMode mode = MoveMode.Horizontal; // 인스펙터에서 모드 선택
    public float distance = 2f;                  // 좌우/위아래 이동 거리
    public float speed = 2f;                     // 이동/회전 속도

    [Header("원 운동 설정 (Circle 모드 사용)")]
    public float radius = 2f;                    // 원 반지름

    [Header("순간이동 설정 (Teleport 모드 사용)")]
    public Transform point1;                     // 1번 위치
    public Transform point2;                     // 2번 위치
    public Transform point3;                     // 3번 위치
    public float teleportInterval = 1.0f;        // 몇 초마다 순간이동할지

    private Vector3 startPos;
    private float teleportTimer = 0f;
    private int teleportIndex = 0;

    void Start()
    {
        startPos = transform.position;

        // Teleport 모드일 때 시작 위치를 첫 포인트로 맞춰줌 (있으면)
        if (mode == MoveMode.Teleport)
        {
            Transform first = GetTeleportTarget(0);
            if (first != null)
                transform.position = first.position;
        }
    }

    void Update()
    {
        switch (mode)
        {
            case MoveMode.Horizontal:
                MoveHorizontal();
                break;

            case MoveMode.Vertical:
                MoveVertical();
                break;

            case MoveMode.Circle:
                MoveCircle();
                break;

            case MoveMode.Teleport:
                TeleportMove();
                break;
        }
    }

    // 1번 모드: 좌우 이동
    void MoveHorizontal()
    {
        float newX = startPos.x + Mathf.Sin(Time.time * speed) * distance;
        transform.position = new Vector3(newX, startPos.y, startPos.z);
    }

    // 2번 모드: 위아래 이동
    void MoveVertical()
    {
        float newY = startPos.y + Mathf.Sin(Time.time * speed) * distance;
        transform.position = new Vector3(startPos.x, newY, startPos.z);
    }

    // 3번 모드: 원을 그리면서 이동
    void MoveCircle()
    {
        float t = Time.time * speed;
        float newX = startPos.x + Mathf.Cos(t) * radius;
        float newY = startPos.y + Mathf.Sin(t) * radius;
        transform.position = new Vector3(newX, newY, startPos.z);
    }

    // 4번 모드: 1,2,3번 위치를 순서대로 순간이동
    void TeleportMove()
    {
        teleportTimer += Time.deltaTime;
        if (teleportTimer >= teleportInterval)
        {
            teleportTimer = 0f;

            teleportIndex = (teleportIndex + 1) % 3; // 0 → 1 → 2 → 0 …

            Transform target = GetTeleportTarget(teleportIndex);
            if (target != null)
            {
                transform.position = target.position;
            }
        }
    }

    // 인덱스에 따라 point1/2/3 반환
    Transform GetTeleportTarget(int index)
    {
        switch (index)
        {
            case 0: return point1;
            case 1: return point2;
            case 2: return point3;
            default: return null;
        }
    }
}