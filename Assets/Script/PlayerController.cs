using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerInteract))]
public class PlayerController : MonoBehaviour
{
    private PlayerMovement movement;   // 이동 전담 스크립트
    private PlayerInteract interact;   // 아이템 전담 스크립트

    private void Awake()
    {
        // 같은 오브젝트에 붙어있는 컴포넌트 찾아두기
        movement = GetComponent<PlayerMovement>();
        interact = GetComponent<PlayerInteract>();
    }

    // ───────── Input System 이벤트들 ─────────

    // 이동 입력 (-1 ~ 1)
    public void OnMove(InputValue value)
    {
        float x = value.Get<float>();
        movement.SetMoveInput(x);
    }

    // 점프 입력
    public void OnJump(InputValue value)
    {
        if (value.isPressed)
        {
            movement.TryJump();
        }
    }

    // 상호작용 입력
    public void OnInteract(InputValue value)
    {
        if (value.isPressed)
        {
            interact.TryInteract();
        }
    }
}
