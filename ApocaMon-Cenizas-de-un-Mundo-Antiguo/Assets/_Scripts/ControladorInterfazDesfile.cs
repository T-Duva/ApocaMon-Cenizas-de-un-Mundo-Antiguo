using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Sistema modular del Desfile: pool fijo de 10 clanes → Oleada Prima (6 al azar) → 3 botones al azar.
/// Color y uso del visual se aplican al objeto asignado en visualApocaMon (p. ej. el hijo triángulo).
/// </summary>
public class ControladorInterfazDesfile : MonoBehaviour
{
    private const int TamanioPool = 10;
    private const int CantidadOleadaPrima = 6;
    private const int CantidadBotonesMostrar = 3;

    [Header("Conexiones")]
    [SerializeField] private ManejadorDesfile manejadorDesfile;
    [Tooltip("SpriteRenderer del objeto hijo (triángulo/visual). Aquí se aplica el color según el clan.")]
    [SerializeField] private SpriteRenderer visualApocaMon;
    [Tooltip("Los 5 botones. Se activan 3 con los clanes sorteados; los otros 2 se desactivan.")]
    [SerializeField] private Button[] botones;
    [Tooltip("Panel que contiene los botones de clanes. Se desactiva al elegir uno.")]
    [SerializeField] private GameObject panelBotones;
    [Tooltip("Botón CONTINUAR (oculto al inicio). Se muestra al elegir un clan.")]
    [SerializeField] private GameObject botonContinuar;

    [Header("Pool de 10 clanes (fijo)")]
    [SerializeField] private ClanConNombreYColor[] poolClanes = new ClanConNombreYColor[TamanioPool];

    private ClanApocaMon[] opcionesActuales = new ClanApocaMon[0];
    private Color colorOriginal;
    private ClanApocaMon clanElegido;

    [System.Serializable]
    public struct ClanConNombreYColor
    {
        public ClanApocaMon clan;
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
        if (poolClanes == null || poolClanes.Length < TamanioPool)
        {
            Debug.LogWarning("ControladorInterfazDesfile: el pool de clanes debe tener 10 elementos.");
            return;
        }
        if (manejadorDesfile == null || botones == null || botones.Length < CantidadBotonesMostrar)
        {
            Debug.LogWarning("ControladorInterfazDesfile: asigna ManejadorDesfile y al menos 3 botones.");
            return;
        }

        // Oleada Prima: 6 clanes al azar de los 10 del pool (sin repetir)
        ClanApocaMon[] oleadaPrima = ElegirAlAzarDePool(CantidadOleadaPrima);

        // De esos 6, elegir 3 para los botones
        manejadorDesfile.ConfigurarOpcionesDesdeOleada(oleadaPrima, CantidadBotonesMostrar);
        opcionesActuales = manejadorDesfile.ObtenerOpcionesActuales();

        // UI: activar 3 botones con nombre y color; desactivar los sobrantes
        for (int i = 0; i < botones.Length; i++)
        {
            if (botones[i] == null) continue;

            if (i < opcionesActuales.Length)
            {
                string nombre = NombreParaClan(opcionesActuales[i]);
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
    /// Elige 'cantidad' clanes al azar del pool de 10, sin repetir.
    /// </summary>
    private ClanApocaMon[] ElegirAlAzarDePool(int cantidad)
    {
        int n = Mathf.Min(cantidad, poolClanes.Length);
        int[] indices = new int[poolClanes.Length];
        for (int i = 0; i < indices.Length; i++)
            indices[i] = i;

        for (int i = indices.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            int t = indices[i];
            indices[i] = indices[j];
            indices[j] = t;
        }

        ClanApocaMon[] resultado = new ClanApocaMon[n];
        for (int i = 0; i < n; i++)
            resultado[i] = poolClanes[indices[i]].clan;
        return resultado;
    }

    private string NombreParaClan(ClanApocaMon c)
    {
        if (poolClanes == null) return c.ToString();
        for (int i = 0; i < poolClanes.Length; i++)
            if (poolClanes[i].clan == c)
                return poolClanes[i].nombreParaBoton;
        return c.ToString();
    }

    private Color ColorParaClan(ClanApocaMon c)
    {
        if (poolClanes == null) return Color.white;
        for (int i = 0; i < poolClanes.Length; i++)
            if (poolClanes[i].clan == c)
                return poolClanes[i].colorVisual;
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
        AplicarColorVisual(ColorParaClan(opcionesActuales[indice]));
    }

    public void OnHoverSalir()
    {
        AplicarColorVisual(colorOriginal);
    }

    public void OnBotonClic(int indice)
    {
        if (manejadorDesfile == null || indice < 0 || indice >= opcionesActuales.Length) return;
        clanElegido = opcionesActuales[indice];
        AplicarColorVisual(ColorParaClan(opcionesActuales[indice]));
        manejadorDesfile.SeleccionarClan(indice);

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
        Debug.Log("Cargando batalla con el clan: " + clanElegido);
        SceneManager.LoadScene("03_Batalla");
    }

    public void OnBotonClic0() => OnBotonClic(0);
    public void OnBotonClic1() => OnBotonClic(1);
    public void OnBotonClic2() => OnBotonClic(2);
    public void OnBotonClic3() => OnBotonClic(3);
    public void OnBotonClic4() => OnBotonClic(4);
}
