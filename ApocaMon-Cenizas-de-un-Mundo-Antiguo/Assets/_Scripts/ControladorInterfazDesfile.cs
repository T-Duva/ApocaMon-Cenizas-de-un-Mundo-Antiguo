using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

/// <summary>
/// Conecta la UI del desfile con ManejadorDesfile: pool de tipos (oleada) de tamaño N = cantidadTiposOleada,
/// sorteo solo sobre esos N, cantidadAMostrar botones. La elección se guarda en el asset asignado en ManejadorDesfile.
/// </summary>
public class ControladorInterfazDesfile : MonoBehaviour
{
    [Header("Conexiones")]
    [SerializeField] private ManejadorDesfile manejadorDesfile;
    [SerializeField] private SpriteRenderer visualApocaMon;
    [Tooltip("Botones donde se muestran los tipos (orden 0, 1, 2, ...). Los que sobren se desactivan.")]
    [SerializeField] private Button[] botones;

    [Header("Cantidades")]
    [Tooltip("Tamaño del pool de la oleada (p. ej. 6). El array poolTipos debe tener exactamente este tamaño.")]
    [SerializeField] private int cantidadTiposOleada = 6;
    [Tooltip("Cuántas opciones mostrar al jugador (p. ej. 3 botones). Debe ser <= cantidadTiposOleada.")]
    [SerializeField] private int cantidadAMostrar = 3;

    [Header("Pool de tipos (tamaño = cantidadTiposOleada)")]
    [Tooltip("Solo se usa este pool para el sorteo. Tamaño controlado por cantidadTiposOleada en el Editor.")]
    [SerializeField] private TipoConNombreYColor[] poolTipos;

    private TipoApocaMon[] opcionesActuales = new TipoApocaMon[0];
    private Color colorOriginal;

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
        int tamPool = poolTipos != null ? poolTipos.Length : 0;
        Debug.Log("Sorteo iniciado con " + tamPool + " tipos");

        if (manejadorDesfile == null || botones == null || botones.Length == 0)
        {
            Debug.LogWarning("ControladorInterfazDesfile: asigna ManejadorDesfile y el array de botones.");
            return;
        }
        if (tamPool == 0)
        {
            Debug.LogWarning("ControladorInterfazDesfile: el pool está vacío. Cargá al menos un tipo.");
            return;
        }
        if (cantidadAMostrar <= 0 || cantidadAMostrar > tamPool)
        {
            Debug.LogWarning("ControladorInterfazDesfile: cantidadAMostrar debe ser positiva y <= cantidad de tipos en el pool (" + tamPool + ").");
            return;
        }

        // Oleada = los tipos que hay en el pool (usa todo lo que encuentre)
        TipoApocaMon[] oleada = new TipoApocaMon[tamPool];
        for (int i = 0; i < tamPool; i++)
            oleada[i] = poolTipos[i].tipo;

        manejadorDesfile.ConfigurarOpcionesDesdeOleada(oleada, cantidadAMostrar);
        opcionesActuales = manejadorDesfile.ObtenerOpcionesActuales();

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
        AplicarColorVisual(ColorParaTipo(opcionesActuales[indice]));
        manejadorDesfile.SeleccionarTipo(indice);
    }

    public void OnBotonClic0() => OnBotonClic(0);
    public void OnBotonClic1() => OnBotonClic(1);
    public void OnBotonClic2() => OnBotonClic(2);
    public void OnBotonClic3() => OnBotonClic(3);
    public void OnBotonClic4() => OnBotonClic(4);
}
