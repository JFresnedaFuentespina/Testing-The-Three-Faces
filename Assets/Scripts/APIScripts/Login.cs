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
    private ApiDTO loginData;
    public TMP_InputField nameInput;
    public TMP_InputField mailInput;
    public Button loginButton;
    void Start()
    {
        loginButton.onClick.AddListener(login);
    }

    public void login()
    {
        Debug.Log("Login");
        user = new UserDTO();
        user.name = nameInput.text;
        user.email = mailInput.text;
        StartCoroutine(TryLogin());
    }

    private IEnumerator TryLogin()
    {
        if (user != null)
        {
            loginData = new ApiDTO();
            UnityWebRequest httpClient = new UnityWebRequest();
            httpClient.method = UnityWebRequest.kHttpVerbPOST;
            httpClient.url = loginData.apiUrl + "/auth/login";
            httpClient.SetRequestHeader("Content-Type", "application/json");
            httpClient.SetRequestHeader("Accept", "application/json");

            LoginBody loginBody = new LoginBody
            {
                name = user.name,
                email = user.email,
                apiToken = loginData.apiToken
            };

            string jsonData = JsonConvert.SerializeObject(loginBody);
            byte[] dataToSend = Encoding.UTF8.GetBytes(jsonData);

            httpClient.uploadHandler = new UploadHandlerRaw(dataToSend);
            httpClient.downloadHandler = new DownloadHandlerBuffer();

            yield return httpClient.SendWebRequest();
            if (httpClient.result == UnityWebRequest.Result.ConnectionError || httpClient.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log("Error: " + httpClient.error);
            }

            string jsonResponse = httpClient.downloadHandler.text;
            user.hasRated = JsonConvert.DeserializeObject<UserDTO>(jsonResponse).hasRated;
            Debug.Log("Login successful. User hasRated: " + user.hasRated);
            SaveUserData();
            httpClient.Dispose();
        }
        yield return null;
    }

    public void SaveUserData()
    {
        string json = JsonConvert.SerializeObject(user);
        string path = Application.persistentDataPath + "/user.json";
        System.IO.File.WriteAllText(path, json);
    }
}
