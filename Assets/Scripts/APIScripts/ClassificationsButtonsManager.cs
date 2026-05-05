using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ClassificationsButtonsManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Button exitButton;
    public Button restartButton;
    private UserDTO user;
    void Start()
    {
        Cursor.visible = true;
        exitButton.onClick.AddListener(ExitGame);
        restartButton.onClick.AddListener(RestartGame);

        string userPath = Application.persistentDataPath + "/user.json";
        if (File.Exists(userPath))
        {
            string json = File.ReadAllText(userPath);
            user = JsonUtility.FromJson<UserDTO>(json);
            if (!user.has_rated)
            {
                TMP_Text buttonText = restartButton.GetComponentInChildren<TMP_Text>();
                if (buttonText != null)
                {
                    buttonText.text = "Valorar";
                }

                restartButton.onClick.RemoveAllListeners();
                restartButton.onClick.AddListener(RateGame);
            }
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }
    public void RestartGame()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void RateGame()
    {
        SceneManager.LoadScene("RateScene");
    }
}
