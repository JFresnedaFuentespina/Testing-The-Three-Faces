using System.Collections;
using UnityEngine;

public class ShieldFlash : MonoBehaviour
{
    public Renderer shieldRenderer;
    public Color flashColor = Color.white;
    public float intensity = 5f;
    public float duration = 0.1f;

    Material mat;
    Color originalEmission;
    void Start()
    {
        mat = shieldRenderer.material;
        originalEmission = mat.GetColor("_EmissionColor");
    }

    public void Flash()
    {
        StartCoroutine(FlashRoutine());
    }

    IEnumerator FlashRoutine()
    {
        mat.EnableKeyword("_EMISSION");
        mat.SetColor("_EmissionColor", flashColor * intensity);
        yield return new WaitForSecondsRealtime(duration);
        mat.SetColor("_EmissionColor", originalEmission);
    }
}
