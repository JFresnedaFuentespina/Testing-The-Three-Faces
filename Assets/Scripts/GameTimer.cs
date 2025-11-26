using TMPro;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

public class GameTimer : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    private float elapsedTime = 0f;
    private bool isRunning = false;

    private string timerPath;

    void Awake()
    {
        // Definir la ruta del archivo  
        timerPath = Application.persistentDataPath + "/timer.json";
        // Intentar cargar tiempo guardado
        if (File.Exists(timerPath))
        {
            try
            {
                string json = File.ReadAllText(timerPath);
                TimerData data = JsonConvert.DeserializeObject<TimerData>(json);
                if (data != null)
                {
                    elapsedTime = data.time;
                    Debug.Log("Tiempo cargado desde archivo: " + elapsedTime);
                }
            }
            catch
            {
                Debug.LogWarning("Error al leer timer.json, se iniciará desde 0.");
                elapsedTime = 0f;
            }
        }
    }

    void Start()
    {
        isRunning = true;
        UpdateTimerText();
    }

    void Update()
    {
        if (!isRunning) return;

        elapsedTime += Time.deltaTime;
        UpdateTimerText();
    }

    private void UpdateTimerText()
    {
        int min = Mathf.FloorToInt(elapsedTime / 60f);
        int sec = Mathf.FloorToInt(elapsedTime % 60f);
        timerText.text = string.Format("Timer: {0:00}:{1:00}", min, sec);
        SaveTimer();
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
        UpdateTimerText();
    }

    public float GetElapsedTime()
    {
        return elapsedTime;
    }

    public void SaveTimer()
    {
        TimerData data = new TimerData { time = elapsedTime };
        string json = JsonConvert.SerializeObject(data);
        File.WriteAllText(timerPath, json);
    }
}

[System.Serializable]
public class TimerData
{
    public float time;
}
