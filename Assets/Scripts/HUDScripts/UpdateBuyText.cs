using System.IO;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;

public class UpdateBuyText : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public TextMeshProUGUI buyMessage;
    public bool isGamepad = false;
    void Start()
    {
        string pathcontrollers = Application.persistentDataPath + "/controllersData.json";

        if (File.Exists(pathcontrollers))
        {
            string json = File.ReadAllText(pathcontrollers);
            ControllersData controllersData = JsonConvert.DeserializeObject<ControllersData>(json);

            isGamepad = !controllersData.usingMouseKeyboard;
        }
        UpdateText();
    }

    public void UpdateText()
    {

        if (isGamepad)
        {
            buyMessage.text = "¡Mantén Botón X para comprar!";
            buyMessage.fontSize = 30;
        }
        else
        {
            buyMessage.text = "¡Mantén F para comprar!";
            buyMessage.fontSize = 36;
        }
    }
}
