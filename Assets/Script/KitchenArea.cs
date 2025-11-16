using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // [중요] Canvas의 Image 컴포넌트를 사용하기 위해 꼭 필요합니다.

/// <summary>
/// '부엌' 영역을 정의하는 스크립트입니다.
/// 1. 플레이어로부터 재료 아이템을 받습니다. (최대 3개)
/// 2. 재료를 받을 때마다 연결된 UI Image 슬롯에 해당 아이템의 스프라이트를 표시합니다.
/// 3. 재료가 3개 모이면 '맛'을 계산하고 콘솔에 출력합니다.
/// 4. 요리 후 재료 리스트와 UI 슬롯을 비웁니다.
/// </summary>
public class KitchenArea : MonoBehaviour
{
    [Header("Cooking Settings")]
    [Tooltip("요리에 필요한 재료 수 (총 3개)")]
    public int maxCapacity = 3; // 최대 재료 수

    [Header("UI Settings")]
    [Tooltip("재료 이미지를 표시할 UI Image 슬롯들 (3개)")]
    // Inspector 창에서 Canvas > Slot1, Slot2, Slot3 (Image) 오브젝트를 순서대로 끌어다 놓아야 함
    public List<Image> ingredientSlots = new List<Image>();

    // 현재 부엌에 놓인 재료 아이템의 '데이터(InteractableItem)'를 저장하는 리스트
    private List<InteractableItem> currentIngredients = new List<InteractableItem>();

    private void Start()
    {
        // 게임 시작 시, UI 슬롯을 한번 비워서 깨끗하게 시작
        UpdateUI();
    }

    /// <summary>
    /// PlayerInteract 스크립트가 호출: 부엌에 재료를 추가합니다.
    /// </summary>
    /// <param name="item">플레이어가 드롭한 재료 아이템의 InteractableItem 스크립트</param>
    /// <returns>추가 성공 시 true, 꽉 찼으면 false</returns>
    public bool AddIngredient(InteractableItem item)
    {
        // 1. 재료가 꽉 찼는지 확인
        if (currentIngredients.Count >= maxCapacity)
        {
            Debug.Log("부엌이 꽉 찼습니다!");
            return false; // 추가 실패
        }

        // 2. 재료 리스트에 '데이터' 추가
        currentIngredients.Add(item);
        Debug.Log(item.itemName + " 추가됨. (현재 " + currentIngredients.Count + " / " + maxCapacity + "개)");

        // 3. UI 업데이트 (재료가 추가됐으니 UI에 반영)
        UpdateUI();

        // 4. 재료가 3개 다 찼으면 자동으로 요리 시작
        if (currentIngredients.Count == maxCapacity)
        {
            Cook();
        }

        return true; // 추가 성공
    }

    /// <summary>
    /// 현재 재료 리스트(currentIngredients)를 기반으로 UI 슬롯(ingredientSlots) 이미지를 업데이트합니다.
    /// </summary>
    private void UpdateUI()
    {
        // 1. Inspector에 연결된 모든 UI 슬롯(Image)을 순회합니다 (총 3번)
        for (int i = 0; i < ingredientSlots.Count; i++)
        {
            // 2. 현재 재료 리스트(currentIngredients)에 i번째 아이템이 있는가?
            // (예: 재료가 1개(Count=1)면, i=0 일때만 true)
            if (i < currentIngredients.Count)
            {
                // 있으면:
                // UI 슬롯(Image)의 sprite를 해당 재료의 itemSprite로 변경
                ingredientSlots[i].sprite = currentIngredients[i].itemSprite;
                // UI 슬롯(Image)을 활성화 (보이게 만듦)
                ingredientSlots[i].enabled = true;
            }
            else
            {
                // 없으면: (i=1, i=2 에 해당)
                // UI 슬롯(Image)의 sprite를 비움
                ingredientSlots[i].sprite = null;
                // UI 슬롯(Image)을 비활성화 (투명하게 만듦)
                ingredientSlots[i].enabled = false;
            }
        }
    }

    /// <summary>
    /// 재료 3개를 조합해 맛을 계산하고, 부엌을 비웁니다.
    /// </summary>
    private void Cook()
    {
        Debug.Log("재료가 3개 모여 요리를 시작합니다...");

        // 1. 맛 값 계산 (요청사항 1)
        int totalValue = 0;
        // 부엌에 있는 모든 재료를 순회
        foreach (InteractableItem item in currentIngredients)
        {
            // 각 재료의 value(0 또는 1)를 모두 더함
            totalValue += (int)item.value;
        }

        // 2. 최종 값을 3으로 나눈 나머지 (0: 맛있음, 1: 매움, 2: 짠 맛)
        int tasteResult = totalValue % 3;

        // 3. 결과 판정
        string tasteString = "";
        switch (tasteResult)
        {
            case 0: tasteString = "맛있음"; break;
            case 1: tasteString = "매움"; break;
            case 2: tasteString = "짠 맛"; break;
        }

        Debug.Log($"요리 완성! [최종 맛: {tasteString}] (계산값: {totalValue} % 3 = {tasteResult})");

        // 4. 요리 후 부엌 비우기
        currentIngredients.Clear();

        // 5. UI 슬롯도 비우기 (UpdateUI를 다시 호출하면 빈 리스트 기준으로 UI가 정리됨)
        UpdateUI();
    }
}