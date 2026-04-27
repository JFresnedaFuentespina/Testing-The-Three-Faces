using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class TooltipUI : MonoBehaviour
{
    public RectTransform panel;
    public TextMeshProUGUI text;
    public CanvasGroup canvasGroup;

    private Coroutine animRoutine;
    // Update is called once per frame
    void Update()
    {
        if (canvasGroup.alpha > 0f)
        {
            Vector2 pos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                panel.parent as RectTransform,
                Input.mousePosition,
                null,
                out pos
            );

            panel.localPosition = pos + new Vector2(150, -15);
        }
    }


    public void Show(string description)
    {
        if (!gameObject.activeInHierarchy) return;

        text.text = description;

        SetHeight(250f);

        if (animRoutine != null)
            StopCoroutine(animRoutine);

        animRoutine = StartCoroutine(FadeIn());
    }

    public void Hide()
    {
        if (!gameObject.activeInHierarchy) return;

        if (animRoutine != null)
            StopCoroutine(animRoutine);

        animRoutine = StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {

        float duration = 0.7f;
        float t = 0f;

        // pequeño efecto de escala
        panel.localScale = new Vector3(0.9f, 0.9f, 1f);

        while (t < duration)
        {
            t += Time.deltaTime;
            float progress = t / duration;

            canvasGroup.alpha = Mathf.Lerp(0f, 1f, progress);
            panel.localScale = Vector3.Lerp(new Vector3(0.9f, 0.9f, 1f), Vector3.one, progress);

            yield return null;
        }

        canvasGroup.alpha = 1f;
        panel.localScale = Vector3.one;
    }

    IEnumerator FadeIn()
    {
        float duration = 0.7f;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float progress = t / duration;

            canvasGroup.alpha = Mathf.Lerp(1f, 0f, progress);
            panel.localScale = Vector3.Lerp(Vector3.one, new Vector3(0.9f, 0.9f, 1f), progress);

            yield return null;
        }

        canvasGroup.alpha = 0f;
    }

    void OnDisable()
    {
        if (canvasGroup != null)
            canvasGroup.alpha = 0f;

    }
    void SetHeight(float height)
    {
        Vector2 size = panel.sizeDelta;
        size.y = height;
        panel.sizeDelta = size;
    }
}
