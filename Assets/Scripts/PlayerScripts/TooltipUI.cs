using System;
using TMPro;
using UnityEngine;

public class TooltipUI : MonoBehaviour
{
    public RectTransform panel;
    public TextMeshProUGUI text;
    // Update is called once per frame
    void Update()
    {
        if (panel.gameObject.activeSelf)
        {
            Vector2 pos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(panel.parent as RectTransform, Input.mousePosition, null, out pos);
            panel.localPosition = pos + new Vector2(150, -15);
        }
    }

    public void Show(string description)
    {
        text.text = description;
        panel.gameObject.SetActive(true);
    }

    public void Hide()
    {
        panel.gameObject.SetActive(false);
    }

    void OnDisable()
    {
        Hide();
    }
}
