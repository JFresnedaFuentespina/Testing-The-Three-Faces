using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PostLogin : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private UserDTO user;
    public TMP_InputField nameInput;
    public TMP_InputField mailInput;
    public Button loginButton;
    void Start()
    {
        user = new UserDTO();
        loginButton.onClick.AddListener(Login);
    }

    public void Login()
    {
        Debug.Log("Login");
        user.name = nameInput.text;
        user.email = mailInput.text;
        StartCoroutine(TryLogin());
    }

    private IEnumerator TryLogin()
    {
        if (user != null)
        {
            ApiDTO loginData = new ApiDTO();
            UnityWebRequest httpClient = new UnityWebRequest();
            httpClient.method = UnityWebRequest.kHttpVerbPOST;
            httpClient.url = loginData.apiUrl + "/api/verify";
            httpClient.SetRequestHeader("Content-Type", "application/json");
            httpClient.SetRequestHeader("Accept", "application/json");

            LoginBody loginBody = new LoginBody
            {
                name = user.name,
                email = user.email,
                api_token = loginData.apiToken
            };

            Debug.Log("Attempting login with Name: " + loginBody.name + " and Email: " + loginBody.email);

            string jsonData = JsonConvert.SerializeObject(loginBody);
            byte[] dataToSend = Encoding.UTF8.GetBytes(jsonData);

            httpClient.uploadHandler = new UploadHandlerRaw(dataToSend);
            httpClient.downloadHandler = new DownloadHandlerBuffer();

            yield return httpClient.SendWebRequest();
            if (httpClient.result == UnityWebRequest.Result.ConnectionError || httpClient.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log("Error: " + httpClient.error);
                StartCoroutine(LoadLevel1Async());
                yield return null;
            }
            string jsonResponse = httpClient.downloadHandler.text;
            VerifyResponseDTO response = JsonConvert.DeserializeObject<VerifyResponseDTO>(jsonResponse);
            user.has_rated = response.rated;
            List<RateDTO> rateList = response.criterion;
            Debug.Log("LOGIN -> User: " + user.name + " " + user.email + " has rated? " + user.has_rated);
            SaveCriterions(rateList);
            SaveUserData();
            httpClient.Dispose();
            StartCoroutine(LoadLevel1Async());
        }
        yield return null;
    }

    private IEnumerator LoadLevel1Async()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync("Level1Scene");
        op.allowSceneActivation = false;

        yield return null;

        op.allowSceneActivation = true;
    }

    public void SaveUserData()
    {
        string json = JsonConvert.SerializeObject(user);
        string path = Application.persistentDataPath + "/user.json";
        File.WriteAllText(path, json);
    }

    public void Logout()
    {
        user = null;
        string path = Application.persistentDataPath + "/user.json";
        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log("User data deleted at: " + path);
        }
        else
        {
            Debug.LogWarning("No user data found to delete at: " + path);
        }
    }

    public void SaveCriterions(List<RateDTO> criterions)
    {
        string path = Application.persistentDataPath + "/citerions.json";
        if (File.Exists(path))
        {
            File.Delete(path);
        }
        string json = JsonConvert.SerializeObject(criterions, Formatting.Indented);
        File.WriteAllText(path, json);
    }
}
