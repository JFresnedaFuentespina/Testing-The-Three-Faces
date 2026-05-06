using System.IO;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;

public class ShowMinimap : MonoBehaviour
{
    public GameObject minimap;
    public GameObject message;
    public bool showMinimap = false;

    private InputSystem_Actions inputActions;
    public bool isGamepad = false;
    private string gamePadText = "Pulsa <Select> para ver el mapa";
    private string keyboardText = "Pulsa <Tab> para ver el mapa";

    void Awake()
    {
        inputActions = new InputSystem_Actions();
    }

    void OnEnable()
    {
        inputActions.Enable();
    }

    void OnDisable()
    {
        inputActions.Disable();
    }

    void Start()
    {
        string pathcontrollers = Application.persistentDataPath + "/controllersData.json";

        if (File.Exists(pathcontrollers))
        {
            string json = File.ReadAllText(pathcontrollers);
            ControllersData controllersData =
                JsonConvert.DeserializeObject<ControllersData>(json);

            isGamepad = !controllersData.usingMouseKeyboard;
        }
        else
        {
            Debug.LogWarning("Fichero controllersData.json no existe");
        }

        if (isGamepad)
        {
            message.GetComponentInChildren<TextMeshProUGUI>().text = gamePadText;
        }
        else
        {
            message.GetComponentInChildren<TextMeshProUGUI>().text = keyboardText;
        }
    }

    void Update()
    {
        if (!isGamepad)
        {
            KeyboardMinimap();
        }
        else
        {
            GamepadMinimap();
        }
    }

    private void KeyboardMinimap()
    {
        message.GetComponentInChildren<TextMeshProUGUI>().text = keyboardText;
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ShowMap(true);
        }
        else if (Input.GetKeyUp(KeyCode.Tab))
        {
            ShowMap(false);
        }
    }

    private void GamepadMinimap()
    {
        message.GetComponentInChildren<TextMeshProUGUI>().text = gamePadText;
        if (inputActions.Player.Select.WasPressedThisFrame())
        {
            ShowMap(true);
        }
        else if (inputActions.Player.Select.WasReleasedThisFrame())
        {
            ShowMap(false);
        }
    }

    private void ShowMap(bool value)
    {
        showMinimap = value;
        minimap.SetActive(value);
        message.SetActive(!value);
    }
}