using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadLevel1 : MonoBehaviour
{
    // Esta función se llamará al hacer clic en el botón
    public void CargarNivel1()
    {
        Debug.Log("Cargando Nivel 1...");
        SceneManager.LoadScene("Level1Scene");
    }
}
