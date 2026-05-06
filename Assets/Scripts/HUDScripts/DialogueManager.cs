using System.Collections;
using System.IO;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Sprite portraitFantasma;
    public Sprite portraitEsqueleto;
    public Sprite portraitBoss;
    public Sprite portraitPlayerGE;
    public Image portraitImage;
    public TextMeshProUGUI message;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI inputText;
    private string displayName;
    public string bossName = "";
    private bool isTyping = false;
    private bool dialogueFinished = false;
    public bool showingEsqueleto = true;
    public bool firstDialogue = false;
    public float letterSpeed = 0.05f;
    public string fullMessage;
    public string inputMessage;
    public string inputGamepadMessage;
    private Coroutine typingCoroutine;
    public GameObject nextDialogue;
    public GameObject hp;
    public GameObject Timer;
    public GameObject money;
    public KeyCode keyToContinue;
    public bool pararTiempo = true;
    public bool isGamepad = false;
    private InputSystem_Actions input;
    public delegate void OnRestoreHealth();
    public static event OnRestoreHealth OnRestoreHealthEvent;
    void Awake()
    {
        input = new InputSystem_Actions();
    }
    void OnEnable()
    {
        input.Enable();
    }
    void OnDisable()
    {
        input.Disable();

        if (pararTiempo)
        {
            Time.timeScale = 1f;
        }

        SetGameplayUI(true);
    }
    void Start()
    {
        string path = Application.persistentDataPath + "/user.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            UserDTO userData = JsonUtility.FromJson<UserDTO>(json);
            displayName = userData.name;
        }
        else
        {
            displayName = "Player";
        }

        if (bossName != "")
        {
            displayName = bossName;
        }

        string pathcontrollers = Application.persistentDataPath + "/controllersData.json";
        if (File.Exists(pathcontrollers))
        {
            string json = File.ReadAllText(pathcontrollers);
            ControllersData controllersData = JsonConvert.DeserializeObject<ControllersData>(json);
            isGamepad = !controllersData.usingMouseKeyboard;
        }
        else
        {
            Debug.LogWarning("Fichero controllersData.json no existe");
        }

        nameText.text = displayName;
        if (!isGamepad)
        {
            inputText.text = inputMessage;
        }
        else
        {
            inputText.text = inputGamepadMessage;
        }
        if (bossName == "")
        {
            RefreshPortraitImage();
        }
        else
        {
            portraitImage.sprite = portraitBoss;
        }

        SetGameplayUI(false);

        gameObject.SetActive(true);
        if (firstDialogue)
        {
            DialogueGlobalManager.Instance.StartDialogue();
        }

        typingCoroutine = StartCoroutine(TypeMessage());
    }

    public void RefreshPortraitImage()
    {
        if (showingEsqueleto)
        {
            portraitImage.sprite = portraitEsqueleto;
        }
        else
        {
            portraitImage.sprite = portraitFantasma;
        }

        if (portraitPlayerGE != null)
        {
            portraitImage.sprite = portraitPlayerGE;
        }
    }

    private void SetGameplayUI(bool value)
    {
        if (hp) hp.SetActive(value);
        if (value)
        {
            if (OnRestoreHealthEvent != null)
            {
                OnRestoreHealthEvent();
            }
        }

        if (Timer)
        {
            Timer.SetActive(value);
        }

        if (money)
        {
            money.SetActive(value);
        }
    }


    IEnumerator TypeMessage()
    {
        if (pararTiempo)
        {
            Time.timeScale = 0f;
        }
        // else
        // {
        //     Time.timeScale = 1f;
        // }
        isTyping = true;
        message.text = "";

        foreach (char letter in fullMessage)
        {
            message.text += letter;
            yield return new WaitForSecondsRealtime(letterSpeed);
        }

        isTyping = false;
        dialogueFinished = true;
    }

    void Update()
    {
        if (!isGamepad)
        {
            SkipDialogueWithKeyboard();
        }
        else
        {
            SkipDialogueWithGamepad();
        }
    }

    private void SkipDialogueWithGamepad()
    {
        if (Input.GetButtonDown("ChangeCharacter"))
        {
            SkipDialogWithE();
        }
        else if (input.Player.Jump.WasPressedThisFrame())
        {
            if (isTyping)
            {
                // Termina de escribir
                StopCoroutine(typingCoroutine);
                message.text = fullMessage;
                isTyping = false;
                dialogueFinished = true;
            }
            else if (dialogueFinished)
            {
                // Cierra el diálogo y activa el siguiente
                if (pararTiempo)
                {
                    Time.timeScale = 1f;
                }

                gameObject.SetActive(false);

                if (nextDialogue != null)
                {
                    nextDialogue.SetActive(true);
                }
                else
                {
                    DialogueGlobalManager.Instance.EndDialogue();
                }
            }
        }
    }
    private void SkipDialogueWithKeyboard()
    {
        if (Input.GetKeyDown(keyToContinue))
        {
            if (keyToContinue == KeyCode.E)
            {
                SkipDialogWithE();
            }
            else
            {
                if (isTyping)
                {
                    // Termina de escribir
                    StopCoroutine(typingCoroutine);
                    message.text = fullMessage;
                    isTyping = false;
                    dialogueFinished = true;
                }
                else if (dialogueFinished)
                {
                    // Cierra el diálogo y activa el siguiente
                    if (pararTiempo)
                    {
                        Time.timeScale = 1f;
                    }

                    gameObject.SetActive(false);

                    if (nextDialogue != null)
                    {
                        nextDialogue.SetActive(true);
                    }
                    else
                    {
                        DialogueGlobalManager.Instance.EndDialogue();
                    }
                }
            }
        }
    }

    private void SkipDialogWithE()
    {
        // Pulso E → termina de escribir y pasa al siguiente de inmediato
        if (isTyping)
        {
            StopCoroutine(typingCoroutine);
            message.text = fullMessage;
            isTyping = false;
            dialogueFinished = true;
        }

        // Pasar al siguiente diálogo inmediatamente
        if (pararTiempo)
        {
            Time.timeScale = 1f;
        }

        gameObject.SetActive(false);

        if (nextDialogue != null)
        {
            nextDialogue.SetActive(true);
            DialogueGlobalManager.Instance.StartDialogue();
        }
        showingEsqueleto = false;
    }

}
