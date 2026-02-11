using System.Collections;
using System.Text;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Login : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private UserDTO user;
    private LoginData loginData;
    public TextMeshProUGUI mailInput;
    public Button loginButton;
    void Start()
    {
        loginButton.onClick.AddListener(login);
    }

    public void login()
    {
        Debug.Log("Login");
        user.email = mailInput.text;
        StartCoroutine(TryLogin());
    }

    private IEnumerator TryLogin()
    {
        if(user != null)
        {
            UnityWebRequest httpClient = new UnityWebRequest();
            httpClient.method = UnityWebRequest.kHttpVerbPOST;
            httpClient.url = loginData.apiUrl + "/auth/login";
            httpClient.SetRequestHeader("Content-Type", "application/json");
            httpClient.SetRequestHeader("Authorization", "Bearer " + loginData.apiToken);
            httpClient.SetRequestHeader("Accept", "application/json");

            string jsonData = JsonConvert.SerializeObject(user);
            byte[] dataToSend = Encoding.UTF8.GetBytes(jsonData);

            httpClient.uploadHandler = new UploadHandlerRaw(dataToSend);
            httpClient.downloadHandler = new DownloadHandlerBuffer();

            yield return httpClient.SendWebRequest();
            if(httpClient.result == UnityWebRequest.Result.ConnectionError || httpClient.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log("Error: " + httpClient.error);
            }
            
            string jsonResponse = httpClient.downloadHandler.text;
            user.hasRated = JsonConvert.DeserializeObject<UserDTO>(jsonResponse).hasRated;
            Debug.Log("Login successful. User hasRated: " + user.hasRated);
            httpClient.Dispose();
        }
        yield return null;
    }
}
