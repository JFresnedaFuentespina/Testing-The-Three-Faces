using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBehavourBadEnding : MonoBehaviour
{
    public float movementSpeed = 2f;
    public float mouseSensitivity = 2f;
    public Transform cameraTransform;
    public float verticalRotation = 0f;
    public Animator animatorEsqueleto;

    private Rigidbody rb;
    private InputSystem_Actions controls;

    private Vector2 moveInput;
    private Vector2 lookInput;

    void Awake()
    {
        controls = new InputSystem_Actions();

        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        controls.Player.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        controls.Player.Look.canceled += ctx => lookInput = Vector2.zero;
    }

    void OnEnable()
    {
        controls.Player.Enable();
    }

    void OnDisable()
    {
        controls.Player.Disable();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        float mouseX = lookInput.x;
        transform.Rotate(Vector3.up * mouseX * mouseSensitivity);
    }

    void FixedUpdate()
    {
        Vector3 movement =
            (transform.forward * moveInput.y + transform.right * moveInput.x).normalized *
            movementSpeed;

        rb.MovePosition(rb.position + movement * Time.fixedDeltaTime);

        float speed = moveInput.magnitude * movementSpeed;
        animatorEsqueleto.SetFloat("Speed", speed);
    }
}