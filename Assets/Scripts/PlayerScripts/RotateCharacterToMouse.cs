using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class RotateCharacterToMouse : MonoBehaviour
{
    // Start is called before the first frame update
    private Camera cam;
    float speed = 720f;
    void Start()
    {
        cam = Camera.main;
        
        Application.targetFrameRate = 144;
        QualitySettings.vSyncCount = 0;

        string pathcontrollers = Application.persistentDataPath + "/controllersData.json";

        if (File.Exists(pathcontrollers))
        {
            string json = File.ReadAllText(pathcontrollers);
            ControllersData controllersData = JsonConvert.DeserializeObject<ControllersData>(json);
            if (controllersData.usingMouseKeyboard)
            {
                enabled = true;
            }
            else
            {
                enabled = false;
            }
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // Lanzamos un rayo desde la camara hacia la posición del ratón
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Plane suelo = new Plane(Vector3.up, Vector3.zero);

        // Calculamos donde el rayo intersecta al plano del suelo
        if (suelo.Raycast(ray, out float distance))
        {
            Vector3 point = ray.GetPoint(distance);
            //Dirección desde el personaje hasta el punto del ratón
            Vector3 direction = point - transform.position;
            //Ignorar eje Y
            direction.y = 0;
            //Calcular la rotación hacia esa dirección
            if (direction.sqrMagnitude > 0.001f) //Evitar dividir entre 0
            {
                Quaternion rotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation,
                    rotation,
                    speed * Time.deltaTime
                );
            }
        }
    }
}
