using System.Collections;
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
    public GameObject exitConfirmationMenu;
    private TextMeshProUGUI tecladoRatonText;
    private TextMeshProUGUI mandoText;
    public Button changeControllersButton;
    public Button resumeButton;
    public Button optionsButton;
    public Button exitOptionsButton;
    public Button applyChangesButton;
    public Button quitButton;
    public Button confirmQuitButton;
    public Button continueButton;
    public bool showingOptions = false;
    private bool usingMouseKeyboard = true;
    private GameObject player;
    private bool initialized = false;
    public static bool blockPlayerInput = false;
    void Awake()
    {
        GameObject hud = GameObject.Find("HUD");

        if (pauseMenu == null)
            pauseMenu = hud.transform.Find("Pause")?.gameObject;

        if (optionsMenu == null)
            optionsMenu = pauseMenu.transform.Find("OptionsMenu")?.gameObject;

        if (optionsButton == null)
            optionsButton = pauseMenu.transform.Find("OptionsButton")?.GetComponent<Button>();

        // Inicializamos los textos de control si existen
        if (controllersConfig != null)
        {
            tecladoRatonText = controllersConfig.transform.Find("TecladoRatonTexto")?.GetComponent<TextMeshProUGUI>();
            mandoText = controllersConfig.transform.Find("MandoTexto")?.GetComponent<TextMeshProUGUI>();
        }

        if (optionsButton != null)
            optionsButton.onClick.AddListener(() => ShowOptions());
    }

    void Start()
    {
        AddButtonsListeners();
        StartCoroutine(WaitForPlayer());
    }

    public void AddButtonsListeners()
    {//Listeners de los botones
        resumeButton.onClick.AddListener(() =>
        {
            StartCoroutine(ResumeNextFrame());
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

        quitButton.onClick.AddListener(() =>
        {
            OpenConfirmExitMenu();
        });
        confirmQuitButton.onClick.AddListener(() =>
        {
            BackToMainMenu();
        });
        continueButton.onClick.AddListener(() =>
        {
            CloseConfirmExitMenu();
        });
    }
    IEnumerator ResumeNextFrame()
    {
        yield return null;
        ShowMenu(false);
    }

    private IEnumerator WaitForPlayer()
    {
        // Esperamos hasta que el jugador sea instanciado
        while (player == null)
        {
            player = GameObject.FindWithTag("Player");
            yield return null; // espera un frame
        }

        // Ahora sí podemos inicializar los controles
        usingMouseKeyboard = player.GetComponent<RotateCharacterToMouse>().enabled;

        string path = Application.persistentDataPath + "/controllersData.json";
        if (File.Exists(path))
        {
            string json = System.IO.File.ReadAllText(path);
            ControllersData controllersData = Newtonsoft.Json.JsonConvert.DeserializeObject<ControllersData>(json);
            usingMouseKeyboard = controllersData.usingMouseKeyboard;
            player.GetComponent<RotateCharacterToMouse>().enabled = usingMouseKeyboard;
            player.GetComponent<RotateCharacterWithJoystick>().enabled = !usingMouseKeyboard;
        }

        tecladoRatonText.gameObject.SetActive(usingMouseKeyboard);
        mandoText.gameObject.SetActive(!usingMouseKeyboard);

        initialized = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.JoystickButton7))
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
        {
            Time.timeScale = 1f;
            StartCoroutine(BlockInputMomentarily());
        }
        else
        {
            Time.timeScale = 0f;
        }
    }
    IEnumerator BlockInputMomentarily()
    {
        blockPlayerInput = true;
        yield return new WaitForSeconds(0.15f);
        blockPlayerInput = false;
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

    public void BackToMainMenu()
    {
        Time.timeScale = 1f;
        string path = Application.persistentDataPath + "/controllersData.json";
        string playerDataPath = Application.persistentDataPath + "/player.json";
        //borrar los archivos de guardado al volver al menú principal
        if (File.Exists(path))
            File.Delete(path);
        if (File.Exists(playerDataPath))
            File.Delete(playerDataPath);
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("MainMenu");
    }

    public void OpenConfirmExitMenu()
    {
        exitConfirmationMenu.SetActive(true);
        pauseMenu.SetActive(false);
    }

    public void CloseConfirmExitMenu()
    {
        exitConfirmationMenu.SetActive(false);
        pauseMenu.SetActive(true);
    }
}
