using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Lógica de combate: aplica ApocaData, recibe daño y pasa a KO (no Destroy) al llegar a 0 vida.
/// </summary>
public class ApocaMotor : MonoBehaviour
{
    public enum EstadoApoca { Activo, KO }

    [SerializeField] private ApocaData apocaData;

    private NavMeshAgent agent;
    private Collider col;
    private EstadoApoca estado = EstadoApoca.Activo;
    private float vidaActual;

    public EstadoApoca Estado => estado;
    public float VidaActual => vidaActual;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        col = GetComponent<Collider>();

        if (apocaData != null)
        {
            vidaActual = apocaData.VIDA;
            if (agent != null && agent.isOnNavMesh)
            {
                agent.speed = apocaData.VELOCIDAD;
                agent.acceleration = apocaData.ACELERACION;
                agent.SetDestination(Vector3.zero);
            }
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
