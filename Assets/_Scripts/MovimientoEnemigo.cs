using UnityEngine;
using UnityEngine.AI;

public class MovimientoEnemigo : MonoBehaviour
{
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent != null && agent.isOnNavMesh)
        {
            agent.SetDestination(Vector3.zero);
            Debug.Log($"<color=green>🏃 {name} avanzando al centro del mapa 3D.</color>");
        }
        else
        {
            Debug.LogError("<color=red>❌ ERROR: El bicho no detecta el suelo azul.</color>");
        }
    }
}
