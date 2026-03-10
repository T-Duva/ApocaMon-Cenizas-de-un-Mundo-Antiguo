using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;

#if UNITY_EDITOR
using UnityEditor;

public class ReparadorDefinitivo : MonoBehaviour
{
    /// <summary>Botón nuclear: borra TODO lo relacionado con NavMesh. No hace Bake. Escena vacía de navegación.</summary>
    [MenuItem("ApocaMon/☢️ LIMPIEZA NUCLEAR (borrar todo azul)")]
    public static void LimpiezaNuclear()
    {
        // 1. Limpiar caché de memoria
        NavMesh.RemoveAllNavMeshData();

        // 2. Borrar CUALQUIER componente NavMeshSurface en la escena
        NavMeshSurface[] surfaces = Object.FindObjectsByType<NavMeshSurface>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (NavMeshSurface s in surfaces)
        {
            if (s != null)
                Object.DestroyImmediate(s);
        }

        // 3. Borrar objetos que suelen tener estos datos (por nombre)
        string[] nombresABorrar = { "ZONA_NAVMESH_3D", "ZONA_NAVMESH_APOCAMON", "NavMesh_System", "NAVMESH_PROYECTOR_3D", "MAPA_APOCAMON_3D", "MAPA_3D_BASE", "PISO_NAVMESH_UNICO" };
        foreach (string nombre in nombresABorrar)
        {
            GameObject obj = GameObject.Find(nombre);
            if (obj != null)
            {
                Object.DestroyImmediate(obj);
                Debug.Log($"<color=red>🗑️ Objeto borrado: {nombre}</color>");
            }
        }

        Debug.Log("<color=red>☢️ LIMPIEZA NUCLEAR COMPLETADA. Ya no debería existir NADA azul en la escena.</color>");
    }

    /// <summary>Resetea el apartado visual del suelo: borra Suelo/Pasto/SUELO_DEFINITIVO y crea un Plane 3D (100m x 100m).</summary>
    [MenuItem("ApocaMon/🌱 Resetear pasto (SUELO_DEFINITIVO)")]
    public static void ResetearPastoWarcraft()
    {
        var escena = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();
        string[] basura = { "Suelo", "Pasto", "SUELO_DEFINITIVO" };
        foreach (GameObject root in escena.GetRootGameObjects())
        {
            BorrarSueloPastoRecursivo(root, basura);
        }

        GameObject suelo = GameObject.CreatePrimitive(PrimitiveType.Plane);
        suelo.name = "SUELO_DEFINITIVO";
        Undo.RegisterCreatedObjectUndo(suelo, "Create SUELO_DEFINITIVO");
        suelo.transform.position = new Vector3(0f, 0f, 0f);
        suelo.transform.localScale = new Vector3(10f, 1f, 10f);
        suelo.transform.rotation = Quaternion.identity;

        Renderer rend = suelo.GetComponent<Renderer>();
        if (rend != null)
        {
            Material matPasto = null;
            string[] guidsMat = AssetDatabase.FindAssets("Pasto t:Material");
            if (guidsMat.Length == 0) guidsMat = AssetDatabase.FindAssets("Grass t:Material");
            if (guidsMat.Length == 0) guidsMat = AssetDatabase.FindAssets("PastoSuelo t:Material");
            if (guidsMat.Length > 0)
                matPasto = AssetDatabase.LoadAssetAtPath<Material>(AssetDatabase.GUIDToAssetPath(guidsMat[0]));
            if (matPasto != null)
            {
                rend.sharedMaterial = matPasto;
            }
            else
            {
                Shader sh = Shader.Find("Universal Render Pipeline/Lit");
                if (sh == null) sh = Shader.Find("Standard");
                rend.sharedMaterial = new Material(sh);
                rend.sharedMaterial.color = new Color(0.2f, 0.7f, 0.2f);
            }
        }

        suelo.layer = LayerMask.NameToLayer("Default");
        EditorUtility.SetDirty(suelo);
        Debug.Log("<color=green>🌱 SUELO 3D CREADO: 100m x 100m en Y=0</color>");
    }

    private static void BorrarSueloPastoRecursivo(GameObject go, string[] nombresABorrar)
    {
        if (go == null) return;
        foreach (string nombre in nombresABorrar)
        {
            if (go.name == nombre)
            {
                Debug.Log($"<color=red>🗑️ Borrando objeto viejo: {nombre}</color>");
                Object.DestroyImmediate(go);
                return;
            }
        }
        for (int i = go.transform.childCount - 1; i >= 0; i--)
        {
            Transform child = go.transform.GetChild(i);
            if (child != null)
                BorrarSueloPastoRecursivo(child.gameObject, nombresABorrar);
        }
    }

