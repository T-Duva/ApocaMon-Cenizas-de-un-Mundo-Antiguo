using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaveManager : MonoBehaviour
{
    [Header("Configuración de Oleada")]
    public GameObject enemigoPrefab;
    public Transform spawnerTransform;
    public int cantidadPorOleada = 10;
    public float tiempoEntreEnemigos = 1.0f; // <--- Acá controlás la cadencia
    public float tiempoEntreOleadas = 5.0f;

    private int oleadaActual = 0;
    private bool spawnando = false;

    void Start()
    {
        // Al empezar, esperamos un poco y lanzamos la primera oleada
        StartCoroutine(SpawnWave());
    }

    IEnumerator SpawnWave()
    {
        spawnando = true;
        oleadaActual++;
        Debug.Log($"🔥 Iniciando Oleada {oleadaActual}");

        for (int i = 0; i < cantidadPorOleada; i++)
        {
            SpawnEnemigo();
            yield return new WaitForSeconds(tiempoEntreEnemigos);
        }

        spawnando = false;
        Debug.Log($"✅ Oleada {oleadaActual} completada. Próxima en {tiempoEntreOleadas}s");
        
        yield return new WaitForSeconds(tiempoEntreOleadas);
        StartCoroutine(SpawnWave()); // Bucle de oleadas infinitas para testear
    }

    void SpawnEnemigo()
    {
        if (enemigoPrefab != null && spawnerTransform != null)
        {
            GameObject nuevo = Instantiate(enemigoPrefab, spawnerTransform.position, Quaternion.identity);
            // Aseguramos que el bicho nazca en Z=0 y con prioridad visual
            nuevo.transform.position = new Vector3(nuevo.transform.position.x, nuevo.transform.position.y, 0f);
        }
    }
}
