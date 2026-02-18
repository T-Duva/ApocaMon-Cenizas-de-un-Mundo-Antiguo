using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Spawner modular de enemigos para la Escena 03 (batalla). Instancia el prefab y le asigna destino a la Meta.
/// Configurado para 2D (XY): agentes no rotan ni cambian el eje para mantenerse en el plano.
/// Requiere: NavMeshPlus (bake en XY) o AI Navigation. Ver NAVMESH_2D_SETUP.md para bake en plano XY.
/// </summary>
public class GeneradorEnemigos : MonoBehaviour
{
    [Header("Spawn")]
    [Tooltip("Prefab del enemigo. Debe tener NavMeshAgent y su propio script de color (basado en zona).")]
    [SerializeField] private GameObject prefabEnemigo;
    [Tooltip("Objeto Meta hacia el que caminan los enemigos (esquivando obstáculos).")]
    [SerializeField] private Transform meta;

    private void Start()
    {
        InvokeRepeating(nameof(Spawn), 0f, 2f);
    }

    /// <summary>
    /// Spawnea un enemigo en la posición indicada. El prefab se encarga de su color (script interno / zona).
    /// Cada enemigo recibe destino = Meta asignada en el Inspector.
    /// </summary>
    public GameObject Spawn(Vector3 posicion)
    {
        if (prefabEnemigo == null || meta == null)
        {
            Debug.LogWarning("GeneradorEnemigos: asigna prefab Enemigo y Meta.");
            return null;
        }

        GameObject enemigo = Instantiate(prefabEnemigo, posicion, Quaternion.identity);

        NavMeshAgent agent = enemigo.GetComponent<NavMeshAgent>();
        if (agent == null)
            agent = enemigo.AddComponent<NavMeshAgent>();
        agent.enabled = true;
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.SetDestination(meta.position);

        return enemigo;
    }

    /// <summary>
    /// Spawnea en la posición del Generador.
    /// </summary>
    public GameObject Spawn()
    {
        return Spawn(transform.position);
    }

    /// <summary>
    /// Asigna un nuevo destino (p. ej. si la Meta se mueve).
    /// </summary>
    public void SetMeta(Transform nuevaMeta)
    {
        meta = nuevaMeta;
    }
}
