using UnityEngine;

/// <summary>
/// Clanes oficiales de ApocaMon (elemento/afinidad). Lista por ID: 0 Desconocido, 1 Océano, 2 Radiactivo, 3 Tóxico, 4 Mutavegetal, 5 Voltaje, 6 Gélido, 7 Impacto, 8 Ceniza, 9 Aéreo, 10 Psíquicos, 11 Plaga, 12 Roca, 13 Espectro, 14 Draco, 15 Siniestro, 16 Acero, 17 Espejismo, 18 Infierno.
/// </summary>
public enum ClanApocaMon
{
    Desconocido,   // 0
    Oceano,        // 1 (Océano)
    Radiactivo,    // 2
    Toxico,        // 3 (Tóxico)
    Mutavegetal,   // 4
    Voltaje,       // 5
    Gelido,        // 6 (Gélido)
    Impacto,       // 7
    Ceniza,        // 8
    Aereo,         // 9 (Aéreo)
    Psiquicos,     // 10 (Psíquicos)
    Plaga,         // 11
    Roca,          // 12
    Espectro,      // 13
    Draco,         // 14
    Siniestro,     // 15
    Acero,         // 16
    Espejismo,     // 17
    Infierno       // 18
}

/// <summary>
/// Escala de 10 niveles de chatarra para el sistema de drops (calidad del material).
/// </summary>
public enum TipoChatarra
{
    Mugre,
    Sobra,
    Recuperado,
    Util,
    Reforzado,
    Legado,
    Industrial,
    Tactico,
    Experimental,
    Reliquia
}

/// <summary>
/// Niveles de rango en escala griega (clasificación de potencial tras dados).
/// </summary>
public enum RankedGriego
{
    Alpha,
    Beta,
    Gamma,
    Delta,
    Epsilon,
    Zeta,
    Eta,
    Theta,
    Iota,
    Omega
}

/// <summary>
/// Datos base de un ApocaMon. ScriptableObject configurable desde el Inspector.
/// </summary>
[CreateAssetMenu(fileName = "NuevoApocaMon", menuName = "ApocaMon/Datos ApocaMon")]
public class DatosApocaMon : ScriptableObject
{
    #region Identidad

    [Tooltip("Nombre del ApocaMon")]
    public string nombre;

    [Tooltip("Prefab con el modelo visual del ApocaMon")]
    public GameObject prefab;

    #endregion

    #region Estadisticas

    [Header("Vida y defensa")]
    [Min(0f)] public float vidaMax;
    [Min(0f)] public float defensa;
    [Min(0f)] public float resistenciaMax;

    [Header("Recuperación")]
    [Min(0f)] public float recuperoDia;
    [Min(0f)] public float recuperoNoche;

    [Header("Combate")]
    [Min(0f)] public float alcance;
    [Min(0f)] public float velocidadAtaque;

    [Header("Movimiento")]
    [Tooltip("Solo para Salvajes")]
    [Min(0f)] public float velocidadMovimiento;

    #endregion

    #region Clanes

    [Tooltip("Clan principal del ApocaMon (elemento/afinidad)")]
    public ClanApocaMon clanPrincipal;

    [Tooltip("Rango/clasificación de potencial (tras dados)")]
    public RankedGriego ranked;

    #endregion

    #region Evolucion

    [Tooltip("ApocaMon en el que evoluciona (null si no evoluciona)")]
    public DatosApocaMon siguienteForma;
    [Tooltip("Nivel en el que ocurre la evolución")]
    [Min(1)] public int nivelEvolucion;

    #endregion

    #region Costos

    [Tooltip("Costo en chatarra para desplegarlo en el campo")]
    [Min(0)] public int costoChatarra;

    [Tooltip("Cantidad de celdas del grid que ocupa (ancho x alto o área)")]
    [Min(1)] public int espacioGrid;

    #endregion
}
