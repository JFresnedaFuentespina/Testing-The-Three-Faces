using UnityEngine;
using UnityEngine.InputSystem;

public class RotateCharacterWithJoystick : MonoBehaviour
{
    public float velocidadRotacion = 1440f;

    private Rigidbody rb;
    // 1. Asegúrate de que este nombre sea el mismo que el de tu archivo de acciones
    private InputSystem_Actions input;
    private Vector2 lookDirection;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null) Debug.LogError("RotateCharacter: No tiene Rigidbody");

        input = new InputSystem_Actions();
    }

    void OnEnable()
    {
        input.Enable();
        // 3. En tu imagen la acción se llama "Look", NO "LookDirection"
        input.Player.Look.performed += OnLook;
        input.Player.Look.canceled += OnLookCanceled;
    }

    void OnDisable()
    {
        input.Player.Look.performed -= OnLook;
        input.Player.Look.canceled -= OnLookCanceled;
        input.Disable();
    }

    private void OnLook(InputAction.CallbackContext ctx)
    {
        lookDirection = ctx.ReadValue<Vector2>();
    }

    private void OnLookCanceled(InputAction.CallbackContext ctx)
    {
        lookDirection = Vector2.zero;
    }

    void FixedUpdate()
    {
        if (lookDirection.sqrMagnitude < 0.1f)
            return;

        Transform cam = Camera.main.transform;

        Vector3 forward = cam.forward;
        Vector3 right = cam.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        Vector3 moveDir =
            forward * lookDirection.y +
            right * lookDirection.x;

        Quaternion targetRotation =
            Quaternion.LookRotation(moveDir, Vector3.up);

        rb.MoveRotation(
            Quaternion.RotateTowards(
                rb.rotation,
                targetRotation,
                velocidadRotacion * Time.fixedDeltaTime
            )
        );
    }
}
