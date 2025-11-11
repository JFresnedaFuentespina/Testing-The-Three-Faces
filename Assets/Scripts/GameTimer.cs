using TMPro;
using UnityEngine;

public class GameTimer : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public TextMeshProUGUI timerText;
    private float elapsedTime = 0f;
    private bool isRunning = false;

    void Start()
    {
        isRunning = true;
        timerText.text = "Timer: 00:00";
    }

    // Update is called once per frame
    void Update()
    {
        if (!isRunning)
        {
            return;
        }
        elapsedTime += Time.deltaTime;
        int min = Mathf.FloorToInt(elapsedTime / 60f);
        int sec = Mathf.FloorToInt(elapsedTime % 60f);

        timerText.text = string.Format("Timer: {0:00}:{1:00}", min, sec);
    }

    public void PauseTimer()
    {
        isRunning = false;
    }

    public void ResumeTimer()
    {
        isRunning = true;
    }

    public void ResetTimer()
    {
        elapsedTime = 0f;
        timerText.text = "00:00";
    }

    public float GetElapsedTime()
    {
        return elapsedTime;
    }
}
