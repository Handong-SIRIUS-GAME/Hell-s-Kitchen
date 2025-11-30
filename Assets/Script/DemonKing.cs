using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class DemonKing : MonoBehaviour
{
    [Header("스프라이트 설정")]
    [Tooltip("눈 뜨고 있는 마왕 이미지")]
    public Sprite eyesOpenSprite;

    [Tooltip("눈 감고 있는 마왕 이미지")]
    public Sprite eyesClosedSprite;

    [Header("연출 설정")]
    [Tooltip("페이드인 속도 (낮을수록 천천히 나타남)")]
    public float fadeInSpeed = 1.0f;

    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // 오브젝트가 활성화(SetActive true) 될 때마다 실행됨
    void OnEnable()
    {
        // 1. 투명하게 시작해서 점점 나타나기
        StartCoroutine(FadeInRoutine());

        // 2. 눈 깜빡이기 루틴 시작
        StartCoroutine(BlinkRoutine());
    }

    // 점점 나타나는 효과 (Fade In)
    IEnumerator FadeInRoutine()
    {
        Color color = spriteRenderer.color;
        color.a = 0f; // 알파값(투명도) 0으로 시작
        spriteRenderer.color = color;

        while (color.a < 1f)
        {
            color.a += Time.deltaTime * fadeInSpeed; // 점점 불투명해짐
            spriteRenderer.color = color;
            yield return null; // 다음 프레임까지 대기
        }
    }

    // 눈 깜빡이는 효과
    IEnumerator BlinkRoutine()
    {
        // 무한 반복
        while (true)
        {
            // 눈 뜨기
            spriteRenderer.sprite = eyesOpenSprite;

            // 2초 ~ 4초 사이의 랜덤한 시간 동안 기다림
            float waitTime = Random.Range(2f, 4f);
            yield return new WaitForSeconds(waitTime);

            // 눈 감기
            spriteRenderer.sprite = eyesClosedSprite;

            // 0.15초 동안 감고 있음 (깜빡!)
            yield return new WaitForSeconds(0.15f);
        }
    }
}