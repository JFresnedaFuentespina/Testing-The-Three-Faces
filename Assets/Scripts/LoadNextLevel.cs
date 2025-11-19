using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadNextLevel : MonoBehaviour
{

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;
        NextLevel();
    }
    public void NextLevel()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        string nextScene = "";
        Debug.Log($"Escena actual: {currentScene}");
        switch (currentScene)
        {
            case "Level1Scene":
                nextScene = "Level2Scene";
                break;
            case "Level2Scene":
                nextScene = "Level3Scene";
                break;
            default:
                Debug.LogWarning("No hay siguiente nivel definido para " + currentScene);
                return;
        }
        SceneManager.LoadScene(nextScene);
    }
}
