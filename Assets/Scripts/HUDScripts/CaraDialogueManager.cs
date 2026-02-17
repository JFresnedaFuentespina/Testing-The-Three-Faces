using UnityEngine;

public class CaraDialogueManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private EnemyLife enemyLife;
    private Transform dialoguePanel;
    void Start()
    {
        enemyLife = GetComponent<EnemyLife>();
        dialoguePanel = GameObject.Find("Dialogue UI").transform;
        GameObject caraDialogue1 = dialoguePanel.Find("CaraDialogue1").gameObject;
        caraDialogue1.SetActive(true);
    }

    void Update()
    {
        if (enemyLife.currentHp <= 0)
        {
            GameObject caraDeathDialogue = dialoguePanel.Find("CaraDeathDialogue").gameObject;
        }
    }
}
