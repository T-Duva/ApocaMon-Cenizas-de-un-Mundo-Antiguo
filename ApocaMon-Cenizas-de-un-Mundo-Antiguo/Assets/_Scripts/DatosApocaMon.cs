using UnityEngine;

/// <summary>
/// Tipos oficiales de ApocaMon (elemento/afinidad). 19 tipos del diseño final.
/// </summary>
public enum TipoApocaMon
{
    Desconocido,
    Oasis,
    Radiactivo,
    Toxico,
    Mutavegetal,
    Voltaje,
    Criogenico,
    Gladiador,
    Erosion,
    Vigia,
    Eco,
    Plaga,
    Escombro,
    Anomalia,
    Leviatan,
    Carroñero,
    Blindado,
    Espejismo,
    Incinerador
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

    #region Tipos

    [Tooltip("Tipo principal del ApocaMon (elemento/afinidad)")]
    public TipoApocaMon tipoPrincipal;

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
