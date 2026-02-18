using UnityEngine;
using UnityEditor;
using NavMesh2D = NavMeshPlus.Components;

public class NavMeshFixer : EditorWindow
{
    [MenuItem("ApocaMon Tools/Reparar NavMesh 2D")]
    public static void FixNavMesh()
    {
        // 1. Configurar SUELO
        GameObject suelo = GameObject.Find("Suelo");
        if (suelo != null)
        {
            var col = suelo.GetComponent<BoxCollider>();
            if (col == null) col = suelo.AddComponent<BoxCollider>();
            col.size = new Vector3(col.size.x, col.size.y, 20f);

            var mod = suelo.GetComponent<NavMesh2D.NavMeshModifier>();
            if (mod == null) mod = suelo.AddComponent<NavMesh2D.NavMeshModifier>();
            mod.overrideArea = true;
            mod.area = 0;
            Debug.Log("✅ Suelo listo.");
        }

        // 2. Configurar NAVMESH_SYSTEM
        GameObject system = GameObject.Find("NavMesh_System");
        if (system == null) system = new GameObject("NavMesh_System");
        system.transform.rotation = Quaternion.Euler(-90, 0, 0);

        var surface = system.GetComponent<NavMesh2D.NavMeshSurface>();
        if (surface == null) surface = system.AddComponent<NavMesh2D.NavMeshSurface>();

        // CORRECCIÓN AQUÍ: Usamos la referencia universal de Unity para la geometría
        surface.useGeometry = UnityEngine.AI.NavMeshCollectGeometry.PhysicsColliders;
        surface.collectObjects = NavMesh2D.CollectObjects.All;
        surface.agentTypeID = -1314334417;

        // 3. BAKE
        surface.BuildNavMesh();
        Debug.Log("🚀 ¡Bake exitoso! El piso debería estar azul.");
    }
}
