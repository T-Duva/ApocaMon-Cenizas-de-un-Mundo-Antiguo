using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class IAEnemigo : MonoBehaviour
{
    private NavMeshAgent agent;
    private SpriteRenderer sprite;

    [Header("--- TUS REFERENCIAS (Segundos) ---")]
    public float Velocidad_FINAL = 2.0f;
    public float Aceleracion_FINAL = 0.1f;

    [Header("--- VALORES CALCULADOS ---")]
    public float velocidadCalculada;
    public float aceleracionCalculada;

    IEnumerator Start()
    {
        agent = GetComponent<NavMeshAgent>();
        sprite = GetComponent<SpriteRenderer>();

        // Forzamos límites altos para que el motor no nos frene
        if (agent != null)
        {
            agent.speed = 999f;
            agent.acceleration = 999f;
            agent.updateRotation = false;
            agent.updateUpAxis = false;
        }

        GameObject metaObj = null;
        while (metaObj == null)
        {
            metaObj = GameObject.Find("Meta");
            yield return null; // Espera al siguiente frame
        }

        float distancia = Vector3.Distance(transform.position, metaObj.transform.position);

        // CÁLCULO REAL
        velocidadCalculada = distancia / Mathf.Max(0.1f, Velocidad_FINAL);
        aceleracionCalculada = velocidadCalculada / Mathf.Max(0.01f, Aceleracion_FINAL);

        // APLICACIÓN REAL
        if (agent != null && agent.isOnNavMesh)
        {
            agent.speed = velocidadCalculada;
            agent.acceleration = aceleracionCalculada;
            agent.SetDestination(metaObj.transform.position);

            // Log para que veas en consola si el número es coherente
            Debug.Log($"<color=cyan>⚡ TURBO: Distancia {distancia:F1}m. Velocidad seteada: {velocidadCalculada:F1} unidades/seg</color>");
        }
    }

    void LateUpdate()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, -1f);
    }
}
