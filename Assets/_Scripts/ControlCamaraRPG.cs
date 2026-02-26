using UnityEngine;

/// <summary>
/// Movimiento WASD/flechas en X y Z + zoom con ruedita. Lejos (100%): Orthographic, X=90°, Y=50. Cerca (0%): Perspective, X=30°, Y=10.
/// Transición suave con Lerp; zoom limitado para no atravesar el piso.
/// </summary>
[RequireComponent(typeof(Camera))]
public class ControlCamaraRPG : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float velocidad = 10f;

    [Header("Zoom (ruedita)")]
    [SerializeField] private float sensibilidadZoom = 0.1f;
    [SerializeField] private float suavizadoZoom = 10f;

    private Camera cam;
    private float zoomT = 1f;
    private float zoomSuave = 1f;

    void Awake()
    {
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        MovimientoWASD();
        ZoomInteligente();
    }

    private void MovimientoWASD()
    {
        float h = Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow) ? 1f : Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow) ? -1f : 0f;
        float v = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow) ? 1f : Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow) ? -1f : 0f;
        if (h == 0f && v == 0f) return;

        Vector3 movimiento = new Vector3(h, 0f, v) * (velocidad * Time.deltaTime);
        transform.Translate(movimiento, Space.World);
    }

    private void ZoomInteligente()
    {
        float scroll = Input.mouseScrollDelta.y * sensibilidadZoom;
        zoomT = Mathf.Clamp01(zoomT - scroll);
        zoomSuave = Mathf.Lerp(zoomSuave, zoomT, suavizadoZoom * Time.deltaTime);

        float rotX = Mathf.Lerp(30f, 90f, zoomSuave);
        float posY = Mathf.Lerp(10f, 50f, zoomSuave);

        Vector3 euler = transform.eulerAngles;
        transform.rotation = Quaternion.Euler(rotX, euler.y, euler.z);

        Vector3 pos = transform.position;
        transform.position = new Vector3(pos.x, posY, pos.z);

        cam.orthographic = zoomSuave >= 0.5f;
        if (cam.orthographic)
            cam.orthographicSize = Mathf.Lerp(15f, 50f, zoomSuave);
    }
}
