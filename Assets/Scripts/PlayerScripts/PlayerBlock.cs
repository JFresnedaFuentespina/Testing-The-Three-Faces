using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBlock : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public ChangeCharacter changeCharacter;
    public Animator animatorEsqueleto;
    public GameObject shield;
    public InputActionAsset InputActions;
    public InputAction m_shieldBlockAction;


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
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnBlockPerformed(InputAction.CallbackContext context)
    {
        if (!changeCharacter.showingGhost)
        {
            StartCoroutine(BlockDuration());
        }
    }

    IEnumerator BlockDuration()
    {
        if (animatorEsqueleto == null || shield == null)
            yield break;

        animatorEsqueleto.SetTrigger("Block");

        var col = shield.GetComponent<Collider>();
        if (col != null)
            col.enabled = true;

        yield return new WaitForSeconds(animatorEsqueleto.GetCurrentAnimatorStateInfo(0).length);

        if (col != null)
            col.enabled = false;
    }
}
