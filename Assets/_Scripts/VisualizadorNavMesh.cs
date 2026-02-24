using UnityEngine;
using Unity.AI.Navigation;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Se asegura de que el NavMeshSurface tenga la visibilidad activada (overlay / datos listos para ver el azul en Scene).
/// Poné este script en el mismo GameObject que tiene NavMeshSurface (ej. Suelo) o en la cámara.
/// </summary>
public class VisualizadorNavMesh : MonoBehaviour
{
    [Tooltip("Si está vacío, usa 'Suelo'. El objeto con NavMeshSurface.")]
    public string nombreObjetoConSurface = "Suelo";

#if UNITY_EDITOR
    void OnEnable()
    {
        AsegurarVisibilidadEnEditor();
    }

    void OnValidate()
    {
        AsegurarVisibilidadEnEditor();
    }

    void AsegurarVisibilidadEnEditor()
    {
        if (!Application.isPlaying && string.IsNullOrEmpty(nombreObjetoConSurface))
            nombreObjetoConSurface = "Suelo";

        GameObject go = GameObject.Find(nombreObjetoConSurface);
        if (go == null) return;

        NavMeshSurface surface = go.GetComponent<NavMeshSurface>();
        if (surface == null) return;

        // Forzamos que el overlay del NavMesh pueda mostrarlo (el paquete usa preferencias de editor).
        // No hay propiedad "visible" en el componente; activamos vía SerializedObject si existe.
        SerializedObject so = new SerializedObject(surface);
        SerializedProperty showProp = so.FindProperty("m_ShowNavMesh");
        if (showProp != null)
        {
            showProp.boolValue = true;
            so.ApplyModifiedPropertiesWithoutUndo();
        }
    }
#endif
}
