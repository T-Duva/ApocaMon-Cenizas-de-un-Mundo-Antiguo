using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class WaveManager : MonoBehaviour
{
    [Header("--- REFERENCIAS ---")]
    public GameObject enemigoPrefab;
    public Transform spawnerTransform;

    [Header("--- AJUSTES MODULARES ---")]
    public int cantidadPorOleada = 10;
    public float cadenciaSpawn = 1.0f;
    public float radioDeBusqueda = 20f;

    void Start()
    {
        if (spawnerTransform == null) Debug.LogError("❌ LOG: SpawnerTransform no asignado en _GameManager.");
        StartCoroutine(SpawnSequence());
    }

    IEnumerator SpawnSequence()
    {
        while (true)
        {
            for (int i = 0; i < cantidadPorOleada; i++)
            {
                SpawnIndividual();
                yield return new WaitForSeconds(cadenciaSpawn);
            }
            yield return new WaitForSeconds(5f);
        }
    }

    void SpawnIndividual()
    {
        if (enemigoPrefab == null) return;

        // 1. Instanciamos el bicho
        GameObject go = Instantiate(enemigoPrefab, spawnerTransform.position, Quaternion.identity);
        NavMeshAgent agent = go.GetComponent<NavMeshAgent>();

        if (agent != null)
        {
            agent.enabled = false; // Lo apagamos un segundo para moverlo

            NavMeshHit hit;
            // Buscamos el punto azul más cercano (radio: radioDeBusqueda)
            if (NavMesh.SamplePosition(spawnerTransform.position, out hit, radioDeBusqueda, NavMesh.AllAreas))
            {
                go.transform.position = hit.position;
                agent.enabled = true; // Lo prendemos ya en el suelo
                bool success = agent.Warp(hit.position); // Forzamos el anclaje
                Debug.Log($"<color=cyan>📦 LOG: Enemigo posicionado. Warp: {success}</color>");
            }
            else
            {
                Debug.LogError("❌ LOG: No se encontró NavMesh cerca del Spawner.");
            }
        }
    }
}
