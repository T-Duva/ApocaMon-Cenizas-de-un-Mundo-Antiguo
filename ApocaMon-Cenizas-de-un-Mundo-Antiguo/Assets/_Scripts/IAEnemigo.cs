using UnityEngine;
using UnityEngine.AI;

public class IAEnemigo : MonoBehaviour
{
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // Configuración crítica para 2D
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        // Forzamos Z inicial a 0
        transform.position = new Vector3(transform.position.x, transform.position.y, 0f);

        // Aseguramos visibilidad
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.sortingOrder = 1000;
        }
    }

    void Update()
    {
        // LEY ABSOLUTA Z=0: Si el agente intenta subir o bajar, lo bajamos de un cachetazo
        if (transform.position.z != 0f)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, 0f);
        }

        // Log para confirmar que ahora sí estamos en 0
        Debug.Log($"Corrigiendo Z. Posición actual: {transform.position}");
    }
}
