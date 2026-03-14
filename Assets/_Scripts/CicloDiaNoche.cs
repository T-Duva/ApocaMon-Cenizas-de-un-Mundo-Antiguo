using UnityEngine;
using System.Collections;

public class CicloDiaNoche : MonoBehaviour
{
    [Header("Configuración de Probabilidad")]
    public float probabilidadAcumulada = 5f;
    public bool esDeNoche = false;

    [Header("Referencias de Luces")]
    public Transform solTransform; // EL PADRE (sol:principal)
    public Light solLight;
    public Light lunaLight;

    [Header("Ajuste de Movimiento")]
    public float arcoTotal = 180f;
    public float velocidadTransicion = 2f;
    public float timerNoche = 60f;

    private bool oleadaActiva = false;
    private float anguloInicialX;
    private float anguloInicialY;

    void Start()
    {
        if (solTransform != null)
        {
            // Guardamos tu rotación de Unity
            anguloInicialX = solTransform.eulerAngles.x;
            anguloInicialY = solTransform.eulerAngles.y;
        }
        if (lunaLight != null) lunaLight.enabled = false;
    }

    void Update()
    {
        if (!esDeNoche && solTransform != null)
        {
            // BARRIDO HORIZONTAL
            float sweepY = arcoTotal * (probabilidadAcumulada / 100f);
            Quaternion targetRot = Quaternion.Euler(anguloInicialX, anguloInicialY + sweepY, 0f);
            solTransform.rotation = Quaternion.Lerp(solTransform.rotation, targetRot, Time.deltaTime * velocidadTransicion);
        }

        if (esDeNoche && !oleadaActiva)
        {
            timerNoche -= Time.deltaTime;
            if (timerNoche <= 0) TerminarNoche();
        }
    }

    public void AlTerminarOleada()
    {
        oleadaActiva = false;
        if (esDeNoche) return;

        if (Random.Range(0f, 100f) <= probabilidadAcumulada || probabilidadAcumulada >= 100f)
            EmpezarNoche();
        else
            probabilidadAcumulada += 5f;
    }

    public void AlEmpezarOleada() => oleadaActiva = true;

    void EmpezarNoche()
    {
        esDeNoche = true;
        timerNoche = 60f;
        if (solLight != null) StartCoroutine(FadeOutLight(solLight));
        if (lunaLight != null) lunaLight.enabled = true;
    }

    void TerminarNoche()
    {
        esDeNoche = false;
        probabilidadAcumulada = 5f;
        if (solLight != null) solLight.enabled = true;
        if (lunaLight != null) lunaLight.enabled = false;
    }

    IEnumerator FadeOutLight(Light lightToFade)
    {
        float startIntensity = lightToFade.intensity;
        while (lightToFade.intensity > 0)
        {
            lightToFade.intensity -= startIntensity * Time.deltaTime;
            yield return null;
        }
        lightToFade.enabled = false;
        lightToFade.intensity = startIntensity;
    }
}