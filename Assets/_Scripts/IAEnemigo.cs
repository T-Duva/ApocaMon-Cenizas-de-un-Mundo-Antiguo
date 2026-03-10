using UnityEngine;
using UnityEngine.AI;
using System.Collections;

/// <summary>
/// Destino: "Meta". 3D, High Quality avoidance. Si velocidad ~0 por 0.5s, ResetPath y recalcula hacia Meta.
/// </summary>
public class IAEnemigo : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform meta;
    private float tiempoVelocidadCero;

    [Header("Velocidad 3D")]
    public float Velocidad_FINAL = 2.0f;
    public float Aceleracion_FINAL = 0.1f;

    IEnumerator Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent == null) yield break;

        GameObject metaObj = null;
        while (metaObj == null)
        {
            metaObj = GameObject.Find("Meta");
            yield return null;
        }
        meta = metaObj.transform;

        if (!agent.isOnNavMesh) yield break;

        agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
        agent.autoRepath = true;
        float distancia = Vector3.Distance(transform.position, meta.position);
        agent.speed = distancia / Mathf.Max(0.1f, Velocidad_FINAL);
        agent.acceleration = agent.speed / Mathf.Max(0.01f, Aceleracion_FINAL);
        agent.SetDestination(meta.position);
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
