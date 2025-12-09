using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class IntroFade : MonoBehaviour
{
    public RawImage the3FacesLogo;
    public RawImage kidneyGamesLogo;
    public float fadeDuration = 1f;
    public float firstImageDuration = 2f;

    private bool isFading = false;

    private void Start()
    {
        StartCoroutine(PlayIntro());
    }

    private IEnumerator PlayIntro()
    {
        // Asegurar estados iniciales
        the3FacesLogo.gameObject.SetActive(true);
        kidneyGamesLogo.gameObject.SetActive(true);

        Color firstColor = the3FacesLogo.color;
        Color secondColor = kidneyGamesLogo.color;

        firstColor.a = 1f;  // se ve completamente
        secondColor.a = 0f; // aún no aparece
        the3FacesLogo.color = firstColor;
        kidneyGamesLogo.color = secondColor;

        // Mantener la primera imagen visible un tiempo
        yield return new WaitForSeconds(firstImageDuration);

        // Iniciar fade
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float t = timer / fadeDuration;

            // Fade out de la primera imagen
            firstColor.a = Mathf.Lerp(1f, 0f, t);
            the3FacesLogo.color = firstColor;

            // Fade in de la segunda
            secondColor.a = Mathf.Lerp(0f, 1f, t);
            kidneyGamesLogo.color = secondColor;

            yield return null;
        }

        // Valores finales asegurados
        firstColor.a = 0f;
        secondColor.a = 1f;

        the3FacesLogo.color = firstColor;
        kidneyGamesLogo.color = secondColor;

        // Ocultar la primera
        the3FacesLogo.gameObject.SetActive(false);
    }
}
