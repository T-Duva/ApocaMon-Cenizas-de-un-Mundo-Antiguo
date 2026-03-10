using UnityEngine;
using UnityEngine.AI;

// V. 3.2.10 - Implementación 3D funcional con Draco FBX
/// <summary>
/// Movimiento 3D: destino fijo "Meta". High Quality avoidance. Si se atasca ~0.5s (ej. contra torre con NavMeshObstacle), recalcula camino.
/// No atraviesa obstáculos con Carve; busca ruta libre.
/// </summary>
public class MovimientoEnemigo : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform meta;
    private float tiempoVelocidadCero;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent == null || !agent.isOnNavMesh) return;

        agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
        agent.autoRepath = true;

        GameObject metaObj = GameObject.Find("Meta");
        if (metaObj != null)
        {
            meta = metaObj.transform;
            agent.SetDestination(meta.position);
        }
        else
        {
            agent.SetDestination(Vector3.zero);
            Debug.LogWarning("<color=orange>No se encontró 'Meta'. Enemigo destino (0,0,0).</color>");
        }
    }

    void Update()
    {
        if (agent == null || !agent.isOnNavMesh || meta == null) return;
        if (agent.velocity.sqrMagnitude < 0.01f)
        {
            tiempoVelocidadCero += Time.deltaTime;
            if (tiempoVelocidadCero >= 0.5f)
            {
                agent.ResetPath();
                agent.SetDestination(meta.position);
                tiempoVelocidadCero = 0f;
            }
        }
        else
            tiempoVelocidadCero = 0f;
    }
}
