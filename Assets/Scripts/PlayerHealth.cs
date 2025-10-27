using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    private float maxHealth = 3f;
    private float minHealth = 0f;
    public float healthPoints;   // Vida inicial (máx = 3)
    public GameObject hud;            // HUD que contiene todos los estados de vida

    private List<GameObject> corazones = new List<GameObject>();

    void Start()
    {
        this.healthPoints = this.maxHealth;
        // Guarda todos los hijos del HUD
        foreach (Transform child in hud.transform)
        {
            corazones.Add(child.gameObject);
        }

        Debug.Log("Cantidad de estados de vida: " + corazones.Count);

        UpdateHUD();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Enemy_Zombie"))
        {
            healthPoints -= 0.5f;              // Baja de medio en medio
        }

        else if (other.gameObject.CompareTag("Heart"))
        {

            healthPoints += 0.5f;
            Destroy(other.gameObject);
        }
        healthPoints = Mathf.Clamp(healthPoints, minHealth, maxHealth); //Evita que salga de los limites
        UpdateHUD();

    }

    private void UpdateHUD()
    {
        // Desactiva todos los estados
        foreach (GameObject vida in corazones)
        {
            vida.SetActive(false);
        }

        // Determina qué HUD debe mostrarse según healthPoints
        string nombreHUD = "";

        switch (healthPoints)
        {
            case 3f:
                nombreHUD = "Vida_3_de_3";
                break;
            case 2.5f:
                nombreHUD = "Vida_2_5_de_3";
                break;
            case 2f:
                nombreHUD = "Vida_2_de_3";
                break;
            case 1.5f:
                nombreHUD = "Vida_1_5_de_3";
                break;
            case 1f:
                nombreHUD = "Vida_1_de_3";
                break;
            case 0.5f:
                nombreHUD = "Vida_0_5_de_3";
                break;
            default:
                nombreHUD = "Vida_0_de_3";
                break;
        }

        // Activa solo el HUD que corresponde
        foreach (GameObject vida in corazones)
        {
            if (vida.name == nombreHUD)
            {
                vida.SetActive(true);
                break;
            }
        }

        Debug.Log("HUD activo: " + nombreHUD);
    }
}
