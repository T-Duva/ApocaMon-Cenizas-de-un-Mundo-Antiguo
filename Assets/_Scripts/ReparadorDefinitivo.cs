using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;

#if UNITY_EDITOR
using UnityEditor;

public class ReparadorDefinitivo : MonoBehaviour
{
    [MenuItem("ApocaMon/Reparacion Prefabs y Escalas")]
    public static void Reparar()
    {
        Debug.Log("<color=cyan>🛠️ ApocaMon: Corrigiendo escalas de Prefabs y Jerarquía...</color>");

        // 1. RESET DE ESCALAS (Padres e Hijos)
        GameObject suelo = GameObject.Find("Suelo");
        if (suelo != null)
        {
            Transform t = suelo.transform;
            while (t != null)
            {
                t.localScale = Vector3.one; // Esto asegura que 100x100 sea el tamaño real en el mundo
                t = t.parent;
            }

            BoxCollider2D col = suelo.GetComponent<BoxCollider2D>();
            if (col != null) col.size = new Vector2(100, 100);
        }

        // 2. BUSCAR EL ARCHIVO DEL ENEMIGO (Prefab en Assets)
        string[] guids = AssetDatabase.FindAssets("Enemigos_Zona_Toxica t:Prefab");
        if (guids.Length > 0)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab != null)
            {
                // Modificamos el archivo original para que el Spawner lo cree ya grande
                prefab.transform.localScale = new Vector3(10, 10, 1);
                EditorUtility.SetDirty(prefab);
                AssetDatabase.SaveAssets();
                Debug.Log("<color=magenta>👾 Prefab 'Enemigos_Zona_Toxica' escalado en la carpeta Assets.</color>");
            }
        }
        else
        {
            Debug.LogWarning("⚠️ No encontré el archivo 'Enemigos_Zona_Toxica' en la carpeta Assets.");
        }

        // 3. HORNEADO FINAL
        NavMeshSurface surface = Object.FindFirstObjectByType<NavMeshSurface>();
        if (surface != null)
        {
            surface.useGeometry = NavMeshCollectGeometry.PhysicsColliders;
            surface.BuildNavMesh();
            Debug.Log("<color=green>✅ NavMesh: ¡Área de 100x100 lista!</color>");
        }
    }
}
#endif
