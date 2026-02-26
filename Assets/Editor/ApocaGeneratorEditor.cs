using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// Fábrica de 180 ApocaData. Estructura: Assets/ApocaDataFiles/[NombreDelClan], 10 por clan (18 clanes, excl. Desconocido).
/// Nombres correlativos 1.asset .. 180.asset. No modifica cámara.
/// </summary>
public static class ApocaGeneratorEditor
{
    private const string CarpetaSalida = "Assets/ApocaDataFiles";
    private const int ClanesExcluyendoDesconocido = 18;
    private const int PorClan = 10;
    private const int Total = 180;

    [MenuItem("ApocaMon/Generar 180 ApocaMon (Organizados)")]
    public static void Generar180Organizados()
    {
        if (Directory.Exists(CarpetaSalida))
        {
            bool borrar = EditorUtility.DisplayDialog("Limpieza previa",
                "La carpeta ApocaDataFiles ya existe. ¿Borrar todo su contenido?",
                "Sí, borrar y generar", "Cancelar");
            if (!borrar) return;
            AssetDatabase.DeleteAsset(CarpetaSalida);
            AssetDatabase.Refresh();
        }

        AsegurarCarpetaBase();
        CrearSubcarpetasPorClan();

        List<ClanApocaMon> lista = new List<ClanApocaMon>(Total);
        for (int c = 1; c <= ClanesExcluyendoDesconocido; c++)
        {
            var clan = (ClanApocaMon)c;
            for (int j = 0; j < PorClan; j++)
                lista.Add(clan);
        }
        Shuffle(lista);

        for (int i = 0; i < Total; i++)
        {
            ClanApocaMon clan = lista[i];
            string nombreClan = clan.ToString();
            string path = $"{CarpetaSalida}/{nombreClan}/{i + 1}.asset";
            CrearApocaDataEn(path, clan, i + 1);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"<color=green>Generados {Total} ApocaData en {CarpetaSalida}</color>");
    }

    private static void AsegurarCarpetaBase()
    {
        if (!Directory.Exists(CarpetaSalida))
            Directory.CreateDirectory(CarpetaSalida);
        if (!AssetDatabase.IsValidFolder(CarpetaSalida))
        {
            AssetDatabase.CreateFolder("Assets", "ApocaDataFiles");
            AssetDatabase.Refresh();
        }
    }

    private static void CrearSubcarpetasPorClan()
    {
        for (int c = 1; c <= ClanesExcluyendoDesconocido; c++)
        {
            string nombreClan = ((ClanApocaMon)c).ToString();
            string subcarpeta = $"{CarpetaSalida}/{nombreClan}";
            if (!AssetDatabase.IsValidFolder(subcarpeta.Replace("\\", "/")))
            {
                AssetDatabase.CreateFolder(CarpetaSalida, nombreClan);
                AssetDatabase.Refresh();
            }
        }
    }

    private static void CrearApocaDataEn(string path, ClanApocaMon clan, int id)
    {
        var asset = ScriptableObject.CreateInstance<ApocaData>();
        asset.nombre = $"{clan}_{id}";
        asset.clan = clan;
        asset.chatarraType = (TipoChatarra)Random.Range(0, 10);
        asset.ranked = (RankedGriego)Random.Range(0, 10);

        asset.VIDA = Random.Range(50f, 301f);
        asset.VELOCIDAD = Random.Range(2f, 7f);
        asset.ACELERACION = Random.Range(5f, 15f);
        asset.DAÑO = Random.Range(5f, 50f);
        asset.DEFENSA = Random.Range(0f, 25f);
        asset.RESISTENCIA = Random.Range(20f, 100f);
        asset.CADENCIA = Random.Range(0.5f, 3f);
        asset.ALCANCE = Random.Range(1.5f, 12f);

        AssetDatabase.CreateAsset(asset, path);
    }

    private static void Shuffle<T>(List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            T t = list[k];
            list[k] = list[n];
            list[n] = t;
        }
    }
}
