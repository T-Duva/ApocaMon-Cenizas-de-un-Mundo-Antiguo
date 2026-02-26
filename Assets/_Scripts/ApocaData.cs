using UnityEngine;

[CreateAssetMenu(fileName = "NuevoApocaData", menuName = "ApocaMon/ApocaData")]
public class ApocaData : ScriptableObject
{
    [Header("Identidad")]
    public string nombre;
    public ClanApocaMon clan;
    public TipoChatarra chatarraType;
    public RankedGriego ranked;

    [Header("Stats base")]
    public float VIDA;
    public float VELOCIDAD;
    public float ACELERACION;
    public float DAÑO;
    public float DEFENSA;
    public float RESISTENCIA;
    public float CADENCIA;
    public float ALCANCE;
}
