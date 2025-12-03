using UnityEngine;
using UnityEngine.UI;

public class ShowPauseMenu : MonoBehaviour
{
    public GameObject hud;
    private GameObject pauseMenu;
    public Button resumeButton;
    // public Button optionsButton;
    // public Button quitButton;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        resumeButton.onClick.AddListener(() =>
        {
            ShowMenu(false);
        });
        pauseMenu = hud.transform.Find("Pause").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
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
        pauseMenu.SetActive(show);
        if (!show)
            Time.timeScale = 1f;
        else
            Time.timeScale = 0f;
    }
}
