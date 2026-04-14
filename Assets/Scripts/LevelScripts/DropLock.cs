using System.Collections;
using UnityEngine;

public class DropLock : MonoBehaviour
{
    private bool isDestroying = false;
    public bool isLocked = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    void OnTriggerEnter(Collider other)
    {
        if (isDestroying) return;

        if (!other.CompareTag("Player")) return;

        if (FindAnyObjectByType<PlayerInventory>().hasKey)
        {
            isDestroying = true;
            StartCoroutine(DestroyLock());
        }
    }

    private IEnumerator DestroyLock()
    {
        Time.timeScale = 0f;
        float duration = 0.5f;
        float time = 0f;

        Vector3 initialScale = transform.localScale;

        while (time < duration)
        {
            float t = time / duration;

            // Reducir escala progresivamente
            transform.localScale = Vector3.Lerp(initialScale, Vector3.zero, t);

            time += Time.unscaledDeltaTime;
            yield return null;
        }

        // Asegurar escala final
        transform.localScale = Vector3.zero;
        Time.timeScale = 1f;
        isLocked = false;
        Destroy(gameObject);
    }
}
