using UnityEngine;
using UnityEditor;
using System.IO;

public class Auditoria2D : EditorWindow
{
    [MenuItem("ApocaMon/Super Escaneo Total 2D")]
    public static void ShowWindow() => GetWindow<Auditoria2D>("Escáner Total");

    void OnGUI()
    {
        GUILayout.Label("Buscador Exhaustivo de Basura 2D", EditorStyles.boldLabel);
        if (GUILayout.Button("¡Escanear TODO el Proyecto!", GUILayout.Height(40)))
        {
            EjecutarEscaneoTotal();
        }
    }

    void EjecutarEscaneoTotal()
    {
        Debug.Log("<color=red><b>--- INICIANDO ESCANEO EXHAUSTIVO (ESCENA + ASSETS) ---</b></color>");
        int totalEncontrados = 0;

        // 1. BUSCAR EN LA ESCENA ACTUAL
        GameObject[] todosEscena = GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        foreach (GameObject obj in todosEscena)
        {
            foreach (Component comp in obj.GetComponents<Component>())
            {
                if (comp != null && comp.GetType().Name.EndsWith("2D"))
                {
                    Debug.Log($"<color=cyan>[ESCENA]</color> Objeto: <b>{obj.name}</b> | Componente: {comp.GetType().Name}");
                    totalEncontrados++;
                }
            }
        }

        // 2. BUSCAR EN TODOS LOS PREFABS DEL PROYECTO
        string[] prefabsGUIDs = AssetDatabase.FindAssets("t:Prefab");
        foreach (string guid in prefabsGUIDs)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab != null)
            {
                foreach (Component comp in prefab.GetComponentsInChildren<Component>(true))
                {
                    if (comp != null && comp.GetType().Name.EndsWith("2D"))
                    {
                        Debug.Log($"<color=yellow>[PREFAB]</color> En: <b>{path}</b> | Componente: {comp.GetType().Name}");
                        totalEncontrados++;
                    }
                }
            }
        }

        // 3. BUSCAR ARCHIVOS CON "2D" EN EL NOMBRE
        string[] todasLasRutas = AssetDatabase.GetAllAssetPaths();
        foreach (string ruta in todasLasRutas)
        {
            if (ruta.Contains("2D") || ruta.Contains("2d"))
            {
                // Ignorar librerías internas de Unity para no ensuciar el log
                if (ruta.StartsWith("Assets"))
                {
                    Debug.Log($"<color=magenta>[ARCHIVO/CARPETA]</color> Ruta: <b>{ruta}</b>");
                    totalEncontrados++;
                }
            }
        }

        Debug.Log($"<color=red><b>--- ESCANEO FINALIZADO: Se encontraron {totalEncontrados} elementos sospechosos ---</b></color>");
    }
}