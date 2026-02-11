using System.Collections;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SocialPlatforms.Impl;

public class PostScore : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private UserDTO user;
    private ScoreDTO scoreData;

    void Start()
    {
        user = new UserDTO();
        scoreData = new ScoreDTO();

        // Cargar el usuario desde user.json
        string userPath = Application.persistentDataPath + "/user.json";
        if (File.Exists(userPath))
        {
            string json = File.ReadAllText(userPath);
            user = JsonUtility.FromJson<UserDTO>(json);
        }
        else
        {
            Debug.LogError("User data not found at: " + userPath);
        }

        // Cargar la puntuación desde score.json
        string scorePath = Application.persistentDataPath + "/score.json";
        if (File.Exists(scorePath))
        {
            string json = File.ReadAllText(scorePath);
            scoreData = JsonUtility.FromJson<ScoreDTO>(json);
            StartCoroutine(TryPostScore());
        }
        else
        {
            Debug.LogError("Score data not found at: " + scorePath);
        }
    }

    private IEnumerator TryPostScore()
    {
        if (user != null && scoreData != null)
        {
            ScoreBody scoreBody = new ScoreBody
            {
                name = user.name,
                email = user.email,
                score = scoreData.score
            };
            ApiDTO apiData = new ApiDTO();

            UnityWebRequest httpClient = new UnityWebRequest();
            httpClient.method = UnityWebRequest.kHttpVerbPOST;
            httpClient.url = apiData.apiUrl + "/api/classification";
            httpClient.SetRequestHeader("Content-Type", "application/json");
            httpClient.SetRequestHeader("Accept", "application/json");

            string jsonData = JsonUtility.ToJson(scoreBody);
            byte[] dataToSend = Encoding.UTF8.GetBytes(jsonData);

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