    /// <summary>Sistema de navegación definitivo: alineación previa obligatoria, luego piso 3D único y Bake.</summary>
    [MenuItem("ApocaMon/🌟 Fundar Mundo 3D")]
    public static void FundarMundo3D()
    {
        CorregirAlineacionGlobal();

        GameObject piso = GameObject.Find("PISO_NAVMESH_UNICO");
        if (piso != null) Object.DestroyImmediate(piso);

        piso = GameObject.CreatePrimitive(PrimitiveType.Cube);
        piso.name = "PISO_NAVMESH_UNICO";
        Undo.RegisterCreatedObjectUndo(piso, "Create PISO_NAVMESH_UNICO");

        piso.transform.position = new Vector3(0f, 0f, 0f);
        piso.transform.rotation = Quaternion.identity;
        piso.transform.localScale = new Vector3(100f, 1f, 100f);

        NavMeshSurface surface = piso.AddComponent<NavMeshSurface>();
        surface.collectObjects = CollectObjects.Children;
        surface.useGeometry = NavMeshCollectGeometry.PhysicsColliders;
        surface.RemoveData();
        surface.BuildNavMesh();

        if (surface.navMeshData != null)
        {
            Bounds b = surface.navMeshData.sourceBounds;
            float area = b.size.x * b.size.z;
            if (area < 100f)
                Debug.LogError("<color=red>❌ ERROR CRÍTICO: El horneado falló o es demasiado pequeño (menor a 100 m²).</color>");
            else
            {
                Debug.Log($"<color=green>🌟 MUNDO FUNDADO (plano XZ): {b.size.x:F1}m x {b.size.z:F1}m = {area:F0} m²</color>");
                if (area >= 9999f)
                    Debug.Log("<color=green>✅ Bake final: 10000 m² confirmados. Tower Defense 3D listo.</color>");
                MeshRenderer mr = piso.GetComponent<MeshRenderer>();
                if (mr != null) mr.enabled = false;
            }
        }
        else
            Debug.LogError("<color=red>❌ ERROR CRÍTICO: El horneado falló o es demasiado pequeño.</color>");

        piso.layer = LayerMask.NameToLayer("Default");
        piso.isStatic = true;
        EditorUtility.SetDirty(piso);

        Debug.Log("<color=gray>💡 Para ver el pasto y no el azul: deseleccioná PISO_NAVMESH_UNICO en la jerarquía o apagá los Gizmos arriba de la ventana Scene.</color>");
        ActualizarPrefabEnemigosPrioridad();
    }

    /// <summary>Corrección de ángulos y posiciones obligatoria antes de Bake. Cámara cenital, suelo visual y prefabs alineados.</summary>
    private static void CorregirAlineacionGlobal()
    {
        Camera cam = Object.FindFirstObjectByType<Camera>(FindObjectsInactive.Include);
        if (cam == null) cam = Camera.main;
        if (cam != null)
        {
            cam.transform.position = new Vector3(0f, 50f, -30f);
            cam.transform.LookAt(Vector3.zero);
            Debug.Log("[Cámara] Main Camera apuntando a (0,0,0). Vista Tower Defense 3D. Luces y cielo se configuran desde Window > Rendering > Lighting.");
        }

        ResetearPastoWarcraft();

        GameObject pisoNav = GameObject.Find("PISO_NAVMESH_UNICO");
        if (pisoNav != null)
        {
            MeshRenderer mrPiso = pisoNav.GetComponent<MeshRenderer>();
            if (mrPiso != null && mrPiso.enabled) { mrPiso.enabled = false; EditorUtility.SetDirty(pisoNav); }
        }

        GameObject enemigosEnEscena = GameObject.Find("Enemigos_Zona_Toxica");
        if (enemigosEnEscena != null)
        {
            Vector3 p = enemigosEnEscena.transform.position;
            enemigosEnEscena.transform.position = new Vector3(p.x, p.y, 0f);
            EditorUtility.SetDirty(enemigosEnEscena);
        }

        AlinearPrefabEnemigosYAgent();
        Debug.Log("<color=cyan>📐 ALINEACIÓN COMPLETADA: Mundo horizontal XZ configurado.</color>");
    }

    private static void AlinearPrefabEnemigosYAgent()
    {
        string[] guids = AssetDatabase.FindAssets("Enemigos_Zona_Toxica t:Prefab");
        if (guids.Length == 0) return;
        string path = AssetDatabase.GUIDToAssetPath(guids[0]);
        string prefabPath = AssetDatabase.GetAssetPath(AssetDatabase.LoadAssetAtPath<GameObject>(path));
        GameObject instance = PrefabUtility.LoadPrefabContents(prefabPath);
        if (instance == null) return;
        try
        {
            bool changed = false;
            if (instance.transform.localRotation != Quaternion.Euler(90f, 0f, 0f))
            {
                instance.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
                changed = true;
            }
            Vector3 pos = instance.transform.localPosition;
            if (pos.z != 0f) { instance.transform.localPosition = new Vector3(pos.x, pos.y, 0f); changed = true; }
            NavMeshAgent agent = instance.GetComponent<NavMeshAgent>();
            if (agent != null)
            {
                if (agent.avoidancePriority != 0) { agent.avoidancePriority = 0; changed = true; }
                if (agent.updateRotation) { agent.updateRotation = false; changed = true; }
                if (agent.updateUpAxis) { agent.updateUpAxis = false; changed = true; }
            }
            if (changed)
            {
                PrefabUtility.SaveAsPrefabAsset(instance, prefabPath);
                Debug.Log("[Enemigos_Zona_Toxica] Rotación (90,0,0), Priority 0, Update Up Axis desactivado.");
            }
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(instance);
        }
    }

