using UnityEngine;

public class DialogueGlobalManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public static DialogueGlobalManager Instance;
    public bool isDialogueActive { get; private set; }
    void Start()
    {
        Instance = this;
    }

    public void StartDialogue()
    {
        isDialogueActive = true;
    }

    public void EndDialogue()
    {
        isDialogueActive = false;
    }
}
