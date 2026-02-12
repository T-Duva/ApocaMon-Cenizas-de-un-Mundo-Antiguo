using UnityEngine;

/// <summary>
/// Índice de las 5 estadísticas que se tiran con D6 en la clasificación.
/// </summary>
public enum StatClasificacion
{
    VidaMax,
    Defensa,
    ResistenciaMax,
    Alcance,
    VelocidadAtaque
}

/// <summary>
/// Genera estadísticas base tirando D6 por estadística. Permite hasta 3 intentos (1 inicial + 2 re-rolls) por stat.
/// No modifica tipoPrincipal (queda Desconocido hasta que el jugador elija en el siguiente paso).
/// </summary>
public class GeneradorStats : MonoBehaviour
{
    [Header("Datos a rellenar")]
    [Tooltip("Referencia al ApocaMon cuyas stats se generan con los dados")]
    [SerializeField] private DatosApocaMon datosApocaMon;

    [Header("Re-rolls (solo lectura en runtime)")]
    [SerializeField] private int[] intentosRestantesPorStat = new int[5];

    private const int TotalIntentosPorStat = 3;
    private const int CarasDado = 6;

    private void Awake()
    {
        if (intentosRestantesPorStat == null || intentosRestantesPorStat.Length != 5)
            intentosRestantesPorStat = new int[5];
    }

    /// <summary>
    /// Tira un D6 por cada una de las 5 estadísticas. VidaMax, defensa, resistenciaMax y alcance reciben el valor directo del dado (1-6).
    /// VelocidadAtaque se traduce a rangos de tiempo. Deja 2 re-rolls por estadística. No modifica tipoPrincipal.
    /// </summary>
    public void TirarDados()
    {
        if (datosApocaMon == null)
        {
            Debug.LogWarning("GeneradorStats: no hay DatosApocaMon asignado.");
            return;
        }

        for (int i = 0; i < 5; i++)
            AplicarTiradaStat(i);

        for (int i = 0; i < 5; i++)
            intentosRestantesPorStat[i] = TotalIntentosPorStat - 1; // 2 re-rolls restantes
    }

    /// <summary>
    /// Re-tira el dado para una estadística (si quedan intentos). Índice: 0=VidaMax, 1=Defensa, 2=ResistenciaMax, 3=Alcance, 4=VelocidadAtaque.
    /// </summary>
    public void RerollStat(int indiceStat)
    {
        if (datosApocaMon == null) return;
        if (indiceStat < 0 || indiceStat > 4) return;
        if (intentosRestantesPorStat[indiceStat] <= 0)
        {
            Debug.Log($"GeneradorStats: no quedan intentos para la stat {indiceStat}.");
            return;
        }

        AplicarTiradaStat(indiceStat);
        intentosRestantesPorStat[indiceStat]--;
    }

    /// <summary>
    /// Re-tira el dado para la estadística indicada por el enum (si quedan intentos).
    /// </summary>
    public void RerollStat(StatClasificacion stat)
    {
        RerollStat((int)stat);
    }

    /// <summary>
    /// Devuelve cuántos intentos (re-rolls) quedan para la estadística (0-4).
    /// </summary>
    public int IntentosRestantes(int indiceStat)
    {
        if (indiceStat < 0 || indiceStat > 4) return 0;
        return intentosRestantesPorStat[indiceStat];
    }

    /// <summary>
    /// Evento de historia: tira un D6 oculto y aplica bonus de vida. Nueva vidaMax = (vidaMax actual * dadoOculto) + 10.
    /// Activar desde cinemática o evento cuando corresponda.
    /// </summary>
    public void AplicarBonusVida()
    {
        if (datosApocaMon == null)
        {
            Debug.LogWarning("GeneradorStats: no hay DatosApocaMon asignado.");
            return;
        }

        int dadoOculto = Random.Range(1, CarasDado + 1); // D6 oculto
        float vidaActual = datosApocaMon.vidaMax;
        float vidaFinal = (vidaActual * dadoOculto) + 10f;

        datosApocaMon.vidaMax = vidaFinal;
        Debug.Log($"Evento de Vida: Se tiró un {dadoOculto}. La vida final es: {vidaFinal}");
    }

    private void AplicarTiradaStat(int indiceStat)
    {
        int d6 = Random.Range(1, CarasDado + 1); // 1-6

        switch (indiceStat)
        {
            case 0: // VidaMax: valor directo del dado (1-6)
                datosApocaMon.vidaMax = d6;
                break;
            case 1: // Defensa: valor directo del dado (1-6)
                datosApocaMon.defensa = d6;
                break;
            case 2: // ResistenciaMax: valor directo del dado (1-6)
                datosApocaMon.resistenciaMax = d6;
                break;
            case 3: // Alcance: valor directo del dado (1-6)
                datosApocaMon.alcance = d6;
                break;
            case 4: // VelocidadAtaque: única que se traduce a rangos de tiempo (segundos)
                datosApocaMon.velocidadAtaque = ValorVelocidadAtaque(d6);
                break;
        }
    }

    /// <summary>
    /// D6 1→5.01-6s, 2→4.01-5s, 3→3.01-4s, 4→2.01-3s, 5→1.01-2s, 6→0.1-6.0s.
    /// </summary>
    private static float ValorVelocidadAtaque(int d6)
    {
        float minSeg, maxSeg;
        switch (d6)
        {
            case 1: minSeg = 5.01f; maxSeg = 6f; break;
            case 2: minSeg = 4.01f; maxSeg = 5f; break;
            case 3: minSeg = 3.01f; maxSeg = 4f; break;
            case 4: minSeg = 2.01f; maxSeg = 3f; break;
            case 5: minSeg = 1.01f; maxSeg = 2f; break;
            case 6: minSeg = 0.1f; maxSeg = 6f; break;
            default: minSeg = 3f;  maxSeg = 4f; break;
        }
        return Random.Range(minSeg, maxSeg);
    }
}
