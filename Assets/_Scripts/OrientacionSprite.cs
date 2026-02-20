using UnityEngine;

/// <summary>
/// Controla la orientación estética del sprite (SpriteRenderer) en 4 direcciones: Arriba, Abajo, Izquierda, Derecha.
/// El SpriteRenderer puede estar en un hijo (p. ej. el triángulo bajo el Circle); se usa visualApocaMon o se busca en hijos.
/// </summary>
public class OrientacionSprite : MonoBehaviour
{
    [Header("Visual")]
    [Tooltip("SpriteRenderer que se orienta (p. ej. el del objeto hijo con el triángulo). Si está vacío, se busca en los hijos.")]
    [SerializeField] private SpriteRenderer visualApocaMon;

    private SpriteRenderer spriteRenderer;

    private static readonly Quaternion RotacionDerecha   = Quaternion.Euler(0f, 0f, 0f);
    private static readonly Quaternion RotacionArriba   = Quaternion.Euler(0f, 0f, 90f);
    private static readonly Quaternion RotacionIzquierda = Quaternion.Euler(0f, 0f, 180f);
    private static readonly Quaternion RotacionAbajo    = Quaternion.Euler(0f, 0f, -90f);

    private void Awake()
    {
        if (visualApocaMon != null)
            spriteRenderer = visualApocaMon;
        else
            spriteRenderer = GetComponentInChildren<SpriteRenderer>(true);

        if (spriteRenderer == null)
            Debug.LogWarning("OrientacionSprite: no hay visualApocaMon asignado ni SpriteRenderer en los hijos.");
    }

    /// <summary>
    /// Orienta el sprite hacia la posición del objetivo (4 direcciones: Arriba, Abajo, Izquierda, Derecha).
    /// </summary>
    public void MirarObjetivo(Vector3 posicionEnemigo)
    {
        if (spriteRenderer == null) return;

        Vector3 dir = (posicionEnemigo - transform.position).normalized;
        float x = dir.x;
        float y = dir.y;

        if (Mathf.Abs(x) >= Mathf.Abs(y))
        {
            if (x >= 0f)
                spriteRenderer.transform.rotation = RotacionDerecha;
            else
                spriteRenderer.transform.rotation = RotacionIzquierda;
        }
        else
        {
            if (y >= 0f)
                spriteRenderer.transform.rotation = RotacionArriba;
            else
                spriteRenderer.transform.rotation = RotacionAbajo;
        }
    }

    private void Update()
    {
        if (spriteRenderer == null) return;

        if (Input.GetKeyDown(KeyCode.RightArrow))
            spriteRenderer.transform.rotation = RotacionDerecha;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            spriteRenderer.transform.rotation = RotacionIzquierda;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            spriteRenderer.transform.rotation = RotacionArriba;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            spriteRenderer.transform.rotation = RotacionAbajo;
    }
}
