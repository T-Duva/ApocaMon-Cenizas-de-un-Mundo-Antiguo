using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using System.Collections;

// V. 3.2.10 - Implementación 3D funcional con Draco FBX
public class WaveManager : MonoBehaviour
{
    [Header("--- REFERENCIAS ---")]
    public GameObject enemigoPrefab;
    public Transform spawnerTransform;

    [Header("--- AJUSTES MODULARES ---")]
    public int cantidadPorOleada = 10;
    public float cadenciaSpawn = 1.0f;
    public float radioDeBusqueda = 20f;

    private CicloDiaNoche cicloDiaNoche;

    void Start()
    {
        RenderSettings.ambientIntensity = 1.5f;
        RenderSettings.ambientMode = AmbientMode.Skybox;
        cicloDiaNoche = Object.FindFirstObjectByType<CicloDiaNoche>(FindObjectsInactive.Include);
        if (spawnerTransform == null) Debug.LogError("❌ LOG: SpawnerTransform no asignado en _GameManager.");
        StartCoroutine(SpawnSequence());
    }

    IEnumerator SpawnSequence()
    {
        while (true)
        {
            if (cicloDiaNoche != null) cicloDiaNoche.AlEmpezarOleada();

            for (int i = 0; i < cantidadPorOleada; i++)
            {
                SpawnIndividual();
                yield return new WaitForSeconds(cadenciaSpawn);
            }

            yield return new WaitUntil(TodosLosEnemigosMurieron);
            if (cicloDiaNoche != null) cicloDiaNoche.AlTerminarOleada();
            yield return new WaitForSeconds(5f);
        }
    }

    bool TodosLosEnemigosMurieron()
    {
        var motores = Object.FindObjectsByType<ApocaMotor>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        foreach (var m in motores)
            if (m != null && m.Estado == ApocaMotor.EstadoApoca.Activo) return false;
        return true;
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
                agent.enabled = true;
                agent.Warp(hit.position);
            }
            else
            {
                Debug.LogError("❌ LOG: No se encontró NavMesh cerca del Spawner.");
            }
        }
    }
}
