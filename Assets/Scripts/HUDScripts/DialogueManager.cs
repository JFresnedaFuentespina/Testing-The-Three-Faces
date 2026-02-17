using System.Collections;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Sprite portrait;
    public Image portraitImage;
    public TextMeshProUGUI message;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI inputText;
    private string displayName;
    public string bossName = "";
    private bool isTyping = false;
    private bool dialogueFinished = false;

    public float letterSpeed = 0.05f;
    public string fullMessage = "Oh sh*t, here we go again...";
    public string inputMessage;
    private Coroutine typingCoroutine;
    public GameObject nextDialogue;
    public KeyCode keyToContinue;
    public bool pararTiempo = true;

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

        nameText.text = displayName;
        portraitImage.sprite = portrait;
        inputText.text = inputMessage;
        gameObject.SetActive(true);
        typingCoroutine = StartCoroutine(TypeMessage());
    }

    IEnumerator TypeMessage()
    {
        if (pararTiempo)
        {
            Time.timeScale = 0f;
        }
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
                    gameObject.SetActive(false);
                    if (nextDialogue != null)
                    {
                        nextDialogue.SetActive(true);
                    }
                    else if (pararTiempo)
                    {
                        Time.timeScale = 1f;
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
        gameObject.SetActive(false);
        if (nextDialogue != null)
        {
            nextDialogue.SetActive(true);
        }
        else if (pararTiempo)
        {
            Time.timeScale = 1f;
        }
    }

}
