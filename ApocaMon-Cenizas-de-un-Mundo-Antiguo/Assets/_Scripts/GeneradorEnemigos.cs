using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Spawner modular de enemigos para la Escena 03 (batalla). Clasificación 3.2.3: posición Z=0, Warp al NavMesh y SetDestination robustos.
/// Usa índice de oleada para consistencia con 3.2.2. Requiere NavMeshPlus (bake XY) o AI Navigation.
/// </summary>
public class GeneradorEnemigos : MonoBehaviour
{
    [Header("Spawn")]
    [Tooltip("Prefab del enemigo (ej. Enemigos_Zona_Toxica). Debe tener NavMeshAgent e IAEnemigo.")]
    [SerializeField] private GameObject prefabEnemigo;
    [Tooltip("Objeto Meta hacia el que caminan los enemigos (esquivando obstáculos).")]
    [SerializeField] private Transform meta;

    [Header("Oleadas (3.2.2)")]
    [Tooltip("Índice de la oleada actual (para consistencia con el sistema de oleadas).")]
    [SerializeField] private int indiceOleada = 0;

    private const float RadioSampleInicial = 5f;
    private const float RadioSampleFallback = 15f;

    private void Awake()
    {
        // EMERGENCIA 3.2.3: Forzar Z=0 lo antes posible (antes que cualquier otro script).
        ForzarZCero();
    }

    private void OnValidate()
    {
        // Ley absoluta: Z siempre 0. Nunca asignar otro valor a position.z.
        if (transform.position.z != 0f)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, 0f);
            Debug.Log("🛡️ Sistema de Seguridad 3.2.3: Z forzado a 0");
        }
    }

    private void Start()
    {
        ForzarZCero();
        InvokeRepeating(nameof(Spawn), 0f, 2f);
    }

    /// <summary>
    /// Fuerza la posición Z del Spawner a 0. Llamar en Awake, Start y al inicio de Spawn.
    /// </summary>
    private void ForzarZCero()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, 0f);
    }

    /// <summary>
    /// Spawnea un enemigo con diagnóstico de Agent Type ID. Detecta desajuste entre prefab (ej. Agente2D) y NavMeshSurface (ej. Default).
    /// </summary>
    public void Spawn()
    {
        // Ley absoluta Z=0
        ForzarZCero();

        if (prefabEnemigo == null)
        {
            Debug.LogWarning("GeneradorEnemigos: asigna el prefab del enemigo.");
            return;
        }

        // 1. Instanciar en Z=0 con Quaternion.identity; posición final forzada a (x, y, 0f)
        Vector3 posicionNacimiento = new Vector3(transform.position.x, transform.position.y, 0f);
        GameObject nuevoEnemigo = Instantiate(prefabEnemigo, posicionNacimiento, Quaternion.identity);
        nuevoEnemigo.transform.position = new Vector3(nuevoEnemigo.transform.position.x, nuevoEnemigo.transform.position.y, 0f);

        SpriteRenderer sr = nuevoEnemigo.GetComponentInChildren<SpriteRenderer>();
        if (sr != null) sr.sortingOrder = 1000;

        NavMeshAgent agent = nuevoEnemigo.GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogWarning($"GeneradorEnemigos: el prefab no tiene NavMeshAgent (oleada {indiceOleada}).");
            return;
        }

        // 2. DIAGNÓSTICO: mostrar qué tipo de agente busca (detecta error "No build settings for agent type ID -1314334417")
        Debug.Log("Agent Type ID: " + agent.agentTypeID);

        agent.enabled = true;
        agent.updateRotation = false;

        // 3. SamplePosition CON EL MISMO agentTypeID que el prefab (si el Surface horneó para otro tipo, aquí falla)
        NavMeshQueryFilter filter = new NavMeshQueryFilter
        {
            agentTypeID = agent.agentTypeID,
            areaMask = -1 // Todas las áreas
        };

        NavMeshHit hit;
        float maxDistance = RadioSampleFallback;
        bool encontroPiso = NavMesh.SamplePosition(posicionNacimiento, out hit, maxDistance, filter);

        if (!encontroPiso)
        {
            Debug.LogError(
                "El NavMeshSurface no tiene configurado el Agente tipo [ID " + agent.agentTypeID + "]. " +
                "Solución: en NavMesh_System > NavMeshSurface, asegurate de que 'Agent Type' coincida con el del prefab del enemigo (ej. Agente2D). " +
                "O en el prefab del enemigo, cambia NavMeshAgent > Agent Type a 'Humanoid' si el Surface horneó para Humanoid."
            );
            agent.Warp(posicionNacimiento);
            return;
        }

        // 4. Piso encontrado para este tipo de agente: Warp y activar
        agent.Warp(hit.position);

        // 5. Destino en Z=0
        if (meta != null)
        {
            Vector3 destino = new Vector3(meta.position.x, meta.position.y, 0f);
            agent.SetDestination(destino);
        }
    }

    /// <summary>
    /// Actualiza el índice de oleada (sistema 3.2.2).
    /// </summary>
    public void SetIndiceOleada(int indice)
    {
        indiceOleada = indice;
    }

    /// <summary>
    /// Asigna un nuevo destino (p. ej. si la Meta se mueve).
    /// </summary>
    public void SetMeta(Transform nuevaMeta)
    {
        meta = nuevaMeta;
    }
}
