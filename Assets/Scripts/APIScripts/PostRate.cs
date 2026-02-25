using System.Collections;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class PostRate : MonoBehaviour
{
    private SendRateDTO rate;
    private ApiDTO api;
    void Start()
    {
        rate = new();
        api = new();
    }

    public void PostRateToAPI()
    {
        // Cargar la valoración desde rate.json
        string ratePath = Application.persistentDataPath + "/rate.json";
        if (File.Exists(ratePath))
        {
            string json = File.ReadAllText(ratePath);
            rate = JsonUtility.FromJson<SendRateDTO>(json);
        }
        else
        {
            Debug.LogWarning("Score data not found at: " + ratePath);
        }
        StartCoroutine(TryPostRate());
    }

    private IEnumerator TryPostRate()
    {
        if (rate != null)
        {
            rate.api_token = api.apiToken;
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
}
