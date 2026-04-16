using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    public float velocity = 5.0f;
    public float currentSpeed;
    public bool hasShield = true;
    private Rigidbody rb;

    private Animator animatorEsqueleto;
    private Animator animatorFantasma;
    private ChangeCharacter changeCharacter;

    private Vector3 lastPosition;
    public GameObject shield;

    public delegate void OnSpeedStatsChanged(float speed);
    public static event OnSpeedStatsChanged OnSpeedStatsChangedEvent;
    public delegate void OnSpeedStatsRequested();
    public static event OnSpeedStatsRequested OnSpeedStatsRequestedEvent;


    void OnEnable()
    {
        OnSpeedStatsRequestedEvent += SendCurrentStats;
    }
    void OnDestroy()
    {
        OnSpeedStatsRequestedEvent -= SendCurrentStats;
        IncreaseSpeedItemPickupBehaviour.OnPlayerSpeedEvent -= UpdateSpeed;
        StarItemPickupBehaviour.OnPlayerSpeedEvent -= UpdateSpeed;
        ShieldItemPickupBehaviour.OnAddShieldEvent -= AddShield;
    }

    void Start()
    {
        SubscribeToPickupEvents();
        changeCharacter = GetComponent<ChangeCharacter>();

        Transform esqueletoHijo = transform.Find("Esqueleto");
        if (esqueletoHijo == null)
        {
            Debug.LogError("No se encontró el hijo llamado 'Esqueleto'");
            return;
        }

        // Obtener el Animator SOLO de ese hijo
        animatorEsqueleto = esqueletoHijo.GetComponent<Animator>();
        if (animatorEsqueleto == null)
        {
            Debug.LogError("El hijo 'Esqueleto' existe, pero no tiene Animator");
            return;
        }

        Transform ghostHijo = transform.Find("Ghost");
        if (ghostHijo == null)
        {
            Debug.LogError("No se encontró el hijo llamado 'Ghost'");
            return;
        }

        // Obtener el Animator SOLO de ese hijo
        animatorFantasma = ghostHijo.GetComponent<Animator>();
        if (animatorFantasma == null)
        {
            Debug.LogError("El hijo 'Ghost' existe, pero no tiene Animator");
            return;
        }


        // Load JSON stats
        string path = Application.persistentDataPath + "/player.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            PlayerData playerData = JsonConvert.DeserializeObject<PlayerData>(json);
            velocity = playerData.velocity;
            hasShield = playerData.hasShield;
            shield.SetActive(hasShield);
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
        IncreaseSpeedItemPickupBehaviour.OnPlayerSpeedEvent += UpdateSpeed;
        StarItemPickupBehaviour.OnPlayerSpeedEvent += UpdateSpeed;
        ShieldItemPickupBehaviour.OnAddShieldEvent += AddShield;
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
        if (animatorEsqueleto == null || rb == null) return;

        animatorEsqueleto.applyRootMotion = false;

        float inputH = Input.GetAxis("Horizontal");
        float inputV = Input.GetAxis("Vertical");

        Camera cam = Camera.main;

        Vector3 camForward = cam.transform.forward;
        Vector3 camRight = cam.transform.right;

        camForward.y = 0;
        camRight.y = 0;

        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDir = camForward * inputV + camRight * inputH;

        if (moveDir.magnitude > 1f)
            moveDir.Normalize();

        Vector3 movement = moveDir * velocity * Time.fixedDeltaTime;

        Vector3 targetVelocity = moveDir * velocity;

        // Mantener la velocidad en Y por si hay gravedad
        targetVelocity.y = rb.linearVelocity.y;

        rb.linearVelocity = targetVelocity;

        // Para calcular la Speed del Animator, usa la magnitud de la velocidad
        float horizontalSpeed = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z).magnitude;

        if (!changeCharacter.showingGhost)
        {
            animatorEsqueleto.SetFloat("Speed", horizontalSpeed);
            animatorEsqueleto.SetBool("HasShield", hasShield);
        }
        else
        {
            animatorFantasma.SetFloat("Speed", horizontalSpeed);
        }
    }

    public void AddShield()
    {
        hasShield = true;
        shield.SetActive(true);
    }


}
