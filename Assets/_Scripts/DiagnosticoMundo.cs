using UnityEngine;
using Unity.AI.Navigation;

#if UNITY_EDITOR
using UnityEditor;

public static class DiagnosticoMundo
{
    [MenuItem("ApocaMon/📋 Diagnóstico Mundo")]
    public static void Ejecutar()
    {
        Debug.Log("<color=white>══════════════ DIAGNÓSTICO DE MUNDO ══════════════</color>");

        // 1. Cámara: ¿Orthographic (2D) o Perspective (3D)?
        Camera cam = Object.FindFirstObjectByType<Camera>(FindObjectsInactive.Include);
        if (cam != null)
        {
            string tipo = cam.orthographic ? "Orthographic (2D)" : "Perspective (3D)";
            Debug.Log($"<color=yellow>📷 Cámara:</color> {tipo} | Posición Z: {cam.transform.position.z}");
        }
        else
            Debug.LogWarning("📷 Cámara: No se encontró ninguna cámara en la escena.");

        // 2. Objetos críticos: SUELO_DEFINITIVO (3D) y Enemigos_Zona_Toxica
        ReportarObjeto("SUELO_DEFINITIVO", GameObject.Find("SUELO_DEFINITIVO"));
        ReportarObjeto("Enemigos_Zona_Toxica", GameObject.Find("Enemigos_Zona_Toxica"));

        // Si Enemigos_Zona_Toxica no está en escena, reportar el prefab
        if (GameObject.Find("Enemigos_Zona_Toxica") == null)
        {
            string[] guids = AssetDatabase.FindAssets("Enemigos_Zona_Toxica t:Prefab");
            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (prefab != null)
                    Debug.Log($"<color=gray>[Prefab] Enemigos_Zona_Toxica no está en escena; componentes del prefab:</color>");
                ReportarComponentes(prefab);
                if (prefab != null)
                    Debug.Log($"  Z (prefab root): {prefab.transform.localPosition.z}");
            }
        }

        // 3. NavMesh: ¿a qué objeto está pegado y qué bounds tiene?
        NavMeshSurface surface = Object.FindFirstObjectByType<NavMeshSurface>(FindObjectsInactive.Include);
        if (surface != null)
        {
            string nombre = surface.gameObject.name;
            Debug.Log($"<color=cyan>🟦 NavMesh:</color> Pegado a objeto \"{nombre}\" | Posición Z: {surface.transform.position.z}");
            if (surface.navMeshData != null)
                Debug.Log($"  Bounds reales: {surface.navMeshData.sourceBounds.size}");
            else
                Debug.LogWarning("  Bounds: navMeshData es null (hacé Bake).");
        }
        else
            Debug.LogWarning("🟦 NavMesh: No se encontró ningún NavMeshSurface en la escena.");

        // 4. Z-Axis: resumen de posiciones Z
        Debug.Log("<color=white>── Z-Axis (resumen) ──</color>");
        ReportarZ("Cámara", cam?.transform);
        ReportarZ("SUELO_DEFINITIVO", GameObject.Find("SUELO_DEFINITIVO")?.transform);
        ReportarZ("Enemigos_Zona_Toxica (escena)", GameObject.Find("Enemigos_Zona_Toxica")?.transform);
        if (surface != null)
            ReportarZ("NavMeshSurface (" + surface.gameObject.name + ")", surface.transform);

        // Advertencia si suelo y bicho tienen Z muy distintos
        Transform tSuelo = GameObject.Find("SUELO_DEFINITIVO")?.transform;
        Transform tBicho = GameObject.Find("Enemigos_Zona_Toxica")?.transform;
        float zNav = surface != null ? surface.transform.position.z : 0f;
        if (tBicho != null && surface != null)
        {
            float diff = Mathf.Abs(tBicho.position.z - zNav);
            if (diff > 0.5f)
                Debug.LogWarning($"<color=orange>⚠️ El bicho (Z={tBicho.position.z}) y el NavMesh (Z={zNav}) están separados. Si el bicho está 'volando', no toca el NavMesh.</color>");
        }

        Debug.Log("<color=white>══════════════ FIN DIAGNÓSTICO ══════════════</color>");
    }

    static void ReportarObjeto(string nombre, GameObject go)
    {
        if (go == null)
        {
            Debug.Log($"<color=gray>❓ {nombre}: No encontrado en la escena.</color>");
            return;
        }
        Debug.Log($"<color=yellow>📦 {nombre}:</color>");
        ReportarComponentes(go);
        Debug.Log($"  Posición Z: {go.transform.position.z}");
    }

    static void ReportarComponentes(GameObject go)
    {
        if (go == null) return;
        bool tieneSprite = go.GetComponent<SpriteRenderer>() != null;
        bool tieneMesh = go.GetComponent<MeshRenderer>() != null;
        bool tieneMeshCollider = go.GetComponent<MeshCollider>() != null;
        bool tieneBox3D = go.GetComponent<BoxCollider>() != null;
        bool tieneBox2D = go.GetComponent<BoxCollider2D>() != null;
        Debug.Log($"  SpriteRenderer: {(tieneSprite ? "Sí" : "No")} | MeshRenderer: {(tieneMesh ? "Sí" : "No")} | MeshCollider: {(tieneMeshCollider ? "Sí" : "No")} | BoxCollider: {(tieneBox3D ? "Sí" : "No")} | BoxCollider2D: {(tieneBox2D ? "Sí" : "No")}");
    }

    static void ReportarZ(string etiqueta, Transform t)
    {
        if (t != null)
            Debug.Log($"  {etiqueta} → Z = {t.position.z}");
        else
            Debug.Log($"  {etiqueta} → (no encontrado)");
    }
}
#endif
