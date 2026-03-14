using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using System.Collections;

public class WaveManager : MonoBehaviour
{
    [Header("--- REFERENCIAS ---")]
    public GameObject enemigoPrefab;
    public Transform spawnerTransform;
    public Transform metaTransform;

    [Header("--- AJUSTES OLEADA ---")]
    public int cantidadPorOleada = 10;
    public float cadenciaSpawn = 1.0f;
    public float radioDeBusqueda = 20f;

    private CicloDiaNoche cicloDiaNoche;
    private ZonaToxicaNocturna zonaToxica;

    void Start()
    {
        cicloDiaNoche = Object.FindFirstObjectByType<CicloDiaNoche>(FindObjectsInactive.Include);
        zonaToxica = Object.FindFirstObjectByType<ZonaToxicaNocturna>(FindObjectsInactive.Include);

        StartCoroutine(SpawnSequence());
    }

    IEnumerator SpawnSequence()
    {
        while (true)
        {
            // Verificación de noche (1% de probabilidad según Zona Tóxica)
            if (cicloDiaNoche != null && cicloDiaNoche.esDeNoche)
            {
                if (zonaToxica != null && !zonaToxica.PuedeAparecerOleada())
                {
                    // Pequeñísima espera de 0.5s solo para no saturar el procesador en el bucle
                    yield return new WaitForSeconds(0.5f);
                    continue;
                }
            }

            // --- INICIO DE OLEADA ---
            if (cicloDiaNoche != null) cicloDiaNoche.AlEmpezarOleada();

            for (int i = 0; i < cantidadPorOleada; i++)
            {
                SpawnIndividual();
                yield return new WaitForSeconds(cadenciaSpawn);
            }

            // Esperar a que todos los motores pasen a KO
            yield return new WaitUntil(TodosLosEnemigosMurieron);

            if (cicloDiaNoche != null) cicloDiaNoche.AlTerminarOleada();

            // Sin pausas. El bucle vuelve arriba al instante.
        }
    }

    void SpawnIndividual()
    {
        if (enemigoPrefab == null || spawnerTransform == null) return;

        GameObject go = Instantiate(enemigoPrefab, spawnerTransform.position, spawnerTransform.rotation);
        NavMeshAgent agent = go.GetComponent<NavMeshAgent>();

        if (agent != null)
        {
            agent.enabled = false;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(spawnerTransform.position, out hit, radioDeBusqueda, NavMesh.AllAreas))
            {
                go.transform.position = hit.position;
                agent.enabled = true;
                if (metaTransform != null) agent.SetDestination(metaTransform.position);
            }
        }
    }

    // Tu lógica original de ApocaMotor
    bool TodosLosEnemigosMurieron()
    {
        var motores = Object.FindObjectsByType<ApocaMotor>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        foreach (var m in motores)
        {
            if (m != null && m.Estado == ApocaMotor.EstadoApoca.Activo) return false;
        }
        return true;
    }
}