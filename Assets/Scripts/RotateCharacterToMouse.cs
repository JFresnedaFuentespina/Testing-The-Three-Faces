using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCharacterToMouse : MonoBehaviour
{
    public GameObject esqueleto;
    public GameObject ghost;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        // Lanzamos un rayo desde la camara hacia la posición del ratón
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
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
                GameObject model = esqueleto.activeSelf ? esqueleto : ghost;
                Quaternion rotation = Quaternion.LookRotation(direction);
                model.transform.rotation = Quaternion.Slerp(
                    model.transform.rotation,
                    Quaternion.LookRotation(direction),
                    Time.deltaTime * 10f
                );
            }
        }
    }
}