    private static void ActualizarPrefabEnemigosPrioridad()
    {
        string[] guids = AssetDatabase.FindAssets("Enemigos_Zona_Toxica t:Prefab");
        if (guids.Length == 0) return;
        string path = AssetDatabase.GUIDToAssetPath(guids[0]);
        string prefabPath = AssetDatabase.GetAssetPath(AssetDatabase.LoadAssetAtPath<GameObject>(path));
        GameObject instance = PrefabUtility.LoadPrefabContents(prefabPath);
        if (instance == null) return;
        try
        {
            NavMeshAgent agent = instance.GetComponent<NavMeshAgent>();
            if (agent != null && agent.avoidancePriority != 0)
            {
                agent.avoidancePriority = 0;
                PrefabUtility.SaveAsPrefabAsset(instance, prefabPath);
                Debug.Log("[Enemigos_Zona_Toxica] NavMeshAgent Priority = 0 (máxima prioridad, evita choques entre ellos).");
            }
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(instance);
        }
    }

    private static System.Collections.Generic.List<Vector3> _lineTargetsParaEscena = new System.Collections.Generic.List<Vector3>();

    /// <summary>Diagnóstico: objetos en un radio de 5m del centro (0,0,0). Reporte y líneas rojas en la Scene.</summary>
    [MenuItem("ApocaMon/🔎 Escanear centro (0,0,0)")]
    public static void EscanearCentro()
    {
        _lineTargetsParaEscena.Clear();
        var reportados = new System.Collections.Generic.HashSet<GameObject>();
        Vector3 centro = Vector3.zero;
        float radio = 5f;

        Collider[] objetosCercanos = Physics.OverlapSphere(centro, radio);
        foreach (Collider col in objetosCercanos)
        {
            if (col != null && col.gameObject != null)
                reportados.Add(col.gameObject);
        }

        NavMeshSurface[] todasSurfaces = Object.FindObjectsByType<NavMeshSurface>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (NavMeshSurface s in todasSurfaces)
        {
            if (s != null && s.gameObject != null)
                reportados.Add(s.gameObject);
        }

        if (reportados.Count == 0)
            Debug.Log("<color=yellow>🔎 ESCANEANDO CENTRO (0,0,0)... Encontrados: 0 (OverlapSphere solo ve Colliders; revisá NavMeshSurface en escena).</color>");
        else
            Debug.Log($"<color=yellow>🔎 ESCANEANDO CENTRO (0,0,0)... Encontrados: {reportados.Count}</color>");

        foreach (GameObject go in reportados)
        {
            if (go == null) continue;
            Vector3 pos = go.transform.position;
            _lineTargetsParaEscena.Add(pos);

            System.Text.StringBuilder comps = new System.Text.StringBuilder();
            foreach (Component c in go.GetComponents<Component>())
            {
                if (c == null) continue;
                string tipo = c.GetType().Name;
                comps.Append(tipo).Append(", ");
            }
            string compStr = comps.ToString().TrimEnd(' ', ',');
            string extra = "";
            if (go.GetComponent<NavMeshAgent>() != null) extra += " [NavMeshAgent]";
            if (go.GetComponent<UnityEngine.SpriteRenderer>() != null) extra += " [SpriteRenderer]";
            if (go.GetComponent<NavMeshSurface>() != null) extra += " [NavMeshSurface ⚠️]";
            foreach (Component c in go.GetComponents<MonoBehaviour>())
            {
                if (c != null && (c.GetType().Name.IndexOf("Jugador", System.StringComparison.OrdinalIgnoreCase) >= 0 || c.GetType().Name.IndexOf("Base", System.StringComparison.OrdinalIgnoreCase) >= 0))
                    extra += " [Script: " + c.GetType().Name + "]";
            }
            Debug.Log($"<color=cyan>📍 OBJETO: {go.name} | Pos: {pos} | Componentes: {compStr}{extra}</color>");
        }

        SceneView.duringSceneGui -= DibujarLineasAlCentro;
        SceneView.duringSceneGui += DibujarLineasAlCentro;
        SceneView.RepaintAll();
    }

    private static void DibujarLineasAlCentro(SceneView view)
    {
        SceneView.duringSceneGui -= DibujarLineasAlCentro;
        if (_lineTargetsParaEscena == null || _lineTargetsParaEscena.Count == 0) return;
        Handles.color = Color.red;
        Vector3 origen = Vector3.zero;
        foreach (Vector3 destino in _lineTargetsParaEscena)
            Handles.DrawLine(origen, destino);
        view.Repaint();
    }
}
#endif
