using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// ADN 3.3.2: Vida = (Base * Dado_1-6) * Rango; Daño/Defensa/Resistencia = Base * Rango. Cadencia/Velocidad/Alcance no escalan. acceleration = speed.
/// </summary>
public class ApocaMotor : MonoBehaviour
{
    public enum EstadoApoca { Activo, KO }

    [SerializeField] private ApocaData apocaData;

    private NavMeshAgent agent;
    private Collider col;
    private EstadoApoca estado = EstadoApoca.Activo;
    private float vidaActual;
    private float dañoFinal;
    private float defensaFinal;
    private float resistenciaFinal;

    public EstadoApoca Estado => estado;
    public float VidaActual => vidaActual;
    public float DañoFinal => dañoFinal;
    public float DefensaFinal => defensaFinal;
    public float ResistenciaFinal => resistenciaFinal;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        col = GetComponent<Collider>();

        if (apocaData == null) return;

        int rango = (int)apocaData.ranked;
        int dadoVida = Random.Range(1, 7);

        vidaActual = (apocaData.VIDA * dadoVida) * rango;
        dañoFinal = apocaData.DAÑO * rango;
        defensaFinal = apocaData.DEFENSA * rango;
        resistenciaFinal = apocaData.RESISTENCIA * rango;

        if (agent != null && agent.isOnNavMesh)
        {
            agent.speed = apocaData.VELOCIDAD;
            agent.acceleration = apocaData.VELOCIDAD;
            agent.SetDestination(Vector3.zero);
        }
    }

    /// <summary>
    /// Recibe daño. Si la vida llega a 0, llama a PasarAKO() (no Destroy).
    /// </summary>
    public void RecibirDano(float cantidad)
    {
        if (estado == EstadoApoca.KO) return;
        vidaActual = Mathf.Max(0f, vidaActual - cantidad);
        if (vidaActual <= 0f)
            PasarAKO();
    }

    /// <summary>
    /// Al llegar a 0 vida: desactiva NavMeshAgent y Collider, estado KO. No usa Destroy().
    /// </summary>
    private void PasarAKO()
    {
        estado = EstadoApoca.KO;
        if (agent != null)
            agent.enabled = false;
        if (col != null)
            col.enabled = false;
    }
}
