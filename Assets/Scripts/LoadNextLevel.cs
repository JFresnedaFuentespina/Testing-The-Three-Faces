using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadNextLevel : MonoBehaviour
{

    public GameObject loadingPanel; // El panel que quieres mostrar

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

        StartCoroutine(LoadSceneAsync(nextScene));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        if (loadingPanel != null)
            loadingPanel.SetActive(true);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;

        // Esperar hasta que la escena esté lista para activarse
        while (!asyncLoad.isDone)
        {
            // Activar la escena cuando haya cargado al 90%
            if (asyncLoad.progress >= 0.9f)
            {
                asyncLoad.allowSceneActivation = true;
            }

            yield return null; // Esperar un frame para que el panel se actualice
        }
    }
}
