using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PostRate : MonoBehaviour
{
    private SendRateDTO rate;
    private UserDTO user;
    private ApiDTO api;
    public List<StarPanelGenerator> stars;
    public Button sendButton;
    void Start()
    {
        string userPath = Application.persistentDataPath + "/user.json";
        if (File.Exists(userPath))
        {
            string json = File.ReadAllText(userPath);
            user = JsonUtility.FromJson<UserDTO>(json);
            if (user.has_rated)
            {
                GoToMainMenu();
            }
        }
        sendButton.onClick.AddListener(PostRateToAPI);
        rate = new();
        api = new();
    }

    public void PostRateToAPI()
    {
        GetStars();
        StartCoroutine(TryPostRate());
    }

    public void GetStars()
    {
        foreach (StarPanelGenerator starPanel in stars)
        {
            switch (starPanel.category)
            {
                case "general": rate.general = starPanel.rate; break;
                case "jugabilitat": rate.jugabilitat = starPanel.rate; break;
                case "dificultat": rate.dificultat = starPanel.rate; break;
                case "grafics": rate.grafics = starPanel.rate; break;
                case "concordancia": rate.concordancia = starPanel.rate; break;
            }
        }
    }

    private IEnumerator TryPostRate()
    {
        if (rate != null)
        {
            rate.api_token = api.apiToken;
            rate.name = user.name;
            rate.email = user.email;
            UnityWebRequest httpClient = new UnityWebRequest();
            httpClient.method = UnityWebRequest.kHttpVerbPOST;
            httpClient.url = api.apiUrl + "/api/rateGame";
            httpClient.SetRequestHeader("Content-Type", "application/json");
            httpClient.SetRequestHeader("Accept", "application/json");

            string jsonData = JsonUtility.ToJson(rate);
            byte[] dataToSend = Encoding.UTF8.GetBytes(jsonData);

            httpClient.uploadHandler = new UploadHandlerRaw(dataToSend);
            httpClient.downloadHandler = new DownloadHandlerBuffer();

            yield return httpClient.SendWebRequest();
            if (httpClient.result == UnityWebRequest.Result.ConnectionError || httpClient.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log("Error: " + httpClient.error);
            }
            else
            {
                Debug.Log(httpClient.downloadHandler.text);
            }
            httpClient.Dispose();
        }
        yield return null;
    }

    private void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
