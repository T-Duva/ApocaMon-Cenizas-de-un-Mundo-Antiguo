using UnityEngine;
using UnityEditor;
using UnityEngine.AI;
using NavMeshPlus.Components;

public class NavMeshFixer : EditorWindow
{
    [MenuItem("ApocaMon Tools/🔥 REPARAR TODO (Z, Cámara y Capas)")]
    public static void FixEverything()
    {
        Debug.Log("🚀 Iniciando Reparación Total 3.2.3...");

        // 1. Reset de Cámara
        GameObject cam = GameObject.Find("Main Camera");
        if (cam != null)
        {
            Undo.RecordObject(cam.transform, "Fix Camera");
            cam.transform.position = new Vector3(0f, 0f, -10f); // Posición estándar 2D
            Debug.Log("🎥 Cámara reseteada a Z = -10");
        }

        // 2. Aplastar objetos a Z=0 y configurar capas
        FixObject("Suelo", 0, -10);          // Fondo
        FixObject("Meta", 0, 1);             // Encima del suelo
        FixObject("Spawner_Enemigos", 0, 5); // Encima de todo

        // 3. Sistema de NavMesh
        GameObject sistema = GameObject.Find("NavMesh_System");
        if (sistema != null)
        {
            Undo.RecordObject(sistema.transform, "Fix NavMesh System");
            sistema.transform.position = new Vector3(sistema.transform.position.x, sistema.transform.position.y, 0f);

            NavMeshSurface surface = sistema.GetComponent<NavMeshSurface>();
            if (surface != null)
            {
                Debug.Log("⚙️ Horneando NavMesh...");
                surface.BuildNavMesh();
            }
        }

        Debug.Log("✅ ¡PROCESO COMPLETADO! Dale al Play.");
    }

    private static void FixObject(string nombre, float zPos, int sortingOrder)
    {
        GameObject obj = GameObject.Find(nombre);
        if (obj != null)
        {
            Undo.RecordObject(obj.transform, "Fix Position");
            obj.transform.position = new Vector3(obj.transform.position.x, obj.transform.position.y, zPos);

            SpriteRenderer renderer = obj.GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                Undo.RecordObject(renderer, "Fix Sorting");
                renderer.sortingOrder = sortingOrder;
            }
            Debug.Log($"✅ {nombre}: Z={zPos}, Order={sortingOrder}");
        }
    }
}
