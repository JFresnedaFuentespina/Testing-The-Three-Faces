using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBlock : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public ChangeCharacter changeCharacter;
    public Animator animatorEsqueleto;
    public AudioClip shieldSFX;
    public AudioSource audioSource;
    public GameObject shield;
    public PlayerHealth playerHealth;
    public ShieldFlash shieldFlash;
    public InputActionAsset InputActions;
    public InputAction m_shieldBlockAction;
    public bool isBlocking = false;

    void OnEnable()
    {
        m_shieldBlockAction = InputActions.FindAction("ShieldBlock");

        if (m_shieldBlockAction != null)
            m_shieldBlockAction.performed += OnBlockPerformed;

        InputActions.Enable();
    }

    void OnDisable()
    {
        InputActions.Disable();

        if (m_shieldBlockAction != null)
            m_shieldBlockAction.performed -= OnBlockPerformed;
    }
    void Start()
    {
        m_shieldBlockAction = InputActions.FindAction("ShieldBlock");
        m_shieldBlockAction.performed += OnBlockPerformed;
        audioSource.clip = shieldSFX;
    }

    void OnBlockPerformed(InputAction.CallbackContext context)
    {
        if (isBlocking) return;
        if (!changeCharacter.showingGhost)
        {
            StartCoroutine(BlockDuration());
        }
    }

    IEnumerator BlockDuration()
    {
        isBlocking = true;
        if (animatorEsqueleto == null || shield == null)
            yield break;

        playerHealth.canGetHit = false;
        animatorEsqueleto.SetTrigger("Block");
        audioSource.Play();
        shieldFlash.Flash();

        yield return new WaitForSecondsRealtime(0.8f);

        playerHealth.canGetHit = true;
        isBlocking = false;
    }
}
