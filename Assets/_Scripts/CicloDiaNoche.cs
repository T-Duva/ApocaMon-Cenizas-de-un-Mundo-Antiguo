using UnityEngine;
using System.Collections;

public class CicloDiaNoche : MonoBehaviour
{
    [Header("Estado Actual")]
    public bool esDeNoche = false;
    public float temporizador = 0f;

    [Header("Configuración (Segundos)")]
    public float duracionDia = 120f;
    public float duracionNoche = 30f;
    public float arcoTotal = 180f;

    [Header("Unión Invisible y Aceleración")]
    [Tooltip("La velocidad mínima a la que se unen el día y la noche (tu '10 km/h').")]
    public float velocidadMinimaUnion = 0.3f;

    [Tooltip("Nivel 1 = Aceleración normal (10,20,30). Nivel 2 = Arranque lento (10,12,14,20). Nivel 3 o 4 = Arranque súper lento.")]
    [Range(1, 4)]
    public int lentitudArranque = 2; // <- ACÁ ESTÁ LA MAGIA QUE PEDISTE

    [Header("Referencias")]
    public Transform solTransform;
    public Light solLight;
    public Light lunaLight;

    private float gradosRecorridos = 0f;
    private float intensidadOriginalSol;

    void Start()
    {
        if (solLight != null)
        {
            intensidadOriginalSol = solLight.intensity;
            solLight.enabled = true;
        }
        if (lunaLight != null) lunaLight.enabled = false;

        temporizador = 0f;
        gradosRecorridos = 0f;
        esDeNoche = false;
    }

    void Update()
    {
        if (solTransform == null) return;

        temporizador += Time.deltaTime;

        float duracionActual = esDeNoche ? duracionNoche : duracionDia;
        float tiempoMatematico = Mathf.Clamp(temporizador, 0f, duracionActual);

        // 1. Calculamos el tiempo de 0 a 1
        float t = tiempoMatematico / duracionActual;

        // 2. LA MAGIA DEL ACHATAMIENTO: 
        // Mientras más alto el factor 'lentitudArranque', más tiempo se queda en 10, 12, 14...
        float curva = t;
        for (int i = 0; i < lentitudArranque; i++)
        {
            curva = Mathf.SmoothStep(0f, 1f, curva);
        }

        // 3. Calculamos el giro exacto garantizando los 180 grados y la velocidad mínima
        float progresoCalculado = (velocidadMinimaUnion * tiempoMatematico) +
                                  (arcoTotal - (velocidadMinimaUnion * duracionActual)) * curva;

        float deltaGiro = progresoCalculado - gradosRecorridos;
        solTransform.Rotate(0f, 0f, deltaGiro, Space.Self);

        gradosRecorridos = progresoCalculado;

        if (temporizador >= duracionActual)
        {
            if (!esDeNoche) EmpezarNoche();
            else TerminarNoche();
        }
    }

    [ContextMenu("Probar: Forzar Noche")]
    public void EmpezarNoche()
    {
        if (esDeNoche) return;

        esDeNoche = true;
        temporizador = 0f;
        gradosRecorridos = 0f;

        if (solLight != null) StartCoroutine(FadeOutLight(solLight));
        if (lunaLight != null) lunaLight.enabled = true;
    }

    [ContextMenu("Probar: Terminar Noche")]
    public void TerminarNoche()
    {
        if (!esDeNoche) return;

        esDeNoche = false;
        temporizador = 0f;
        gradosRecorridos = 0f;

        if (solLight != null) StartCoroutine(FadeInLight(solLight, intensidadOriginalSol));
        if (lunaLight != null) lunaLight.enabled = false;
    }

    // --- MÉTODOS REQUERIDOS POR WAVEMANAGER ---
    public void AlEmpezarOleada() { }
    public void AlTerminarOleada() { }

    IEnumerator FadeOutLight(Light lightToFade)
    {
        float startIntensity = lightToFade.intensity;
        while (lightToFade.intensity > 0.05f)
        {
            lightToFade.intensity -= startIntensity * Time.deltaTime;
            yield return null;
        }
        lightToFade.enabled = false;
    }

    IEnumerator FadeInLight(Light lightToFade, float targetIntensity)
    {
        lightToFade.intensity = 0f;
        lightToFade.enabled = true;

        while (lightToFade.intensity < targetIntensity)
        {
            lightToFade.intensity += targetIntensity * Time.deltaTime;
            yield return null;
        }
        lightToFade.intensity = targetIntensity;
    }
}