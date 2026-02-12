using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Gestiona el Desfile de ApocaMon: recibe los tipos de la oleada, elige N al azar para mostrar y asigna el elegido al DatosApocaMon.
/// La fuente de datos es siempre lo que pasa la oleada; no hay lista fija de 19 tipos.
/// </summary>
public class ManejadorDesfile : MonoBehaviour
{
    [Header("Datos a actualizar")]
    [Tooltip("Asset del ApocaMon al que se asignará el tipo elegido en el desfile")]
    [SerializeField] private DatosApocaMon datosApocaMon;

    /// <summary>
    /// Las opciones que ve el jugador (p. ej. 5 tipos elegidos al azar de la oleada). Persistencia para la selección.
    /// </summary>
    private TipoApocaMon[] opcionesActuales;

    /// <summary>
    /// Configura las opciones del desfile a partir de la oleada. De tiposOleada (p. ej. 10) elige cantidadAMostrar (p. ej. 5) al azar sin repetir y las guarda en opcionesActuales.
    /// </summary>
    /// <param name="tiposOleada">Tipos de la oleada (p. ej. los 10 de esta ronda)</param>
    /// <param name="cantidadAMostrar">Cuántas opciones mostrar al jugador (p. ej. 5)</param>
    public void ConfigurarOpcionesDesdeOleada(TipoApocaMon[] tiposOleada, int cantidadAMostrar)
    {
        if (tiposOleada == null || tiposOleada.Length == 0)
        {
            Debug.LogWarning("ManejadorDesfile: tiposOleada nulo o vacío.");
            opcionesActuales = new TipoApocaMon[0];
            return;
        }

        List<TipoApocaMon> lista = new List<TipoApocaMon>(tiposOleada);
        int tomar = Mathf.Min(cantidadAMostrar, lista.Count);

        // Barajar y tomar los primeros 'tomar' (aleatorio sin repetir)
        for (int i = lista.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            TipoApocaMon temp = lista[i];
            lista[i] = lista[j];
            lista[j] = temp;
        }

        opcionesActuales = new TipoApocaMon[tomar];
        for (int i = 0; i < tomar; i++)
            opcionesActuales[i] = lista[i];
    }

    /// <summary>
    /// Asigna al DatosApocaMon el tipo elegido por el jugador (índice en opcionesActuales).
    /// </summary>
    public void SeleccionarTipo(int indice)
    {
        if (datosApocaMon == null)
        {
            Debug.LogWarning("ManejadorDesfile: no hay DatosApocaMon asignado.");
            return;
        }

        if (opcionesActuales == null || opcionesActuales.Length == 0)
        {
            Debug.LogWarning("ManejadorDesfile: no hay opciones. Llamar antes a ConfigurarOpcionesDesdeOleada.");
            return;
        }

        if (indice < 0 || indice >= opcionesActuales.Length)
        {
            Debug.LogWarning($"ManejadorDesfile: índice {indice} fuera de rango (0-{opcionesActuales.Length - 1}).");
            return;
        }

        datosApocaMon.tipoPrincipal = opcionesActuales[indice];
        Debug.Log($"Desfile: tipo asignado = {datosApocaMon.tipoPrincipal}");
    }

    /// <summary>
    /// Devuelve las opciones actuales (las que ve el jugador) para mostrar en la UI.
    /// </summary>
    public TipoApocaMon[] ObtenerOpcionesActuales()
    {
        return opcionesActuales ?? new TipoApocaMon[0];
    }
}
