using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    private float maxHealth = 3f;
    private float minHealth = 0f;
    public float healthPoints;
    public GameObject hud;

    private List<GameObject> corazones = new List<GameObject>();

    void Start()
    {
        // Si no se asignó en el Inspector, buscar dinámicamente
        if (hud == null)
        {
            // Buscar todos los Canvas activos
            Canvas[] canvases = FindObjectsOfType<Canvas>();
            foreach (Canvas c in canvases)
            {
                if (c.gameObject.name == "HUD")
                {
                    // Buscar el panel HealthPoints dentro del HUD
                    hud = c.transform.Find("HealthPoints")?.gameObject;
                    break;
                }
            }
        }

        if (hud == null)
        {
            Debug.LogWarning("HUD o HealthPoints no encontrado.");
            return;
        }

        // Inicializar vida
        healthPoints = maxHealth;

        // Guardar referencias a los iconos de corazones
        corazones.Clear();
        foreach (Transform child in hud.transform)
        {
            corazones.Add(child.gameObject);
        }

        // Actualizar HUD inicial
        UpdateHUD();
    }




    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Enemy_Zombie"))
        {
            healthPoints -= 0.5f;
        }
        else if (other.gameObject.CompareTag("Heart"))
        {
            healthPoints += 0.5f;
            Destroy(other.gameObject);
        }

        // Limita el valor antes de actualizar HUD
        healthPoints = Mathf.Clamp(healthPoints, minHealth, maxHealth);

        UpdateHUD();
    }

    private void UpdateHUD()
    {
        // Desactiva todos los estados
        foreach (GameObject vida in corazones)
        {
            vida.SetActive(false);
        }

        // Redondea para evitar errores de precisión de float
        float vidaRedondeada = Mathf.Round(healthPoints * 2f) / 2f;

        // Determina qué HUD debe mostrarse
        string nombreHUD = $"Vida_{vidaRedondeada.ToString().Replace(',', '_').Replace('.', '_')}_de_3";

        // Si la vida es 0 o menor, asegura que muestre el vacío
        if (vidaRedondeada <= 0)
            nombreHUD = "Vida_0_de_3";

        // Activa solo el HUD correcto
        foreach (GameObject vida in corazones)
        {
            if (vida.name == nombreHUD)
            {
                vida.SetActive(true);
                break;
            }
        }

        Debug.Log($"HUD activo: {nombreHUD} (vida: {healthPoints})");
    }
}
