using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 3f;
    public float minHealth = 0f;
    public float healthPoints = 3;
    public GameObject hud;
    private List<GameObject> corazones = new List<GameObject>();
    public bool canDie = false;
    private Rigidbody rb;
    private Animator animator;
    private RotateCharacterToMouse rotateCharacterToMouse;
    private RotateCharacterWithJoystick rotateCharacterWithJoystick;
    private PlayerBehaviour playerBehaviour;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rotateCharacterToMouse = GetComponent<RotateCharacterToMouse>();
        rotateCharacterWithJoystick = GetComponent<RotateCharacterWithJoystick>();
        playerBehaviour = GetComponent<PlayerBehaviour>();
        Transform esqueletoHijo = transform.Find("Esqueleto");
        animator = esqueletoHijo != null ? esqueletoHijo.GetComponent<Animator>() : null;
        if (animator == null)
        {
            Debug.LogError("No se encontró el Animator dentro del hijo 'Esqueleto'");
            return;
        }
        string path = Application.persistentDataPath + "/player.json";
        bool loadedFromFile = false;
        if (File.Exists(path))
        {
            try
            {
                string json = File.ReadAllText(path);
                PlayerData data = JsonConvert.DeserializeObject<PlayerData>(json);

                if (data != null && data.maxHealth > 0)
                {
                    maxHealth = data.maxHealth;
                    healthPoints = Mathf.Clamp(data.health, 0f, data.maxHealth);
                    loadedFromFile = true;
                }
            }
            catch
            {
                Debug.LogWarning("Error cargando JSON de vida");
            }
        }

        if (!loadedFromFile)
        {
            // solo si no hay JSON, usar valor por defecto
            healthPoints = maxHealth;
        }

        // Buscar HUD si no está asignado
        if (hud == null)
        {
            Canvas[] all = FindObjectsOfType<Canvas>();
            foreach (var c in all)
            {
                if (c.gameObject.name == "HUD")
                {
                    hud = c.transform.Find("HealthPoints")?.gameObject;
                    break;
                }
            }
        }

        if (hud == null) return;

        corazones.Clear();
        foreach (Transform t in hud.transform)
            corazones.Add(t.gameObject);

        UpdateHUD(false);

        Invoke(nameof(EnableDeath), 0.1f);
    }
    void EnableDeath() => canDie = true;
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Enemy_Zombie") || other.gameObject.CompareTag("Enemy_Ghost") || other.gameObject.CompareTag("BossCara")|| other.gameObject.CompareTag("EnemyProjectile"))
        {
            healthPoints -= 0.5f;
        }
        // Limita el valor antes de actualizar HUD
        healthPoints = Mathf.Clamp(healthPoints, minHealth, maxHealth);
        UpdateHUD();
    }
    public void CheckDeath()
    {
        if (!canDie || healthPoints > 0) return;

        animator.SetTrigger("Death");

        // Bloquear movimiento y rotación
        BlockPlayerControl();

        // Borrar JSON si existe
        string path = Application.persistentDataPath + "/player.json";
        if (File.Exists(path))
            File.Delete(path);

        // Volver al menú tras delay
        StartCoroutine(DeathAndReturnToMenu());
    }

    void BlockPlayerControl()
    {
        if (playerBehaviour != null)
            playerBehaviour.enabled = false;

        if (rotateCharacterToMouse != null)
            rotateCharacterToMouse.enabled = false;

        if (rotateCharacterWithJoystick != null)
            rotateCharacterWithJoystick.enabled = false;
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }
    }

    private IEnumerator DeathAndReturnToMenu()
    {
        float deathDuration = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(deathDuration + 5f);
        SceneManager.LoadScene("MainMenu");
    }



    private void UpdateHUD(bool checkDeath = true)
    {
        foreach (GameObject vida in corazones)
            vida.SetActive(false);

        float vidaRedondeada = Mathf.Round(healthPoints * 2f) / 2f;
        string nombreHUD = $"Vida_{vidaRedondeada.ToString().Replace(',', '_').Replace('.', '_')}_de_3";

        if (vidaRedondeada <= 0)
            nombreHUD = "Vida_0_de_3";

        foreach (GameObject vida in corazones)
        {
            if (vida.name == nombreHUD)
            {
                vida.SetActive(true);
                break;
            }
        }

        if (checkDeath)
            CheckDeath();
    }

    public void Damage()
    {
        healthPoints -= 0.5f;
        healthPoints = Mathf.Clamp(healthPoints, minHealth, maxHealth);
        UpdateHUD();
        Debug.Log("Player damaged. Current health: " + healthPoints);
    }

}
