using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerInteract))]
public class PlayerController : MonoBehaviour
{
    private PlayerMovement movement;
    private PlayerInteract interact;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        interact = GetComponent<PlayerInteract>();
    }

    // 1. 이동 (Move)
    public void OnMove(InputValue value)
    {
        float x = value.Get<float>();
        movement.SetMoveInput(x);
    }

    // 2. 점프 (Jump - Space)
    public void OnJump(InputValue value)
    {
        if (value.isPressed)
        {
            Debug.Log("Jump(Space) 키 입력됨");
            movement.TryJump();
        }
    }

    // 3. 하향 점프 (JumpDown - S)
    // [중요] Input Action 이름을 "JumpDown"(띄어쓰기 없음)으로 해주세요.
    public void OnJumpDown(InputValue value)
    {
        if (value.isPressed)
        {
            Debug.Log("JumpDown(S) 키 입력됨");
            movement.TryJumpDown();
        }
    }

    // 4. 상호작용 (Interact - F)
    public void OnInteract(InputValue value)
    {
        if (value.isPressed)
        {
            interact.TryInteract();
        }
    }
}