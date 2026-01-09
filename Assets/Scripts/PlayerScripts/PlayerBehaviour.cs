using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    public float velocity = 10.0f;
    private Rigidbody rb;

    private Animator animator;
    private ChangeCharacter changeCharacter;

    public delegate void OnSpeedStatsChanged(float speed);
    public static event OnSpeedStatsChanged OnSpeedStatsChangedEvent;
    public delegate void OnSpeedStatsRequested();
    public static event OnSpeedStatsRequested OnSpeedStatsRequestedEvent;

    void OnEnable()
    {
        OnSpeedStatsRequestedEvent += SendCurrentStats;
    }

    void Start()
    {
        SubscribeToPickupEvents();
        changeCharacter = GetComponent<ChangeCharacter>();

        // Buscar el TRANSFORM del hijo que se llama "Esqueleto"
        Transform esqueletoHijo = transform.Find("Esqueleto");
        if (esqueletoHijo == null)
        {
            Debug.LogError("No se encontró el hijo llamado 'Esqueleto'");
            return;
        }

        // Obtener el Animator SOLO de ese hijo
        animator = esqueletoHijo.GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("El hijo 'Esqueleto' existe, pero no tiene Animator");
            return;
        }

        // Prueba para confirmar que es el correcto
        Debug.Log("Animator correcto asignado: " + animator.gameObject.name);

        // Load JSON stats
        string path = Application.persistentDataPath + "/player.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            PlayerData playerData = JsonConvert.DeserializeObject<PlayerData>(json);
            velocity = playerData.velocity;
        }

        rb = GetComponent<Rigidbody>();
        NotifySpeedStatsChanged();
    }
    public static void RequestBehaviourStats()
    {
        OnSpeedStatsRequestedEvent?.Invoke();
    }
    public void SubscribeToPickupEvents()
    {
        PickupItem.OnPlayerSpeedEvent += UpdateSpeed;
    }

    public void UpdateSpeed(float amount)
    {
        velocity += amount;
    }
    void SendCurrentStats()
    {
        NotifySpeedStatsChanged();
    }
    public void NotifySpeedStatsChanged()
    {
        OnSpeedStatsChangedEvent?.Invoke(this.velocity);
    }
    private void FixedUpdate()
    {
        if (animator == null) return;
        animator.applyRootMotion = false;

        float inputH = Input.GetAxis("Horizontal");
        float inputV = Input.GetAxis("Vertical");

        Vector3 movement = (Vector3.forward * inputV + Vector3.right * inputH);
        bool seEstaMoviendo = movement.magnitude > 0.01f;

        if (!changeCharacter.showingGhost)
        {
            animator.SetFloat("Action", seEstaMoviendo ? 2f : 0f);
        }
        else
        {
            animator.SetFloat("Action", 0f);
        }

        if (movement.magnitude > 1f)
            movement.Normalize();

        movement *= velocity * Time.deltaTime;
        rb.MovePosition(rb.position + movement);
    }

}
