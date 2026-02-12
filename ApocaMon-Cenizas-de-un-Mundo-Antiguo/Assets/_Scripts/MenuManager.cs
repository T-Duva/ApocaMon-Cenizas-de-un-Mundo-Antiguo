using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Control del menú principal. Asignar a un GameObject de la escena y enlazar sus métodos a los botones del Canvas.
/// </summary>
public class MenuManager : MonoBehaviour
{
    [Header("Escenas")]
    [Tooltip("Nombre de la escena a cargar al pulsar Jugar (ej: 02_MapaSeleccion o escena de tutorial)")]
    [SerializeField] private string nombreEscenaJugar = "02_MapaSeleccion";

    /// <summary>
    /// Carga la escena de mapa/tutorial (por defecto 02_MapaSeleccion).
    /// </summary>
    public void Jugar()
    {
        SceneManager.LoadScene(nombreEscenaJugar);
    }

    /// <summary>
    /// Abre el menú de opciones. Por ahora solo registra en consola.
    /// </summary>
    public void AbrirOpciones()
    {
        Debug.Log("Abriendo Opciones...");
    }

    /// <summary>
    /// Cierra la aplicación. No tiene efecto en el Editor.
    /// </summary>
    public void Salir()
    {
        Debug.Log("Cerrando aplicación...");
        Application.Quit();
    }
}
