using UnityEngine;

public class ZonaToxicaNocturna : MonoBehaviour
{
    [Header("Configuración de Peligro Nocturno")]
    [Tooltip("Probabilidad de que aparezca una oleada mientras es de noche (1% por defecto)")]
    [Range(0f, 100f)]
    public float probabilidadOleadaNocturna = 1f;

    // Función para que otros scripts consulten la probabilidad
    public bool PuedeAparecerOleada()
    {
        float random = Random.Range(0f, 100f);
        return random <= probabilidadOleadaNocturna;
    }
}