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
    private bool isTyping = false;
    private bool dialogueFinished = false;

    public float letterSpeed = 0.05f;
    public string fullMessage = "Oh sh*t, here we go again...";
    public string inputMessage;
    private Coroutine typingCoroutine;
    public GameObject nextDialogue;
    public KeyCode keyToContinue;

    void Start()
    {
        string path = Application.persistentDataPath + "/user.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            UserDTO userData = JsonUtility.FromJson<UserDTO>(json);
            nameText.text = userData.name;
        }
        else
        {
            nameText.text = "Player";
        }

        portraitImage.sprite = portrait;
        inputText.text = inputMessage;
        gameObject.SetActive(true);
        typingCoroutine = StartCoroutine(TypeMessage());
    }

    IEnumerator TypeMessage()
    {
        isTyping = true;
        message.text = "";

        foreach (char letter in fullMessage)
        {
            message.text += letter;
            yield return new WaitForSeconds(letterSpeed);
        }

        isTyping = false;
        dialogueFinished = true;
    }

    void Update()
    {
        if (Input.GetKeyDown(keyToContinue))
        {
            if (isTyping)
            {
                StopCoroutine(typingCoroutine);
                message.text = fullMessage;
                isTyping = false;
                dialogueFinished = true;
            }
            else if (dialogueFinished)
            {
                gameObject.SetActive(false);
                if (nextDialogue != null)
                {
                    nextDialogue.SetActive(true);
                }
            }
        }
    }
}
