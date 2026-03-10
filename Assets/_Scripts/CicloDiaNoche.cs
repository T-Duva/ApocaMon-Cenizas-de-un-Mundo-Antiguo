using UnityEngine;

public class CicloDiaNoche : MonoBehaviour
{
    [Header("Configuración de Probabilidad")]
    public float probabilidadAcumulada = 5f;
    public bool esDeNoche = false;

    [Header("Referencias de Luces")]
    public Transform solTransform; // Tu Sol_Principal
    public Light solLight;
    public Light lunaLight; // La que crearemos ahora

    [Header("Tiempos")]
    public float timerNoche = 60f;
    private bool oleadaActiva = false;

    void Update()
    {
        // 1. Lógica de avance del Sol basada en Probabilidad
        ActualizarPosicionSol();

        // 2. Lógica de Noche (Solo corre si es de noche y NO hay oleada)
        if (esDeNoche && !oleadaActiva)
        {
            timerNoche -= Time.deltaTime;
            if (timerNoche <= 0)
            {
                TerminarNoche();
            }
        }
    }

    void ActualizarPosicionSol()
    {
        if (esDeNoche) return;

        // Mapeo: 0% prob = 90 grados (Cenit). 100% prob = 0 grados (Ocaso).
        // Si ahora estamos en 50% prob, el sol estará en 45-50 grados (lo que pediste).
        float rotacionX = Mathf.Lerp(90f, 0f, probabilidadAcumulada / 100f);
        solTransform.rotation = Quaternion.Euler(rotacionX, -30f, 0f);
    }

    public void AlTerminarOleada()
    {
        oleadaActiva = false;

        if (esDeNoche) return; // Si ya es de noche, no calculamos más prob.

        // Tirar dados
        float random = Random.Range(0f, 100f);
        if (random <= probabilidadAcumulada || probabilidadAcumulada >= 100f)
        {
            EmpezarNoche();
        }
        else
        {
            probabilidadAcumulada += 5f; // Acumula para la próxima
        }
    }

    public void AlEmpezarOleada() => oleadaActiva = true;

    void EmpezarNoche()
    {
        esDeNoche = true;
        timerNoche = 60f;
        if (solTransform != null)
            solTransform.rotation = Quaternion.Euler(0f, -30f, 0f);
        if (solLight != null) solLight.enabled = false;
        if (lunaLight != null)
        {
            lunaLight.intensity = 0.2f;
            lunaLight.color = new Color(0.6f, 0.7f, 1f);
            lunaLight.enabled = true;
        }
        Debug.Log("<color=blue>🌙 ¡Se hizo de noche en la Zona Tóxica!</color>");
    }

    void TerminarNoche()
    {
        esDeNoche = false;
        probabilidadAcumulada = 5f; // Reinicio
        solLight.enabled = true;
        lunaLight.enabled = false;
        Debug.Log("<color=yellow>☀️ El sol vuelve a salir.</color>");
    }
}