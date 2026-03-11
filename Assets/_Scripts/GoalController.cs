using UnityEngine;

public class GoalController : MonoBehaviour
{
    [Header("Configuración de la Meta")]
    public string enemyTag = "Enemy"; // Asegurate que tus dragones tengan este tag

    private void OnTriggerEnter(Collider other)
    {
        // Verificamos si lo que entró al sensor es un enemigo
        if (other.CompareTag(enemyTag))
        {
            Debug.Log("<color=red>¡Un ApocaMon escapó!</color> Nombre: " + other.gameObject.name);

            // Destruimos al dragón para que no siga ocupando memoria
            Destroy(other.gameObject);

            // TODO: Aquí restaremos vida al jugador más adelante
        }
    }
}