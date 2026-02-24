using UnityEngine;

public class StatsManager : MonoBehaviour
{
    public static StatsManager Instance;

    [Header("--- ESTADÍSTICAS DEL GRUPO ---")]
    public float vidaTotalGrupo = 1000f;
    public float vidaActual;

    void Awake()
    {
        Instance = this;
        vidaActual = vidaTotalGrupo;
    }

    public void RecibirDaño(float cantidad)
    {
        vidaActual -= cantidad;
        Debug.Log($"<color=red>❤️ VIDA GRUPAL: {vidaActual} / {vidaTotalGrupo}</color>");

        if (vidaActual <= 0)
        {
            Debug.LogError("💀 GAME OVER: Te quedaste sin vida grupal.");
            Time.timeScale = 0; // Pausa el juego al perder
        }
    }
}
