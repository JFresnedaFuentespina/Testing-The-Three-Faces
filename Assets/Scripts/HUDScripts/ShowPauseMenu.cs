using System.IO;
using Newtonsoft.Json;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ShowPauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject optionsMenu;
    public GameObject controllersConfig;
    private TextMeshProUGUI tecladoRatonText;
    private TextMeshProUGUI mandoText;
    public Button changeControllersButton;
    public Button resumeButton;
    public Button optionsButton;
    public Button exitOptionsButton;
    public Button applyChangesButton;
    public bool showingOptions = false;
    private bool usingMouseKeyboard = true;

    private GameObject player;
    void Awake()
    {
        GameObject hud = GameObject.Find("HUD");
        if (pauseMenu == null)
            pauseMenu = hud.transform.Find("Pause")?.gameObject;

        if (optionsMenu == null)
            optionsMenu = pauseMenu.transform.Find("OptionsMenu")?.gameObject;

        if (optionsButton == null)
            optionsButton = pauseMenu.transform.Find("OptionsButton").GetComponent<Button>();
        
        if(optionsButton != null)
            optionsButton.onClick.AddListener(() => ShowOptions());
    }

    void Start()
    {
        //Listeners de los botones
        resumeButton.onClick.AddListener(() =>
        {
            ShowMenu(false);
        });

        optionsButton.onClick.AddListener(() =>
        {
            ShowOptions();
        });
        exitOptionsButton.onClick.AddListener(() =>
        {
            ShowOptions(false);
        });
        changeControllersButton.onClick.AddListener(() =>
        {
            ChangeControllers();
        });
        applyChangesButton.onClick.AddListener(() =>
        {
            ApplyChanges();
        });

        tecladoRatonText = controllersConfig.transform.Find("TecladoRatonTexto").GetComponent<TextMeshProUGUI>();
        mandoText = controllersConfig.transform.Find("MandoTexto").GetComponent<TextMeshProUGUI>();

        player = GameObject.FindWithTag("Player");
        usingMouseKeyboard = player.GetComponent<RotateCharacterToMouse>().enabled;
        string path = Application.persistentDataPath + "/controllersData.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            ControllersData controllersData = JsonConvert.DeserializeObject<ControllersData>(json);
            usingMouseKeyboard = controllersData.usingMouseKeyboard;
            player.GetComponent<RotateCharacterToMouse>().enabled = usingMouseKeyboard;
            player.GetComponent<RotateCharacterWithJoystick>().enabled = !usingMouseKeyboard;
        }

        if(optionsButton == null)
        {
            Debug.LogError("No se encontró el botón de opciones en el menú de pausa.");
        }
        tecladoRatonText.gameObject.SetActive(usingMouseKeyboard);
        mandoText.gameObject.SetActive(!usingMouseKeyboard);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pauseMenu.activeSelf)
            {
                ShowMenu(false);
            }
            else
            {
                ShowMenu(true);
            }
        }
    }

    public void ShowMenu(bool show)
    {
        if (showingOptions)
        {
            ShowOptions(show);
        }
        else
        {
            pauseMenu.SetActive(show);
        }
        if (!show)
            Time.timeScale = 1f;
        else
            Time.timeScale = 0f;
    }

    public void ShowOptions(bool show = true)
    {
        optionsMenu.SetActive(show);
        showingOptions = show;
    }

    public void ChangeControllers()
    {
        usingMouseKeyboard = !usingMouseKeyboard;
        tecladoRatonText.gameObject.SetActive(usingMouseKeyboard);
        mandoText.gameObject.SetActive(!usingMouseKeyboard);
    }


    public void ApplyChanges()
    {
        player.GetComponent<RotateCharacterToMouse>().enabled = usingMouseKeyboard;
        player.GetComponent<RotateCharacterWithJoystick>().enabled = !usingMouseKeyboard;
        string json = JsonConvert.SerializeObject(new ControllersData { usingMouseKeyboard = usingMouseKeyboard });
        string path = Application.persistentDataPath + "/controllersData.json";
        File.WriteAllText(path, json);
    }
}
