using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KitchenArea : MonoBehaviour
{
    [Header("Cooking Settings")]
    [Tooltip("요리에 필요한 재료 수 (총 3개)")]
    public int maxCapacity = 3;

    [Header("UI Settings")]
    [Tooltip("재료 이미지를 표시할 UI Image 슬롯들 (3개)")]
    public List<Image> ingredientSlots = new List<Image>();

    // 현재 부엌에 놓인 재료 리스트
    private List<InteractableItem> currentIngredients = new List<InteractableItem>();

    private void Start()
    {
        UpdateUI();
    }

    public bool AddIngredient(InteractableItem item)
    {
        if (currentIngredients.Count >= maxCapacity)
        {
            Debug.Log("부엌이 꽉 찼습니다!");
            return false;
        }

        currentIngredients.Add(item);
        Debug.Log(item.itemName + " 추가됨. (현재 " + currentIngredients.Count + " / " + maxCapacity + "개)");

        UpdateUI();

        if (currentIngredients.Count == maxCapacity)
        {
            Cook();
        }

        return true;
    }

    /// <summary>
    /// UI 업데이트 (흰색 사각형 방지 기능 추가됨)
    /// </summary>
    private void UpdateUI()
    {
        for (int i = 0; i < ingredientSlots.Count; i++)
        {
            if (i < currentIngredients.Count)
            {
                // [수정된 부분] 아이템에 이미지가 등록되어 있는지 확인!
                Sprite spriteToShow = currentIngredients[i].itemSprite;

                if (spriteToShow != null)
                {
                    // 이미지가 있으면 정상적으로 보여줌
                    ingredientSlots[i].sprite = spriteToShow;
                    ingredientSlots[i].enabled = true;

                    // (팁: 이미지가 찌그러지지 않게 하려면 아래 주석 해제)
                    // ingredientSlots[i].preserveAspect = true; 
                }
                else
                {
                    // [핵심] 이미지가 없으면(null) 그냥 투명하게 꺼버림 -> 흰색 사각형 방지
                    ingredientSlots[i].sprite = null;
                    ingredientSlots[i].enabled = false;
                }
            }
            else
            {
                // 재료가 없는 슬롯은 끄기
                ingredientSlots[i].sprite = null;
                ingredientSlots[i].enabled = false;
            }
        }
    }

    private void Cook()
    {
        Debug.Log("재료가 3개 모여 요리를 시작합니다...");

        int totalValue = 0;
        foreach (InteractableItem item in currentIngredients)
        {
            totalValue += (int)item.value;
        }
        int tasteResult = totalValue % 3;

        string tasteString = "";
        switch (tasteResult)
        {
            case 0: tasteString = "맛있음"; break;
            case 1: tasteString = "매움"; break;
            case 2: tasteString = "짠 맛"; break;
        }
        Debug.Log($"요리 완성! [최종 맛: {tasteString}]");

        currentIngredients.Clear();
        UpdateUI();
    }
}