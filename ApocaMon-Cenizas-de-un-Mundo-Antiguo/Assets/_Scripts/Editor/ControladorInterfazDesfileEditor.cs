using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ControladorInterfazDesfile))]
public class ControladorInterfazDesfileEditor : Editor
{
    private SerializedProperty cantidadTiposOleada;
    private SerializedProperty poolTipos;

    private void OnEnable()
    {
        cantidadTiposOleada = serializedObject.FindProperty("cantidadTiposOleada");
        poolTipos = serializedObject.FindProperty("poolTipos");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawDefaultInspector();

        int n = cantidadTiposOleada.intValue;
        if (n < 0) n = 0;
        if (poolTipos.arraySize != n)
            poolTipos.arraySize = n;

        serializedObject.ApplyModifiedProperties();
    }
}
