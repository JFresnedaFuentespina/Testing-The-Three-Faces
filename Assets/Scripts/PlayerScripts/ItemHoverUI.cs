using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemHoverUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string description;
    public TooltipUI tooltipUI;

    public void OnPointerEnter(PointerEventData eventData)
    {
        tooltipUI.Show(description);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltipUI.Hide();
    }
}
