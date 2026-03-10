using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering.Universal;

public class AuditoriaProyectoSistema : EditorWindow
{
    [MenuItem("Herramientas/Auditoría y Limpieza 3D")]
    public static void ShowWindow()
    {
        GetWindow<AuditoriaProyectoSistema>("Auditoría 3D");
    }

    void OnGUI()
    {
        if (GUILayout.Button("Ejecutar Auditoría Completa"))
        {
            EjecutarRevision();
        }
    }

    void EjecutarRevision()
    {
        Debug.Log("<color=cyan>--- INICIANDO AUDITORÍA DE ESCENA ---</color>");

        // 1. Verificar Componentes 2D prohibidos
        GameObject[] todos = GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        foreach (GameObject obj in todos)
        {
            if (obj.GetComponent<Collider2D>() != null)
            {
                Debug.LogWarning($"[CONFLICTO 2D]: '{obj.name}' tiene un Collider2D. Debe ser reemplazado por BoxCollider o MeshCollider.");
            }
            if (obj.GetComponent<Rigidbody2D>() != null)
            {
                Debug.LogWarning($"[CONFLICTO 2D]: '{obj.name}' tiene un Rigidbody2D. Debe ser Rigidbody (3D).");
            }

            // 2. Verificar Materiales (Culpable del terreno oscuro)
            Renderer rend = obj.GetComponent<Renderer>();
            if (rend != null)
            {
                foreach (Material mat in rend.sharedMaterials)
                {
                    if (mat != null && (mat.shader.name.Contains("Standard") || mat.shader.name.Contains("Diffuse")))
                    {
                        Debug.LogError($"[SHADER OBSOLETO]: '{obj.name}' usa '{mat.shader.name}'. En URP esto se verá negro o mal iluminado.");
                    }
                }
            }
        }

        // 3. Auditoría de Cámara
        Camera cam = Camera.main;
        if (cam != null)
        {
            var urpData = cam.GetUniversalAdditionalCameraData();
            if (urpData.scriptableRenderer == null)
            {
                Debug.LogError("[CÁMARA]: La cámara no tiene un Renderer asignado o es inválido.");
            }
            if (cam.orthographic)
            {
                Debug.Log("[INFO]: Cámara en modo Orthographic. Verificar que el Far Clipping Plane sea > 1000 para ver el cielo.");
            }
        }

        // 4. Auditoría de Iluminación Ambiental
        if (RenderSettings.ambientIntensity < 0.5f)
        {
            Debug.LogWarning($"[LUZ]: La intensidad ambiental es muy baja ({RenderSettings.ambientIntensity}). Por eso el terreno se ve oscuro.");
        }

        Debug.Log("<color=cyan>--- AUDITORÍA FINALIZADA ---</color>");
    }
}