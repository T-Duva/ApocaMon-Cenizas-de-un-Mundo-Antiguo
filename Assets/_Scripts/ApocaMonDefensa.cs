using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Defensa colocable en el Mazing TD. Bloquea el paso de enemigos (NavMesh Obstacle con Carve).
/// Al nacer lee Mutante_Prueba y se pinta del clan elegido por el jugador. Clic derecho para quitar/reordenar.
/// Requiere un Collider2D o Collider en el objeto para detectar el clic y para el obstáculo.
/// </summary>
[RequireComponent(typeof(NavMeshObstacle))]
public class ApocaMonDefensa : MonoBehaviour
{
    private const int TamanioPool = 10;

    [Header("Datos del jugador")]
    [Tooltip("Asset Mutante_Prueba (DatosApocaMon) con el clan elegido en la escena anterior.")]
    [SerializeField] private DatosApocaMon mutantePrueba;

    [Header("Pool de 10 clanes/colores (mismo orden que en Desfile para coincidir)")]
    [SerializeField] private ClanYColorDefensa[] poolClanes = new ClanYColorDefensa[TamanioPool];

    [Header("Visual")]
    [Tooltip("Si está vacío, se usa el SpriteRenderer de este objeto o de un hijo.")]
    [SerializeField] private SpriteRenderer visual;

    private NavMeshObstacle obstaculo;

    [System.Serializable]
    public struct ClanYColorDefensa
    {
        public ClanApocaMon clan;
        public Color colorVisual;
    }

    private void Awake()
    {
        obstaculo = GetComponent<NavMeshObstacle>();
        if (obstaculo != null)
        {
            obstaculo.carving = true;
        }

        if (visual == null)
            visual = GetComponent<SpriteRenderer>();
        if (visual == null)
            visual = GetComponentInChildren<SpriteRenderer>(true);
    }

    private void Start()
    {
        PintarSegunMutantePrueba();
    }

    /// <summary>
    /// Lee el clan de Mutante_Prueba y aplica el color del pool. Llamado al nacer en esta escena.
    /// </summary>
    private void PintarSegunMutantePrueba()
    {
        if (mutantePrueba == null || poolClanes == null || poolClanes.Length == 0)
            return;

        ClanApocaMon clanElegido = mutantePrueba.clanPrincipal;
        for (int i = 0; i < poolClanes.Length; i++)
        {
            if (poolClanes[i].clan == clanElegido)
            {
                if (visual != null)
                    visual.color = poolClanes[i].colorVisual;
                return;
            }
        }

        if (visual != null)
            visual.color = Color.white;
    }

    private void Update()
    {
        if (!Input.GetMouseButtonDown(1))
            return;

        Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero);
        if (hit.collider != null && hit.collider.gameObject == gameObject)
        {
            gameObject.SetActive(false);
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit3D) && hit3D.collider.gameObject == gameObject)
        {
            gameObject.SetActive(false);
        }
    }
}
