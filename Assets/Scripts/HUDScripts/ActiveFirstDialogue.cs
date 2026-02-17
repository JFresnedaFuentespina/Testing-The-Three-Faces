using UnityEngine;

public class ActiveFirstDialogue : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject dialoguePanel;
    void Start()
    {
        dialoguePanel.SetActive(true);
    }
}
