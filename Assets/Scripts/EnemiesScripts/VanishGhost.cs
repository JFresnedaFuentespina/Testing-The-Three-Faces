using System.Collections;
using UnityEngine;

public class VanishGhost : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private EnemyLife enemyLife;
    private Renderer[] renderers;
    public float fadeDuration = 2.0f;
    void Start()
    {
        enemyLife = GetComponent<EnemyLife>();
        renderers = GetComponentsInChildren<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (enemyLife.GetIsAlive() == false)
        {
            StartCoroutine(FadeOut());
            this.enabled = false;
        }
    }
    private IEnumerator FadeOut()
    {
        float elapsed = 0f;
        Color[][] originalColors = new Color[renderers.Length][];
        for (int i = 0; i < renderers.Length; i++)
        {
            originalColors[i] = new Color[renderers[i].materials.Length];
            for (int j = 0; j < renderers[i].materials.Length; j++)
            {
                originalColors[i][j] = renderers[i].materials[j].color;
                Material mat = renderers[i].materials[j];
                mat.SetFloat("_Mode", 2); // 2 = transparent (Standard Shader)
                mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                mat.SetInt("_ZWrite", 0);
                mat.DisableKeyword("_ALPHATEST_ON");
                mat.EnableKeyword("_ALPHABLEND_ON");
                mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                mat.renderQueue = 3000;
            }
        }
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);

            for (int i = 0; i < renderers.Length; i++)
            {
                for (int j = 0; j < renderers[i].materials.Length; j++)
                {
                    Color c = originalColors[i][j];
                    c.a = alpha;
                    renderers[i].materials[j].color = c;
                }
            }

            yield return null;
        }
    }
}

