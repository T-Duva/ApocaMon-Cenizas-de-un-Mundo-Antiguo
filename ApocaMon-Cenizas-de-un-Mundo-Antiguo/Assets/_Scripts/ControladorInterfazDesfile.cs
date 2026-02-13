using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Sistema modular del Desfile: pool fijo de 10 tipos → Oleada Prima (6 al azar) → 3 botones al azar.
/// Color y uso del visual se aplican al objeto asignado en visualApocaMon (p. ej. el hijo triángulo).
/// </summary>
public class ControladorInterfazDesfile : MonoBehaviour
{
    private const int TamanioPool = 10;
    private const int CantidadOleadaPrima = 6;
    private const int CantidadBotonesMostrar = 3;

    [Header("Conexiones")]
    [SerializeField] private ManejadorDesfile manejadorDesfile;
    [Tooltip("SpriteRenderer del objeto hijo (triángulo/visual). Aquí se aplica el color según el tipo.")]
    [SerializeField] private SpriteRenderer visualApocaMon;
    [Tooltip("Los 5 botones. Se activan 3 con los tipos sorteados; los otros 2 se desactivan.")]
    [SerializeField] private Button[] botones;
    [Tooltip("Panel que contiene los botones de tipos. Se desactiva al elegir uno.")]
    [SerializeField] private GameObject panelBotones;
    [Tooltip("Botón CONTINUAR (oculto al inicio). Se muestra al elegir un tipo.")]
    [SerializeField] private GameObject botonContinuar;

    [Header("Pool de 10 tipos (fijo)")]
    [SerializeField] private TipoConNombreYColor[] poolTipos = new TipoConNombreYColor[TamanioPool];

    private TipoApocaMon[] opcionesActuales = new TipoApocaMon[0];
    private Color colorOriginal;
    private TipoApocaMon tipoElegido;

    [System.Serializable]
    public struct TipoConNombreYColor
    {
        public TipoApocaMon tipo;
        public string nombreParaBoton;
        public Color colorVisual;
    }

    private void Awake()
    {
        if (visualApocaMon != null)
            colorOriginal = visualApocaMon.color;
    }

    private void Start()
    {
        if (poolTipos == null || poolTipos.Length < TamanioPool)
        {
            Debug.LogWarning("ControladorInterfazDesfile: el pool debe tener 10 elementos.");
            return;
        }
        if (manejadorDesfile == null || botones == null || botones.Length < CantidadBotonesMostrar)
        {
            Debug.LogWarning("ControladorInterfazDesfile: asigna ManejadorDesfile y al menos 3 botones.");
            return;
        }

        // Oleada Prima: 6 tipos al azar de los 10 del pool (sin repetir)
        TipoApocaMon[] oleadaPrima = ElegirAlAzarDePool(CantidadOleadaPrima);

        // De esos 6, elegir 3 para los botones
        manejadorDesfile.ConfigurarOpcionesDesdeOleada(oleadaPrima, CantidadBotonesMostrar);
        opcionesActuales = manejadorDesfile.ObtenerOpcionesActuales();

        // UI: activar 3 botones con nombre y color; desactivar los sobrantes
        for (int i = 0; i < botones.Length; i++)
        {
            if (botones[i] == null) continue;

            if (i < opcionesActuales.Length)
            {
                string nombre = NombreParaTipo(opcionesActuales[i]);
                TMP_Text texto = botones[i].GetComponentInChildren<TMP_Text>(true);
                if (texto != null)
                    texto.text = nombre;

                botones[i].gameObject.SetActive(true);
                botones[i].enabled = true;
                botones[i].onClick.RemoveAllListeners();
                int indice = i;
                botones[i].onClick.AddListener(() => OnBotonClic(indice));

                EventTrigger et = botones[i].gameObject.GetComponent<EventTrigger>();
                if (et == null) et = botones[i].gameObject.AddComponent<EventTrigger>();
                et.triggers.Clear();
                var enter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
                enter.callback.AddListener((data) => OnHoverEntrar(indice));
                et.triggers.Add(enter);
                var exit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
                exit.callback.AddListener((data) => OnHoverSalir());
                et.triggers.Add(exit);
            }
            else
            {
                botones[i].gameObject.SetActive(false);
            }
        }

        if (botonContinuar != null)
            botonContinuar.SetActive(false);
    }

    /// <summary>
    /// Elige 'cantidad' tipos al azar del pool de 10, sin repetir.
    /// </summary>
    private TipoApocaMon[] ElegirAlAzarDePool(int cantidad)
    {
        int n = Mathf.Min(cantidad, poolTipos.Length);
        int[] indices = new int[poolTipos.Length];
        for (int i = 0; i < indices.Length; i++)
            indices[i] = i;

        for (int i = indices.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            int t = indices[i];
            indices[i] = indices[j];
            indices[j] = t;
        }

        TipoApocaMon[] resultado = new TipoApocaMon[n];
        for (int i = 0; i < n; i++)
            resultado[i] = poolTipos[indices[i]].tipo;
        return resultado;
    }

    private string NombreParaTipo(TipoApocaMon t)
    {
        if (poolTipos == null) return t.ToString();
        for (int i = 0; i < poolTipos.Length; i++)
            if (poolTipos[i].tipo == t)
                return poolTipos[i].nombreParaBoton;
        return t.ToString();
    }

    private Color ColorParaTipo(TipoApocaMon t)
    {
        if (poolTipos == null) return Color.white;
        for (int i = 0; i < poolTipos.Length; i++)
            if (poolTipos[i].tipo == t)
                return poolTipos[i].colorVisual;
        return Color.white;
    }

    /// <summary>
    /// Aplica color al visual asignado (objeto hijo en visualApocaMon).
    /// </summary>
    private void AplicarColorVisual(Color c)
    {
        if (visualApocaMon != null)
            visualApocaMon.color = c;
    }

    public void OnHoverEntrar(int indice)
    {
        if (indice < 0 || indice >= opcionesActuales.Length) return;
        AplicarColorVisual(ColorParaTipo(opcionesActuales[indice]));
    }

    public void OnHoverSalir()
    {
        AplicarColorVisual(colorOriginal);
    }

    public void OnBotonClic(int indice)
    {
        if (manejadorDesfile == null || indice < 0 || indice >= opcionesActuales.Length) return;
        tipoElegido = opcionesActuales[indice];
        AplicarColorVisual(ColorParaTipo(opcionesActuales[indice]));
        manejadorDesfile.SeleccionarTipo(indice);

        if (panelBotones != null)
            panelBotones.SetActive(false);
        if (botonContinuar != null)
            botonContinuar.SetActive(true);
    }

    /// <summary>
    /// Carga la escena de combate. Asignar al botón CONTINUAR.
    /// </summary>
    public void CargarBatalla()
    {
        Debug.Log("Cargando batalla con el tipo: " + tipoElegido);
        SceneManager.LoadScene("03_Batalla");
    }

    public void OnBotonClic0() => OnBotonClic(0);
    public void OnBotonClic1() => OnBotonClic(1);
    public void OnBotonClic2() => OnBotonClic(2);
    public void OnBotonClic3() => OnBotonClic(3);
    public void OnBotonClic4() => OnBotonClic(4);
}
