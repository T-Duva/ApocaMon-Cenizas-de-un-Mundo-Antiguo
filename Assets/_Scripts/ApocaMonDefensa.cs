using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Defensa 3D: NavMeshObstacle con Carve. Clic derecho (raycast 3D) para quitar. Requiere BoxCollider (3D).
/// </summary>
[RequireComponent(typeof(NavMeshObstacle))]
[RequireComponent(typeof(BoxCollider))]
public class ApocaMonDefensa : MonoBehaviour
{
    private const int TamanioPool = 10;

    [Header("Datos del jugador")]
    [Tooltip("Asset Mutante_Prueba (DatosApocaMon) con el clan elegido en la escena anterior.")]
    [SerializeField] private DatosApocaMon mutantePrueba;

    [Header("Pool de 10 clanes/colores (mismo orden que en Desfile para coincidir)")]
    [SerializeField] private ClanYColorDefensa[] poolClanes = new ClanYColorDefensa[TamanioPool];

    [Header("Visual (SpriteRenderer o MeshRenderer para color clan)")]
    [SerializeField] private SpriteRenderer visual;
    [SerializeField] private MeshRenderer visual3D;

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

        if (visual == null) visual = GetComponentInChildren<SpriteRenderer>(true);
        if (visual3D == null) visual3D = GetComponentInChildren<MeshRenderer>(true);
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
                Color c = poolClanes[i].colorVisual;
                if (visual != null) visual.color = c;
                if (visual3D != null && visual3D.material != null) visual3D.material.color = c;
                return;
            }
        }

        if (visual != null) visual.color = Color.white;
        if (visual3D != null && visual3D.material != null) visual3D.material.color = Color.white;
    }

    private void Update()
    {
        if (!Input.GetMouseButtonDown(1) || Camera.main == null)
            return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit) && hit.collider != null && hit.collider.gameObject == gameObject)
            gameObject.SetActive(false);
    }
}
